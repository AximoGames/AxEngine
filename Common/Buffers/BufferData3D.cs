// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using OpenToolkit;

namespace Aximo
{

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

        public override BufferData Clone()
        {
            throw new NotImplementedException();
        }

    }

}
