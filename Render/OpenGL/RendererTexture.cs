﻿// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.IO;
using Aximo.Render.Objects;
using OpenToolkit.Graphics.OpenGL4;
using OpenToolkit.Mathematics;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using Image = SixLabors.ImageSharp.Image;

namespace Aximo.Render.OpenGL
{
    public class RendererTexture : RenderObjectBase, IObjectLabel, IDisposable
    {
        private static Serilog.ILogger Log = Aximo.Log.ForContext<RendererTexture>();

        public int Handle { get; private set; }
        private TextureTarget Target = TextureTarget.Texture2D;

        private int Level;
        private PixelInternalFormat InternalFormat;
        private int Border;
        public PixelFormat Format { get; private set; }
        private PixelType Type;

        public void SetPixelFormat(PixelFormat pixelFormat)
        {
            Format = pixelFormat;
        }

        private void AddRef()
        {
            // if (ObjectLabel.IsUnset())
            //     throw new Exception("ObjectLabel not set!");

            Log.Verbose("Alloc Texture #{Handle} {ObjectLabel}", Handle, ObjectLabel);
            InternalTextureManager.AddRef(this);
        }

        private void RemoveRef()
        {
            InternalTextureManager.RemoveRef(this);
            Log.Verbose("Free Texture #{Handle} {ObjectLabel}", Handle, ObjectLabel);
        }

        public RendererTexture(string objectLabel, TextureTarget target, int level, PixelInternalFormat internalformat, int width, int height, int border, PixelFormat format, PixelType type, IntPtr pixels)
        {
            Width = width;
            Height = height;
            Target = target;
            InternalFormat = internalformat;
            Border = border;
            Format = format;
            Type = type;
            Level = level;

            int handle;
            GL.GenTextures(1, out handle);
            Handle = handle;
            Bind();
            ObjectLabel = objectLabel;
            AddRef();
            AllocData();
        }

        private void AllocData()
        {
            Bind();
            GL.TexImage2D(Target, Level, InternalFormat, Width, Height, Border, Format, Type, IntPtr.Zero);
        }

        private RendererTexture(int handle, TextureTarget target, int width, int height)
        {
            Handle = handle;
            Target = target;
            Width = width;
            Height = height;
        }

        public void Resize(int width, int height)
        {
            Width = width;
            Height = height;
            AllocData();
        }

        public static RendererTexture CreateCubeShadowMap(PixelInternalFormat internalformat, int width, int height, int border, PixelFormat format, PixelType type, IntPtr pixels)
        {
            int handle;
            GL.GenTextures(1, out handle);
            var txt = new RendererTexture(handle, TextureTarget.TextureCubeMap, width, height);
            txt.Bind();
            txt.ObjectLabel = "CubeShadowMap";
            txt.AddRef();
            for (var i = 0; i < 6; i++)
                GL.TexImage2D(TextureTarget.TextureCubeMapPositiveX + i, 0, PixelInternalFormat.DepthComponent16, txt.Width, txt.Height, border, format, type, pixels);

            txt.SetNearestFilter();
            txt.SetClampToEdgeWrap();
            return txt;
        }

        public static RendererTexture CreateArrayShadowMap(PixelInternalFormat internalformat, int width, int height, int layers, int border, PixelFormat format, PixelType type, IntPtr pixels)
        {
            int handle;
            GL.GenTextures(1, out handle);
            var txt = new RendererTexture(handle, TextureTarget.Texture2DArray, width, height);
            txt.Bind();
            txt.ObjectLabel = "ShadowMapArray";
            txt.AddRef();
            GL.TexImage3D(TextureTarget.Texture2DArray, 0, PixelInternalFormat.DepthComponent16, width, height, layers, border, format, type, pixels);

            txt.SetNearestFilter();
            txt.SetClampToBordreWrap();
            txt.SetBorderColor();

            return txt;
        }

        public static RendererTexture CreateCubeArrayShadowMap(PixelInternalFormat internalformat, int width, int height, int layers, int border, PixelFormat format, PixelType type, IntPtr pixels)
        {
            int handle;
            GL.GenTextures(1, out handle);
            var txt = new RendererTexture(handle, TextureTarget.TextureCubeMapArray, width, height);
            txt.Bind();
            txt.ObjectLabel = "CubeShadowMapArray";
            txt.AddRef();
            GL.TexImage3D(TextureTarget.TextureCubeMapArray, 0, PixelInternalFormat.DepthComponent16, width, height, layers * 6, border, format, type, pixels);

            txt.SetNearestFilter();
            txt.SetClampToEdgeWrap();
            return txt;
        }

