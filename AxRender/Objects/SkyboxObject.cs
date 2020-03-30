// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using OpenTK;
using OpenTK.Graphics.OpenGL4;

namespace Aximo.Render
{

    // TODO: Use a Quad instead of a cube
    // https://stackoverflow.com/questions/30015940/why-dont-people-use-tetrahedrons-for-skyboxes/30038392#30038392
    // together with https://stackoverflow.com/questions/2588875/whats-the-best-way-to-draw-a-fullscreen-quad-in-opengl-3-2/59739538#59739538
    // https://community.khronos.org/t/rendering-a-skybox-after-drawing-post-process-effects-to-a-screen-quad/74002

    public class SkyboxObject : GameObject, IRenderableObject
    {
        public Camera Camera => Context.Camera;

        private Shader _shader;
        private VertexArrayObject vao;

        private Texture txt;

        private float[] _vertices = DataHelper.SkyBox;

        public override void Init()
        {
            UsePipeline<ForwardRenderPipeline>();

            _shader = new Shader("Shaders/skybox.vert", "Shaders/skybox.frag");
            //txt = Texture.LoadCubeMap("Textures/desert-skybox/#.tga");
            txt = Texture.LoadCubeMap("Textures/water-skybox/#.jpg");

            var layout = new VertexLayoutBinded();
            layout.AddAttribute<float>(_shader.GetAttribLocation("aPos"), 3);

            vao = new VertexArrayObject(layout);
            vao.SetData(BufferData.Create(_vertices));
        }

        public void OnRender()
        {
            //return;

            vao.Bind();
            _shader.Bind();

            _shader.SetMatrix4("View", Camera.GetViewMatrix(Vector3.Zero));
            _shader.SetMatrix4("Projection", Camera.ProjectionMatrix);

            txt.Bind(TextureUnit.Texture0);
            _shader.SetInt("skybox", 0);

            GL.DepthMask(false);
            GL.DepthFunc(DepthFunction.Equal);
            vao.Draw();
            GL.DepthFunc(DepthFunction.Less);
            GL.DepthMask(true);
        }

        public override void Free()
        {
            vao.Free();
            _shader.Free();
        }

    }

}
