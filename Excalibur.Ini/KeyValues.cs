using System.Collections;
using System.Collections.Generic;

namespace Excalibur.Ini
{
    /// <summary>
    /// 用于快速查找的Key-Values结构
    /// </summary>
    /// <typeparam name="T">存储的对象的类型</typeparam>
    public class KeyValues<T> : ICloneable<KeyValues<T>>, IEnumerable<T> where T : class, ICloneable<T>
    {
        private readonly List<T> _items;
        private readonly Dictionary<string, List<T>> _dicItems;
        private readonly IEqualityComparer<string> _comparer;

        /// <summary>
        /// 存储的对象数量
        /// </summary>
        public int Length => _items.Count;

        /// <summary>
        /// 构造函数
        /// </summary>
        public KeyValues() : this(EqualityComparer<string>.Default) { }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="comparer">关键字比较方式</param>
        public KeyValues(IEqualityComparer<string> comparer)
        {
            _comparer = comparer;
            _items = new List<T>();
            _dicItems = new Dictionary<string, List<T>>(comparer);
        }

        /// <summary>
        /// 复制对象构造函数
        /// </summary>
        /// <param name="other">其他对象</param>
        /// <param name="comparer">关键字比较方式</param>
        public KeyValues(KeyValues<T> other, IEqualityComparer<string> comparer) : this(comparer)
        {
            _items.Clear();
            _dicItems.Clear();

            var otherItemsCount = other._items.Count;
            for (int i = 0; i < otherItemsCount; i++)
            {
                _items.Add(default);
            }
            foreach (var dicItem in other._dicItems)
            {
                var key = dicItem.Key;
                var items = dicItem.Value;
                if (!_dicItems.ContainsKey(key)) _dicItems[key] = new List<T>();

                foreach (var item in items)
                {
                    var clone = item.Clone();
                    var index = other._items.IndexOf(item);
                    _items[index] = clone;
                    _dicItems[key].Add(clone);
                }
            }
        }

        /// <summary>
        /// 添加对象
        /// </summary>
        /// <param name="key">关键字</param>
        /// <param name="item">需添加对象</param>
        /// <param name="canRepeat">是否可以重复添加</param>
        /// <returns>true：添加成功；false：不可重复添加或key为空</returns>
        public bool Add(string key, T item, bool canRepeat = true)
        {
            if (string.IsNullOrEmpty(key))
            {
                return false;
            }

            var exists = _dicItems.ContainsKey(key);
            if(exists && !canRepeat)
            {
                return false;
            }

            _items.Add(item);
            if (!exists) _dicItems[key] = new List<T>();
            _dicItems[key].Add(item);

            return true;
        }

        /// <summary>
        /// 在某个对象前插入对象
        /// </summary>
        /// <param name="key">关键字</param>
        /// <param name="item">需插入的对象</param>
        /// <param name="beforeKey">在对象之前</param>
        /// <param name="lastBeforeKey">是否逆序查找对象</param>
        /// <param name="canRepeat">是否支持重复对象</param>
        /// <returns>true：插入成功；false：不可重复添加或key为空</returns>
        public bool Insert(string key, T item, string beforeKey, bool lastBeforeKey, bool canRepeat = true)
        {
            if (string.IsNullOrEmpty(key))
            {
                return false;
            }

            var exists = _dicItems.ContainsKey(key);
            if(exists && !canRepeat)
            {
                return false;
            }

            var afterItem = lastBeforeKey ? FindLast(beforeKey) : Find(beforeKey);
            if (afterItem == null)
            {
                _items.Add(item);
            }
            else
            {
                var index = _items.IndexOf(afterItem);
                _items.Insert(index, item);
            }
            
            if (!exists) _dicItems[key] = new List<T>();
            _dicItems[key].Add(item);

            return true;
        }

        /// <summary>
        /// 在某个下标插入对象
        /// </summary>
        /// <param name="key">关键字</param>
        /// <param name="item">需插入的对象</param>
        /// <param name="index">下标</param>
        /// <param name="canRepeat">是否支持重复对象</param>
        /// <returns>true：插入成功；false：不可重复添加或下标不在有效范围内</returns>
        public bool Insert(string key, T item, int index, bool canRepeat = true)
        {
            if (string.IsNullOrEmpty(key))
            {
                return false;
            }

            var exists = _dicItems.ContainsKey(key);
            if (exists && !canRepeat)
            {
                return false;
            }

            if(index < 0 || index >= _items.Count)
            {
                return false;
            }

            _items.Insert(index, item);

            if (!exists) _dicItems[key] = new List<T>();
            _dicItems[key].Add(item);

            return true;
        }

