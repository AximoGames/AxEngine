// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using OpenToolkit;
using OpenToolkit.Mathematics;

namespace Aximo
{
    public abstract class BufferData2D : BufferData
    {
        public abstract int SizeX { get; }
        public abstract int SizeY { get; }

        public int Width => SizeX;
        public int Height => SizeY;

        public GamePixelFormat PixelFormat { get; set; }

        public abstract void FlipY();
    }
}
