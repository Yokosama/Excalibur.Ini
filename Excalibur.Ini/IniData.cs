using System;
using System.Collections.Generic;
using System.Text;

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
    }
}
