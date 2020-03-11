// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using OpenTK;

namespace Aximo.Render
{
    public class GTextureSource
    {

        public int Width => Size.X;
        public int Height => Size.Y;

        public Vector2i Size { get; private set; }

    }

}
