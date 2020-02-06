using OpenTK.Graphics.OpenGL4;

namespace ProcEngine
{
    public class BufferObject
    {

        public BufferObject()
        {
        }

        private BufferTarget Target = BufferTarget.ArrayBuffer;

        private int _Handle;
        public int Handle => _Handle;

        public void Create()
        {
            _Handle = GL.GenBuffer();
        }

        internal void SetData(float[] data)
        {
            Use();
            GL.BufferData(Target, data.Length * sizeof(float), data, BufferUsageHint.StaticDraw);
        }

        private static int CurrentHandle;

        public void Use()
        {
            if (CurrentHandle == _Handle)
                return;
            CurrentHandle = _Handle;

            GL.BindBuffer(Target, _Handle);
        }

        private void UseDefault()
        {
            GL.BindBuffer(Target, 0);
        }

        public void Free()
        {
            GL.DeleteBuffer(_Handle);
        }

    }

}
