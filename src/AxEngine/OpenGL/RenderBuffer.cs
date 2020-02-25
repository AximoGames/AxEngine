using OpenTK.Graphics.OpenGL4;
//using System.Drawing.Imaging;

namespace ProcEngine
{
    public class RenderBuffer : IObjectLabel
    {

        private int _Handle;
        public int Handle => _Handle;

        private string _ObjectLabel;
        public string ObjectLabel { get => _ObjectLabel; set { _ObjectLabel = value; ObjectManager.SetLabel(this); } }

        public ObjectLabelIdentifier ObjectLabelIdentifier => ObjectLabelIdentifier.Renderbuffer;

        public RenderBuffer(FrameBuffer fb, RenderbufferStorage renderbufferStorage, FramebufferAttachment framebufferAttachment)
        {
            fb.Use();

            GL.GenRenderbuffers(1, out _Handle);
            Use();
            GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, renderbufferStorage, fb.Width, fb.Height);
            GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, framebufferAttachment, RenderbufferTarget.Renderbuffer, _Handle);
        }

        public void Use()
        {
            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, _Handle);
        }

    }

}
