// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using OpenToolkit;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp.PixelFormats;

namespace Aximo
{
    public static class BufferDataExtentions
    {
        public static void Normalize(this BufferData2D<float> target)
        {
            var collection = target.Where(v => v != 1);
            var min = collection.Min();
            var max = collection.Max();
            var span = max - min;

            var factor = 1.0f / span;

            for (var y = 0; y < target.SizeY; y++)
            {
                for (var x = 0; x < target.SizeX; x++)
                {
                    var value = target[x, y];
                    if (value != 1.0f)
                    {
                        value -= min;
                        value *= factor;
                        //value = 1 - value;
                        //value *= 300;
                        //value = 1 - value;
                        value = Math.Max(0, Math.Min(value, 1f));
                        target[x, y] = value;
                    }
                }
            }
        }

        public static void FlipY<TPixel>(this Image<TPixel> array)
            where TPixel : unmanaged, IPixel<TPixel>
        {
            // TODO: Copy complete Row

            int rows = array.Height;
            int cols = array.Width;

            for (int i = 0; i < cols; i++)
            {
                int start = 0;
                int end = rows - 1;
                while (start < end)
                {
                    TPixel temp = array[i, start];
                    array[i, start] = array[i, end];
                    array[i, end] = temp;
                    start++;
                    end--;
                }
            }
        }

        public static unsafe void CopyTo<T>(this BufferData2D<T> source, Image destination)
            where T : struct
        {
            // var copy = (BufferData2D<int>)source.Clone();
            // if (copy.PixelFormat == GamePixelFormat.Bgra32)
            //     copy.ConvertBgraToRgba();

            if (destination is Image<Rgba32> destinationRgba)
            {
                var sourceSpan = source.Span;
                var sourceIntSpan = MemoryMarshal.Cast<T, int>(sourceSpan);
#pragma warning disable CS0618 // Type or member is obsolete
                var destColorSpan = destinationRgba.GetPixelSpan();
#pragma warning restore CS0618 // Type or member is obsolete
                var destIntSpan = MemoryMarshal.Cast<Rgba32, int>(destColorSpan);
                sourceIntSpan.CopyTo(destIntSpan);
                destinationRgba.FlipY();

                return;
            }

            throw new NotImplementedException();
        }

        public static Image CreateBitmap(this BufferData2D<int> source)
        {
            var bmp = new Image<Rgba32>(source.Width, source.Height);
            CopyTo(source, bmp);
            return bmp;
        }

        public static void ConvertBgraToRgba<T>(this BufferData2D<T> source)
            where T : struct
        {
            var intArray = source.Span;
            var bgrArray = MemoryMarshal.Cast<T, Bgra32>(intArray);
            var rgbArray = MemoryMarshal.Cast<T, Rgba32>(intArray);
            for (var i = 0; i < source.Length; i++)
                rgbArray[i].FromBgra32(bgrArray[i]);
        }
    }
}
