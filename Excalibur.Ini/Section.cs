using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Excalibur.Ini
{
    public class Section : ICloneable<Section>
    {
        private readonly IEqualityComparer<string> _searchComparer;

        public string _name;
        public string Name
        {
            get
            {
                return _name;
            }

            set
            {
                if (!string.IsNullOrEmpty(value))
                    _name = value;
            }
        }

        private List<string> _comments;
        public List<string> Comments
        {
            get
            {
                if (_comments == null)
                {
                    _comments = new List<string>();
                }
                return _comments;
            }
            set
            {
                if (_comments == null)
                {
                    _comments = new List<string>();
                }
                _comments.Clear();
                _comments.AddRange(value);
            }
        }

        public string CommentAfterSectionName { get; set; }

        public KeyValues<Property> Properties { get; set; }

        public int Count { get { return Properties.Length; } }

        public Section(string name) : this(name, EqualityComparer<string>.Default)
        {
        }

        public Section(string name, IEqualityComparer<string> searchComparer)
        {
            _searchComparer = searchComparer;

            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("Section name can not be null or empty", nameof(name));

            Properties = new KeyValues<Property>(_searchComparer);
            Name = name;
        }

        public Section(Section other, IEqualityComparer<string> searchComparer = null)
        {
            Name = other.Name;

            _searchComparer = searchComparer;
            Comments = other.Comments;
            Properties = new KeyValues<Property>(other.Properties, searchComparer ?? other._searchComparer);
        }

        public bool Add(string key, string value = "", bool canRepeat = true)
        {
            return Properties.Add(key, new Property(key, value), canRepeat);
        }

        public bool Add(Property property, bool canRepeat = true)
        {
            if (property == null) return false;
            return Properties.Add(property.Key, property, canRepeat);
        }

        public Property GetProperty(string key, bool last = false)
        {
            if (string.IsNullOrEmpty(key))
            {
                return null;
            }

            return last ? Properties.FindLast(key) : Properties.Find(key);
        }

        public List<Property> GetProperties(string key)
        {
            return Properties.FindAll(key);
        }

        public string GetPropertyRawValue(string key, string nullValue, bool last = false)
        {
            var find = last ? Properties.FindLast(key) : Properties.Find(key);
            if (find == null)
            {
                return nullValue;
            }

            return find.Value;
        }

        public T GetPropertyValue<T>(string key, T nullValue, bool last = false)
        {
            var find = last ? Properties.FindLast(key) : Properties.Find(key);
            if (find == null)
            {
                return nullValue;
            }

            return ConvertValue(find.Value, nullValue);
        }

        public void SetPropertyValue<T>(string key, T value, bool addNew = true, bool last = false)
        {
            var v = Convert.ToString(value);
            var find =  last ? Properties.FindLast(key) : Properties.Find(key);
            if (find == null)
            {
                if (addNew) Properties.Add(key, new Property(key, v));
                return;
            }

            find.Value = v;
        }

        private T ConvertValue<T>(string value, T nullValue)
        {
            try
            {
                var type = typeof(T);

                if (type.IsEnum)
                {
                    return (T)Enum.Parse(typeof(T), value);
                }
                var converter = TypeDescriptor.GetConverter(type);
                if (converter != null && converter.CanConvertTo(type))
                {
                    return (T)converter.ConvertFromString(value);
                }
            }
            catch
            {
            }
            return nullValue;
        }

        public void Clear()
        {
            ClearProperties();
            ClearComments();
        }

        public void ClearComments()
        {
            Comments.Clear();
            foreach (var property in Properties)
            {
                property.Comments.Clear();
            }
        }

		public void ClearProperties()
        {
            Properties.Clear();
        }

        public Section Clone()
        {
            return new Section(this);
        }
    }
}
