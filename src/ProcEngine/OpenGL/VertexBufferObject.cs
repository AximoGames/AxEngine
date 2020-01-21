using OpenTK.Graphics.OpenGL4;

namespace ProcEngine
{
    public class VertexBufferObject
    {

        public VertexBufferObject()
        {
        }

        private int _Handle;
        public int Handle => _Handle;

        public void Create()
        {
            _Handle = GL.GenBuffer();
        }

        internal void SetData(float[] vertices)
        {
            Use();
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);
        }

        public static int CurrentHandle;

        public void Use()
        {
            if (CurrentHandle == _Handle)
                return;
            CurrentHandle = _Handle;

            GL.BindBuffer(BufferTarget.ArrayBuffer, _Handle);
        }

        public void Free()
        {
            GL.DeleteBuffer(_Handle);
        }

    }

}
