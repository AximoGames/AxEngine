using OpenTK.Graphics.OpenGL4;
using System;
using System.Drawing;
//using System.Drawing.Imaging;

namespace ProcEngine
{

    public class FrameBuffer : IObjectLabel
    {
        [Obsolete("Framebuffer can have multiple Targets")]
        private Texture _DestinationTexture;
        [Obsolete("Framebuffer can have multiple Targets")]
        public Texture DestinationTexture => _DestinationTexture;

        private int _Handle;
        public int Handle => _Handle;

        private string _ObjectLabel;
        public string ObjectLabel { get => _ObjectLabel; set { _ObjectLabel = value; ObjectManager.SetLabel(this); } }

        public ObjectLabelIdentifier ObjectLabelIdentifier => ObjectLabelIdentifier.Framebuffer;

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

            bitmap.RotateFlip(System.Drawing.RotateFlipType.RotateNoneFlipY);
            //bitmap.Save("test.bmp");

            return bitmap;
        }

        public Bitmap GetDepthTexture()
        {
            return DataHelper.GetDepthTexture(Width, Height, (ptr) => GL.ReadPixels(0, 0, Width, Height, PixelFormat.DepthComponent, PixelType.UnsignedByte, ptr));
        }

        public FrameBuffer(int width, int height)
        {
            Width = width;
            Height = height;
        }

        public void InitNormal()
        {
            GL.GenFramebuffers(1, out _Handle);
            Use();

            _DestinationTexture = new Texture(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb, Width, Height, 0, PixelFormat.Rgb, PixelType.UnsignedByte, IntPtr.Zero);
            _DestinationTexture.SetLinearFilter();
            _DestinationTexture.Use();

            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D, _DestinationTexture.Handle, 0);

            Check();
        }

        public void BindTexture(Texture txt)
        {
            Use();
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D, txt.Handle, 0);
        }

        public void InitDepth()
        {
            GL.GenFramebuffers(1, out _Handle);
            Use();

            var layers = 1;

            _DestinationTexture = Texture.CreateArrayShadowMap(PixelInternalFormat.DepthComponent, Width, Height, layers, 0, PixelFormat.DepthComponent, PixelType.Float, IntPtr.Zero);
            _DestinationTexture.Use();

            GL.FramebufferTexture(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, _DestinationTexture.Handle, 0);
            GL.DrawBuffer(DrawBufferMode.None);
            GL.ReadBuffer(ReadBufferMode.None);

            Check();
        }

        public void InitCubeDepth()
        {
            GL.GenFramebuffers(1, out _Handle);
            Use();

            var layers = 2;

            _DestinationTexture = Texture.CreateCubeArrayShadowMap(PixelInternalFormat.DepthComponent, Width, Height, layers, 0, PixelFormat.DepthComponent, PixelType.Float, IntPtr.Zero);
            _DestinationTexture.Use();

            GL.FramebufferTexture(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, _DestinationTexture.Handle, 0);
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

        public void CreateRenderBuffer()
        {
            RenderBuffer = new RenderBuffer(this);
        }

        public void Use()
        {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, _Handle);
        }

    }

}
