using System.Collections.Generic;

namespace Excalibur.Ini
{
    public class IniData : ICloneable<IniData>
    {
        public const string GlobalSection = "global";

        /// <summary>
        /// Global Properties
        /// </summary>
        public Section Global { get; protected set; }
        public KeyValues<Section> Sections { get; protected set; }

        protected IniScheme _scheme;
        /// <summary>
        /// ini content format
        /// </summary>
        public IniScheme Scheme
        {
            get
            {
                if (_scheme == null)
                {
                    _scheme = new IniScheme();
                }

                return _scheme;
            }

            set
            {
                if (value == null) return;
                _scheme = value.Clone();
            }
        }

        protected IniParserConfiguration _parserConfiguration;
        /// <summary>
        /// ini内容的解析配置
        /// </summary>
        public IniParserConfiguration ParserConfiguration
        {
            get
            {
                if (_parserConfiguration == null)
                {
                    _parserConfiguration = new IniParserConfiguration();
                }

                return _parserConfiguration;
            }

            set
            {
                if (value == null) return;
                _parserConfiguration = value.Clone();
            }
        }

        private List<string> _endOfFileComments;
        /// <summary>
        /// 文件末尾的注释，不属于任何节点或属性
        /// </summary>
        public List<string> EndOfFileComments
        {
            get
            {
                if (_endOfFileComments == null)
                {
                    _endOfFileComments = new List<string>();
                }
                return _endOfFileComments;
            }
            set
            {
                if (_endOfFileComments == null)
                {
                    _endOfFileComments = new List<string>();
                }
                _endOfFileComments.Clear();
                _endOfFileComments.AddRange(value);
            }
        }

        public int SectionCount => Sections.Length;

        public IniData()
        {
            Global = new Section(GlobalSection);
            Sections = new KeyValues<Section>();
        }

        public IniData(IniScheme scheme)
        {
            Global = new Section(GlobalSection);
            Sections = new KeyValues<Section>();
            _scheme = scheme.Clone();
        }

        public IniData(IniData other)
        {
            Global = other.Global.Clone();
            Sections = other.Sections.Clone();
            Scheme = other.Scheme;
            ParserConfiguration = other.ParserConfiguration;
        }

        public Section Add(string sectionName, bool canRepeat = true)
        {
            if (string.IsNullOrEmpty(sectionName)) return null;

            var section = new Section(sectionName);
            Sections.Add(sectionName, section, canRepeat);
            return section;
        }

        public Section Add(Section section, bool canRepeat = true)
        {
            if (section == null) return null;
             
            Sections.Add(section.Name, section, canRepeat);
            return section;
        }

        public Section GetSection(string sectionName, bool lastSection = false)
        {
            if (string.IsNullOrEmpty(sectionName))
            {
                return null;
            }

            return lastSection ? Sections.FindLast(sectionName) : Sections.Find(sectionName);
        }

        public List<Section> GetSections(string sectionName)
        {
            if (string.IsNullOrEmpty(sectionName))
            {
                return null;
            }

            return Sections.FindAll(sectionName);
        }

        public bool ContainsSection(string sectionName)
        {
            return Sections.ContainsKey(sectionName);
        }

        public string GetPropertyRawValue(string sectionName, string key, string nullValue, bool lastSection = false, bool lastProperty = false)
        {
            if (string.IsNullOrEmpty(sectionName))
            {
                return Global.GetPropertyRawValue(key, nullValue, lastProperty);
            }

            var section = lastSection ? Sections.FindLast(sectionName) : Sections.Find(sectionName);
            if (section == null)
            {
                return nullValue;
            }

            return section.GetPropertyRawValue(key, nullValue, lastProperty);
        }

        public T GetPropertyValue<T>(string sectionName, string key, T nullValue, bool lastSection = false, bool lastProperty = false)
        {
            if (string.IsNullOrEmpty(sectionName))
            {
                return Global.GetPropertyValue(key, nullValue, lastProperty);
            }

            var section = lastSection ? Sections.FindLast(sectionName) : Sections.Find(sectionName);
            if (section == null)
            {
                return nullValue;
            }

            return section.GetPropertyValue(key, nullValue, lastProperty);
        }

        public void SetPropertyValue<T>(string sectionName, string key, T value, bool addNew = true, bool lastSection = false, bool lastProperty = false)
        {
            if (string.IsNullOrEmpty(sectionName))
            {
                Global.SetPropertyValue(key, value, addNew, lastProperty);
            }

            var section = lastSection ? Sections.FindLast(sectionName) : Sections.Find(sectionName);
            if (section == null)
            {
                return;
            }

            section.SetPropertyValue(key, value, addNew, lastProperty);
        }

        public IniData Clone()
        {
            return new IniData(this);
        }

        public void Clear()
        {
            Global.Clear();
            Sections.Clear();
        }
    }
}
