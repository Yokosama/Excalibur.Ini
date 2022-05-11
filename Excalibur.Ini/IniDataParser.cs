using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;

namespace Excalibur.Ini
{
    /// <summary>
    /// ini数据解析器
    /// </summary>
    public class IniDataParser
    {
        /// <summary>
        /// ini内容格式定义
        /// </summary>
        public IniScheme Scheme { get; protected set; }

        /// <summary>
        /// 解析器配置
        /// </summary>
        public virtual IniParserConfiguration Configuration { get; protected set; }

        private readonly List<Exception> _errorExceptions;
        /// <summary>
        /// 异常信息列表
        /// </summary>
        public ReadOnlyCollection<Exception> ErrorExceptions => _errorExceptions.AsReadOnly();

        /// <summary>
        /// 是否存在解析错误
        /// </summary>
        public bool HasError => _errorExceptions.Count > 0;

        private readonly List<string> _currentComments = new List<string>();
        private string _currentSectionName;
        private uint _currentLineNumber;

        public IniDataParser()
        {
            Scheme = new IniScheme();
            Configuration = new IniParserConfiguration();
            _errorExceptions = new List<Exception>();
        }

        public IniData Parse(string iniString)
        {
            return Parse(new StringReader(iniString));
        }

        public IniData Parse(TextReader reader)
        {
            IniData iniData = Configuration.CaseInsensitive ? new IniDataCaseInsensitive(Scheme) : new IniData(Scheme);

            Parse(reader, iniData);

            return iniData;
        }

        private void Parse(TextReader reader, IniData iniData)
        {
            iniData.Clear();
            _errorExceptions.Clear();
            if (Configuration.ParseComments)
            {
                _currentComments.Clear();
            }
            _currentSectionName = string.Empty;
            _currentLineNumber = 0;

            string line;
            while ((line = reader.ReadLine()) != null)
            {
                _currentLineNumber++;

                try
                {
                    ParseLine(line, iniData);
                }
                catch (Exception e)
                {
                    _errorExceptions.Add(e);
                    if (Configuration.ThrowExceptionsOnError)
                    {
                        throw;
                    }
                }
            }

            // 文件末尾注释
            if (Configuration.ParseComments && _currentComments.Count > 0)
            {
                iniData.EndOfFileComments = _currentComments;
                _currentComments.Clear();
            }

            if (HasError)
            {
                iniData.Clear();
            }
        }

        protected virtual void ParseLine(string currentLine, IniData iniData)
        {
            if (string.IsNullOrWhiteSpace(currentLine))
            {
                if (Configuration.ParseComments && Configuration.ParseBlankLineAsComment)
                {
                    _currentComments.Add(currentLine);
                }
                return;
            }

            var currentLineTrimmed = currentLine.Trim();

            if (ParseComment(currentLine, currentLineTrimmed)) return;

            if (ParseSection(currentLine, currentLineTrimmed, iniData)) return;

            if (ParseProperty(currentLine, iniData)) return;

            if (Configuration.SkipInvalidLines) return;

            var error = $"Couldn't parse text. Please see configuration option {nameof(Configuration)}.{nameof(Configuration.SkipInvalidLines)} to ignore this error.";

            throw new IniParsingException(error, _currentLineNumber, currentLine);
        }

        protected virtual bool ParseComment(string currentLine, string currentLineTrimmed)
        {
            if (!Configuration.ParseComments)
            {
                return true;
            }

            var commentString = Scheme.CommentStrings.Find(x => currentLineTrimmed.StartsWith(x));
            if (string.IsNullOrEmpty(commentString))
            {
                return false;
            }

            if (Configuration.RemoveCommentString)
            {
                var commentStart = currentLine.IndexOf(commentString);
                var startIndex = commentStart + commentString.Length;
                var length = currentLine.Length - startIndex;

                var comment = currentLineTrimmed.Substring(startIndex, length);
                if (Configuration.TrimComments)
                {
                    comment = comment.Trim();
                }

                _currentComments.Add(comment);
            }
            else
            {
                // 直接写入原有的注释行
                _currentComments.Add(currentLine);
            }

            return true;
        }

        protected virtual bool ParseSection(string currentLine, string currentLineTrimmed, IniData iniData)
        {
            // 如果不是第一个符号为section起始符，最后一个符号不是section结束符，则返回
            // 'SectionStartString'section_name'SectionEndString' eg: [section_name]
            if (!(currentLineTrimmed.StartsWith(Scheme.SectionStartString) && currentLineTrimmed.EndsWith(Scheme.SectionEndString))) return false;

            var startIndex = Scheme.SectionStartString.Length;
            var length = currentLineTrimmed.Length - startIndex - Scheme.SectionEndString.Length;
            var sectionName = currentLineTrimmed.Substring(startIndex, length);

            if (string.IsNullOrWhiteSpace(sectionName))
            {
                if (Configuration.SkipInvalidLines) return false;

                var error = $"Section name can not be null or whitespace. Please see configuration option {nameof(Configuration)}.{nameof(Configuration.SkipInvalidLines)} 以忽略这个错误";
                throw new IniParsingException(error, _currentLineNumber, currentLine);
            }

            if (Configuration.TrimSections)
            {
                sectionName = sectionName.Trim();
            }

            _currentSectionName = sectionName;

            var exists = iniData.ContainsSection(sectionName);

            switch (Configuration.DuplicateSectionsBehaviour)
            {
                case IniParserConfiguration.DuplicateBehaviour.AllowAndRepeat:
                    break;
                case IniParserConfiguration.DuplicateBehaviour.DisallowAndStopWithError:
                    if (exists)
                    {
                        if (Configuration.SkipInvalidLines) return false;
                        var error = $"Duplicate section with name '{sectionName}'. Please see configuration option {nameof(Configuration)}.{nameof(Configuration.DuplicateSectionsBehaviour)} to fix this error.";
                        throw new IniParsingException(error, _currentLineNumber, currentLine);
                    }
                    break;
                case IniParserConfiguration.DuplicateBehaviour.AllowAndKeepFirstValue:
                    return false;
                case IniParserConfiguration.DuplicateBehaviour.AllowAndKeepLastValue:
                    iniData.Sections.RemoveFirst(sectionName);
                    break;
            }

            var section = iniData.Add(sectionName);

            if (Configuration.ParseComments && _currentComments.Count > 0)
            {
                section.Comments = _currentComments;
                _currentComments.Clear();
            }

            return true;
        }

