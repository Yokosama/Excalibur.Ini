namespace Excalibur.Ini
{
    /// <summary>
    /// 解析ini内容时的配置项
    /// </summary>
    public class IniParserConfiguration : ICloneable<IniParserConfiguration>
    {
        /// <summary>
        /// 按名称检索节点/属性关键字时不区分大小写，默认值：false
        /// </summary>
        public bool CaseInsensitive { get; set; } = false;

        /// <summary>
        /// true：允许全局节点存在，无需先定义节点再写属性，默认值：true
        /// </summary>
        public bool AllowKeysWithoutSection { get; set; } = true;

        /// <summary>
        /// 解析到重复节点时的行为，默认值：AllowAndRepeat
        /// </summary>
        public DuplicateBehaviour DuplicateSectionsBehaviour { get; set; } = DuplicateBehaviour.AllowAndRepeat;

        /// <summary>
        /// 解析到重复的节点属性时的行为，默认值：AllowAndRepeat
        /// </summary>
        public DuplicateBehaviour DuplicatePropertiesBehaviour { get; set; } = DuplicateBehaviour.AllowAndRepeat;

        /// <summary>
        /// true：错误时抛出异常；false：遇到错误时不抛出异常，默认值：true
        /// </summary>
        public bool ThrowExceptionsOnError { get; set; } = true;

        /// <summary>
        /// true：直接跳过无效行，不报错；false：遇到无效行时报错，默认值：false
        /// </summary>
        public bool SkipInvalidLines { get; set; } = false;

        /// <summary>
        /// true：移除节点名称的前后空白字符；false：节点名称包括前后空白字符，默认值：true
        /// </summary>
        public bool TrimSections { get; set; } = true;

        /// <summary>
        /// true：移除属性的关键字的前后空白字符；false：属性的关键字包括前后空白字符，默认值：true
        /// </summary>
        public bool TrimPropertiesKey { get; set; } = true;

        /// <summary>
        /// true：移除属性的值的前后空白字符；false：属性的值包括前后空白字符，默认值：false
        /// </summary>
        public bool TrimPropertiesValue { get; set; } = false;

        /// <summary>
        /// true：解析时保留注释；false：解析时不保留注释，默认值：true
        /// </summary>
        public bool ParseComments { get; set; } = true;

        /// <summary>
        /// true：移除注释的前后空白字符；false：注释包括前后空白字符，默认值：false
        /// </summary>
        public bool TrimComments { get; set; } = false;

        /// <summary>
        /// true：移除注释的起始字符串；false：保留起始字符串，默认值：true
        /// </summary>
        public bool RemoveCommentString { get; set; } = true;

        /// <summary>
        /// true：保留空白行到注释中；false：不保留空白行，默认值：false
        /// </summary>
        public bool ParseBlankLineAsComment { get; set; } = false;

        /// <summary>
        /// true：解析属性行后的注释；false：不解析属性行的注释，默认值：true
        /// </summary>
        public bool ParseCommentAfterProperty { get; set; } = true;

        /// <summary>
        /// true：解析节点定义名称后的注释；false：不解析节点定义后的注释，默认值：true
        /// </summary>
        public bool ParseCommentAfterSection { get; set; } = true;

        /// <summary>
        /// true：当SkipInvalidLines=true时，解析时将无效行作为注释；false：不解析无效行为注释，默认值：false
        /// </summary>
        public bool InvalidLineAsComment { get; set; } = false;

        public IniParserConfiguration()
        {
        }

        public IniParserConfiguration(IniParserConfiguration other)
        {
            CaseInsensitive = other.CaseInsensitive;
            AllowKeysWithoutSection = other.AllowKeysWithoutSection;
            DuplicateSectionsBehaviour = other.DuplicateSectionsBehaviour;
            DuplicatePropertiesBehaviour = other.DuplicatePropertiesBehaviour;
            ThrowExceptionsOnError = other.ThrowExceptionsOnError;
            SkipInvalidLines = other.SkipInvalidLines;
            TrimSections = other.TrimSections;
            TrimPropertiesKey = other.TrimPropertiesKey;
            TrimPropertiesValue = other.TrimPropertiesValue;
            TrimComments = other.TrimComments;
            ParseComments = other.ParseComments;
            RemoveCommentString = other.RemoveCommentString;
            ParseBlankLineAsComment = other.ParseBlankLineAsComment;
            ParseCommentAfterProperty = other.ParseCommentAfterProperty;
            ParseCommentAfterSection = other.ParseCommentAfterSection;
            InvalidLineAsComment = other.InvalidLineAsComment;
        }

        public IniParserConfiguration Clone()
        {
            return new IniParserConfiguration(this);
        }

        /// <summary>
        /// 解析到重复节点或属性时的行为
        /// </summary>
        public enum DuplicateBehaviour
        {
            /// <summary>
            /// 允许，重复添加节点/属性
            /// </summary>
            AllowAndRepeat,
            /// <summary>
            /// 不允许，停止解析并报错
            /// </summary>
            DisallowAndStopWithError,
            /// <summary>
            /// 允许，保留第一个节点/属性值
            /// </summary>
            AllowAndKeepFirstValue,
            /// <summary>
            /// 允许，保留最后一个节点/属性值
            /// </summary>
            AllowAndKeepLastValue,
        }
    }
}
