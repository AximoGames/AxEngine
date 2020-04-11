using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp.Drawing;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Formats.Bmp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

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
            };
            //if (color == Color.Transparent)
            graphicsOptions.AlphaCompositionMode = PixelAlphaCompositionMode.Src;
            //graphicsOptions.ColorBlendingMode=PixelColorBlendingMode.

            var size = source.GetCurrentSize();
            //source.Fill(graphicsOptions, Color.Transparent, new RectangleF(0, 0, size.Width, size.Height));
            //graphicsOptions.AlphaCompositionMode = PixelAlphaCompositionMode.Clear;
            source.Fill(graphicsOptions, color, new RectangleF(0, 0, size.Width, size.Height));
        }

        public static IImageProcessingContext DrawButton(this IImageProcessingContext processingContext, float borderTickness, Color borderColor, float cornerRadius)
        {
            var size = processingContext.GetCurrentSize();
            var options = new ShapeGraphicsOptions();
            processingContext.DrawLines(options, borderColor, borderTickness * 2, new PointF(0, 0), new PointF(size.Width, 0), new PointF(size.Width, size.Height), new PointF(0, size.Height), new PointF(0, 0));
            return processingContext.ApplyRoundedCorners(borderTickness, borderColor, cornerRadius);
        }

        // This method can be seen as an inline implementation of an `IImageProcessor`:
        // (The combination of `IImageOperations.Apply()` + this could be replaced with an `IImageProcessor`)
        private static IImageProcessingContext ApplyRoundedCorners(this IImageProcessingContext ctx, float borderThickness, Color borderColor, float cornerRadius)
        {
            Size size = ctx.GetCurrentSize();
            var corners = BuildCorners(size.Width, size.Height, cornerRadius);

            ctx.Draw(borderColor, borderThickness * 2, corners);

            var graphicOptions = new GraphicsOptions()
            {
                AlphaCompositionMode = PixelAlphaCompositionMode.DestOut, // enforces that any part of this shape that has color is punched out of the background
            };
            // mutating in here as we already have a cloned original
            // use any color (not Transparent), so the corners will be clipped
            return ctx.Fill(graphicOptions, Color.LimeGreen, new PathCollection(corners));
        }

        private static IPathCollection BuildCorners(int imageWidth, int imageHeight, float cornerRadius)
        {
            // first create a square
            var rect = new RectangularPolygon(-0.5f, -0.5f, cornerRadius, cornerRadius);

            // then cut out of the square a circle so we are left with a corner
            IPath cornerTopLeft = rect.Clip(new EllipsePolygon(cornerRadius - 0.5f, cornerRadius - 0.5f, cornerRadius));

            // corner is now a corner shape positions top left
            //lets make 3 more positioned correctly, we can do that by translating the original around the center of the image

            float rightPos = imageWidth - cornerTopLeft.Bounds.Width + 1;
            float bottomPos = imageHeight - cornerTopLeft.Bounds.Height + 1;

            // move it across the width of the image - the width of the shape
            IPath cornerTopRight = cornerTopLeft.RotateDegree(90).Translate(rightPos, 0);
            IPath cornerBottomLeft = cornerTopLeft.RotateDegree(-90).Translate(0, bottomPos);
            IPath cornerBottomRight = cornerTopLeft.RotateDegree(180).Translate(rightPos, bottomPos);

            return new PathCollection(cornerTopLeft, cornerBottomLeft, cornerTopRight, cornerBottomRight);
        }

    }

}