        protected virtual bool ParseProperty(string currentLine, IniData iniData)
        {
            var propertyAssignmentIndex = currentLine.IndexOf(Scheme.PropertyAssignmentString);

            if (propertyAssignmentIndex < 0) return false;

            var keyLength = propertyAssignmentIndex - 1;
            var key = currentLine.Substring(0, keyLength);
            if (string.IsNullOrWhiteSpace(key))
            {
                if (Configuration.SkipInvalidLines) return false;

                var error = $"Property key is null or whitespace. Please see configuration option {nameof(Configuration)}.{nameof(Configuration.SkipInvalidLines)} to ignore this error";
                throw new IniParsingException(error, _currentLineNumber, currentLine);
            }

            var valueStartIndex = propertyAssignmentIndex + Scheme.PropertyAssignmentString.Length + 1;
            var value = currentLine.Substring(valueStartIndex);

            var commentAfterValue = "";
            if (Configuration.ParseComments && Configuration.ParseCommentAfterProperty)
            {
                if (HasComment(value, Scheme.CommentStrings, out int commentStart, out string currentCommentString))
                {
                    if (commentStart > 0)
                    {
                        value = value.Substring(0, commentStart);
                    }
                    else
                    {
                        value = "";
                    }

                    var comment = value.Substring(commentStart);
                    if (Configuration.RemoveCommentString)
                    {
                        var startIndex = commentStart + currentCommentString.Length;
                        comment = comment.Substring(startIndex);
                        if (Configuration.TrimComments)
                        {
                            comment = comment.Trim();
                        }

                        commentAfterValue = comment;
                    }
                    else
                    {
                        // 直接写入原有的注释行
                        commentAfterValue = comment;
                    }
                }
            }

            if (Configuration.TrimPropertiesKey) key.Trim();
            if (Configuration.TrimPropertiesValue) value.Trim();

            if (string.IsNullOrEmpty(_currentSectionName))
            {
                if (!Configuration.AllowKeysWithoutSection)
                {
                    var error = $"Properties must be contained inside a section. Please see configuration option {nameof(Configuration)}.{nameof(Configuration.AllowKeysWithoutSection)} to ignore this error";
                    throw new IniParsingException(error, _currentLineNumber, currentLine);
                }

                AddProperty(key.ToString(), value.ToString(), iniData.Global, "global", commentAfterValue);
            }
            else
            {
                AddProperty(key.ToString(), value.ToString(), iniData.GetSection(_currentSectionName, true), _currentSectionName, commentAfterValue);
            }

            return true;
        }

        private static bool HasComment(string valueLine, List<string> commentStrings, out int commentStartIndex, out string currentCommentString)
        {
            commentStartIndex = -1;
            currentCommentString = "";
            foreach (var commentString in commentStrings)
            {
                var valueLineLength = valueLine.Length;
                for (int i = 0; i < valueLine.Length; i++)
                {
                    if (IsCommentStart(commentString, valueLine, valueLineLength, i))
                    {
                        if (commentStartIndex > i || commentStartIndex < 0)
                        {
                            commentStartIndex = i;
                            currentCommentString = commentString;
                        }
                        break;
                    }
                }
            }

            return commentStartIndex >= 0;
        }

        private static bool IsCommentStart(string commentString, string currentLine, int currentLineLength, int currentLineIndex)
        {
            for (int j = 0; j < commentString.Length; j++)
            {
                var lineIndex = currentLineIndex + j;
                var preLineIndex = lineIndex - 1;
                if (lineIndex >= currentLineLength)
                {
                    return false;
                }

                if (currentLine[lineIndex] != commentString[j])
                {
                    return false;
                }

                if (preLineIndex >= 0 && currentLine[preLineIndex] == '\\')
                {
                    return false;
                }
            }
            return true;
        }

        private void AddProperty(string key, string value, Section section, string sectionName, string commentAfterValue)
        {
            if (section == null)
            {
                return;
            }

            bool add = false;
            if (section.Properties.ContainsKey(key))
            {
                switch (Configuration.DuplicatePropertiesBehaviour)
                {
                    case IniParserConfiguration.DuplicateBehaviour.AllowAndRepeat:
                        section.Add(key, value);
                        add = true;
                        break;
                    case IniParserConfiguration.DuplicateBehaviour.DisallowAndStopWithError:
                        var error = $"Duplicate property key with name '{key}' in section: {sectionName}.";
                        throw new IniParsingException(error, _currentLineNumber);
                    case IniParserConfiguration.DuplicateBehaviour.AllowAndKeepFirstValue:
                        return;
                    case IniParserConfiguration.DuplicateBehaviour.AllowAndKeepLastValue:
                        section.SetPropertyValue(key, value, false);
                        break;
                }
            }
            else
            {
                section.Add(key, value);
                add = true;
            }

            if (Configuration.ParseComments && add)
            {
                var p = section.Properties.FindLast(key);
                p.Comments = _currentComments;
                p.CommentAfterValue = commentAfterValue;
                _currentComments.Clear();
            }
        }
    }
}
