// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Runtime.InteropServices;

namespace Aximo
{
    public class BufferData1D<T> : BufferData1D
    {
        public BufferData1D(T[] data)
        {
            SetData(data);
        }

        public void SetData(T[] data)
        {
            _Data = data;
            _Length = data.Length;
            _ElementSize = Marshal.SizeOf<T>();
            _Memory = _Data.AsMemory();
        }

        public override GCHandle CreateHandle()
        {
            return GCHandle.Alloc(_Data, GCHandleType.Pinned);
        }

        private Memory<T> _Memory;
        public Memory<T> Memory => _Memory;

        public Span<T> Span => new Span<T>(_Data);

        private T[] _Data;
        public T this[int index]
        {
            get { return _Data[index]; }
            set { _Data[index] = value; }
        }

        private int _Length;
        public override int Length => _Length;

        public override int SizeX => _Length;

        private int _ElementSize;
        public override int ElementSize => _ElementSize;

        public override BufferData Clone()
        {
            var copy = (T[])_Data.Clone();
            return new BufferData1D<T>(copy);
        }
    }
}
