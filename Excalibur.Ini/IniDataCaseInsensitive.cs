using System;

namespace Excalibur.Ini
{
    /// <summary>
    /// 忽略节点名称和属性关键字的大小写的ini数据
    /// </summary>
    public class IniDataCaseInsensitive : IniData
    {
        public IniDataCaseInsensitive()
        {
            Global = new Section(GlobalSection, StringComparer.OrdinalIgnoreCase);
            Sections = new KeyValues<Section>(StringComparer.OrdinalIgnoreCase);
        }

        public IniDataCaseInsensitive(IniScheme scheme)
        {
            Global = new Section(GlobalSection, StringComparer.OrdinalIgnoreCase);
            Sections = new KeyValues<Section>(StringComparer.OrdinalIgnoreCase);
            _scheme = scheme.Clone();
        }

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