        /// <summary>
        /// 获取对象所处的下标
        /// </summary>
        /// <param name="item">对象</param>
        /// <returns>对象所处的下标，-1：不在对象集合内</returns>
        public int IndexOf(T item)
        {
            return _items.IndexOf(item);
        }

        /// <summary>
        /// 通过关键字查找对象
        /// </summary>
        /// <param name="key">关键字</param>
        /// <returns>返回查找到的对象；未找到返回null</returns>
        public T Find(string key)
        {
            if (!_dicItems.ContainsKey(key) || _dicItems[key].Count == 0)
            {
                return null;
            }
            
            return _dicItems[key][0];
        }

        /// <summary>
        /// 通过关键字逆序查找对象
        /// </summary>
        /// <param name="key">关键字</param>
        /// <returns>返回查找到的对象；未找到返回null</returns>
        public T FindLast(string key)
        {
            if (!_dicItems.ContainsKey(key) || _dicItems[key].Count == 0)
            {
                return null;
            }

            return _dicItems[key][_dicItems[key].Count - 1];
        }

        /// <summary>
        /// 通过关键字查找对象集合
        /// </summary>
        /// <param name="key">关键字</param>
        /// <returns>返回查找到的对象集合；未找到返回null</returns>
        public List<T> FindAll(string key)
        {
            if (!_dicItems.ContainsKey(key))
            {
                return null;
            }
            return _dicItems[key];
        }

        /// <summary>
        /// 是否存在关键字
        /// </summary>
        /// <param name="key">关键字</param>
        /// <returns>true：存在；false：不存在</returns>
        public bool ContainsKey(string key)
        {
            return _dicItems.ContainsKey(key);
        }

        /// <summary>
        /// 移除关键字的某个对象
        /// </summary>
        /// <param name="key">关键字</param>
        /// <param name="item">需移除的对象</param>
        /// <returns>true：移除成功；false：移除失败或对象不在集合内</returns>
        public bool Remove(string key, T item)
        {
            if(_dicItems.ContainsKey(key) && _dicItems[key].Count > 0)
            {
                var items = _dicItems[key];
                items.Remove(item);
                if(items.Count == 0)
                {
                    _dicItems.Remove(key);
                }
                return _items.Remove(item);
            }
            return false;
        }

        /// <summary>
        /// 移除顺序查找下的第一个符合关键字的对象
        /// </summary>
        /// <param name="key">关键字</param>
        /// <returns>true：移除成功；false：移除失败或不存在关键字对象</returns>
        public bool RemoveFirst(string key)
        {
            if (_dicItems.ContainsKey(key) && _dicItems[key].Count > 0)
            {
                var items = _dicItems[key];
                var item = items[0];
                items.Remove(item);
                if (items.Count == 0)
                {
                    _dicItems.Remove(key);
                }
                return _items.Remove(item);
            }
            return false;
        }

        /// <summary>
        /// 移除逆序查找下的第一个符合关键字的对象
        /// </summary>
        /// <param name="key">关键字</param>
        /// <returns>true：移除成功；false：移除失败或不存在关键字对象</returns>
        public bool RemoveLast(string key)
        {
            if (_dicItems.ContainsKey(key) && _dicItems[key].Count > 0)
            {
                var items = _dicItems[key];
                var item = items[items.Count - 1];
                items.Remove(item);
                if (_dicItems[key].Count == 0)
                {
                    _dicItems.Remove(key);
                }
                return _items.Remove(item);
            }
            return false;
        }

        /// <summary>
        /// 移除符合关键字的所有对象
        /// </summary>
        /// <param name="key">关键字</param>
        /// <returns>true：移除成功；false：移除失败或不存在关键字对象</returns>
        public bool RemoveAll(string key)
        {
            if (_dicItems.ContainsKey(key))
            {
                var items = _dicItems[key];
                foreach (var item in items)
                {
                    _items.Remove(item);
                }
                return _dicItems.Remove(key);
            }
            return false;
        }

        /// <summary>
        /// 获取符合关键字的对象数量
        /// </summary>
        /// <param name="key">关键字</param>
        /// <returns>0：不存在关键字对象；否则返回对应的数量</returns>
        public int Count(string key)
        {
            if (_dicItems.ContainsKey(key))
            {
                return _dicItems[key].Count;
            }

            return 0;
        }

        /// <summary>
        /// 清空集合
        /// </summary>
        public void Clear()
        {
            _items.Clear();
            _dicItems.Clear();
        }

        /// <summary>
        /// 复制当前对象
        /// </summary>
        /// <returns>复制后的对象</returns>
        public KeyValues<T> Clone()
        {
            return new KeyValues<T>(this, _comparer);
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        public IEnumerator<T> GetEnumerator()
        {
            foreach (var item in _items)
            {
                yield return item;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _items.GetEnumerator();
        }
    }
}
