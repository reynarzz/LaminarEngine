using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace Engine.Utils
{
    public class BiDictionary<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>>
    {
        [SerializedField] private Dictionary<TKey, TValue> _keyToValue = new();
        [SerializedField] private Dictionary<TValue, TKey> _valueToKey = new();

        public int Count => _keyToValue.Count;

        public void Add(TKey key, TValue value)
        {
            _keyToValue[key] = value;
            _valueToKey[value] = key;
        }

        public bool RemoveByKey(TKey key)
        {
            if (!_keyToValue.TryGetValue(key, out TValue value))
                return false;

            _keyToValue.Remove(key);
            _valueToKey.Remove(value);
            return true;
        }

        public bool RemoveByValue(TValue value)
        {
            if (!_valueToKey.TryGetValue(value, out TKey key))
                return false;

            _valueToKey.Remove(value);
            _keyToValue.Remove(key);
            return true;
        }

        public TValue GetByKey(TKey key) { return _keyToValue[key]; }
        public TKey GetByValue(TValue value) { return _valueToKey[value]; }
        public bool TryGetByKey(TKey key, out TValue value) { return _keyToValue.TryGetValue(key, out value); }
        public bool TryGetByValue(TValue value, out TKey key) { return _valueToKey.TryGetValue(value, out key); }
        public bool ContainsKey(TKey key) { return _keyToValue.ContainsKey(key); }
        public bool ContainsValue(TValue value) { return _valueToKey.ContainsKey(value); }

        public void Clear()
        {
            _keyToValue.Clear();
            _valueToKey.Clear();
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() { return _keyToValue.GetEnumerator(); }
        IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }
    }

}
