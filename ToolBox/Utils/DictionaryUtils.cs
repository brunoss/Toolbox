using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

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

        public static V AddOrUpdate<K, V>(this IDictionary<K, V> dictionary, K key, V value, Func<V, V> newValue)
        {
            if (dictionary.ContainsKey(key))
            {
                return dictionary[key] = newValue(dictionary[key]);
            }
            return dictionary[key] = value;
        }

        public static V GetEntry<V>(this IDictionary<Type, V> dictionary, Type key, bool verifyInterfaces = false)
        {
            if (verifyInterfaces)
            {
                var interfaces = key.GetTypeInfo().ImplementedInterfaces.ToArray();
                var entry = dictionary.Keys.ContainsOneOf(interfaces);
                if (entry != null)
                {
                    return dictionary[entry];
                }
            }
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