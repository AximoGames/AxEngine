using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Input;
using LearnOpenTK.Common;

namespace ProcEngine
{

    public abstract class GameObject : IGameObject
    {
        public readonly int Id;

        public RenderContext Context;

        public GameObject()
        {
            Id = GetNextGameObjectId();
        }

        private static int CurrentGameObjectId;
        private static int GetNextGameObjectId()
        {
            return Interlocked.Increment(ref CurrentGameObjectId);
        }

        public abstract void Init();

        public abstract void Free();
    }

    public interface IGameObject
    {
        void Init();
        void Free();
    }

    public interface IRenderableObject : IGameObject
    {

        void OnRender();
    }

    public interface IRenderTarget : IRenderableObject
    {
    }

    public interface IMeshObject : IRenderableObject
    {
        Vector3[] GetVertices();
        int[] GetIndices();
    }

    //public class MeshObject : GameObject, IMeshObject
    //{
    //    public int[] GetIndices()
    //    {
    //        return DefaultCoordinates.DEFAULT_BOX_COORDINATES;
    //    }

    //    public Vector3[] GetVertices()
    //    {
    //        return DefaultCoordinates.DEFAULT_BOX_VERTICES;
    //    }
    //}

    public class RenderContext
    {

        public Cam Camera;

    }

    internal static class DataHelper
    {         // Here we now have added the normals of the vertices
        // Remember to define the layouts to the VAO's
        public static readonly float[] Cube =
        {
             // Position          Normal
             0.5f, -0.5f, -0.5f,  0.0f,  0.0f, -1.0f, // Front face
            -0.5f, -0.5f, -0.5f,  0.0f,  0.0f, -1.0f,
             0.5f,  0.5f, -0.5f,  0.0f,  0.0f, -1.0f,
            -0.5f,  0.5f, -0.5f,  0.0f,  0.0f, -1.0f,
             0.5f,  0.5f, -0.5f,  0.0f,  0.0f, -1.0f,
            -0.5f, -0.5f, -0.5f,  0.0f,  0.0f, -1.0f,

            -0.5f, -0.5f,  0.5f,  0.0f,  0.0f,  1.0f, // Back face
             0.5f, -0.5f,  0.5f,  0.0f,  0.0f,  1.0f,
             0.5f,  0.5f,  0.5f,  0.0f,  0.0f,  1.0f,
             0.5f,  0.5f,  0.5f,  0.0f,  0.0f,  1.0f,
            -0.5f,  0.5f,  0.5f,  0.0f,  0.0f,  1.0f,
            -0.5f, -0.5f,  0.5f,  0.0f,  0.0f,  1.0f,

            -0.5f,  0.5f,  0.5f, -1.0f,  0.0f,  0.0f, // Left face
            -0.5f,  0.5f, -0.5f, -1.0f,  0.0f,  0.0f,
            -0.5f, -0.5f, -0.5f, -1.0f,  0.0f,  0.0f,
            -0.5f, -0.5f, -0.5f, -1.0f,  0.0f,  0.0f,
            -0.5f, -0.5f,  0.5f, -1.0f,  0.0f,  0.0f,
            -0.5f,  0.5f,  0.5f, -1.0f,  0.0f,  0.0f,

             0.5f,  0.5f, -0.5f,  1.0f,  0.0f,  0.0f, // Right face 0 1 0
             0.5f,  0.5f,  0.5f,  1.0f,  0.0f,  0.0f,
             0.5f, -0.5f, -0.5f,  1.0f,  0.0f,  0.0f,
             0.5f, -0.5f,  0.5f,  1.0f,  0.0f,  0.0f,
             0.5f, -0.5f, -0.5f,  1.0f,  0.0f,  0.0f,
             0.5f,  0.5f,  0.5f,  1.0f,  0.0f,  0.0f,

            -0.5f, -0.5f, -0.5f,  0.0f, -1.0f,  0.0f, // Bottom face
             0.5f, -0.5f, -0.5f,  0.0f, -1.0f,  0.0f,
             0.5f, -0.5f,  0.5f,  0.0f, -1.0f,  0.0f,
             0.5f, -0.5f,  0.5f,  0.0f, -1.0f,  0.0f,
            -0.5f, -0.5f,  0.5f,  0.0f, -1.0f,  0.0f,
            -0.5f, -0.5f, -0.5f,  0.0f, -1.0f,  0.0f,

            0.5f,  0.5f, -0.5f,  0.0f,  1.0f,  0.0f, // Top face
           -0.5f,  0.5f, -0.5f,  0.0f,  1.0f,  0.0f,
            0.5f,  0.5f,  0.5f,  0.0f,  1.0f,  0.0f,
           -0.5f,  0.5f,  0.5f,  0.0f,  1.0f,  0.0f,
            0.5f,  0.5f,  0.5f,  0.0f,  1.0f,  0.0f,
           -0.5f,  0.5f, -0.5f,  0.0f,  1.0f,  0.0f
        };

