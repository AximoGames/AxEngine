using System;
using System.Linq;
using System.Collections.Generic;
using Aximo.Render;

namespace Aximo.Engine.Mesh2
{
    public struct MeshFace<T>
        where T : IVertex
    {

        // Global List
        private IList<T> VertexView;
        private IList<int> Indicies;

        internal InternalMeshFace InternalFace;

        public int[] GetIndicies()
        {
            var array = new int[Count];
            CopyIndiciesTo(array, 0);
            return array;
        }

        public void CopyIndiciesTo(int[] array, int arrayIndex)
        {
            for (var i = 0; i < Count; i++)
                array[arrayIndex + i] = GetIndex(i);
        }

        internal MeshFace(IList<T> vertexView, IList<int> indicies, InternalMeshFace internalFace)
        {
            VertexView = vertexView;
            Indicies = indicies;
            InternalFace = internalFace;
        }

        public int Count => InternalFace.Count;

        public bool IsPoint => InternalFace.IsPoint;
        public bool IsLine => InternalFace.IsLine;
        public bool IsTriangle => InternalFace.IsTriangle;
        public bool IsQuad => InternalFace.IsQuad;
        public bool IsNgon => InternalFace.IsNgon;
        public MeshFaceType Type => InternalFace.Type;

        public int GetIndex(int index) => Indicies[InternalFace[index]];

        public T this[int index] => VertexView[GetIndex(index)];
    }

}