        public static unsafe RendererTexture LoadCubeMap(string path)
        {
            //var faces = new string[] { "right", "left", "back", "front", "top", "bottom" };
            //var faces = new string[] { "left", "right", "top", "top", "top", "top" };
            //var faces = new string[] { "top", "top", "top", "top", "left", "top" };
            //var faces = new string[] { "right", "left", "top", "bottom", "back", "front" };
            var faces = new string[] { "right", "left", "top", "bottom", "front", "back" };
            //var faces = new string[] { "right", "left", "front", "back", "top", "bottom" };
            var images = new List<Image>();
            foreach (var face in faces)
                images.Add(LoadBitmap(path.Replace("#", face)));

            var name = Path.GetDirectoryName(path);

            // images[1].RotateFlip(RotateFlipType.Rotate90FlipY);
            // images[3].RotateFlip(RotateFlipType.RotateNoneFlipX);
            // images[2].RotateFlip(RotateFlipType.Rotate180FlipX);
            // images[0].RotateFlip(RotateFlipType.Rotate90FlipX);
            // images[4].RotateFlip(RotateFlipType.RotateNoneFlipY);

            int handle;
            GL.GenTextures(1, out handle);
            var txt = new RendererTexture(handle, TextureTarget.TextureCubeMap, images[0].Width, images[0].Height);
            //txt.ObjectLabel = Path.GetFileName(path);
            txt.Bind();
            txt.ObjectLabel = name;
            txt.AddRef();

            for (var i = 0; i < images.Count; i++)
            {
                images[i].UseData(ptr =>
                {
                    GL.TexImage2D(TextureTarget.TextureCubeMapPositiveX + i, 0, PixelInternalFormat.Rgba, txt.Width, txt.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, ptr);
                });
                images[i].Dispose();
            }
            txt.SetNearestFilter();
            txt.SetClampToEdgeWrap();

            //Console.WriteLine($"Loaded Cubemap #{txt.Handle} {path}");
            return txt;
        }

        public int Width { get; private set; }
        public int Height { get; private set; }

        private string _ObjectLabel;
        public string ObjectLabel { get => _ObjectLabel; set { if (_ObjectLabel == value) return; _ObjectLabel = value; ObjectManager.SetLabel(this); } }

        public ObjectLabelIdentifier ObjectLabelIdentifier => ObjectLabelIdentifier.Texture;

        public void GetTexture(BufferData2D<int> data)
        {
            GL.BindTexture(Target, Handle);

            data.Resize(Width, Height);

            var handle = data.CreateHandle();
            try
            {
                GL.GetTexImage(Target, 0, PixelFormat.Rgba, PixelType.UnsignedByte, handle.AddrOfPinnedObject());
            }
            finally
            {
                handle.Free();
            }
        }

        public void GetDepthTexture(BufferData2D<float> target, bool normalize = false)
        {
            GL.BindTexture(Target, Handle);
            DataHelper.GetDepthData(target, (ptr) => GL.GetTexImage(Target, 0, PixelFormat.DepthComponent, PixelType.Float, ptr));
        }

        public void SetLinearFilter()
        {
            Bind();
            GL.TexParameter(Target, TextureParameterName.TextureMinFilter, (int)All.Linear);
            GL.TexParameter(Target, TextureParameterName.TextureMagFilter, (int)All.Linear);
        }

        public void SetNearestFilter()
        {
            Bind();
            GL.TexParameter(Target, TextureParameterName.TextureMinFilter, (int)All.Nearest);
            GL.TexParameter(Target, TextureParameterName.TextureMagFilter, (int)All.Nearest);
        }

        public void SetClampToBordreWrap()
        {
            Bind();
            GL.TexParameter(Target, TextureParameterName.TextureWrapS, (int)All.ClampToBorder);
            GL.TexParameter(Target, TextureParameterName.TextureWrapT, (int)All.ClampToBorder);
        }

        public void SetClampToEdgeWrap()
        {
            Bind();
            GL.TexParameter(Target, TextureParameterName.TextureWrapS, (int)All.ClampToEdge);
            GL.TexParameter(Target, TextureParameterName.TextureWrapT, (int)All.ClampToEdge);
            GL.TexParameter(Target, TextureParameterName.TextureWrapR, (int)All.ClampToEdge);
        }

