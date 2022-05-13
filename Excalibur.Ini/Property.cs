using System;
using System.Collections.Generic;

namespace Excalibur.Ini
{
    /// <summary>
    /// Key-Value属性
    /// </summary>
    public class Property : ICloneable<Property>
    {
        /// <summary>
        /// 属性关键字
        /// </summary>
        public string Key { get; set; }
        
        /// <summary>
        /// 属性值
        /// </summary>
        public string Value { get; set; }

        private List<string> _comments;
        /// <summary>
        /// 属性前的注释内容
        /// </summary>
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

        /// <summary>
        /// 值后面的注释，比如：Key=Value ;comment
        /// </summary>
        public string CommentAfterValue { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="key">属性关键字</param>
        /// <param name="value">属性值</param>
        /// <exception cref="ArgumentException">参数异常：关键字为空</exception>
        public Property(string key, string value = "")
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("key can not be null or empty", nameof(Key));

            Key = key;
            Value = value;
        }

        /// <summary>
        /// 复制属性构造函数
        /// </summary>
        /// <param name="other">其他属性</param>
        public Property(Property other)
        {
            Key = other.Key;
            Value = other.Value;
            Comments = other.Comments;
            CommentAfterValue = other.CommentAfterValue;
        }

        /// <summary>
        /// 复制当前属性
        /// </summary>
        /// <returns></returns>
        public Property Clone()
        {
            return new Property(this);
        }
    }
}
