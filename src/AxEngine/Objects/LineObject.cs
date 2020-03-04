﻿using OpenTK;
using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;

namespace AxEngine
{
    public class LineObject : GameObject, IRenderableObject
    {

        public Camera Camera => Context.Camera;
        public Matrix4 ModelMatrix { get; set; } = Matrix4.Identity;

        private Shader _Shader;

        private float[] _vertices = DataHelper.Line;
        private VertexArrayObject vao;
        private VertexBufferObject vbo;

        public void SetPoint1(Vector3 pos)
        {
            _vertices[0] = pos.X;
            _vertices[1] = pos.Y;
            _vertices[2] = pos.Z;
        }
        public void SetPoint2(Vector3 pos)
        {
            _vertices[7] = pos.X;
            _vertices[8] = pos.Y;
            _vertices[9] = pos.Z;
        }

        public void UpdateData()
        {
            vao.SetData(_vertices);
        }

        public override void Init()
        {
            UsePipeline<ForwardRenderPipeline>();

            _Shader = new Shader("Shaders/lines.vert", "Shaders/lines.frag");

            vbo = new VertexBufferObject();
            vbo.Create();
            vbo.Use();

            var layout = new VertexLayout();
            layout.AddAttribute<float>(_Shader.GetAttribLocation("aPos"), 3);
            layout.AddAttribute<float>(_Shader.GetAttribLocation("aColor"), 4);

            vao = new VertexArrayObject(layout, vbo);
            vao.PrimitiveType = PrimitiveType.Lines;
            vao.Create();

            vao.SetData(_vertices);
        }

        public void OnRender()
        {
            vao.Use();

            _Shader.Use();

            _Shader.SetMatrix4("model", ModelMatrix);
            _Shader.SetMatrix4("view", Camera.ViewMatrix);
            _Shader.SetMatrix4("projection", Camera.ProjectionMatrix);

            _Shader.SetVector3("objectColor", new Vector3(1.0f, 0.5f, 0.31f));
            _Shader.SetVector3("viewPos", Camera.Position);

            //GL.Disable(EnableCap.DepthTest);
            vao.Draw();
            //GL.Enable(EnableCap.DepthTest);
        }

        public override void Free()
        {
            vao.Free();
            vbo.Free();
            _Shader.Free();
        }

    }

}