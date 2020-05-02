// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq;

namespace Aximo
{
    internal static class DynamicArrayExtensions
    {
        internal static void SetValueWithExpand<T>(this IDynamicArray<T> array, int index, T value)
        {
            array.SetLength(index + 1);
            array[index] = value;
        }

        internal static T GetValueWithExpand<T>(this IDynamicArray<T> array, int index)
        {
            array.SetLength(index + 1);
            return array[index];
        }

        internal static void Clear<T>(this IDynamicArray<T> array)
        {
            array.SetLength(0);
        }

        internal static bool Contains<T>(this IDynamicArray<T> array, T value)
        {
            var length = array.Count;
            for (var i = 0; i < length; i++)
                if (array[i].Equals(value))
                    return true;
            return false;
        }

        internal static bool Remove<T>(this IDynamicArray<T> array, T value)
        {
            throw new NotImplementedException();
        }

        internal static void RemoveAt<T>(this IDynamicArray<T> array, int index)
        {
            throw new NotImplementedException();
        }
    }
}
