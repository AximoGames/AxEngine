using System;
using System.IO;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp.Formats.Bmp;
using SixLabors.ImageSharp.PixelFormats;

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
            fixed (void* ptr = &image.DangerousGetPinnableReferenceToPixelBuffer())
            {
                ptrCallback((IntPtr)ptr);
            }
        }

        public unsafe static void UseData(this Image<Argb32> image, Action<IntPtr> ptrCallback)
        {
            fixed (void* ptr = &image.DangerousGetPinnableReferenceToPixelBuffer())
            {
                ptrCallback((IntPtr)ptr);
            }
        }

        public unsafe static void UseData(this Image<Bgra32> image, Action<IntPtr> ptrCallback)
        {
            fixed (void* ptr = &image.DangerousGetPinnableReferenceToPixelBuffer())
            {
                ptrCallback((IntPtr)ptr);
            }
        }

        public unsafe static void UseData(this Image<Rgb24> image, Action<IntPtr> ptrCallback)
        {
            fixed (void* ptr = &image.DangerousGetPinnableReferenceToPixelBuffer())
            {
                ptrCallback((IntPtr)ptr);
            }
        }

        public unsafe static void UseData(this Image<Bgr24> image, Action<IntPtr> ptrCallback)
        {
            fixed (void* ptr = &image.DangerousGetPinnableReferenceToPixelBuffer())
            {
                ptrCallback((IntPtr)ptr);
            }
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

    }

}