using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp.Formats.Bmp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.Primitives;

namespace Aximo
{

    public static class ImageLib
    {
        public static Image Load(string path)
        {
            if (path.ToLower().EndsWith(".tga"))
                return TgaDecoder.FromFile(path);
            else
                return Image.Load(path);
        }
    }

    public static class ImageExtensions
    {

        public unsafe static void UseData(this Image<Rgba32> image, Action<IntPtr> ptrCallback)
        {
            ref var pixels = ref MemoryMarshal.GetReference(image.GetPixelSpan());
            ptrCallback((IntPtr)Unsafe.AsPointer(ref pixels));
        }

        public unsafe static void UseData(this Image<Argb32> image, Action<IntPtr> ptrCallback)
        {
            ref var pixels = ref MemoryMarshal.GetReference(image.GetPixelSpan());
            ptrCallback((IntPtr)Unsafe.AsPointer(ref pixels));
        }

        public unsafe static void UseData(this Image<Bgra32> image, Action<IntPtr> ptrCallback)
        {
            ref var pixels = ref MemoryMarshal.GetReference(image.GetPixelSpan());
            ptrCallback((IntPtr)Unsafe.AsPointer(ref pixels));
        }

        public unsafe static void UseData(this Image<Rgb24> image, Action<IntPtr> ptrCallback)
        {
            ref var pixels = ref MemoryMarshal.GetReference(image.GetPixelSpan());
            ptrCallback((IntPtr)Unsafe.AsPointer(ref pixels));
        }

        public unsafe static void UseData(this Image<Bgr24> image, Action<IntPtr> ptrCallback)
        {
            ref var pixels = ref MemoryMarshal.GetReference(image.GetPixelSpan());
            ptrCallback((IntPtr)Unsafe.AsPointer(ref pixels));
        }

        public unsafe static void UseData(this Image image, Action<IntPtr> ptrCallback)
        {
            if (image is Image<Rgba32> rgba32)
            {
                UseData(rgba32, ptrCallback);
                return;
            }

            if (image is Image<Argb32> argb32)
            {
                UseData(argb32, ptrCallback);
                return;
            }

            if (image is Image<Bgra32> bgra32)
            {
                UseData(bgra32, ptrCallback);
                return;
            }

            if (image is Image<Rgb24> rgb24)
            {
                UseData(rgb24, ptrCallback);
                return;
            }

            if (image is Image<Bgr24> bgr24)
            {
                UseData(bgr24, ptrCallback);
                return;
            }

            throw new Exception("Not supported yet");

            // Workarround

            // var ms = new MemoryStream();
            // image.SaveAsBmp(ms);

            // var data = ms.ToArray();
            // ms.Dispose();
            // ms = null;

            // fixed (void* ptr = &data[0])
            // {
            //     ptrCallback((IntPtr)ptr);
            // }
        }

        public static void Clear(this IImageProcessingContext source, Color color)
        {
            var graphicsOptions = new GraphicsOptions
            {
                AlphaCompositionMode = PixelAlphaCompositionMode.Clear
            };
            var size = source.GetCurrentSize();
            source.Fill(graphicsOptions, color, new Rectangle(0, 0, size.Width, size.Height));
        }

    }

}