using OpenTK.Graphics.OpenGL4;

namespace ProcEngine
{
    public class VertexArrayObject
    {
        private int _Handle;

        public int Handle => _Handle;
        private int _Stride;

        private VertexBufferObject _vbo;
        public VertexBufferObject VertextBufferObject;

        public VertexArrayObject(VertexBufferObject vbo, int stride)
        {
            _Stride = stride;
            _vbo = vbo;
        }

        public void Init()
        {
            _Handle = GL.GenVertexArray();
        }

        public static int CurrentHandle;
        public void Use()
        {
            if (CurrentHandle == _Handle)
                return;
            CurrentHandle = _Handle;

            GL.BindVertexArray(_Handle);
        }

        public void VertexAttribPointer(int index, int size, VertexAttribPointerType type, bool normalized, int offset)
        {
            _vbo.Use();
            Use();
            GL.EnableVertexAttribArray(index);
            GL.VertexAttribPointer(index, size, type, normalized, _Stride, offset);
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
