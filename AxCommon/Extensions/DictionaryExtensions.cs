using System;
using System.Collections.Generic;

namespace Aximo
{

    public static class DictionaryExtensions
    {

        public static bool TryAdd<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, TValue value)
        {
            lock (dict)
            {
                if (dict.ContainsKey(key))
                    return false;

                dict.Add(key, value);
                return true;
            }
        }

        public static void Set<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, TValue value)
        {
            lock (dict)
            {
                if (dict.ContainsKey(key))
                    dict[key] = value;
                else
                    dict.Add(key, value);
            }
        }

        public static void Set<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, Func<TValue> getValue)
        {
            lock (dict)
            {
                if (dict.ContainsKey(key))
                    dict[key] = getValue();
                else
                    dict.Add(key, getValue());
            }
        }

    }
}
