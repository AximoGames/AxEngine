// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using OpenToolkit.Mathematics;

namespace Aximo
{
    public static class VectorExtensions
    {
        public static Vector2i InvertY(this Vector2i value)
        {
            return new Vector2i(value.X, -value.Y);
        }

        public static Vector2 InvertY(this Vector2 value)
        {
            return new Vector2(value.X, -value.Y);
        }

    }

}
