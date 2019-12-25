using LearnOpenTK.Common;
using OpenTK.Graphics.OpenGL4;
using System;
using System.Drawing;
//using System.Drawing.Imaging;

namespace ProcEngine
{

    public class RenderBuffer
    {

        private int _Handle;
        public int Handle => _Handle;

        public RenderBuffer(FrameBuffer fb)
        {
            fb.Use();

            GL.GenRenderbuffers(1, out _Handle);
            Use();
            GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, RenderbufferStorage.Depth24Stencil8, 800, 600);
            GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthStencilAttachment, RenderbufferTarget.Renderbuffer, _Handle);
        }

        public void Use()
        {
            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, _Handle);
        }

    }

    public class FrameBuffer
    {
        public static Texture txt;
        public static int bufNum;

        public int Width { get; private set; }
        public int Height { get; private set; }

        public Bitmap GetTexture()
        {
            Bitmap bitmap = new Bitmap(Width, Height);
            var bits = bitmap.LockBits(new Rectangle(0, 0, Width, Height), System.Drawing.Imaging.ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format32bppRgb);
            //BindToRead(ReadBufferMode.ColorAttachment0 + AttachmentIndex);
            GL.ReadPixels(0, 0, Width, Height, PixelFormat.Bgra, PixelType.UnsignedByte, bits.Scan0);
            //GL.BindTexture(TextureTarget.Texture2D, texColorBuffer);
            //GL.GetTexImage(TextureTarget.Texture2D, 0, PixelFormat.Rgb, PixelType.UnsignedByte, bits.Scan0);
            bitmap.UnlockBits(bits);

            bitmap.Save("test.bmp");

            return bitmap;
        }

        private static void BindToRead(ReadBufferMode Mode)
        {
            GL.BindFramebuffer(FramebufferTarget.ReadFramebuffer, bufNum);
            GL.ReadBuffer(Mode);
        }

        public FrameBuffer(int width, int height)
        {
            Width = width;
            Height = height;
        }

        public void Init()
        {
            GL.GenFramebuffers(1, out bufNum);
            Use();

            txt = new Texture(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb, Width, Height, 0, PixelFormat.Rgb, PixelType.UnsignedByte, IntPtr.Zero);
            txt.SetLinearFilter();
            txt.Use();

            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D, txt.Handle, 0);



            if (GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer) != FramebufferErrorCode.FramebufferComplete)
                throw new Exception();
        }

        public RenderBuffer RenderBuffer { get; private set; }

        public void CreateRenderBuffer()
        {
            RenderBuffer = new RenderBuffer(this);
        }

        public void Use()
        {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, bufNum);
        }

    }

}
