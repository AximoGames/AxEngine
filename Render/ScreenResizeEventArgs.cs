// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Aximo.Render;
using OpenToolkit.Mathematics;
using OpenToolkit.Windowing.Common;
using OpenToolkit.Windowing.Common.Input;

namespace Aximo
{
    public class ScreenResizeEventArgs
    {
        public Vector2i OldSize { get; private set; }
        public Vector2i Size { get; private set; }

        internal ScreenResizeEventArgs(Vector2i oldSize, Vector2i size)
        {
            OldSize = oldSize;
            Size = size;
        }
    }
}
