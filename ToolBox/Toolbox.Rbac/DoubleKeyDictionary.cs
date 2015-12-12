using System.Collections.Generic;

namespace Toolbox.Rbac
{
    public class DoubleKeyDictionary<K1, K2, V> : Dictionary<K1, IDictionary<K2, V>>
    {
        private IEqualityComparer<K2> keyComparer;
        public DoubleKeyDictionary(IEqualityComparer<K1> key1Comparer, IEqualityComparer<K2> key2Comparer) : base(key1Comparer)
        {
            keyComparer = key2Comparer;
        }
        public DoubleKeyDictionary() { }
        public V this[K1 key1, K2 key2]
        {
            get { return this[key1][key2]; }
            set
            {
                this[key1][key2] = value;
            }
        }

        public void Add(K1 k1, K2 k2, V v)
        {
            if (this[k1] == null)
            {
                this[k1] = new Dictionary<K2, V>(keyComparer);
            }
            this[k1].Add(k2, v);
        }
    }
}
