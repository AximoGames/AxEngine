using OpenTK.Graphics.OpenGL4;
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
            GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, RenderbufferStorage.Depth24Stencil8, fb.Width, fb.Height);
            GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthStencilAttachment, RenderbufferTarget.Renderbuffer, _Handle);
        }

        public void Use()
        {
            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, _Handle);
        }

    }

}
