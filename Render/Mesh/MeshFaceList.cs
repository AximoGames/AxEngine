// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections;
using System.Collections.Generic;
using Aximo.Render;
using Aximo.Render.VertexData;

namespace Aximo
{
    internal class MeshFaceList<T> : IList<MeshFace<T>>
        where T : IVertex
    {
        private Mesh Mesh;
        public MeshFaceList(Mesh mesh)
        {
            Mesh = mesh;
            VertexView = Mesh.View<T>();
        }

        private IList<T> VertexView;
        private IList<InternalMeshFace> Faces => Mesh.InternalMeshFaces;

        public int Count => Faces.Count;

        public bool IsReadOnly => false;

        public MeshFace<T> this[int index]
        {
            get => GetFace(index);
            set => throw new NotImplementedException();
        }

        private MeshFace<T> GetFace(InternalMeshFace face)
        {
            return new MeshFace<T>(VertexView, Mesh.Indicies, face);
        }

        private MeshFace<T> GetFace(int faceIndex)
        {
            return GetFace(Faces[faceIndex]);
        }

        public int IndexOf(MeshFace<T> item)
        {
            throw new NotImplementedException();
        }

        public void Insert(int index, MeshFace<T> item)
        {
            throw new NotImplementedException();
        }

        public void RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        public void Add(MeshFace<T> item)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool Contains(MeshFace<T> item)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(MeshFace<T>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public bool Remove(MeshFace<T> item)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<MeshFace<T>> GetEnumerator()
        {
            return new FaceEnumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new FaceEnumerator(this);
        }

        private class FaceEnumerator : IEnumerator<MeshFace<T>>
        {
            private MeshFaceList<T> Faces;
            private int Index = -1;
            public FaceEnumerator(MeshFaceList<T> faces)
            {
                Faces = faces;
            }

            public MeshFace<T> Current => Faces[Index];

            object IEnumerator.Current => Current;

            public void Dispose()
            {
            }

            public bool MoveNext()
            {
                if (Index >= Faces.Count - 1)
                    return false;

                Index++;
                return true;
            }

            public void Reset()
            {
                Index = -1;
            }
        }
    }
}
