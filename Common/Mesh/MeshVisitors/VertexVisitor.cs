// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using Aximo.Render;
using Aximo.VertexData;

namespace Aximo
{
    internal abstract class VertexVisitor<T> : IDisposable, IVertex, IDynamicArray<T>
        where T : IVertex
    {
        public static VertexVisitor<T> CreateVisitor(Mesh mesh)
        {
            var type = typeof(T);

            if (type == typeof(IVertexPosition3))
                return (VertexVisitor<T>)(object)new VertexPosition3Visitor(mesh);

            if (type == typeof(IVertexPosNormalUV))
                return (VertexVisitor<T>)(object)new VertexPosNormalUVVisitor(mesh);

            if (type == typeof(IVertexPosNormalColor))
                return (VertexVisitor<T>)(object)new VertexPosNormalColorVisitor(mesh);

            if (type == typeof(IVertexPosColor))
                return (VertexVisitor<T>)(object)new VertexPosColorVisitor(mesh);

            if (type == typeof(IVertexPos2UV))
                return (VertexVisitor<T>)(object)new VertexPos2UVVisitor(mesh);

            throw new NotSupportedException(type.Name);
        }

        internal VertexVisitor(Mesh mesh)
        {
            Mesh = mesh;
        }

        protected Mesh Mesh;
        public int Index;
        public IVertex Value
        {
            get => this;
            set => Set(value);
        }

        public int Count => Mesh.Components[0].Count;

        T IArray<T>.this[int index] { get => (T)this[index]; set => this[index] = value; }

        public IVertex this[int index]
        {
            get
            {
                Index = index;
                return this;
            }
            set
            {
                Index = index;
                Set(value);
            }
        }

        protected virtual void Set(IVertex vertex)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
        }

        public abstract void SetLength(int length);

        public abstract IVertex Clone();
    }
}
