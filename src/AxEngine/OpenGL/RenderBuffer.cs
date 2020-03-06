using OpenTK.Graphics.OpenGL4;
//using System.Drawing.Imaging;

namespace AxEngine
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
            fb.Bind();

            GL.GenRenderbuffers(1, out _Handle);
            Bind();
            GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, renderbufferStorage, fb.Width, fb.Height);
            GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, framebufferAttachment, RenderbufferTarget.Renderbuffer, _Handle);
        }

        public void Bind()
        {
            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, _Handle);
        }

    }

}
