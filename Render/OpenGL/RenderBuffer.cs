// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using OpenToolkit.Graphics.OpenGL4;

namespace Aximo.Render
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
            GL.RenderbufferStorage(Target, renderbufferStorage, fb.Width, fb.Height);
            GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, framebufferAttachment, Target, _Handle);
        }

        public void Bind()
        {
            GL.BindRenderbuffer(Target, _Handle);
        }

        public void Resize(FrameBuffer fb)
        {
            fb.Bind();
            GL.RenderbufferStorage(Target, RenderBufferStorage, fb.Width, fb.Height);
        }
    }
}
