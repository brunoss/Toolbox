using System;
using System.Collections.Generic;

namespace ToolBox
{
    public static class DictionaryUtils
    {
        public static V TryGetOrValue<K, V>(this IDictionary<K, V> dictionary, K key, V value)
        {
            V ret;
            if (dictionary.TryGetValue(key, out ret))
                return ret;
            return value;
        }

        public static V TryGetOrAdd<K, V>(this IDictionary<K, V> dictionary, K key, V value)
        {
            V ret;
            if (dictionary.TryGetValue(key, out ret))
                return ret;
            dictionary.Add(key, value);
            return value;
        }

        public static V GetEntry<V>(this IDictionary<Type, V> dictionary, Type key)
        {
            for (; key != null; key = key.BaseType)
            {
                V value;
                if (dictionary.TryGetValue(key, out value))
                {
                    return value;
                }
            }
            return default(V);
        }
    }
}