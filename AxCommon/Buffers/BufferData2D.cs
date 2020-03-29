// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using OpenTK;

namespace Aximo
{

    public abstract class BufferData2D : BufferData
    {
        public abstract int SizeX { get; }
        public abstract int SizeY { get; }

        public int Width => SizeX;
        public int Height => SizeY;

        // public abstract Bitmap CreateBitmap();
        // public abstract void SetData(Bitmap bmp);
    }

    public class BufferData2D<T> : BufferData2D, IEnumerable<T>
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

        public BufferData2D(T[,] data)
        {
            SetData(data);
        }

        public void SetData(T[,] data)
        {
            _Data = data;
            _Length = data.Length;
            _SizeX = data.GetUpperBound(0) + 1;
            _SizeY = data.GetUpperBound(1) + 1;
            _ElementSize = Marshal.SizeOf<T>();
        }

        public void Resize(int width, int height)
        {
            SetData(new T[width, height]);
        }

        private T[,] _Data;

        public T this[int x, int y]
        {
            get { return _Data[x, y]; }
            set { _Data[x, y] = value; }
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
    }

}
