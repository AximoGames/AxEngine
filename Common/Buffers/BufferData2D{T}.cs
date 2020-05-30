// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using OpenToolkit.Mathematics;

namespace Aximo
{
    public class BufferData2D<T> : BufferData2D, IEnumerable<T>
        where T : struct
    {
        public BufferData2D(Vector2i size) : this(size.X, size.Y)
        {
        }

        public BufferData2D()
        {
            Resize(0, 0);
        }

        public BufferData2D(int width, int height)
        {
            Resize(width, height);
        }

        public BufferData2D(T[] data, int width, int height)
        {
            SetData(data, width, height);
        }

        public BufferData2D(T[,] data)
        {
            SetData(data);
        }

        public ref T FirtElement
        {
            get
            {
                return ref _Data[0];
            }
        }

        private Memory<T> _Memory;
        public Memory<T> Memory => _Memory;

        public Span<T> Span => new Span<T>(_Data);

        public void SetData(T[] data)
        {
            SetData(data, Width, Height);
        }

        public void SetData(T[] data, int width, int height)
        {
            _Data = data;
            _Length = width * height;
            _SizeX = width;
            _SizeY = height;
            _ElementSize = Marshal.SizeOf<T>();
            _Memory = _Data.AsMemory();
        }

        public void SetData(T[,] data)
        {
            _Data = data.Cast<T>().ToArray();
            _Length = data.Length;
            _SizeX = data.GetUpperBound(0) + 1;
            _SizeY = data.GetUpperBound(1) + 1;
            _ElementSize = Marshal.SizeOf<T>();
            _Memory = _Data.AsMemory();
        }

        public void Resize(int width, int height)
        {
            SetData(new T[width, height]);
        }

        private T[] _Data;

        public T this[int x, int y]
        {
            get { return _Data[(y * Width) + y]; }
            set { _Data[(y * Width) + y] = value; }
        }

        private int _Length;
        public override int Length => _Length;

        private int _SizeX;
        public override int SizeX => _SizeX;

        private int _SizeY;
        public override int SizeY => _SizeY;

        public override GCHandle CreateHandle()
        {
            return GCHandle.Alloc(_Data, GCHandleType.Pinned);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _Data.Cast<T>().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _Data.GetEnumerator();
        }

        private int _ElementSize;
        public override int ElementSize => _ElementSize;

        public override BufferData Clone()
        {
            var copy = (T[])_Data.Clone();
            return new BufferData2D<T>(copy, Width, Height)
            {
                PixelFormat = PixelFormat,
            };
        }

        public override void FlipY()
        {
            // TODO: Copy complete Row
            int rows = Height;
            int cols = Width;

            for (int i = 0; i < cols; i++)
            {
                int start = 0;
                int end = rows - 1;
                while (start < end)
                {
                    var temp = this[i, start];
                    this[i, start] = this[i, end];
                    this[i, end] = temp;
                    start++;
                    end--;
                }
            }
        }
    }
}
