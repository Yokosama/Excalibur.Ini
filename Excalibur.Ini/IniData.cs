using System.Collections.Generic;

namespace Excalibur.Ini
{
    /// <summary>
    /// ini数据内容
    /// </summary>
    public class IniData : ICloneable<IniData>
    {
        /// <summary>
        /// 全局节点名称
        /// </summary>
        public const string GlobalSection = "global";

        /// <summary>
        /// 全局节点
        /// </summary>
        public Section Global { get; protected set; }
        /// <summary>
        /// 非全局节点集合
        /// </summary>
        public KeyValues<Section> Sections { get; protected set; }

        /// <summary>
        /// ini内容格式字段
        /// </summary>
        protected IniScheme _scheme;
        /// <summary>
        /// ini内容格式
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

        /// <summary>
        /// ini内容的解析配置字段
        /// </summary>
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

        /// <summary>
        /// 非全局节点集合数量
        /// </summary>
        public int SectionCount => Sections.Length;

        /// <summary>
        /// 默认构造函数
        /// </summary>
        public IniData()
        {
            Global = new Section(GlobalSection);
            Sections = new KeyValues<Section>();
        }

        /// <summary>
        /// 带ini格式的构造函数
        /// </summary>
        /// <param name="scheme"></param>
        public IniData(IniScheme scheme)
        {
            Global = new Section(GlobalSection);
            Sections = new KeyValues<Section>();
            _scheme = scheme.Clone();
        }

        /// <summary>
        /// 复制其他IniData的构造函数
        /// </summary>
        /// <param name="other"></param>
        public IniData(IniData other)
        {
            Global = other.Global.Clone();
            Sections = other.Sections.Clone();
            Scheme = other.Scheme;
            ParserConfiguration = other.ParserConfiguration;
        }

        /// <summary>
        /// 添加节点
        /// </summary>
        /// <param name="sectionName">节点名称</param>
        /// <param name="canRepeat">是否可重复</param>
        /// <returns>新添加的节点</returns>
        public Section Add(string sectionName, bool canRepeat = true)
        {
            if (string.IsNullOrEmpty(sectionName)) return null;

            var section = new Section(sectionName);
            Sections.Add(sectionName, section, canRepeat);
            return section;
        }

        /// <summary>
        /// 添加新节点
        /// </summary>
        /// <param name="section">节点</param>
        /// <param name="canRepeat">是否可重复</param>
        /// <returns>当前添加的节点</returns>
        public Section Add(Section section, bool canRepeat = true)
        {
            if (section == null) return null;
             
            Sections.Add(section.Name, section, canRepeat);
            return section;
        }

        /// <summary>
        /// 通过节点名称获取节点
        /// </summary>
        /// <param name="sectionName">节点名称</param>
        /// <param name="lastSection">是否逆序查找</param>
        /// <returns>查找到的节点，未找到为null</returns>
        public Section GetSection(string sectionName, bool lastSection = false)
        {
            if (string.IsNullOrEmpty(sectionName))
            {
                return null;
            }

            return lastSection ? Sections.FindLast(sectionName) : Sections.Find(sectionName);
        }

        /// <summary>
        /// 通过节点名称获取所有节点
        /// </summary>
        /// <param name="sectionName">节点名称</param>
        /// <returns>查找到的所有节点</returns>
        public List<Section> GetSections(string sectionName)
        {
            if (string.IsNullOrEmpty(sectionName))
            {
                return null;
            }

            return Sections.FindAll(sectionName);
        }

        /// <summary>
        /// 是否包含节点
        /// </summary>
        /// <param name="sectionName">节点名称</param>
        /// <returns>true：存在节点；false：不存在</returns>
        public bool ContainsSection(string sectionName)
        {
            return Sections.ContainsKey(sectionName);
        }

        /// <summary>
        /// 获取属性的字符串值
        /// </summary>
        /// <param name="sectionName">节点名称</param>
        /// <param name="key">属性关键字</param>
        /// <param name="nullValue">无属性时的默认值</param>
        /// <param name="lastSection"></param>
        /// <param name="lastProperty"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 获取转换为类型T后的值
        /// </summary>
        /// <typeparam name="T">需转换的值类型</typeparam>
        /// <param name="sectionName">节点名称</param>
        /// <param name="key">属性关键字</param>
        /// <param name="nullValue">无属性或转换失败时的默认值</param>
        /// <param name="lastSection">逆序查找节点</param>
        /// <param name="lastProperty">逆序查找属性</param>
        /// <returns>转换为类型T后的值</returns>
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

        /// <summary>
        /// 设置属性值
        /// </summary>
        /// <typeparam name="T">值类型</typeparam>
        /// <param name="sectionName">节点名称</param>
        /// <param name="key">属性关键字</param>
        /// <param name="value">属性值</param>
        /// <param name="addNew">未找到是否添加新属性</param>
        /// <param name="lastSection">逆序查找节点</param>
        /// <param name="lastProperty">逆序查找属性</param>
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

        /// <summary>
        /// 复制IniData
        /// </summary>
        /// <returns></returns>
        public IniData Clone()
        {
            return new IniData(this);
        }

        /// <summary>
        /// 清空节点内容
        /// </summary>
        public void Clear()
        {
            Global.Clear();
            Sections.Clear();
        }
    }
}
