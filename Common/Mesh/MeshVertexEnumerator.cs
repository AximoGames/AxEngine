// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections;
using System.Collections.Generic;
using Aximo.Render;
using Aximo.VertexData;

namespace Aximo
{
    internal class MeshVertexEnumerator<T> : IEnumerator<T>
        where T : IVertex
    {
        public MeshVertexEnumerator(Mesh mesh)
        {
            Mesh = mesh;
            Visitor = VertexVisitor<T>.CreateVisitor(mesh);
        }

        private Mesh Mesh;

        internal VertexVisitor<T> Visitor;

        public T Current => (T)Visitor.Value;

        object IEnumerator.Current => Visitor.Value;

        public void Dispose()
        {
            Visitor?.Dispose();
            Visitor = null;
        }

        public bool MoveNext()
        {
            if (Visitor.Index >= Mesh.VertexCount - 1)
                return false;

            Visitor.Index++;
            return true;
        }

        public void Reset()
        {
            Visitor.Index = -1;
        }
    }
}
