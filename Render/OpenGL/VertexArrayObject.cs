// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Security.Policy;
using OpenToolkit.Graphics.OpenGL4;

namespace Aximo.Render.OpenGL
{
    public class VertexArrayObject
    {
        private int _Handle = -1;
        public int Handle => _Handle;

        public int VertexCount { get; private set; }

        public VertexLayoutBinded Layout { get; set; }

        private VertexBufferObject _vbo;
        public VertexBufferObject Vbo => _vbo;

        private ElementsBufferObject _ebo;
        public ElementsBufferObject Ebo => _ebo;

        public void SetVbo(VertexBufferObject vbo)
        {
            _vbo = vbo;
        }

        public void SetEbo(ElementsBufferObject ebo)
        {
            _ebo = ebo;
        }

        public VertexArrayObject(VertexLayoutBinded layout, VertexBufferObject vbo = null, ElementsBufferObject ebo = null)
        {
            Layout = layout;
            _vbo = vbo;
            _ebo = ebo;
        }

        public VertexBufferObject CreateVBO()
        {
            _vbo = new VertexBufferObject();
            _vbo.Create();
            _vbo.Bind();
            return _vbo;
        }

        public ElementsBufferObject CreateEBO()
        {
            _ebo = new ElementsBufferObject();
            _ebo.Create();
            _ebo.Bind();
            return _ebo;
        }

        public void Create()
        {
            _Handle = GL.GenVertexArray();
            Bind();
            // if (Layout != null)
            //     Layout.InitAttributes();
        }

        public static int CurrentHandle;
        public void Bind()
        {
            if (CurrentHandle == _Handle)
                return;
            CurrentHandle = _Handle;

            GL.BindVertexArray(_Handle);
        }

        public void BindDefault()
        {
            CurrentHandle = 0;
            GL.BindVertexArray(0);
        }

        public PrimitiveType PrimitiveType = PrimitiveType.Triangles;

        public void Draw()
        {
            if (_ebo == null)
                GL.DrawArrays(PrimitiveType, 0, VertexCount);
            else
                GL.DrawElements(PrimitiveType, _ebo.Size, DrawElementsType.UnsignedShort, 0);

            if (Renderer.Current.FlushRenderBackend == FlushRenderBackend.Draw)
                GL.Finish();
        }

        private bool Initialized;
        private void EnsureInitialized()
        {
            if (Initialized)
                return;
            if (_Handle == -1)
                Create();
            Bind();
            if (_vbo == null)
                _vbo = CreateVBO();
            // if (_ebo == null)
            //     _ebo = CreateEBO();
            Layout.InitAttributes();
            Initialized = true;
        }

        private int HashCode;
        private static Dictionary<int, ValueTuple<VertexArrayObject, int>> Cache = new Dictionary<int, (VertexArrayObject, int)>();

        internal void SetData(BufferData1D vertices, BufferData1D<ushort> indicies = null)
        {
            var vertHashCode = vertices.GetHashCode();
            var indicieshashCode = indicies?.GetHashCode() ?? 0;
            var layoutHashCode = Layout.GetHashCode();
            HashCode = Hashing.HashInteger(vertHashCode, indicieshashCode, layoutHashCode, (int)PrimitiveType);

            if (Cache.TryGetValue(HashCode, out var cachedEntry))
            {
                var cached = cachedEntry.Item1;
                cachedEntry.Item2++;
                Cache[HashCode] = cachedEntry;

                _Handle = cached._Handle;
                Layout = cached.Layout;
                Initialized = true;
                PrimitiveType = cached.PrimitiveType;
                VertexCount = cached.VertexCount;
                _vbo = cached._vbo;
                _ebo = cached._ebo;
                return;
            }
            else
            {
                Cache.TryAdd(HashCode, (this, 1));
            }

            EnsureInitialized();
            _vbo.SetData(vertices);
            VertexCount = vertices.Length * vertices.ElementSize / Layout.Stride;
            //          UseDefault();
            if (indicies != null)
            {
                if (_ebo == null)
                    _ebo = CreateEBO();
                _ebo.SetData(indicies);
            }
        }

        internal void SetData(MeshData data)
        {
            PrimitiveType = GetPrimitiveType(data.PrimitiveType);
            SetData(data.Data, data.Indicies);
        }

        private PrimitiveType GetPrimitiveType(AxPrimitiveType primitiveType)
        {
            switch (primitiveType)
            {
                case AxPrimitiveType.Triangles:
                    return PrimitiveType.Triangles;
                case AxPrimitiveType.Lines:
                    return PrimitiveType.Lines;
                default:
                    throw new InvalidOperationException();
            }
        }

        //public void AddPosition()
        //{
        //}

        private bool Disposed;

        public void Free()
        {
            if (Disposed)
                return;

            Disposed = true;
            if (Cache.TryGetValue(HashCode, out var cachedEntry))
            {
                if (cachedEntry.Item2 > 1)
                {
                    cachedEntry.Item2--;
                    Cache[HashCode] = cachedEntry;
                }
                else
                {
                    Cache.Remove(HashCode);
                    GL.DeleteVertexArray(_Handle);
                }
            }
        }
    }
}
