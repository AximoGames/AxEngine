using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace AxEngine
{

    public class VertexArrayObject
    {
        private int _Handle = -1;
        public int Handle => _Handle;

        public int VertexCount { get; private set; }

        public VertexLayout Layout { get; set; }

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

        public VertexArrayObject(VertexLayout layout, VertexBufferObject vbo = null, ElementsBufferObject ebo = null)
        {
            Layout = layout;
            _vbo = vbo;
            _ebo = ebo;
        }

        public VertexBufferObject CreateVBO()
        {
            _vbo = new VertexBufferObject();
            _vbo.Create();
            _vbo.Use();
            return _vbo;
        }

        public ElementsBufferObject CreateEBO()
        {
            _ebo = new ElementsBufferObject();
            _ebo.Create();
            _ebo.Use();
            return _ebo;
        }

        public void Create()
        {
            _Handle = GL.GenVertexArray();
            Use();
            // if (Layout != null)
            //     Layout.InitAttributes();
        }

        public static int CurrentHandle;
        public void Use()
        {
            if (CurrentHandle == _Handle)
                return;
            CurrentHandle = _Handle;

            GL.BindVertexArray(_Handle);
        }

        public void UseDefault()
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
        }

        private bool Initialized;
        private void EnsureInitialized()
        {
            if (Initialized)
                return;
            if (_Handle == -1)
                Create();
            Use();
            if (_vbo == null)
                _vbo = CreateVBO();
            // if (_ebo == null)
            //     _ebo = CreateEBO();
            Layout.InitAttributes();
            Initialized = true;
        }

        internal void SetData(float[] vertices, ushort[] indicies = null)
        {
            EnsureInitialized();
            _vbo.SetData(vertices);
            VertexCount = (vertices.Length * sizeof(float)) / Layout.Stride;
            //          UseDefault();
            if (indicies != null)
            {
                if (_ebo == null)
                    _ebo = CreateEBO();
                _ebo.SetData(indicies);
            }
        }

        internal void SetData<T>(T[] vertices, uint[] indicies = null)
        where T : struct
        {
            EnsureInitialized();
            _vbo.SetData<T>(vertices);
            VertexCount = (vertices.Length * Marshal.SizeOf(typeof(T))) / Layout.Stride;
            //            UseDefault();
            if (_ebo != null && indicies != null)
            {
                if (_ebo == null)
                    _ebo = CreateEBO();
                _ebo.SetData(indicies);
            }
        }

        //public void AddPosition()
        //{
        //}

        public void Free()
        {
            GL.DeleteVertexArray(_Handle);
        }
    }

}
