// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;
using Aximo.Render;
using Aximo.Render.VertexData;

namespace Aximo
{
    // public class TestClass
    // {
    //     public ref VertexDataPosNormalUV Test
    //     {
    //         get
    //         {
    //             var sp = new Span<VertexDataPosNormalUV>(null);
    //             return ref sp[0];
    //         }
    //     }

    //     public void sdfdf()
    //     {
    //         Test.Normal.X = 5;
    //     }

    // }

    public static class ListExtensions
    {
        public static void AddRange<T>(this IList<T> list, ICollection<T> items)
        {
            if (list is List<T> l)
            {
                l.AddRange(items);
                return;
            }

            foreach (var itm in items)
                list.Add(itm);
        }

        public static void AddRange(this IList<IVertexPosNormalUV> list, ICollection<VertexDataPosNormalUV> items)
        {
            if (list is List<IVertexPosNormalUV> list_)
            {
                list_.AddRange(items);
                return;
            }

            foreach (var itm in items)
                list.Add((IVertexPosNormalUV)itm);
        }

        public static void AddRange(this IList<IVertexPosNormalUV> list, params VertexDataPosNormalUV[] items)
        {
            AddRange(list, (ICollection<VertexDataPosNormalUV>)items);
        }

        public static void Reverse<T>(this IList<T> list, int startIndex, int count)
        {
            ReverseInternal(list, startIndex, startIndex + count - 1);
        }

        private static void ReverseInternal<T>(this IList<T> list, int startIndex, int endIndex)
        {
            while (startIndex < endIndex)
            {
                var temp = list[startIndex];
                list[startIndex] = list[endIndex];
                list[endIndex] = temp;
                startIndex++;
                endIndex--;
            }
        }

        public static T[] Copy<T>(this T[] array)
        {
            return array.ToArray();
        }
    }
}
