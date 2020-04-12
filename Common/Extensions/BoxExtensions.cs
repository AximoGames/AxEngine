// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using OpenToolkit.Mathematics;
using SixLabors.ImageSharp;

namespace Aximo
{
    public static class BoxExtensions
    {
        public static RectangleF ToRectangleF(this Box2 value)
        {
            return new RectangleF(value.Min.X, value.Min.Y, value.Size.X, value.Size.Y);
        }
    }
}
