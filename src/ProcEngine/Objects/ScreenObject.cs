using OpenTK;
using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;

namespace ProcEngine
{
    public class ScreenObject : GameObject, IRenderTarget
    {
        private Shader _shader;

        private float[] _vertices = DataHelper.Quad;

        private VertexArrayObject vao;
        private BufferObject vbo;

        private Texture SourceTexture;

        public override void Init()
        {
            UsePipeline<ScreenPipeline>();

            _shader = new Shader("Shaders/screen.vert", "Shaders/screen.frag");

            SourceTexture = Context.GetPipeline<ForwardRenderPipeline>().FrameBuffer.DestinationTexture;

            vbo = new BufferObject();
            vbo.Create();
            vbo.Use();

            var layout = new VertexLayout();
            layout.AddAttribute<float>(_shader.GetAttribLocation("aPos"), 2);
            layout.AddAttribute<float>(_shader.GetAttribLocation("aTexCoords"), 2);

            vao = new VertexArrayObject(layout, vbo);
            vao.Create();

            vao.SetData(_vertices);
        }

        public void OnRender()
        {
            if (!(Context.CurrentPipeline is ScreenPipeline))
                return;

            vao.Use();

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            GL.Disable(EnableCap.DepthTest);
            GL.ClearColor(1.0f, 0.0f, 1.0f, 1.0f);
            GL.Clear(ClearBufferMask.ColorBufferBit);

            _shader.Use();
            SourceTexture.Use();

            //GL.Disable(EnableCap.CullFace);
            vao.Draw();
            //GL.Enable(EnableCap.CullFace);
        }

        public override void Free()
        {
            vao.Free();
            vbo.Free();
            _shader.Free();
        }

    }

}
