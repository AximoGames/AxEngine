// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Runtime.InteropServices;

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

        public abstract BufferData Clone();
    }
}
