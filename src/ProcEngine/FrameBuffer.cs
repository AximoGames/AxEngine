using OpenTK.Graphics.OpenGL4;
using System;
using System.Drawing;
//using System.Drawing.Imaging;

namespace ProcEngine
{
    public class FrameBuffer
    {
        public static int texColorBuffer;
        public static int bufNum;

        public static Bitmap GetTexture(int AttachmentIndex)
        {
            Bitmap bitmap = new Bitmap(800, 600);
            var bits = bitmap.LockBits(new Rectangle(0, 0, 800, 600), System.Drawing.Imaging.ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format32bppRgb);
            //BindToRead(ReadBufferMode.ColorAttachment0 + AttachmentIndex);
            //GL.ReadPixels(0, 0, 800, 600, PixelFormat.Rgb, PixelType.Float, bits.Scan0);
            GL.BindTexture(TextureTarget.Texture2D, texColorBuffer);
            GL.GetTexImage(TextureTarget.Texture2D, 0, PixelFormat.Bgra, PixelType.UnsignedByte, bits.Scan0);
            bitmap.UnlockBits(bits);

            bitmap.Save("test.bmp");

            return bitmap;
        }

        public static Bitmap GetTexture2(int AttachmentIndex)
        {
            Bitmap bitmap = new Bitmap(800, 600);
            var bits = bitmap.LockBits(new Rectangle(0, 0, 800, 600), System.Drawing.Imaging.ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format32bppRgb);
            //BindToRead(ReadBufferMode.ColorAttachment0 + AttachmentIndex);
            GL.ReadPixels(0, 0, 800, 600, PixelFormat.Bgra, PixelType.UnsignedByte, bits.Scan0);
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

        public void Init()
        {
            GL.GenFramebuffers(1, out bufNum);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, bufNum);
            GL.GenTextures(1, out texColorBuffer);
            GL.BindTexture(TextureTarget.Texture2D, texColorBuffer);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb, 800, 600, 0, PixelFormat.Rgb, PixelType.UnsignedByte, IntPtr.Zero);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)All.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)All.Linear);
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D, texColorBuffer, 0);

            int rbo;
            GL.GenRenderbuffers(1, out rbo);
            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, rbo);
            GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, RenderbufferStorage.Depth24Stencil8, 800, 600);
            GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthStencilAttachment, RenderbufferTarget.Renderbuffer, rbo);
            if (GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer) != FramebufferErrorCode.FramebufferComplete)
                throw new Exception();
        }
    }

}
