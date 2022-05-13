using System.Collections.Generic;
using System.Linq;

namespace Excalibur.Ini
{
    /// <summary>
    /// ini content format
    /// </summary>
    public class IniScheme : ICloneable<IniScheme>
    {
        private readonly List<string> _commentStrings = new List<string> { ";" };
        /// <summary>
        /// ini content comment start strings. default value { ";" }
        /// </summary>
        public List<string> CommentStrings
        {
            get
            {
                if(_commentStrings.Count == 0)
                {
                    _commentStrings.Add(";");
                }
                return _commentStrings;
            }
            set
            {
                if(value == null || value.Count == 0)
                {
                    return;
                }
                _commentStrings.Clear();
                _commentStrings.AddRange(value.Select(x => x.Trim()));
            }
        }

        private string _sectionStartString = "[";
        /// <summary>
        /// 节点起始字符
        /// </summary>
        public string SectionStartString
        {
            get => string.IsNullOrWhiteSpace(_sectionStartString) ? "[" : _sectionStartString;
            set => _sectionStartString = value?.Trim();
        }

        private string _sectionEndString = "]";
        /// <summary>
        /// 节点结束字符
        /// </summary>
        public string SectionEndString
        {
            get => string.IsNullOrWhiteSpace(_sectionEndString) ? "]" : _sectionEndString;
            set => _sectionEndString = value?.Trim();
        }

        private string _propertyAssignmentString = "=";
        /// <summary>
        /// 属性键值赋值符
        /// </summary>
        public string PropertyAssignmentString
        {
            get => string.IsNullOrWhiteSpace(_propertyAssignmentString) ? "=" : _propertyAssignmentString;
            set => _propertyAssignmentString = value?.Trim();
        }

        /// <summary>
        /// Default ini content format
        /// <para>Comment：';'</para>
        /// <para>Section define：'[' ']'</para>
        /// <para>Property assignment：'='</para>
        /// </summary>
        public IniScheme()
        {
        }

        /// <summary>
        /// 复制构造函数
        /// </summary>
        /// <param name="other"></param>
        public IniScheme(IniScheme other)
        {
            CommentStrings = other.CommentStrings;
            SectionStartString = other.SectionStartString;
            SectionEndString = other.SectionEndString;
            PropertyAssignmentString = other.PropertyAssignmentString;
        }

        /// <summary>
        /// 复制当前对象
        /// </summary>
        /// <returns></returns>
        public IniScheme Clone()
        {
            return new IniScheme(this);
        }
    }
}
