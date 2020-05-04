// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using OpenToolkit;
using OpenToolkit.Mathematics;

namespace Aximo.Render
{
    public class LightObject : RenderObject, IRenderableObject, IPosition, ILightObject
    {
        public Camera Camera => Context.Camera;

        public Vector3 Position { get; set; }
        public Vector4 Color { get; set; } = Vector4.One;
        public float Linear { get; set; } = 0.1f;
        public float Quadric { get; set; } = 0.0f;

        public bool Shadows { get; set; }
        public int ShadowTextureIndex { get; set; }
        public LightType LightType { get; set; }

        public Camera LightCamera
        {
            get
            {
                if (LightType == LightType.Directional)
                {
                    //var shadowCamera = new PerspectiveFieldOfViewCamera(light.Position, 1.0f)
                    var shadowCamera = new OrthographicCamera(Position)
                    {
                        NearPlane = 1.0f,
                        FarPlane = 25f,
                    };
                    var box = Context.GetObjectByName("Box1"); // TODO: Remove Debug
                    if (box != null)
                        shadowCamera.LookAt = (box as IPosition).Position;
                    else
                        shadowCamera.LookAt = new Vector3(0, 0, 0);

                    shadowCamera.SetExraData("Light", this);

                    return shadowCamera;
                }
                else
                {
                    var cam = new PerspectiveFieldOfViewCamera(Position, 1.0f)
                    {
                        NearPlane = 0.1f,
                        FarPlane = 25f,
                        Fov = 90f,
                    };
                    cam.SetExraData("Light", this);
                    return cam;
                }
            }
        }

        private Shader _shader;
        private VertexArrayObject vao;

        private VertexDataPosNormalUV[] _vertices = DataHelper.DefaultCube;

        public override void Init()
        {
            UsePipeline<ForwardRenderPipeline>();

            _shader = new Shader("Shaders/forward.vert", "Shaders/white.frag");

            var layout = VertexLayoutDefinition.CreateDefinitionFromVertexStruct<VertexDataPosNormalUV>().BindToShader(_shader);
            vao = new VertexArrayObject(layout);
            vao.SetData(BufferData.Create(_vertices));
        }

        public void OnRender()
        {
            vao.Bind();
            _shader.Bind();

            Matrix4 lampMatrix = Matrix4.Identity;
            lampMatrix *= Matrix4.CreateScale(0.1f);
            lampMatrix *= Matrix4.CreateTranslation(Position);

            _shader.SetMatrix4("Model", lampMatrix);
            _shader.SetMatrix4("View", Camera.ViewMatrix);
            _shader.SetMatrix4("Projection", Camera.ProjectionMatrix);

            vao.Draw();
        }

        public override void Free()
        {
            vao.Free();
            _shader.Free();
        }
    }
}
