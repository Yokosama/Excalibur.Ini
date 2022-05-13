using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;

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

        /// <summary>
        /// 默认构造函数
        /// </summary>
        public IniDataParser()
        {
            Scheme = new IniScheme();
            Configuration = new IniParserConfiguration();
            _errorExceptions = new List<Exception>();
        }

        /// <summary>
        /// 解析ini字符串
        /// </summary>
        /// <param name="iniString">ini字符串</param>
        /// <returns>解析后的IniData</returns>
        public IniData Parse(string iniString)
        {
            return Parse(new StringReader(iniString));
        }

        /// <summary>
        /// 通过TextReader解析ini数据
        /// </summary>
        /// <param name="reader">TextReader</param>
        /// <returns>解析后的IniData</returns>
        public IniData Parse(TextReader reader)
        {
            IniData iniData = Configuration.CaseInsensitive ? new IniDataCaseInsensitive(Scheme) : new IniData(Scheme);

            Parse(reader, iniData);

            return iniData;
        }

        private void Parse(TextReader reader, IniData iniData)
        {
            iniData.Clear();
            iniData.ParserConfiguration = Configuration;
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
            _currentSectionName = null;
            _currentLineNumber = 0;
        }

        /// <summary>
        /// 解析文本的行
        /// </summary>
        /// <param name="currentLine">当前文本行</param>
        /// <param name="iniData">当前的ini数据</param>
        /// <exception cref="IniParsingException">Ini解析异常</exception>
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

            if (Configuration.SkipInvalidLines)
            {
                if (Configuration.InvalidLineAsComment)
                {
                    _currentComments.Add(currentLine);
                }
                return;
            }

            var error = $"Couldn't parse text. Please see configuration option {nameof(Configuration)}.{nameof(Configuration.SkipInvalidLines)} to ignore this error.";

            throw new IniParsingException(error, _currentLineNumber, currentLine);
        }

        /// <summary>
        /// 解析注释行
        /// </summary>
        /// <param name="currentLine">当前文本行</param>
        /// <param name="currentLineTrimmed">移除前后空格的文本行</param>
        /// <returns>true：解析注释成功；false：非注释行</returns>
        protected virtual bool ParseComment(string currentLine, string currentLineTrimmed)
        {
            var commentString = Scheme.CommentStrings.Find(x => currentLineTrimmed.StartsWith(x));
            if (string.IsNullOrEmpty(commentString))
            {
                return false;
            }

            if (!Configuration.ParseComments)
            {
                return true;
            }

            if (Configuration.RemoveCommentString)
            {
                var commentStart = currentLine.IndexOf(commentString);
                var startIndex = commentStart + commentString.Length;
                var length = currentLine.Length - startIndex;

                var comment = currentLine.Substring(startIndex, length);
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

        /// <summary>
        /// 解析节点行
        /// </summary>
        /// <param name="currentLine">当前文本行</param>
        /// <param name="currentLineTrimmed">移除前后空格的文本行</param>
        /// <param name="iniData">当前的ini数据</param>
        /// <returns>true：解析节点行成功；false：非节点行</returns>
        /// <exception cref="IniParsingException">Ini解析异常</exception>
        protected virtual bool ParseSection(string currentLine, string currentLineTrimmed, IniData iniData)
        {
            // 'SectionStartString'section_name'SectionEndString' eg: [section_name]
            // 如果不是第一个符号为section起始符，则返回
            if (!currentLineTrimmed.StartsWith(Scheme.SectionStartString)) return false;

            var startIndex = currentLine.IndexOf(Scheme.SectionStartString) + Scheme.SectionStartString.Length;

            var endIndex = startIndex;
            var lineLength = currentLine.Length;
            for (int i = endIndex; i < lineLength; i++)
            {
                if (ContainsSubString(Scheme.SectionEndString, currentLine, lineLength, i))
                {
                    endIndex = i;
                    break;
                }
            }

            var commentAfterSectionName = "";
            if (Configuration.ParseComments && Configuration.ParseCommentAfterSection)
            {
                if (HasComment(currentLine, Scheme.CommentStrings, endIndex + 1, out int commentStart, out string currentCommentString))
                {
                    var comment = currentLine.Substring(commentStart);

                    if (Configuration.RemoveCommentString)
                    {
                        var commentStartIndex = currentCommentString.Length;
                        if (commentStartIndex < comment.Length)
                        {
                            comment = comment.Substring(commentStartIndex);
                        }
                        if (Configuration.TrimComments)
                        {
                            comment = comment.Trim();
                        }

                        commentAfterSectionName = comment;
                    }
                    else
                    {
                        // 直接写入原有的注释行
                        commentAfterSectionName = comment;
                    }
                }
            }

            var length = endIndex - startIndex;
            var sectionName = currentLine.Substring(startIndex, length);

            if (string.IsNullOrWhiteSpace(sectionName))
            {
                if (Configuration.SkipInvalidLines) return false;

                var error = $"Section name can not be null or whitespace. Please see configuration option {nameof(Configuration)}.{nameof(Configuration.SkipInvalidLines)} to ignore this error.";
                throw new IniParsingException(error, _currentLineNumber, currentLine);
            }

            if (Configuration.TrimSections)
            {
                sectionName = sectionName.Trim();
            }

            _currentSectionName = sectionName;

            var exists = iniData.ContainsSection(sectionName);
            if (exists)
            {
                switch (Configuration.DuplicateSectionsBehaviour)
                {
                    case IniParserConfiguration.DuplicateBehaviour.AllowAndRepeat:
                        break;
                    case IniParserConfiguration.DuplicateBehaviour.DisallowAndStopWithError:
                        if (Configuration.SkipInvalidLines) return false;

                        var error = $"Duplicate section with name '{sectionName}'. Please see configuration option {nameof(Configuration)}.{nameof(Configuration.DuplicateSectionsBehaviour)} to fix this error, or configuration option {nameof(Configuration)}.{nameof(Configuration.SkipInvalidLines)} to ignore this error.";
                        throw new IniParsingException(error, _currentLineNumber, currentLine);
                    case IniParserConfiguration.DuplicateBehaviour.AllowAndKeepFirstValue:
                        return false;
                    case IniParserConfiguration.DuplicateBehaviour.AllowAndKeepLastValue:
                        iniData.Sections.RemoveFirst(sectionName);
                        break;
                }
            }

            var section = iniData.Add(sectionName);
            section.CommentAfterSectionName = commentAfterSectionName;
            if (Configuration.ParseComments && _currentComments.Count > 0)
            {
                section.Comments = _currentComments;
                _currentComments.Clear();
            }

            return true;
        }

        /// <summary>
        /// 解析属性行
        /// </summary>
        /// <param name="currentLine">当前文本行</param>
        /// <param name="iniData">当前的ini数据</param>
        /// <returns>true：解析属性行成功；false：非属性行</returns>
        /// <exception cref="IniParsingException">Ini解析异常</exception>
        protected virtual bool ParseProperty(string currentLine, IniData iniData)
        {
            var propertyAssignmentIndex = currentLine.IndexOf(Scheme.PropertyAssignmentString);

            if (propertyAssignmentIndex < 0) return false;

            var keyLength = propertyAssignmentIndex;
            var key = currentLine.Substring(0, keyLength);
            if (string.IsNullOrWhiteSpace(key))
            {
                if (Configuration.SkipInvalidLines) return false;

                var error = $"Property key is null or whitespace. Please see configuration option {nameof(Configuration)}.{nameof(Configuration.SkipInvalidLines)} to ignore this error.";
                throw new IniParsingException(error, _currentLineNumber, currentLine);
            }

            var valueStartIndex = propertyAssignmentIndex + Scheme.PropertyAssignmentString.Length;
            var value = currentLine.Substring(valueStartIndex);

            var commentAfterValue = "";
            if (Configuration.ParseComments && Configuration.ParseCommentAfterProperty)
            {
                if (HasComment(value, Scheme.CommentStrings, 0, out int commentStart, out string currentCommentString))
                {
                    var comment = value.Substring(commentStart);

                    if (commentStart > 0)
                    {
                        value = value.Substring(0, commentStart);
                    }
                    else
                    {
                        value = "";
                    }

                    if (Configuration.RemoveCommentString)
                    {
                        var startIndex = currentCommentString.Length;
                        if(startIndex < comment.Length)
                        {
                            comment = comment.Substring(startIndex);
                        }
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

            if (Configuration.TrimPropertiesKey) key = key.Trim();
            if (Configuration.TrimPropertiesValue) value = value.Trim();

            if (string.IsNullOrEmpty(_currentSectionName))
            {
                if (!Configuration.AllowKeysWithoutSection)
                {
                    var error = $"Properties must be contained inside a section. Please see configuration option {nameof(Configuration)}.{nameof(Configuration.AllowKeysWithoutSection)} to fix this error.";
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

        private static bool HasComment(string valueLine, List<string> commentStrings, int startIndex, out int commentStartIndex, out string currentCommentString)
        {
            commentStartIndex = -1;
            currentCommentString = "";
            foreach (var commentString in commentStrings)
            {
                var valueLineLength = valueLine.Length;
                for (int i = startIndex; i < valueLine.Length; i++)
                {
                    if (ContainsSubString(commentString, valueLine, valueLineLength, i))
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

        private static bool ContainsSubString(string subString, string content, int contentLength, int currentContentIndex)
        {
            for (int j = 0; j < subString.Length; j++)
            {
                var lineIndex = currentContentIndex + j;
                var preLineIndex = lineIndex - 1;
                if (lineIndex >= contentLength)
                {
                    return false;
                }

                if (content[lineIndex] != subString[j])
                {
                    return false;
                }

                if (preLineIndex >= 0 && content[preLineIndex] == '\\')
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
