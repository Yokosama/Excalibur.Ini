using System;
using System.Collections.Generic;

namespace Excalibur.Ini
{
    public class Property : ICloneable<Property>
    {
        public string Key { get; set; }
        
        public string Value { get; set; }

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

        public string CommentAfterValue { get; set; }

        public Property(string key, string value = "")
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("key can not be null or empty", nameof(Key));

            Key = key;
            Value = value;
        }

        public Property(Property other)
        {
            Key = other.Key;
            Value = other.Value;
            Comments = other.Comments;
            CommentAfterValue = other.CommentAfterValue;
        }

        public Property Clone()
        {
            return new Property(this);
        }
    }
}
