// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using OpenToolkit.Graphics.OpenGL4;

namespace Aximo.Render
{
    public static class PixelFormatExtensions
    {
        public static AxPixelFormat ToGamePixelFormat(this PixelFormat format)
        {
            switch (format)
            {
                case PixelFormat.Rgba:
                    return AxPixelFormat.Rgba32;
                case PixelFormat.Bgra:
                    return AxPixelFormat.Bgra32;
                case PixelFormat.Rgb:
                    return AxPixelFormat.Rgb24;
                case PixelFormat.Bgr:
                    return AxPixelFormat.Bgr24;
                default:
                    return AxPixelFormat.None;
            }
        }
    }
}
