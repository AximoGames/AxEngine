using System;
using System.Drawing;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace Net3dBoolDemo
{

    public abstract class ExtendedGameWindow : GameWindow
    {

        public Cam Camera;

        private float[] MouseSpeed = new float[3];
        private Vector2 MouseDelta;
        private float UpDownDelta;

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            VSync = VSyncMode.On;
            Title = "Net3dBool Demo with OpenTK";

            Camera = new Cam();
            Camera.Location = new Vector3(1f, -5f, 2f);

            MouseDelta = new Vector2();

            MouseMove += new EventHandler<MouseMoveEventArgs>(OnMouseMove);

            CreateMesh();
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            var kbState = Keyboard.GetState();
            if (kbState[Key.W])
            {
                Camera.Location.X += (float)Math.Cos(Camera.Facing) * 0.1f;
                Camera.Location.Y += (float)Math.Sin(Camera.Facing) * 0.1f;
            }

            if (kbState[Key.S])
            {
                Camera.Location.X -= (float)Math.Cos(Camera.Facing) * 0.1f;
                Camera.Location.Y -= (float)Math.Sin(Camera.Facing) * 0.1f;
            }

            if (kbState[Key.A])
            {
                Camera.Location.X += (float)Math.Cos(Camera.Facing + Math.PI / 2) * 0.1f;
                Camera.Location.Y += (float)Math.Sin(Camera.Facing + Math.PI / 2) * 0.1f;
            }

            if (kbState[Key.D])
            {
                Camera.Location.X -= (float)Math.Cos(Camera.Facing + Math.PI / 2) * 0.1f;
                Camera.Location.Y -= (float)Math.Sin(Camera.Facing + Math.PI / 2) * 0.1f;
            }

            if (kbState[Key.Left])
                MouseDelta.X = -2;

            if (kbState[Key.Right])
                MouseDelta.X = 2;

            if (kbState[Key.Up])
                MouseDelta.Y = -1;

            if (kbState[Key.Down])
                MouseDelta.Y = 1;

            if (kbState[Key.PageUp])
                UpDownDelta = -3;

            if (kbState[Key.PageDown])
                UpDownDelta = 3;

            MouseSpeed[0] *= 0.9f;
            MouseSpeed[1] *= 0.9f;
            MouseSpeed[2] *= 0.9f;
            MouseSpeed[0] -= MouseDelta.X / 1000f;
            MouseSpeed[1] -= MouseDelta.Y / 1000f;
            MouseSpeed[2] -= UpDownDelta / 1000f;
            MouseDelta = new Vector2();
            UpDownDelta = 0;

            Camera.Facing += MouseSpeed[0] * 2;
            Camera.Pitch += MouseSpeed[1] * 2;
            Camera.Location.Z += MouseSpeed[2] * 2;

            if (kbState[Key.Escape])
                Exit();
        }

        void OnMouseMove(object sender, MouseMoveEventArgs e)
        {
            if (e.Mouse.LeftButton == ButtonState.Pressed)
                MouseDelta = new Vector2(e.XDelta, e.YDelta);
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            Camera.Fov -= e.DeltaPrecise;
            base.OnMouseWheel(e);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            GL.Viewport(ClientRectangle.X, ClientRectangle.Y, ClientRectangle.Width, ClientRectangle.Height);
            Camera.SetAspectRatio(Width, Height);

            Matrix4 projection = Camera.GetProjectionMatrix();
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadMatrix(ref projection);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            GL.Enable(EnableCap.CullFace);

            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.Blend);

            GL.MatrixMode(MatrixMode.Modelview);
            var viewMatrix = Camera.GetViewMatrix();
            GL.LoadMatrix(ref viewMatrix);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.ClearColor(0.0f, 0.0f, 0.0f, 0.0f);

            GL.Enable(EnableCap.ColorMaterial);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            GL.ShadeModel(ShadingModel.Smooth);

            GL.Light(LightName.Light1, LightParameter.Ambient, Color4.Gray);
            GL.Light(LightName.Light1, LightParameter.Diffuse, Color4.White);
            GL.Light(LightName.Light1, LightParameter.Position, (new Vector4(0f, 0f, 0f, 1f)));
            GL.Enable(EnableCap.Light1);

            RenderMesh();
            RenderLines();

            SwapBuffers();
        }

        public abstract void CreateMesh();

        public abstract void RenderMesh();

        public void RenderLines()
        {
            GL.Begin(PrimitiveType.Lines);
            float dist = 10f;
            for (float i = -dist; i <= dist; i += dist / 10)
            {
                GL.Color3(0.2, 0.2, 0.2);

                if (i == 0)
                    GL.Color3(Color.DarkBlue);

                GL.Vertex3(i, dist, 0);
                GL.Vertex3(i, -dist, 0);

                if (i == 0)
                    GL.Color3(Color.DarkRed);

                GL.Vertex3(dist, i, 0);
                GL.Vertex3(-dist, i, 0);
            }

            GL.Color3(Color.DarkGreen);
            GL.Vertex3(0, 0, dist);
            GL.Vertex3(0, 0, -dist);

            GL.End();

            return;
        }

    }

}

