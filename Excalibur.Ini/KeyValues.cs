using System.Collections;
using System.Collections.Generic;

namespace Excalibur.Ini
{
    public class KeyValues<T> : ICloneable<KeyValues<T>>, IEnumerable<T> where T : class, ICloneable<T>
    {
        private readonly List<T> _items;
        private readonly Dictionary<string, List<T>> _dicItems;
        private readonly IEqualityComparer<string> _comparer;

        public int Length => _items.Count;

        public KeyValues() : this(EqualityComparer<string>.Default) { }

        public KeyValues(IEqualityComparer<string> comparer)
        {
            _comparer = comparer;
            _items = new List<T>();
            _dicItems = new Dictionary<string, List<T>>(comparer);
        }

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

            return false;
        }

        public T Find(string key)
        {
            if (!_dicItems.ContainsKey(key) || _dicItems[key].Count == 0)
            {
                return null;
            }
            
            return _dicItems[key][0];
        }

        public T FindLast(string key)
        {
            if (!_dicItems.ContainsKey(key) || _dicItems[key].Count == 0)
            {
                return null;
            }

            return _dicItems[key][_dicItems[key].Count - 1];
        }

        public List<T> FindAll(string key)
        {
            if (!_dicItems.ContainsKey(key))
            {
                return null;
            }
            return _dicItems[key];
        }

        public bool ContainsKey(string key)
        {
            return _dicItems.ContainsKey(key);
        }

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

        public int Count(string key)
        {
            if (_dicItems.ContainsKey(key))
            {
                return _dicItems[key].Count;
            }

            return 0;
        }

        public void Clear()
        {
            _items.Clear();
            _dicItems.Clear();
        }

        public KeyValues<T> Clone()
        {
            return new KeyValues<T>(this, _comparer);
        }

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
