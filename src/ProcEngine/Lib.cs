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

namespace Net3dBoolDemo
{



    public abstract class GameObject : IGameObject
    {
        public readonly int Id;

        public GameContext Context;

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

    public class GameContext
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
    }

    public class TestObject : GameObject, IRenderableObject
    {

        public Cam Camera => Context.Camera;

        private readonly Vector3 _lightPos = new Vector3(1.2f, 1.0f, 2.0f);

        private int _vertexBufferObject;
        private int _vaoModel;
        private int _vaoLamp;

        private Shader _lampShader;
        private Shader _lightingShader;

        private float[] _vertices = DataHelper.Cube;

        public override void Init()
        {
            _vertexBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, _vertices.Length * sizeof(float), _vertices, BufferUsageHint.StaticDraw);

            _lightingShader = new Shader("Shaders/shader.vert", "Shaders/lighting.frag");
            _lampShader = new Shader("Shaders/shader.vert", "Shaders/shader.frag");

            _vaoModel = GL.GenVertexArray();
            GL.BindVertexArray(_vaoModel);

            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);

            var positionLocation = _lightingShader.GetAttribLocation("aPos");
            GL.EnableVertexAttribArray(positionLocation);
            // Remember to change the stride as we now have 6 floats per vertex
            GL.VertexAttribPointer(positionLocation, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);

            // We now need to define the layout of the normal so the shader can use it
            var normalLocation = _lightingShader.GetAttribLocation("aNormal");
            GL.EnableVertexAttribArray(normalLocation);
            GL.VertexAttribPointer(normalLocation, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 3 * sizeof(float));

            _vaoLamp = GL.GenVertexArray();
            GL.BindVertexArray(_vaoLamp);

            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);

            positionLocation = _lampShader.GetAttribLocation("aPos");
            GL.EnableVertexAttribArray(positionLocation);
            // Also change the stride here as we now have 6 floats per vertex. Now we don't define the normal for the lamp VAO
            // this is because it isn't used, it might seem like a waste to use the same VBO if they dont have the same data
            // The two cubes still use the same position, and since the position is already in the graphics memory it is actually
            // better to do it this way. Look through the web version for a much better understanding of this.
            GL.VertexAttribPointer(positionLocation, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);
        }

        public void OnRender()
        {
            GL.BindVertexArray(_vaoModel);

            _lightingShader.Use();

            _lightingShader.SetMatrix4("model", Matrix4.Identity);
            _lightingShader.SetMatrix4("view", Camera.GetViewMatrix());
            _lightingShader.SetMatrix4("projection", Camera.GetProjectionMatrix());

            _lightingShader.SetVector3("objectColor", new Vector3(1.0f, 0.5f, 0.31f));
            _lightingShader.SetVector3("lightColor", new Vector3(1.0f, 1.0f, 1.0f));
            _lightingShader.SetVector3("lightPos", _lightPos);
            _lightingShader.SetVector3("viewPos", Camera.Position);

            GL.DrawArrays(PrimitiveType.Triangles, 0, 36);

            GL.BindVertexArray(_vaoLamp);

            _lampShader.Use();

            Matrix4 lampMatrix = Matrix4.Identity;
            lampMatrix *= Matrix4.CreateScale(0.2f);
            lampMatrix *= Matrix4.CreateTranslation(_lightPos);

            _lampShader.SetMatrix4("model", lampMatrix);
            _lampShader.SetMatrix4("view", Camera.GetViewMatrix());
            _lampShader.SetMatrix4("projection", Camera.GetProjectionMatrix());

            GL.DrawArrays(PrimitiveType.Triangles, 0, 36);
        }

        public override void Free()
        {
            GL.DeleteBuffer(_vertexBufferObject);
            GL.DeleteVertexArray(_vaoModel);
            GL.DeleteVertexArray(_vaoLamp);

            GL.DeleteProgram(_lampShader.Handle);
            GL.DeleteProgram(_lightingShader.Handle);
        }

    }

}
