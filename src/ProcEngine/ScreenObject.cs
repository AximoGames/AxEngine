using LearnOpenTK.Common;
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
        private VertexBufferObject vbo;

        public override void Init()
        {
            _shader = new Shader("Shaders/screen.vert", "Shaders/screen.frag");

            vbo = new VertexBufferObject();
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
            vao.Use();

            //txt0.Use(TextureUnit.Texture0);
            //txt1.Use(TextureUnit.Texture1);

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            GL.Disable(EnableCap.DepthTest);
            GL.ClearColor(1.0f, 0.0f, 1.0f, 1.0f);
            GL.Clear(ClearBufferMask.ColorBufferBit);

            _shader.Use();
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, FrameBuffer.texColorBuffer);

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
