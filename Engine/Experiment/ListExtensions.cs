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

        public static void AddRange<IVertexPosNormalUV>(this IList<IVertexPosNormalUV> list, ICollection<VertexDataPosNormalUV> items)
        {
            if (list is List<IVertexPosNormalUV> l)
            {
                l.AddRange(items);
                return;
            }

            IVertexPosition3 v = new VertexDataPosNormalUV();
            IVertexPosNormalUV v2 = new VertexDataPosNormalUV();


            // foreach (var itm in items)
            // {
            //     list.Add((IVertexPosNormalUV)itm);
            // }
        }
    }

}
