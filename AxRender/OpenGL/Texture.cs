// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using OpenTK.Graphics.OpenGL4;
using PixelFormat = OpenTK.Graphics.OpenGL4.PixelFormat;

namespace Aximo.Render
{

    public class Texture : IObjectLabel
    {
        public int Handle { get; private set; }
        private TextureTarget Target = TextureTarget.Texture2D;

        private int Level;
        private PixelInternalFormat InternalFormat;
        private int Border;
        private PixelFormat Format;
        private PixelType Type;

        public Texture(TextureTarget target, int level, PixelInternalFormat internalformat, int width, int height, int border, PixelFormat format, PixelType type, IntPtr pixels)
        {
            Width = width;
            Height = height;
            Target = target;
            InternalFormat = internalformat;
            Border = border;
            Format = format;
            Type = type;

            int handle;
            GL.GenTextures(1, out handle);
            Handle = handle;
            Bind();
            AllocData();
        }

        private void AllocData()
        {
            Bind();
            GL.TexImage2D(Target, Level, InternalFormat, Width, Height, Border, Format, Type, IntPtr.Zero);
        }

        private Texture(int handle, TextureTarget target, int width, int height)
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

        public static Texture CreateCubeShadowMap(PixelInternalFormat internalformat, int width, int height, int border, PixelFormat format, PixelType type, IntPtr pixels)
        {
            int handle;
            GL.GenTextures(1, out handle);
            var txt = new Texture(handle, TextureTarget.TextureCubeMap, width, height);
            txt.Bind();
            for (var i = 0; i < 6; i++)
                GL.TexImage2D(TextureTarget.TextureCubeMapPositiveX + i, 0, PixelInternalFormat.DepthComponent, txt.Width, txt.Height, border, format, type, pixels);

            txt.SetNearestFilter();
            txt.SetClampToEdgeWrap();
            return txt;
        }

        public static Texture CreateArrayShadowMap(PixelInternalFormat internalformat, int width, int height, int layers, int border, PixelFormat format, PixelType type, IntPtr pixels)
        {
            int handle;
            GL.GenTextures(1, out handle);
            var txt = new Texture(handle, TextureTarget.Texture2DArray, width, height);
            txt.Bind();
            GL.TexImage3D(TextureTarget.Texture2DArray, 0, PixelInternalFormat.DepthComponent, width, height, layers, border, format, type, pixels);

            txt.SetNearestFilter();
            txt.SetClampToBordreWrap();
            return txt;
        }

        public static Texture CreateCubeArrayShadowMap(PixelInternalFormat internalformat, int width, int height, int layers, int border, PixelFormat format, PixelType type, IntPtr pixels)
        {
            int handle;
            GL.GenTextures(1, out handle);
            var txt = new Texture(handle, TextureTarget.TextureCubeMapArray, width, height);
            txt.Bind();
            GL.TexImage3D(TextureTarget.TextureCubeMapArray, 0, PixelInternalFormat.DepthComponent, width, height, layers * 6, border, format, type, pixels);

            txt.SetNearestFilter();
            txt.SetClampToEdgeWrap();
            return txt;
        }

