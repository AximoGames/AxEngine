using OpenTK;
using OpenTK.Graphics.OpenGL4;
using System.Collections.Generic;

namespace AxEngine
{

    // TODO: Use a Quad instead of a cube
    // https://stackoverflow.com/questions/30015940/why-dont-people-use-tetrahedrons-for-skyboxes/30038392#30038392
    // together with https://stackoverflow.com/questions/2588875/whats-the-best-way-to-draw-a-fullscreen-quad-in-opengl-3-2/59739538#59739538
    // https://community.khronos.org/t/rendering-a-skybox-after-drawing-post-process-effects-to-a-screen-quad/74002

    public class Skybox : GameObject, IRenderableObject
    {
        public Camera Camera => Context.Camera;

        private Shader _shader;
        private VertexArrayObject vao;
        private BufferObject vbo;

        private Texture txt;

        private float[] _vertices = DataHelper.SkyBox;

        public override void Init()
        {
            UsePipeline<ForwardRenderPipeline>();

            _shader = new Shader("Shaders/skybox.vert", "Shaders/skybox.frag");
            //txt = Texture.LoadCubeMap("Ressources/desert-skybox/#.tga");
            txt = Texture.LoadCubeMap("Ressources/water-skybox/#.jpg");

            vbo = new BufferObject();
            vbo.Create();
            vbo.Use();

            var layout = new VertexLayout();
            layout.AddAttribute<float>(_shader.GetAttribLocation("aPos"), 3);

            vao = new VertexArrayObject(layout, vbo);
            vao.Create();

            vao.SetData(_vertices);
        }

        public void OnRender()
        {
            //return;

            vao.Use();
            _shader.Use();

            _shader.SetMatrix4("view", Camera.GetViewMatrix(Vector3.Zero));
            _shader.SetMatrix4("projection", Camera.GetProjectionMatrix());

            txt.Use(TextureUnit.Texture0);
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
            vbo.Free();
            _shader.Free();
        }

    }

}
