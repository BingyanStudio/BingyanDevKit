using System.Collections;
using System.Collections.Generic;

namespace Bingyan
{
    public class BiDictionary<K, V> : IDictionary<K, V>
    {
        private readonly Dictionary<K, V> forward = new();
        private readonly Dictionary<V, K> backward = new();

        public V this[K key]
        {
            get => forward[key];
            set
            {
                forward[key] = value;
                backward[value] = key;
            }
        }

        public K this[V key]
        {
            get => backward[key];
            set
            {
                backward[key] = value;
                forward[value] = key;
            }
        }

        public ICollection<K> Keys => forward.Keys;
        public ICollection<V> Values => forward.Values;
        public int Count => forward.Count;
        public bool IsReadOnly => false;

        public void Add(K key, V value)
        {
            forward.Add(key, value);
            backward.Add(value, key);
        }

        public void Add(V key, K value)
        {
            backward.Add(key, value);
            forward.Add(value, key);
        }

        public void Add(KeyValuePair<K, V> item)
        {
            forward.Add(item.Key, item.Value);
            backward.Add(item.Value, item.Key);
        }

        public void Clear()
        {
            forward.Clear();
            backward.Clear();
        }

        public bool Contains(KeyValuePair<K, V> item) => forward.ContainsKey(item.Key);

        public bool ContainsKey(K key) => forward.ContainsKey(key);

        public void CopyTo(KeyValuePair<K, V>[] array, int arrayIndex)
        {
            foreach (var item in forward)
                array[arrayIndex++] = item;
        }

        public void Dispose() { }

        public IEnumerator<KeyValuePair<K, V>> GetEnumerator() => forward.GetEnumerator();

        public bool Remove(K key)
        {
            if (!forward.TryGetValue(key, out var val)) return false;
            backward.Remove(val);
            return forward.Remove(key);
        }

        public bool Remove(V key)
        {
            if (!backward.TryGetValue(key, out var val)) return false;
            forward.Remove(val);
            return backward.Remove(key);
        }

        public bool Remove(KeyValuePair<K, V> item)
        {
            if (!forward.TryGetValue(item.Key, out var val)) return false;
            backward.Remove(val);
            return forward.Remove(item.Key);
        }

        public bool TryGetValue(K key, out V value)
            => forward.TryGetValue(key, out value);

        public bool TryGetValue(V key, out K value)
            => backward.TryGetValue(key, out value);

        IEnumerator IEnumerable.GetEnumerator() => forward.GetEnumerator();
    }
}