// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Aximo.Render.OpenGL;
using Aximo.Render.Pipelines;
using Aximo.VertexData;
using OpenToolkit.Mathematics;

namespace Aximo.Render.Objects
{
    // TODO: Use a Quad instead of a cube
    // https://stackoverflow.com/questions/30015940/why-dont-people-use-tetrahedrons-for-skyboxes/30038392#30038392
    // together with https://stackoverflow.com/questions/2588875/whats-the-best-way-to-draw-a-fullscreen-quad-in-opengl-3-2/59739538#59739538
    // https://community.khronos.org/t/rendering-a-skybox-after-drawing-post-process-effects-to-a-screen-quad/74002

    public class SkyboxObject : RenderObject, IRenderableObject
    {
        public Camera Camera => Context.Camera;

        private RendererShader _shader;
        private VertexArrayObject vao;

        private RendererTexture txt;

        private VertexDataPos[] _vertices = DataHelper.SkyBox;

        public override void Init()
        {
            UsePipeline<ForwardRenderPipeline>();

            _shader = new RendererShader("Shaders/skybox.vert", "Shaders/skybox.frag");
            //txt = Texture.LoadCubeMap("Textures/desert-skybox/#.tga");
            txt = RendererTexture.LoadCubeMap("Textures/water-skybox/#.jpg");

            vao = new VertexArrayObject(VertexLayoutDefinition.CreateDefinitionFromVertexStruct<VertexDataPos>().BindToShader(_shader));
            vao.SetData(BufferData.Create(_vertices));
        }

        public void OnRender()
        {
            //return;

            vao.Bind();
            _shader.Bind();

            _shader.SetMatrix4("View", Camera.GetViewMatrix(Vector3.Zero));
            _shader.SetMatrix4("Projection", Camera.ProjectionMatrix);

            txt.Bind(0);
            _shader.SetInt("Skybox", 0);

            GraphicsDevice.Default.DepthMask = false;
            //GL.DepthFunc(DepthFunction.Equal);
            GraphicsDevice.Default.DepthFunc = DepthFunction.Lequal;
            vao.Draw();
            GraphicsDevice.Default.DepthFunc = DepthFunction.Less;
            GraphicsDevice.Default.DepthMask = true;
        }

        public override void Free()
        {
            vao.Free();
            _shader.Free();
        }
    }
}
