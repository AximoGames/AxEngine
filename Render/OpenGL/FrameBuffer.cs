// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using OpenToolkit.Graphics.OpenGL4;

namespace Aximo.Render
{

    public class FrameBuffer : IObjectLabel
    {

        private static FrameBuffer _Default;
        public static FrameBuffer Default
        {
            get
            {
                if (_Default == null)
                    _Default = FrameBuffer.CreateFromHandle(0, RenderContext.Current.ScreenSize.X, RenderContext.Current.ScreenSize.Y);
                return _Default;
            }
        }

        public List<Texture> DestinationTextures = new List<Texture>();

        public Texture GetDestinationTexture()
        {
            return DestinationTextures[0];
        }

        public Texture GetDestinationTexture(int attachmentIndex)
        {
            return DestinationTextures[attachmentIndex];
        }

        private int _Handle;
        public int Handle => _Handle;

        private string _ObjectLabel;
        public string ObjectLabel { get => _ObjectLabel; set { _ObjectLabel = value; ObjectManager.SetLabel(this); } }

        public ObjectLabelIdentifier ObjectLabelIdentifier => ObjectLabelIdentifier.Framebuffer;

        public int Width { get; private set; }
        public int Height { get; private set; }

        public void GetData(BufferData2D<int> target)
        {
            GL.ReadBuffer(ReadBufferMode.Back);
            DataHelper.GetData(target, (ptr) => GL.ReadPixels(0, 0, Width, Height, PixelFormat.DepthComponent, PixelType.UnsignedByte, ptr));
            GL.ReadBuffer(ReadBufferMode.None);

            // Bitmap bitmap = new Bitmap(Width, Height);
            // var bits = bitmap.LockBits(new Rectangle(0, 0, Width, Height), System.Drawing.Imaging.ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format32bppRgb);
            // //BindToRead(ReadBufferMode.ColorAttachment0 + AttachmentIndex);
            // GL.ReadPixels(0, 0, Width, Height, PixelFormat.Bgra, PixelType.UnsignedByte, bits.Scan0);
            // //GL.BindTexture(TextureTarget.Texture2D, texColorBuffer);
            // //GL.GetTexImage(TextureTarget.Texture2D, 0, PixelFormat.Rgb, PixelType.UnsignedByte, bits.Scan0);
            // bitmap.UnlockBits(bits);

            // //bitmap.RotateFlip(System.Drawing.RotateFlipType.RotateNoneFlipY);
            // //bitmap.Save("test.bmp");

            // return bitmap;
        }

        public void GetDepthData(BufferData2D<float> target)
        {
            DataHelper.GetDepthData(target, (ptr) => GL.ReadPixels(0, 0, Width, Height, PixelFormat.DepthComponent, PixelType.UnsignedByte, ptr));
        }

        public FrameBuffer(int width, int height)
        {
            Width = width;
            Height = height;

            GL.GenFramebuffers(1, out _Handle);
            Bind();
        }

        private FrameBuffer(int handle, int width, int height)
        {
            _Handle = handle;
            Width = width;
            Height = height;
        }

        public static FrameBuffer CreateFromHandle(int handle, int width, int height)
        {
            return new FrameBuffer(handle, width, height);
        }

        public void InitNormal()
        {
            var txt = new Texture(ObjectLabel, TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, Width, Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, IntPtr.Zero);
            txt.SetLinearFilter();
            txt.Bind();
            DestinationTextures.Add(txt);

            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D, txt.Handle, 0);

            // Bind();
            // GL.GetFramebufferParameter(FramebufferTarget.Framebuffer, (FramebufferDefaultParameter)All.ImplementationColorReadFormat, out int value);

            //if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            txt.SetPixelFormat(PixelFormat.Bgra);

            Check();
        }

        public void Resize(int width, int height)
        {
            //return;
            Width = width;
            Height = height;
            foreach (var txt in DestinationTextures)
                txt.Resize(width, height);

            if (RenderBuffer != null)
                RenderBuffer.Resize(this);
        }

        public void BindTexture(Texture txt, FramebufferAttachment attachment)
        {
            Bind();
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, attachment, TextureTarget.Texture2D, txt.Handle, 0);
        }

        public void InitDepth()
        {
            var layers = 2;

            var txt = Texture.CreateArrayShadowMap(PixelInternalFormat.DepthComponent, Width, Height, layers, 0, PixelFormat.DepthComponent, PixelType.Float, IntPtr.Zero);
            txt.Bind();
            DestinationTextures.Add(txt);

            GL.FramebufferTexture(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, txt.Handle, 0);
            GL.DrawBuffer(DrawBufferMode.None);
            GL.ReadBuffer(ReadBufferMode.None);

            Check();
        }

        public void InitCubeDepth()
        {
            var layers = 2;

            var txt = Texture.CreateCubeArrayShadowMap(PixelInternalFormat.DepthComponent, Width, Height, layers, 0, PixelFormat.DepthComponent, PixelType.Float, IntPtr.Zero);
            txt.Bind();
            DestinationTextures.Add(txt);

            GL.FramebufferTexture(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, txt.Handle, 0);
            GL.DrawBuffer(DrawBufferMode.None);
            GL.ReadBuffer(ReadBufferMode.None);

            Check();
        }

        public void Check()
        {
            var status = GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer);
            if (status != FramebufferErrorCode.FramebufferComplete)
                throw new Exception(status.ToString());
        }

        public RenderBuffer RenderBuffer { get; private set; }

        public void CreateRenderBuffer(RenderbufferStorage renderbufferStorage, FramebufferAttachment framebufferAttachment)
        {
            RenderBuffer = new RenderBuffer(this, renderbufferStorage, framebufferAttachment);
        }

        public void Bind()
        {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, _Handle);
        }

    }

}
