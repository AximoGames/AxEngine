
using OpenTK;
using OpenTK.Graphics.OpenGL4;
using LearnOpenTK.Common;

namespace ProcEngine
{
    public class LightObject : GameObject, IRenderableObject, IPosition, ILightObject
    {
        public Cam Camera => Context.Camera;

        public Vector3 Position { get; set; } = new Vector3(1.2f, 1.0f, 2.0f);

        private int _vertexBufferObject;
        private int _vaoModel;

        private Shader _shader;

        private float[] _vertices = DataHelper.Cube;

        public override void Init()
        {
            _vertexBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, _vertices.Length * sizeof(float), _vertices, BufferUsageHint.StaticDraw);

            _shader = new Shader("Shaders/shader.vert", "Shaders/shader.frag");

            _vaoModel = GL.GenVertexArray();
            GL.BindVertexArray(_vaoModel);

            // BindBuffer(GL_ARRAY_BUFFER) does not directly touch the bound VAO
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
        }

        public void OnRender()
        {
            _shader.Use();

            Matrix4 lampMatrix = Matrix4.Identity;
            lampMatrix *= Matrix4.CreateScale(0.2f);
            lampMatrix *= Matrix4.CreateTranslation(Position);

            _shader.SetMatrix4("model", lampMatrix);
            _shader.SetMatrix4("view", Camera.GetViewMatrix());
            _shader.SetMatrix4("projection", Camera.GetProjectionMatrix());

            GL.DrawArrays(PrimitiveType.Triangles, 0, 36);
        }

        public override void Free()
        {
            GL.DeleteBuffer(_vertexBufferObject);
            GL.DeleteVertexArray(_vaoModel);
            GL.DeleteProgram(_shader.Handle);
        }

    }

}
