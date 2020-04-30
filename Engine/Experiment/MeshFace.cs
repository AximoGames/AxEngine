using System;
using System.Linq;
using System.Collections.Generic;
using Aximo.Render;

namespace Aximo.Engine.Mesh2
{
    public struct MeshFace
    {

        public MeshFace(params int[] indicies)
        {
            //var triangeles = MemoryMarshal.Cast<int, Triangle>(indicies.AsSpan());

            int n = 14;
            int i = 3;
            var ngon = indicies.AsSpan().Slice(i * n, n);

            if (indicies.Length > 0)
                Indicies = indicies.ToArray();
            else
                Indicies = new int[] { };
        }

        public void Add(int vertexIndex)
        {
            var newArray = new int[Indicies.Length + 1];
            Indicies.CopyTo(newArray, 0);
            newArray[newArray.Length - 1] = vertexIndex;
            Indicies = newArray;
        }

        public int this[int index] => Indicies[index];

        public int[] Indicies { get; private set; }
        public int Count => Indicies.Length;

        public bool IsPoint => Indicies.Length == 1;
        public bool IsLine => Indicies.Length == 2;
        public bool IsTriangle => Indicies.Length == 3;
        public bool IsQuad => Indicies.Length == 4;
        public bool IsNgon => Indicies.Length > 4;

        public MeshFaceType Type
        {
            get
            {
                var cnt = Indicies.Length;
                if (cnt > 4)
                    return MeshFaceType.Ngon;
                else
                    return (MeshFaceType)cnt;
            }
        }
    }

    public struct MeshFace<T>
        where T : IVertex
    {

        // Global List
        private IList<T> VertexView;

        internal InternalMeshFace InternalFace;

        internal MeshFace(IList<T> vertexView, InternalMeshFace internalFace)
        {
            VertexView = vertexView;
            InternalFace = internalFace;
        }

        public T this[int index] => VertexView[InternalFace[index]];
    }

}
