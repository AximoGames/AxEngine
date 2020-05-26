// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Aximo
{
    public abstract class BufferData3D : BufferData
    {
        public abstract int SizeX { get; }
        public abstract int SizeY { get; }
        public abstract int SizeZ { get; }
    }
}