        public void SetBorderColor()
        {
            Bind();
            float[] borderColor = new float[] { 1.0f, 1.0f, 1.0f, 1.0f };
            GL.TexParameter(Target, TextureParameterName.TextureBorderColor, borderColor);
        }

        public RendererTexture(Vector3 color, string name)
        {
            using (var bmp = new Image<Rgba32>(1, 1))
            {
                bmp[0, 0] = new Rgba32((int)Math.Round(color.X * 255), (int)Math.Round(color.Y * 255), (int)Math.Round(color.Z * 255));
                InitFromBitmap(bmp, "Color/" + name);
            }
        }

        // Create texture from path.
        public RendererTexture(Image image, string name)
        {
            InitFromBitmap(image, name);
        }

        public void SetData(Image image)
        {
            Bind();

            // Now that our pixels are prepared, it's time to generate a texture. We do this with GL.TexImage2D
            // Arguments:
            //   The type of texture we're generating. There are various different types of textures, but the only one we need right now is Texture2D.
            //   Level of detail. We can use this to start from a smaller mipmap (if we want), but we don't need to do that, so leave it at 0.
            //   Target format of the pixels. This is the format OpenGL will store our image with.
            //   Width of the image
            //   Height of the image.
            //   Border of the image. This must always be 0; it's a legacy parameter that Khronos never got rid of.
            //   The format of the pixels, explained above. Since we loaded the pixels as ARGB earlier, we need to use BGRA.
            //   Data type of the pixels.
            //   And finally, the actual pixels.

            image.UseData(ptr =>
            {
                GL.TexImage2D(
                    Target,
                    0,
                    PixelInternalFormat.Rgba,
                    image.Width,
                    image.Height,
                    0,
                    PixelFormat.Rgba,
                    PixelType.UnsignedByte,
                    ptr);
            });

            // Next, generate mipmaps.
            // Mipmaps are smaller copies of the texture, scaled down. Each mipmap level is half the size of the previous one
            // Generated mipmaps go all the way down to just one pixel.
            // OpenGL will automatically switch between mipmaps when an object gets sufficiently far away.
            // This prevents distant objects from having their colors become muddy, as well as saving on memory.
            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
        }

        private void InitFromBitmap(Image image, string objectLabel)
        {
            Target = TextureTarget.Texture2D;
            // Generate handle
            Handle = GL.GenTexture();

            // Bind the handle
            Bind();
            ObjectLabel = objectLabel;
            AddRef();

            // Now that our texture is loaded, we can set a few settings to affect how the image appears on rendering.

            // First, we set the min and mag filter. These are used for when the texture is scaled down and up, respectively.
            // Here, we use Linear for both. This means that OpenGL will try to blend pixels, meaning that textures scaled too far will look blurred.
            // You could also use (amongst other options) Nearest, which just grabs the nearest pixel, which makes the texture look pixelated if scaled too far.
            // NOTE: The default settings for both of these are LinearMipmap. If you leave these as default but don't generate mipmaps,
            // your image will fail to render at all (usually resulting in pure black instead).
            GL.TexParameter(Target, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(Target, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

            // Now, set the wrapping mode. S is for the X axis, and T is for the Y axis.
            // We set this to Repeat so that textures will repeat when wrapped. Not demonstrated here since the texture coordinates exactly match
            GL.TexParameter(Target, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GL.TexParameter(Target, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);

            SetData(image);
        }

        private static Image LoadBitmap(string path)
        {
            var imagePath = AssetManager.GetAssetsPath(path);
            //Console.WriteLine(imagePath);
            return Image.Load(imagePath);
        }

        // Activate texture
        // Multiple textures can be bound, if your shader needs more than just one.
        // If you want to do that, use GL.ActiveTexture to set which slot GL.BindTexture binds to.
        // The OpenGL standard requires that there be at least 16, but there can be more depending on your graphics card.
        public void Bind(TextureUnit unit = TextureUnit.Texture0)
        {
            GL.ActiveTexture(unit);
            GL.BindTexture(Target, Handle);
        }

        public void Bind(int textureUnit)
        {
            Bind(TextureUnit.Texture0 + textureUnit);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (Handle == 0)
                    return;
                RemoveRef();
                var h = Handle;
                Bind();
                //GL.DeleteTextures(1, ref h);
                //Handle = 0;
            }
            base.Dispose(disposing);
        }

        public override void Init()
        {
        }

        public override void Free()
        {
        }
    }
}
