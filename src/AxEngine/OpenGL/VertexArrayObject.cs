using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace AxEngine
{

    public class VertexArrayObject
    {
        private int _Handle;
        public int Handle => _Handle;

        public int VertexCount { get; private set; }

        public VertexLayout Layout { get; private set; }

        private VertexBufferObject _vbo;
        public VertexBufferObject VertextBufferObject;

        public VertexArrayObject(VertexLayout layout, VertexBufferObject vbo)
        {
            Layout = layout;
            _vbo = vbo;
        }

        public void Create()
        {
            _Handle = GL.GenVertexArray();
            Use();
            Layout.InitAttributes();
        }

        public static int CurrentHandle;
        public void Use()
        {
            if (CurrentHandle == _Handle)
                return;
            CurrentHandle = _Handle;

            GL.BindVertexArray(_Handle);
        }

        public PrimitiveType PrimitiveType = PrimitiveType.Triangles;

        public void Draw()
        {
            GL.DrawArrays(PrimitiveType, 0, VertexCount);
        }

        internal void SetData(float[] vertices)
        {
            Use();
            _vbo.SetData(vertices);
            VertexCount = (vertices.Length * sizeof(float)) / Layout.Stride;
        }

        internal void SetData<T>(T[] vertices)
        where T : struct
        {
            Use();
            _vbo.SetData<T>(vertices);
            VertexCount = (vertices.Length * Marshal.SizeOf(typeof(T))) / Layout.Stride;
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
