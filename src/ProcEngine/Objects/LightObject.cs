using OpenTK;
using System.Collections.Generic;

namespace ProcEngine
{

    public class LightObject : GameObject, IRenderableObject, IPosition, ILightObject
    {
        public Camera Camera => Context.Camera;

        public Vector3 Position { get; set; }

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

                    shadowCamera.SetData("Light", this);

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
                    cam.SetData("Light", this);
                    return cam;
                }
            }
        }

        private Shader _shader;
        private VertexArrayObject vao;
        private BufferObject vbo;

        private float[] _vertices = DataHelper.Cube;

        public override void Init()
        {
            UsePipeline<ForwardRenderPipeline>();

            _shader = new Shader("Shaders/shader.vert", "Shaders/shader.frag");

            vbo = new BufferObject();
            vbo.Create();
            vbo.Use();

            var layout = new VertexLayout();
            layout.AddAttribute<float>(_shader.GetAttribLocation("aPos"), 3);
            layout.AddAttribute<float>(_shader.GetAttribLocation("aNormal"), 3);
            layout.AddAttribute<float>(_shader.GetAttribLocation("aTexCoords"), 2);

            vao = new VertexArrayObject(layout, vbo);
            vao.Create();

            vao.SetData(_vertices);
        }

        public void OnRender()
        {
            vao.Use();
            _shader.Use();

            Matrix4 lampMatrix = Matrix4.Identity;
            lampMatrix *= Matrix4.CreateScale(0.2f);
            lampMatrix *= Matrix4.CreateTranslation(Position);

            _shader.SetMatrix4("model", lampMatrix);
            _shader.SetMatrix4("view", Camera.GetViewMatrix());
            _shader.SetMatrix4("projection", Camera.GetProjectionMatrix());

            vao.Draw();
        }

        public override void Free()
        {
            vao.Free();
            vbo.Free();
            _shader.Free();
        }

    }

}
