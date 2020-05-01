using System.Collections.Generic;
using Aximo.Render;

namespace Aximo.Engine
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
            if (list is List<IVertexPosNormalUV> _list)
            {
                _list.AddRange(items);
                return;
            }

            foreach (var itm in items)
                list.Add((IVertexPosNormalUV)itm);
        }

        public static void AddRange(this IList<IVertexPosNormalUV> list, params VertexDataPosNormalUV[] items)
        {
            AddRange(list, (ICollection<VertexDataPosNormalUV>)items);
        }
    }

}