        public static readonly float[] CubeDebug =
        {
             // Position          Normal
             0.5f, -0.5f, -0.5f,  0.0f,  0.0f, -1.0f, // Front face
            -0.5f, -0.5f, -0.5f,  0.0f,  0.0f, -1.0f,
             0.5f,  0.5f, -0.5f,  0.0f,  0.0f, -1.0f,
            -0.5f,  0.5f, -0.5f,  0.0f,  0.0f, -1.0f,
             0.5f,  0.5f, -0.5f,  0.0f,  0.0f, -1.0f,
            -0.5f, -0.5f, -0.5f,  0.0f,  0.0f, -1.0f,

            -0.5f, -0.5f,  0.5f,  0.0f,  0.0f,  1.0f, // Back face
             0.5f, -0.5f,  0.5f,  0.0f,  0.0f,  1.0f,
             0.5f,  0.5f,  0.5f,  0.0f,  0.0f,  1.0f,
             0.5f,  0.5f,  0.5f,  0.0f,  0.0f,  1.0f,
            -0.5f,  0.5f,  0.5f,  0.0f,  0.0f,  1.0f,
            -0.5f, -0.5f,  0.5f,  0.0f,  0.0f,  1.0f,

            -0.5f,  0.5f,  0.5f, -1.0f,  0.0f,  0.0f, // Left face
            -0.5f,  0.5f, -0.5f, -1.0f,  0.0f,  0.0f,
            -0.5f, -0.5f, -0.5f, -1.0f,  0.0f,  0.0f,
            -0.5f, -0.5f, -0.5f, -1.0f,  0.0f,  0.0f,
            -0.5f, -0.5f,  0.5f, -1.0f,  0.0f,  0.0f,
            -0.5f,  0.5f,  0.5f, -1.0f,  0.0f,  0.0f,

             0.6f,  0.5f, -0.5f,  1.0f,  0.0f,  0.0f, // Right face 0 1 0
             0.6f,  0.5f,  0.5f,  1.0f,  0.0f,  0.0f,
             0.6f, -0.5f, -0.5f,  1.0f,  0.0f,  0.0f,
             0.6f, -0.5f,  0.5f,  1.0f,  0.0f,  0.0f,
             0.6f, -0.5f, -0.5f,  1.0f,  0.0f,  0.0f,
             0.6f,  0.5f,  0.5f,  1.0f,  0.0f,  0.0f,

            -0.5f, -0.5f, -0.5f,  0.0f, -1.0f,  0.0f, // Bottom face
             0.5f, -0.5f, -0.5f,  0.0f, -1.0f,  0.0f,
             0.5f, -0.5f,  0.5f,  0.0f, -1.0f,  0.0f,
             0.5f, -0.5f,  0.5f,  0.0f, -1.0f,  0.0f,
            -0.5f, -0.5f,  0.5f,  0.0f, -1.0f,  0.0f,
            -0.5f, -0.5f, -0.5f,  0.0f, -1.0f,  0.0f,

            0.5f,  0.5f, -0.5f,  0.0f,  1.0f,  0.0f, // Top face
           -0.5f,  0.5f, -0.5f,  0.0f,  1.0f,  0.0f,
            0.5f,  0.5f,  0.5f,  0.0f,  1.0f,  0.0f,
           -0.5f,  0.5f,  0.5f,  0.0f,  1.0f,  0.0f,
            0.5f,  0.5f,  0.5f,  0.0f,  1.0f,  0.0f,
           -0.5f,  0.5f, -0.5f,  0.0f,  1.0f,  0.0f
        };
    }

