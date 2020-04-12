// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using OpenToolkit;
using OpenToolkit.Graphics.OpenGL4;
using OpenToolkit.Mathematics;

namespace Aximo.Render
{
    public class GridObject : RenderObject, IRenderableObject
    {
        public Camera Camera => Context.Camera;
        public Matrix4 ModelMatrix { get; set; } = Matrix4.Identity;

        public int Size = 10;
        public bool Center = true;

        private Shader _Shader;

        private VertexArrayObject vao;

        public override void Init()
        {
            UsePipeline<ForwardRenderPipeline>();

            _Shader = new Shader("Shaders/lines.vert", "Shaders/lines.frag");

            var layout = new VertexLayoutBinded();
            layout.AddAttribute<float>(_Shader.GetAttribLocation("aPos"), 3);
            layout.AddAttribute<float>(_Shader.GetAttribLocation("aColor"), 4);

            vao = new VertexArrayObject(layout)
            {
                PrimitiveType = PrimitiveType.Lines,
            };

            var vertices = new List<VertexDataPosColor>();

            var size = Size;
            var color = new Vector4(0.45f, 0.45f, 0.0f, 1.0f);

            int start;
            int end;
            float startPos;
            float endPos;
            if (Center)
            {
                start = -size;
                end = size;
                startPos = -size;
                endPos = size;
            }
            else
            {
                start = 0;
                end = size;
                startPos = 0f;
                endPos = size;
            }

            for (var i = start; i <= end; i++)
            {
                vertices.Add(new Vector3(startPos, i, 0), color);
                vertices.Add(new Vector3(endPos, i, 0), color);

                vertices.Add(new Vector3(i, startPos, 0), color);
                vertices.Add(new Vector3(i, endPos, 0), color);
            }

            vao.SetData(BufferData.Create(vertices.ToArray()));
        }

        public void OnRender()
        {
            vao.Bind();

            _Shader.Bind();

            _Shader.SetMatrix4("Model", ModelMatrix);
            _Shader.SetMatrix4("View", Camera.ViewMatrix);
            _Shader.SetMatrix4("Projection", Camera.ProjectionMatrix);

            _Shader.SetVector3("objectColor", new Vector3(1.0f, 0.5f, 0.31f));
            _Shader.SetVector3("ViewPos", Camera.Position);

            //GL.Disable(EnableCap.DepthTest);
            vao.Draw();
            //GL.Enable(EnableCap.DepthTest);
        }

        public override void Free()
        {
            vao.Free();
            _Shader.Free();
        }
    }
}
