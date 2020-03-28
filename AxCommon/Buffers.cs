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

    public abstract class BufferData
    {
        public abstract int Length { get; }
        public abstract int ElementSize { get; }

        public abstract GCHandle CreateHandle();

        public static BufferData1D<T> Create<T>(T[] data)
        {
            return new BufferData1D<T>(data);
        }

        public static BufferData2D<T> Create<T>(T[,] data)
        {
            return new BufferData2D<T>(data);
        }

        public static BufferData3D<T> Create<T>(T[,,] data)
        {
            return new BufferData3D<T>(data);
        }

        public int Bytes => Length * ElementSize;

    }

    public abstract class BufferData1D : BufferData
    {
        public abstract int SizeX { get; }
    }

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
        }

        public override GCHandle CreateHandle()
        {
            return GCHandle.Alloc(_Data, GCHandleType.Pinned);
        }

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
    }

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

    public abstract class BufferData3D : BufferData
    {
        public abstract int SizeX { get; }
        public abstract int SizeY { get; }
        public abstract int SizeZ { get; }
    }

    public class BufferData3D<T> : BufferData3D
    {
        public BufferData3D(T[,,] data)
        {
            SetData(data);
        }

        public void SetData(T[,,] data)
        {
            _Data = data;
            _Length = data.Length;
            _SizeX = data.GetUpperBound(0) + 1;
            _SizeY = data.GetUpperBound(1) + 1;
            _SizeZ = data.GetUpperBound(2) + 1;
            _ElementSize = Marshal.SizeOf<T>();
        }

        private T[,,] _Data;

        public T this[int x, int y, int z]
        {
            get { return _Data[x, y, z]; }
            set { _Data[x, y, z] = value; }
        }

        private int _Length;
        public override int Length => _Length;

        private int _SizeX;
        public override int SizeX => _SizeX;

        private int _SizeY;
        public override int SizeY => _SizeY;

        private int _SizeZ;
        public override int SizeZ => _SizeZ;

        public override GCHandle CreateHandle()
        {
            return GCHandle.Alloc(_Data, GCHandleType.Pinned);
        }

        private int _ElementSize;
        public override int ElementSize => _ElementSize;
    }

    public static class BufferDataExtentions
    {

        public static void Normalize(this BufferData2D<float> target)
        {
            var collection = target.Where(v => v != 1);
            var min = collection.Min();
            var max = collection.Max();
            var span = max - min;

            var factor = 1.0f / span;

            for (var y = 0; y < target.SizeY; y++)
            {
                for (var x = 0; x < target.SizeX; x++)
                {
                    var value = target[x, y];
                    if (value != 1.0f)
                    {
                        value -= min;
                        value *= factor;
                        //value = 1 - value;
                        //value *= 300;
                        //value = 1 - value;
                        value = Math.Max(0, Math.Min(value, 1f));
                        target[x, y] = value;
                    }
                }
            }
        }

        public unsafe static void SetData(this BufferData2D<int> target, Bitmap source)
        {
            var targetData = new int[source.Width, source.Height];
            target.SetData(targetData);
            var lockedBits = source.LockBits(new Rectangle(0, 0, source.Width, source.Height), ImageLockMode.ReadOnly, source.PixelFormat);
            var destHandle = target.CreateHandle();
            try
            {
                Buffer.MemoryCopy((void*)lockedBits.Scan0, (void*)destHandle.AddrOfPinnedObject(), target.Bytes, target.Bytes);
            }
            finally
            {
                source.UnlockBits(lockedBits);
                destHandle.Free();
            }
        }

        public unsafe static void CopyTo(this BufferData2D<int> source, Bitmap bmp)
        {
            var lockedBits = bmp.LockBits(new Rectangle(0, 0, source.SizeX, source.SizeY), ImageLockMode.ReadOnly, bmp.PixelFormat);
            var destHandle = source.CreateHandle();
            try
            {
                Buffer.MemoryCopy((void*)destHandle.AddrOfPinnedObject(), (void*)lockedBits.Scan0, source.Bytes, source.Bytes);
            }
            finally
            {
                bmp.UnlockBits(lockedBits);
                destHandle.Free();
            }
            bmp.RotateFlip(System.Drawing.RotateFlipType.RotateNoneFlipY);
        }

        public static Bitmap CreateBitmap(this BufferData2D<int> source)
        {
            var bmp = new Bitmap(source.Width, source.Height);
            CopyTo(source, bmp);
            return bmp;
        }

    }


}