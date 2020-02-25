using OpenTK.Graphics.OpenGL4;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System;

namespace AxEngine
{
    public class BufferObject
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

        internal void SetData(float[] data)
        {
            Use();
            GL.BufferData(Target, data.Length * sizeof(float), data, BufferUsageHint.StaticDraw);
            UseDefault();
        }

        internal void SetData<T>(T[] data)
            where T : struct
        {
            Use();
            var structSize = Marshal.SizeOf(typeof(T));
            GL.BufferData(Target, data.Length * structSize, data, BufferUsageHint.StaticDraw);
            UseDefault();
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

    public class UniformBufferObject : BufferObject
    {

        public UniformBufferObject()
        {
            Target = BufferTarget.UniformBuffer;
        }

        public void SetBindingPoint(BindingPoint bindingPoint)
        {
            if (Target != BufferTarget.UniformBuffer)
                throw new InvalidOperationException();

            GL.BindBufferBase(BufferRangeTarget.UniformBuffer, bindingPoint.Number, Handle);
        }

    }

}
