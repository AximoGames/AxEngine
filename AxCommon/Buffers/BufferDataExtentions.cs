// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using OpenToolkit;
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

        public static unsafe void SetData(this BufferData2D<int> target, System.Drawing.Bitmap source)
        {
            var targetData = new int[source.Width, source.Height];
            target.SetData(targetData);
            var lockedBits = source.LockBits(new System.Drawing.Rectangle(0, 0, source.Width, source.Height), ImageLockMode.ReadOnly, source.PixelFormat);
            var destHandle = target.CreateHandle();
            try
            {
                Buffer.MemoryCopy((void*)lockedBits.Scan0, (void*)destHandle.AddrOfPinnedObject(), target.Bytes, target.Bytes);
            }
            finally
            {
                source.UnlockBits(lockedBits);
                destHandle.Free();
            }
        }

        public static unsafe void CopyTo(this BufferData2D<int> source, System.Drawing.Bitmap bmp)
        {
            var lockedBits = bmp.LockBits(new System.Drawing.Rectangle(0, 0, source.SizeX, source.SizeY), ImageLockMode.ReadOnly, bmp.PixelFormat);
            var destHandle = source.CreateHandle();
            try
            {
                Buffer.MemoryCopy((void*)destHandle.AddrOfPinnedObject(), (void*)lockedBits.Scan0, source.Bytes, source.Bytes);
            }
            finally
            {
                bmp.UnlockBits(lockedBits);
                destHandle.Free();
            }
            bmp.RotateFlip(System.Drawing.RotateFlipType.RotateNoneFlipY);
        }

        public static System.Drawing.Bitmap CreateBitmap(this BufferData2D<int> source)
        {
            var bmp = new System.Drawing.Bitmap(source.Width, source.Height);
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
