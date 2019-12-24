using OpenTK.Graphics.OpenGL4;

namespace ProcEngine
{
    public class VertexBufferObject
    {

        private int _Handle;
        public int Handle => _Handle;

        public void Init()
        {
            _Handle = GL.GenBuffer();
        }

        public void SetData(float[] vertices)
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
