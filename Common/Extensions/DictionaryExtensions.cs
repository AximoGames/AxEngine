// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Aximo
{
    public static class DictionaryExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static bool TryAdd<TKey, TValue>(this IDictionary<TKey, TValue> dict, in TKey key, in TValue value)
        {
            if (dict.ContainsKey(key))
                return false;

            dict.Add(key, value);
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static void Set<TKey, TValue>(this IDictionary<TKey, TValue> dict, in TKey key, in TValue value)
        {
            //if (dict.ContainsKey(key))
            dict[key] = value;
            //else
            //    dict.Add(key, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static void Set<TKey, TValue>(this IDictionary<TKey, TValue> dict, in TKey key, in Func<TValue> getValue)
        {
            //if (dict.ContainsKey(key))
            dict[key] = getValue();
            //else
            //    dict.Add(key, getValue());
        }
    }
}
