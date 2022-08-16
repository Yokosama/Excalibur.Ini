using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Excalibur.Ini
{
    /// <summary>
    /// Ini节点类
    /// </summary>
    public class Section : ICloneable<Section>
    {
        private readonly IEqualityComparer<string> _searchComparer;

        private string _name;
        /// <summary>
        /// 节点名称
        /// </summary>
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
        /// <summary>
        /// 节点前的注释
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
                if (value != null) _comments.AddRange(value);
            }
        }

        /// <summary>
        /// 属性名称后的注释，比如：[SectionName] ;comment
        /// </summary>
        public string CommentAfterSectionName { get; set; }

        /// <summary>
        /// 节点的属性集合
        /// </summary>
        public KeyValues<Property> Properties { get; set; }

        /// <summary>
        /// 属性数量
        /// </summary>
        public int Count { get { return Properties.Length; } }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="name">节点名称</param>
        public Section(string name) : this(name, EqualityComparer<string>.Default)
        {
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="name">节点名称</param>
        /// <param name="searchComparer">节点名称比较类</param>
        /// <exception cref="ArgumentException">参数异常：节点名称为空</exception>
        public Section(string name, IEqualityComparer<string> searchComparer)
        {
            _searchComparer = searchComparer;

            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("Section name can not be null or empty", nameof(name));

            Properties = new KeyValues<Property>(_searchComparer);
            Name = name;
        }

        /// <summary>
        /// 复制对象构造函数
        /// </summary>
        /// <param name="other">其他节点</param>
        /// <param name="searchComparer">参数异常：节点名称为空</param>
        public Section(Section other, IEqualityComparer<string> searchComparer = null)
        {
            Name = other.Name;

            _searchComparer = searchComparer;
            Comments = other.Comments;
            Properties = new KeyValues<Property>(other.Properties, searchComparer ?? other._searchComparer);
        }

        /// <summary>
        /// 添加属性
        /// </summary>
        /// <param name="key">关键字</param>
        /// <param name="value">值</param>
        /// <param name="canRepeat">属性关键字是否能重复</param>
        /// <returns>true：添加成功；false：添加失败</returns>
        public bool Add(string key, string value = "", bool canRepeat = true)
        {
            return Properties.Add(key, new Property(key, value), canRepeat);
        }

        /// <summary>
        /// 添加属性对象
        /// </summary>
        /// <param name="property">属性对象</param>
        /// <param name="canRepeat">属性关键字是否能重复</param>
        /// <returns>true：添加成功；false：添加失败</returns>
        public bool Add(Property property, bool canRepeat = true)
        {
            if (property == null) return false;
            return Properties.Add(property.Key, property, canRepeat);
        }

        /// <summary>
        /// 插入属性
        /// </summary>
        /// <param name="key">关键字</param>
        /// <param name="value">值</param>
        /// <param name="beforeKey">在某个属性前，空直接插到最后面</param>
        /// <param name="lastBeforeKey">逆序查找beforeKey的对象</param>
        /// <param name="canRepeat">是否可以重复</param>
        /// <returns>true：插入成功；false：插入失败</returns>
        public bool Insert(string key, string value = "", string beforeKey = "", bool lastBeforeKey = false, bool canRepeat = true)
        {
            return Properties.Insert(key, new Property(key, value), beforeKey, lastBeforeKey, canRepeat);
        }

        /// <summary>
        /// 插入属性
        /// </summary>
        /// <param name="property">属性对象</param>
        /// <param name="beforeKey">在某个属性后，空直接插到最后面</param>
        /// <param name="lastBeforeKey">逆序查找beforeKey的对象</param>
        /// <param name="canRepeat">是否可以重复</param>
        /// <returns>true：插入成功；false：插入失败</returns>
        public bool Insert(Property property, string beforeKey = "", bool lastBeforeKey = false, bool canRepeat = true)
        {
            return Properties.Insert(property.Key, property, beforeKey, lastBeforeKey, canRepeat);
        }

        /// <summary>
        /// 获取属性对象
        /// </summary>
        /// <param name="key">属性关键字</param>
        /// <param name="last">是否逆序查找</param>
        /// <returns>属性对象</returns>
        public Property GetProperty(string key, bool last = false)
        {
            if (string.IsNullOrEmpty(key))
            {
                return null;
            }

            return last ? Properties.FindLast(key) : Properties.Find(key);
        }

        /// <summary>
        /// 获取相同关键字的属性集合
        /// </summary>
        /// <param name="key">属性关键字</param>
        /// <returns>属性集合</returns>
        public List<Property> GetProperties(string key)
        {
            return Properties.FindAll(key);
        }

        /// <summary>
        /// 获取属性的字符串值
        /// </summary>
        /// <param name="key">关键字</param>
        /// <param name="nullValue">默认值</param>
        /// <param name="last">是否逆序查找属性</param>
        /// <returns>属性的字符串值</returns>
        public string GetPropertyRawValue(string key, string nullValue, bool last = false)
        {
            var find = last ? Properties.FindLast(key) : Properties.Find(key);
            if (find == null)
            {
                return nullValue;
            }

            return find.Value;
        }

        /// <summary>
        /// 获取属性的字符串值
        /// </summary>
        /// <typeparam name="T">需转换的类型</typeparam>
        /// <param name="key">关键字</param>
        /// <param name="nullValue">默认值</param>
        /// <param name="last">是否逆序查找属性</param>
        /// <returns>转换后的属性值</returns>
        public T GetPropertyValue<T>(string key, T nullValue, bool last = false)
        {
            var find = last ? Properties.FindLast(key) : Properties.Find(key);
            if (find == null)
            {
                return nullValue;
            }

            return ConvertValue(find.Value, nullValue);
        }

        /// <summary>
        /// 设置属性值
        /// </summary>
        /// <typeparam name="T">值类型</typeparam>
        /// <param name="key">关键字</param>
        /// <param name="value">值</param>
        /// <param name="comments">属性注释</param>
        /// <param name="addNew">找不到是否新增</param>
        /// <param name="last">是否逆序查找属性</param>
        public void SetPropertyValue<T>(string key, T value, List<string> comments = null, bool addNew = true, bool last = false)
        {
            var v = Convert.ToString(value);
            var find =  last ? Properties.FindLast(key) : Properties.Find(key);
            if (find == null)
            {
                if (addNew)
                {
                    var p = new Property(key, v);
                    if(comments != null) { p.Comments = comments; }
                    Properties.Add(key, p);
                }
                return;
            }

            find.Value = v;
            if (comments != null) { find.Comments = comments; }
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

        /// <summary>
        /// 清空属性和节点注释
        /// </summary>
        public void Clear()
        {
            ClearProperties();
            ClearComments();
        }

        /// <summary>
        /// 清空注释，包括节点后注释
        /// </summary>
        public void ClearComments()
        {
            Comments.Clear();
            foreach (var property in Properties)
            {
                property.Comments.Clear();
                property.CommentAfterValue = "";
            }
            CommentAfterSectionName = "";
        }

        /// <summary>
        /// 清空属性
        /// </summary>
        public void ClearProperties()
        {
            Properties.Clear();
        }

        /// <summary>
        /// 复制当前节点
        /// </summary>
        /// <returns></returns>
        public Section Clone()
        {
            return new Section(this);
        }
    }
}
