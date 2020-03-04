using OpenTK.Graphics.OpenGL4;
using System.Runtime.InteropServices;
using System.Collections.Generic;

namespace AxEngine
{
    public abstract class BufferObject
    {

        public BufferObject()
        {
        }

        protected BufferTarget Target = BufferTarget.ArrayBuffer;

        private int _Handle;
        public int Handle => _Handle;

        public void Create()
        {
            _Handle = GL.GenBuffer();
        }

        public int Size { get; private set; }

        internal void SetData(float[] data)
        {
            var currentBuffer = CurrentBuffer;
            Use();
            Size = data.Length;
            GL.BufferData(Target, data.Length * sizeof(float), data, BufferUsageHint.StaticDraw);

            // if (currentBuffer == null)
            //     UseDefault();
            // else
            //     currentBuffer.Use();
            //UseDefault();
        }

        internal void SetData<T>(T[] data)
            where T : struct
        {
            var currentBuffer = CurrentBuffer;
            Use();
            Size = data.Length;
            var structSize = Marshal.SizeOf(typeof(T));
            GL.BufferData(Target, data.Length * structSize, data, BufferUsageHint.StaticDraw);

            // if (currentBuffer == null)
            //     UseDefault();
            // else
            //     currentBuffer.Use();
            //UseDefault();
        }

        private static int CurrentHandle;
        private static BufferObject CurrentBuffer;

        public void Use()
        {
            Use(Target, _Handle);
            CurrentBuffer = this;
        }

        private static void Use(BufferTarget target, int handle)
        {
            // if (CurrentHandle == handle)
            //     return;
            CurrentHandle = handle;
            GL.BindBuffer(target, handle);
        }

        public void UseDefault()
        {
            Use(Target, 0);
            CurrentBuffer = null;
        }

        public void Free()
        {
            GL.DeleteBuffer(_Handle);
        }

    }

    public class BindingPoint
    {

        private int _Number;
        public int Number => _Number;

        private static HashSet<int> UsedNumbers = new HashSet<int>();
        private static List<int> FreeNumbers;

        static BindingPoint()
        {
            FreeNumbers = new List<int>();
            for (var i = 1; i < 16; i++)
            {
                FreeNumbers.Add(i);
            }
        }

        public static BindingPoint Default { get; private set; } = new BindingPoint(false) { _Number = 0 };

        public BindingPoint() : this(true)
        {
        }

        private BindingPoint(bool alloc)
        {
            if (alloc)
            {
                _Number = FreeNumbers[FreeNumbers.Count - 1];
                FreeNumbers.Remove(_Number);
                UsedNumbers.Add(_Number);
            }
        }

        public void Free()
        {
            UsedNumbers.Remove(_Number);
            FreeNumbers.Add(_Number);
            _Number = -1;
        }
    }

}
