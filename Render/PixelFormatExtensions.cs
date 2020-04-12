// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using OpenToolkit;
using OpenToolkit.Graphics.OpenGL4;

namespace Aximo.Render
{
    public static class PixelFormatExtensions
    {
        public static GamePixelFormat ToGamePixelFormat(this PixelFormat format)
        {
            switch (format)
            {
                case PixelFormat.Rgba:
                    return GamePixelFormat.Rgba32;
                case PixelFormat.Bgra:
                    return GamePixelFormat.Bgra32;
                case PixelFormat.Rgb:
                    return GamePixelFormat.Rgb24;
                case PixelFormat.Bgr:
                    return GamePixelFormat.Bgr24;
                default:
                    return GamePixelFormat.None;
            }
        }
    }
}
