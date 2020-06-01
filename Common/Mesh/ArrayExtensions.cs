// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Aximo.VertexData;

namespace Aximo
{
    public static class ArrayExtensions
    {
        public static T[] EnsureSize<T>(this T[] input, int length)
        {
            if (input == null)
                input = Array.Empty<T>();
            if (input.Length >= length)
                return input;
            var ar = new T[length];
            input.CopyTo(ar, 0);
            return ar;
        }

        public static T[] AppendElement<T>(this T[] input, T element)
        {
            if (input == null)
                input = Array.Empty<T>();
            var ar = EnsureSize(input, input.Length + 1);
            ar[ar.Length - 1] = element;
            return ar;
        }

        public static T[] RemoveElement<T>(this T[] input, T element)
        {
            // TODO: Don't use List<>
            var list = new List<T>(input);
            list.Remove(element);
            return list.ToArray();
        }
    }
}
