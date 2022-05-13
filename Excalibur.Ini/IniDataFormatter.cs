using System.Collections.Generic;
using System.Text;

namespace Excalibur.Ini
{
    /// <summary>
    /// Ini数据格式化
    /// </summary>
    public class IniDataFormatter : IIniDataFormatter
    {
        /// <summary>
        /// 格式化IniData
        /// </summary>
        /// <param name="iniData">ini数据</param>
        /// <param name="format">格式化配置</param>
        /// <returns>格式化后的字符串</returns>
        public string Format(IniData iniData, IniFormattingConfiguration format)
        {
            var sb = new StringBuilder();

            // 全局属性
            WriteProperties(iniData.Global.Properties, sb, iniData.Scheme, iniData.ParserConfiguration, format);

            foreach (var section in iniData.Sections)
            {
                WriteSection(section, sb, iniData.Scheme, iniData.ParserConfiguration, format);
            }

            WriteComments(iniData.EndOfFileComments, sb, iniData.Scheme, iniData.ParserConfiguration, format);

            var newLineLength = format.NewLineString.Length;
            // 移除最后一个空行
            sb.Remove(sb.Length - newLineLength, newLineLength);
            return sb.ToString();
        }

        private void WriteSection(Section section, StringBuilder sb, IniScheme scheme, IniParserConfiguration parserConfiguration, IniFormattingConfiguration format)
        {
            WriteComments(section.Comments, sb, scheme, parserConfiguration, format);

            if(format.NewLineBeforeSectionName && sb.Length > 0) sb.Append(format.NewLineString);

            var commentAfterSectionName = GetCommentString(section.CommentAfterSectionName, scheme, parserConfiguration);
            sb.Append($"{scheme.SectionStartString}{section.Name}{scheme.SectionEndString}{commentAfterSectionName}{format.NewLineString}");

            if (format.NewLineAfterSectionName) sb.Append(format.NewLineString);

            WriteProperties(section.Properties, sb, scheme, parserConfiguration, format);

            if (format.NewLineAfterSection) sb.Append(format.NewLineString);
        }

        private void WriteProperties(KeyValues<Property> properties, StringBuilder sb, IniScheme scheme, IniParserConfiguration parserConfiguration, IniFormattingConfiguration format)
        {
            foreach (Property property in properties)
            {
                WriteComments(property.Comments, sb, scheme, parserConfiguration, format);

                if (format.NewLineBeforeProperty)
                {
                    sb.Append(format.NewLineString);
                }

                var commentAfterValue = GetCommentString(property.CommentAfterValue, scheme, parserConfiguration);
                sb.Append($"{property.Key}{format.SpacesBetweenKeyAndAssignment}{scheme.PropertyAssignmentString}{format.SpacesBetweenAssignmentAndValue}{property.Value}{commentAfterValue}{format.NewLineString}");

                if (format.NewLineAfterProperty)
                {
                    sb.Append(format.NewLineString);
                }
            }
        }

        private void WriteComments(List<string> comments, StringBuilder sb, IniScheme scheme, IniParserConfiguration parserConfiguration, IniFormattingConfiguration format)
        {
            if (parserConfiguration.RemoveCommentString)
            {
                var commentString = scheme.CommentStrings.Count > 0 ? scheme.CommentStrings[0] : ";";
                foreach (string comment in comments)
                {
                    sb.Append($"{commentString}{comment}{format.NewLineString}");
                }
            }
            else
            {
                foreach (string comment in comments)
                {
                    sb.Append($"{comment}{format.NewLineString}");
                }
            }
        }

        private string GetCommentString(string comment, IniScheme scheme, IniParserConfiguration parserConfiguration)
        {
            if (string.IsNullOrEmpty(comment))
            {
                return "";
            }

            if (parserConfiguration.RemoveCommentString)
            {
                var commentString = scheme.CommentStrings.Count > 0 ? scheme.CommentStrings[0] : ";";
                return $"{commentString}{comment}";
            }
            else
            {
                return comment;
            }
        }
    }
}
