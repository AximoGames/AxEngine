using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;

namespace ProcEngine
{
    public class VertexArrayObject
    {
        private int _Handle;

        public int Handle => _Handle;
        private int _Stride;

        public int VertexCount { get; private set; }

        private VertexBufferObject _vbo;
        public VertexBufferObject VertextBufferObject;

        public VertexArrayObject(VertexBufferObject vbo)
        {
            _vbo = vbo;
        }

        public void Create()
        {
            _Handle = GL.GenVertexArray();
        }

        private bool AttribuesInitialized = false;

        public static int CurrentHandle;
        public void Use()
        {
            if (CurrentHandle == _Handle)
                return;
            CurrentHandle = _Handle;

            GL.BindVertexArray(_Handle);
        }

        public void Draw()
        {
            GL.DrawArrays(PrimitiveType.Triangles, 0, VertexCount);
        }

        internal void SetData(float[] vertices)
        {
            if (!AttribuesInitialized)
                InitAttributes();

            Use();
            _vbo.SetData(vertices);
            VertexCount = vertices.Length / 6;
        }

        public void AddAttribute(int index, int size, Type type, bool normalized, int offset)
        {
            _Stride += size * GetSizeOf(type);
            InitAttribsList.Add(() =>
            {
                GL.EnableVertexAttribArray(index);
                GL.VertexAttribPointer(index, size, GetVertexAttribPointerType(type), normalized, _Stride, offset);
            });
        }

        private void InitAttributes()
        {
            if (AttribuesInitialized)
                return;
            AttribuesInitialized = true;

            _vbo.Use();
            Use();

            foreach (var act in InitAttribsList)
            {
                act();
            }
        }

        private List<Action> InitAttribsList = new List<Action>();

        private class Attrib
        {
        }

        private static VertexAttribPointerType GetVertexAttribPointerType(Type type)
        {
            if (type == typeof(float))
                return VertexAttribPointerType.Float;
            throw new NotImplementedException();
        }

        private static int GetSizeOf(Type type)
        {
            if (type == typeof(float))
                return 4;
            throw new NotImplementedException();
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
