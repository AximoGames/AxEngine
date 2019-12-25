using System;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Input;
using LearnOpenTK.Common;
using ProcEngine;

namespace LearnOpenTK
{
    // In this tutorial we set up some basic lighting and look at how the phong model works
    // For more insight into how it all works look at the web version. If you are just here for the source,
    // most of the changes are in the shaders, specifically most of the changes are in the fragment shader as this is
    // where the lighting calculations happens.
    public class Window : GameWindow
    {

        private float[] MouseSpeed = new float[3];
        private Vector2 MouseDelta;
        private float UpDownDelta;

        //public static Matrix4 CameraMatrix;
        //private float[] MouseSpeed = new float[3];
        //private Vector2 MouseDelta;
        //private float UpDownDelta;
        //private Vector3 CameraLocation;
        //private Vector3 Up = Vector3.UnitZ;
        //private float Pitch = -0.3f;
        //private float Facing = (float)Math.PI / 2 + 0.15f;

        public Window(int width, int height, string title) : base(width, height, GraphicsMode.Default, title, GameWindowFlags.Default, DisplayDevice.Default, 4, 0, GraphicsContextFlags.Default) { }

        public Cam Camera => ctx.Camera;

        protected override void OnLoad(EventArgs e)
        {
            GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);

            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.CullFace);

            ctx = new RenderContext();
            ctx.Camera = new Cam(new Vector3(1f, -5f, 2f), Width / (float)Height);

            lightObj = new LightObject()
            {
                Context = ctx,
            };
            lightObj.Init();

            obj = new TestObject()
            {
                Context = ctx,
                ModelMatrix = Matrix4.Identity,
                Light = lightObj,
            };
            obj.Init();

            obj2 = new TestObject()
            {
                Context = ctx,
                ModelMatrix = Matrix4.CreateTranslation(1.5f, 1.5f, 0.0f),
                Light = lightObj,
                Debug = true,
            };
            obj2.Init();

            obj3 = new TestObject()
            {
                Context = ctx,
                ModelMatrix = Matrix4.CreateScale(8, 8, 8) * Matrix4.CreateTranslation(0f, 0f, -4.5f),
                Light = lightObj,
            };
            obj3.Init();

            target = new ScreenObject
            {
            };
            target.Init();

            //CursorVisible = false;

            fb = new FrameBuffer(Width, Height);
            fb.Init();
            fb.CreateRenderBuffer();

            base.OnLoad(e);
        }

        private FrameBuffer fb;

        private IRenderableObject obj;
        private IRenderableObject obj2;
        private IRenderableObject obj3;
        private ILightObject lightObj;
        private RenderContext ctx;
        private IRenderTarget target;

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, FrameBuffer.bufNum);
            //GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            GL.Enable(EnableCap.DepthTest);
            GL.ClearColor(0.0f, 0.0f, 1.0f, 1.0f);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            (lightObj as IRenderableObject).OnRender();
            obj.OnRender();
            obj2.OnRender();
            obj3.OnRender();

            //FrameBuffer.GetTexture2(0);
            //GL.BindTexture(TextureTarget.Texture2D, FrameBuffer.texColorBuffer);
            //FrameBuffer.GetTexture(0);

            target.OnRender();

            SwapBuffers();

            base.OnRenderFrame(e);
        }


        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            if (!Focused)
            {
                return;
            }

            var input = Keyboard.GetState();

            if (input.IsKeyDown(Key.Escape))
            {
                Exit();
            }

            var kbState = Keyboard.GetState();
            if (kbState[Key.W])
            {
                Camera.Position.X += (float)Math.Cos(Camera.Facing) * 0.1f;
                Camera.Position.Y += (float)Math.Sin(Camera.Facing) * 0.1f;
            }

            if (kbState[Key.S])
            {
                Camera.Position.X -= (float)Math.Cos(Camera.Facing) * 0.1f;
                Camera.Position.Y -= (float)Math.Sin(Camera.Facing) * 0.1f;
            }

            if (kbState[Key.A])
            {
                Camera.Position.X += (float)Math.Cos(Camera.Facing + Math.PI / 2) * 0.1f;
                Camera.Position.Y += (float)Math.Sin(Camera.Facing + Math.PI / 2) * 0.1f;
            }

            if (kbState[Key.D])
            {
                Camera.Position.X -= (float)Math.Cos(Camera.Facing + Math.PI / 2) * 0.1f;
                Camera.Position.Y -= (float)Math.Sin(Camera.Facing + Math.PI / 2) * 0.1f;
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
            Camera.Position.Z += MouseSpeed[2] * 2;

            if (kbState[Key.Escape])
                Exit();

            base.OnUpdateFrame(e);
        }

        protected override void OnMouseMove(MouseMoveEventArgs e)
        {
            if (e.Mouse.LeftButton == ButtonState.Pressed)
                MouseDelta = new Vector2(e.XDelta, e.YDelta);

            base.OnMouseMove(e);
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            Camera.Fov -= e.DeltaPrecise;
            base.OnMouseWheel(e);
        }

        protected override void OnResize(EventArgs e)
        {
            GL.Viewport(0, 0, Width, Height);
            Camera.AspectRatio = Width / (float)Height;
            base.OnResize(e);
        }

        protected override void OnUnload(EventArgs e)
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);
            GL.UseProgram(0);

            obj.Free();
            obj2.Free();
            obj3.Free();
            lightObj.Free();

            base.OnUnload(e);
        }
    }
}