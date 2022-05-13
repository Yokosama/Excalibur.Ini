using System;

namespace Excalibur.Ini
{
    /// <summary>
    /// 忽略节点名称和属性关键字的大小写的ini数据
    /// </summary>
    public class IniDataCaseInsensitive : IniData
    {
        /// <summary>
        /// 默认构造函数
        /// </summary>
        public IniDataCaseInsensitive()
        {
            Global = new Section(GlobalSection, StringComparer.OrdinalIgnoreCase);
            Sections = new KeyValues<Section>(StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// 带IniScheme的构造函数
        /// </summary>
        /// <param name="scheme"></param>
        public IniDataCaseInsensitive(IniScheme scheme)
        {
            Global = new Section(GlobalSection, StringComparer.OrdinalIgnoreCase);
            Sections = new KeyValues<Section>(StringComparer.OrdinalIgnoreCase);
            _scheme = scheme.Clone();
        }

        /// <summary>
        /// 复制其他IniData的构造函数
        /// </summary>
        /// <param name="other"></param>
        public IniDataCaseInsensitive(IniData other)
        : this()
        {
            Global = new Section(other.Global, StringComparer.OrdinalIgnoreCase);
            Sections = new KeyValues<Section>(other.Sections, StringComparer.OrdinalIgnoreCase);
            Scheme = other.Scheme;
            ParserConfiguration = other.ParserConfiguration;
        }
    }
}
