using OpenToolkit.Graphics.OpenGL4;
//using System.Drawing.Imaging;

namespace AxEngine
{
    public class RenderBuffer : IObjectLabel
    {

        private int _Handle;
        public int Handle => _Handle;

        private string _ObjectLabel;
        public string ObjectLabel { get => _ObjectLabel; set { _ObjectLabel = value; ObjectManager.SetLabel(this); } }

        private RenderbufferTarget Target = RenderbufferTarget.Renderbuffer;
        public RenderbufferStorage RenderBufferStorage;
        private FramebufferAttachment FrameBufferAttachment;

        public ObjectLabelIdentifier ObjectLabelIdentifier => ObjectLabelIdentifier.Renderbuffer;

        public RenderBuffer(FrameBuffer fb, RenderbufferStorage renderbufferStorage, FramebufferAttachment framebufferAttachment)
        {
            Target = RenderbufferTarget.Renderbuffer;
            RenderBufferStorage = renderbufferStorage;
            FrameBufferAttachment = framebufferAttachment;

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

        public void Resize(FrameBuffer fb)
        {
            fb.Bind();
            GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, RenderBufferStorage, fb.Width, fb.Height);
        }

    }

}