    public interface IPosition : IGameObject
    {
        Vector3 Position { get; set; }
    }

    public interface ILightObject : IPosition, IGameObject
    {
    }

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

    public class VertexArrayObject
    {
        private int _Handle;

        public int Handle => _Handle;
        private int _Stride;

        private VertexBufferObject _vbo;
        public VertexBufferObject VertextBufferObject;

        public VertexArrayObject(VertexBufferObject vbo, int stride)
        {
            _Stride = stride;
            _vbo = vbo;
        }

        public void Init()
        {
            _Handle = GL.GenVertexArray();
        }

        public static int CurrentHandle;
        public void Use()
        {
            if (CurrentHandle == _Handle)
                return;
            CurrentHandle = _Handle;

            GL.BindVertexArray(_Handle);
        }

        public void VertexAttribPointer(int index, int size, VertexAttribPointerType type, bool normalized, int offset)
        {
            _vbo.Use();
            Use();
            GL.EnableVertexAttribArray(index);
            GL.VertexAttribPointer(index, size, type, normalized, _Stride, offset);
        }

        //public void AddPosition()
        //{
        //}

        public void Free()
        {
            GL.DeleteVertexArray(_Handle);
        }
    }

    public class VertexBufferObject
    {

        private int _Handle;
        public int Handle => _Handle;

        public void Init()
        {
            _Handle = GL.GenBuffer();
        }

        public void SetData(float[] vertices)
        {
            Use();
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);
        }

        public static int CurrentHandle;

        public void Use()
        {
            if (CurrentHandle == _Handle)
                return;
            CurrentHandle = _Handle;

            GL.BindBuffer(BufferTarget.ArrayBuffer, _Handle);
        }

        public void Free()
        {
            GL.DeleteBuffer(_Handle);
        }

    }

    public class TestObject : GameObject, IRenderableObject
    {

        public Cam Camera => Context.Camera;
        public Matrix4 ModelMatrix { get; set; } = Matrix4.Identity;

        public bool Debug;

        public ILightObject Light;

        private Shader _shader;

        private float[] _vertices = DataHelper.Cube;

        private VertexArrayObject vao;
        private VertexBufferObject vbo;

        public override void Init()
        {
            if (Debug)
                _vertices = DataHelper.CubeDebug;

            _shader = new Shader("Shaders/shader.vert", "Shaders/lighting.frag");

            vbo = new VertexBufferObject();
            vbo.Init();
            vbo.SetData(_vertices);
            vbo.Use();

            vao = new VertexArrayObject(vbo, 6 * sizeof(float));
            vao.Init();

            vao.VertexAttribPointer(_shader.GetAttribLocation("aPos"), 3, VertexAttribPointerType.Float, false, 0);
            vao.VertexAttribPointer(_shader.GetAttribLocation("aNormal"), 3, VertexAttribPointerType.Float, false, 3 * sizeof(float));
        }

        public void OnRender()
        {
            vao.Use();

            _shader.Use();

            _shader.SetMatrix4("model", ModelMatrix);
            _shader.SetMatrix4("view", Camera.GetViewMatrix());
            _shader.SetMatrix4("projection", Camera.GetProjectionMatrix());

            _shader.SetVector3("objectColor", new Vector3(1.0f, 0.5f, 0.31f));
            _shader.SetVector3("lightColor", new Vector3(1.0f, 1.0f, 1.0f));
            _shader.SetVector3("lightPos", Light.Position);
            _shader.SetVector3("viewPos", Camera.Position);

            GL.DrawArrays(PrimitiveType.Triangles, 0, 36);
        }

        public override void Free()
        {
            vao.Free();
            vbo.Free();
            _shader.Free();
        }

    }

}
