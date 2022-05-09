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
        /// ini content comment start strings
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
        public string SectionStartString
        {
            get => string.IsNullOrWhiteSpace(_sectionStartString) ? "[" : _sectionStartString;
            set => _sectionStartString = value?.Trim();
        }

        private string _sectionEndString = "]";
        public string SectionEndString
        {
            get => string.IsNullOrWhiteSpace(_sectionEndString) ? "]" : _sectionEndString;
            set => _sectionEndString = value?.Trim();
        }

        private string _propertyAssignmentString = "=";
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

        public IniScheme(IniScheme other)
        {
            CommentStrings = other.CommentStrings;
            SectionStartString = other.SectionStartString;
            SectionEndString = other.SectionEndString;
            PropertyAssignmentString = other.PropertyAssignmentString;
        }

        public IniScheme Clone()
        {
            return new IniScheme(this);
        }
    }
}