        public static Texture LoadCubeMap(string path)
        {
            //var faces = new string[] { "right", "left", "back", "front", "top", "bottom" };
            //var faces = new string[] { "left", "right", "top", "top", "top", "top" };
            //var faces = new string[] { "top", "top", "top", "top", "left", "top" };
            //var faces = new string[] { "right", "left", "top", "bottom", "back", "front" };
            var faces = new string[] { "right", "left", "top", "bottom", "front", "back" };
            //var faces = new string[] { "right", "left", "front", "back", "top", "bottom" };
            var images = new List<Bitmap>();
            foreach (var face in faces)
                images.Add(LoadBitmap(path.Replace("#", face)));

            // images[1].RotateFlip(RotateFlipType.Rotate90FlipY);
            // images[3].RotateFlip(RotateFlipType.RotateNoneFlipX);
            // images[2].RotateFlip(RotateFlipType.Rotate180FlipX);
            // images[0].RotateFlip(RotateFlipType.Rotate90FlipX);
            // images[4].RotateFlip(RotateFlipType.RotateNoneFlipY);

            int handle;
            GL.GenTextures(1, out handle);
            var txt = new Texture(handle, TextureTarget.TextureCubeMap, images[0].Width, images[0].Height);
            //txt.ObjectLabel = Path.GetFileName(path);
            txt.Bind();
            txt.ObjectLabel = Path.GetFileName(path);

            for (var i = 0; i < images.Count; i++)
            {
                var data = images[i].LockBits(
                    new Rectangle(0, 0, images[i].Width, images[i].Height),
                    ImageLockMode.ReadOnly,
                    System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                GL.TexImage2D(TextureTarget.TextureCubeMapPositiveX + i, 0, PixelInternalFormat.Rgba, txt.Width, txt.Height, 0, PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);

                images[i].Dispose();
            }
            txt.SetNearestFilter();
            txt.SetClampToEdgeWrap();

            Console.WriteLine($"Loaded Cubemap #{txt.Handle} {path}");
            return txt;
        }

        public int Width { get; private set; }
        public int Height { get; private set; }

        private string _ObjectLabel;
        public string ObjectLabel { get => _ObjectLabel; set { if (_ObjectLabel == value) return; _ObjectLabel = value; ObjectManager.SetLabel(this); } }

        public ObjectLabelIdentifier ObjectLabelIdentifier => ObjectLabelIdentifier.Texture;

        public Bitmap GetTexture()
        {
            Bitmap bitmap = new Bitmap(Width, Height);
            var bits = bitmap.LockBits(new Rectangle(0, 0, Width, Height), System.Drawing.Imaging.ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format32bppRgb);
            //BindToRead(ReadBufferMode.ColorAttachment0 + AttachmentIndex);
            //GL.ReadPixels(0, 0, 800, 600, PixelFormat.Rgb, PixelType.Float, bits.Scan0);
            GL.BindTexture(Target, Handle);
            GL.GetTexImage(Target, 0, PixelFormat.Bgra, PixelType.UnsignedByte, bits.Scan0);
            bitmap.UnlockBits(bits);

            bitmap.RotateFlip(System.Drawing.RotateFlipType.RotateNoneFlipY);
            //bitmap.Save("test.bmp");

            return bitmap;
        }

        public Bitmap GetDepthTexture()
        {
            GL.BindTexture(Target, Handle);
            return DataHelper.GetDepthTexture(Width, Height, (ptr) => GL.GetTexImage(Target, 0, PixelFormat.DepthComponent, PixelType.Float, ptr));
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

        public Texture(string path)
        {
            using (var image = LoadBitmap(path))
            {
                InitFromBitmap(image);
            }
            Console.WriteLine($"Loaded Texture #{Handle} {path}");
            ObjectLabel = Path.GetFileName(path);
        }

        // Create texture from path.
        public Texture(Bitmap image)
        {
            InitFromBitmap(image);
        }

        public void SetData(Bitmap image)
        {
            Bind();
            // Load the image
            // First, we get our pixels from the bitmap we loaded.
            // Arguments:
            //   The pixel area we want. Typically, you want to leave it as (0,0) to (width,height), but you can
            //   use other rectangles to get segments of textures, useful for things such as spritesheets.
            //   The locking mode. Basically, how you want to use the pixels. Since we're passing them to OpenGL,
            //   we only need ReadOnly.
            //   Next is the pixel format we want our pixels to be in. In this case, ARGB will suffice.
            //   We have to fully qualify the name because OpenTK also has an enum named PixelFormat.
            var data = image.LockBits(
                new Rectangle(0, 0, image.Width, image.Height),
                ImageLockMode.ReadOnly,
                System.Drawing.Imaging.PixelFormat.Format32bppArgb);

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
            GL.TexImage2D(
                Target,
                0,
                PixelInternalFormat.Rgba,
                image.Width,
                image.Height,
                0,
                PixelFormat.Bgra,
                PixelType.UnsignedByte,
                data.Scan0);

            image.UnlockBits(data);

            // Next, generate mipmaps.
            // Mipmaps are smaller copies of the texture, scaled down. Each mipmap level is half the size of the previous one
            // Generated mipmaps go all the way down to just one pixel.
            // OpenGL will automatically switch between mipmaps when an object gets sufficiently far away.
            // This prevents distant objects from having their colors become muddy, as well as saving on memory.
            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
        }

        private void InitFromBitmap(Bitmap image)
        {
            Target = TextureTarget.Texture2D;
            // Generate handle
            Handle = GL.GenTexture();

            // Bind the handle
            Bind();

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

        private static Bitmap LoadBitmap(string path)
        {
            var imagePath = DirectoryHelper.GetAssetsPath(path);
            Console.WriteLine(imagePath);
            if (path.EndsWith(".tga"))
                return TgaDecoder.FromFile(imagePath);
            else
                return new Bitmap(imagePath);
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
    }
}
