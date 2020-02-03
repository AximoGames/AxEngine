using System;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Input;
using ProcEngine;
using System.IO;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace ProcEngine
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

        public Window(int width, int height, string title) : base(width, height, GraphicsMode.Default, title, GameWindowFlags.Default, DisplayDevice.Default, 4, 3, GraphicsContextFlags.Default) { }

        public Camera Camera => ctx.Camera;

        private double CamAngle = 0;

        private DebugProc _debugProcCallback;
        private GCHandle _debugProcCallbackHandle;
        public static void DebugCallback(DebugSource source, DebugType type, int id, DebugSeverity severity, int length, IntPtr message, IntPtr userParam)
        {
            string messageString = Marshal.PtrToStringAnsi(message, length);

            Console.WriteLine($"{severity} {type} | {messageString}");

            if (type == DebugType.DebugTypeError)
            {
                throw new Exception(messageString);
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            // _debugProcCallback = DebugCallback;
            // _debugProcCallbackHandle = GCHandle.Alloc(_debugProcCallback);
            // GL.DebugMessageCallback(_debugProcCallback, IntPtr.Zero);
            // GL.Enable(EnableCap.DebugOutput);
            // GL.Enable(EnableCap.DebugOutputSynchronous);

            var vendor = GL.GetString(StringName.Vendor);
            var version = GL.GetString(StringName.Version);
            var shadingLanguageVersion = GL.GetString(StringName.ShadingLanguageVersion);
            var renderer = GL.GetString(StringName.Renderer);

            Console.WriteLine($"Vendor: {vendor}, version: {version}, shadinglangVersion: {shadingLanguageVersion}, renderer: {renderer}");

            GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);

            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.CullFace);

            ctx = new RenderContext();
            ctx.SceneOpitons = new SceneOptions
            {

            };

            ctx.LogInfoMessage("Window.OnLoad");

            ctx.Camera = new PerspectiveFieldOfViewCamera(new Vector3(1f, -5f, 2f), Width / (float)Height)
            {
                NearPlane = 0.01f,
                FarPlane = 100.0f,
            };
            //ctx.Camera = new PerspectiveFieldOfViewCamera(lightPosition, Width / (float)Height);
            //ctx.Camera = new OrthographicCamera(lightPosition);

            ctx.AddObject(new Skybox()
            {
                Name = "Sky",
                // RenderShadow = false,
            });

            ctx.AddObject(new TestObject()
            {
                Name = "Ground",
                Scale = new Vector3(50, 50, 1),
                Position = new Vector3(0f, 0f, -0.5f),
                // RenderShadow = false,
            });
            ctx.AddObject(new Grid()
            {
                Name = "Grid",
                ModelMatrix = Matrix4.CreateTranslation(0f, 0f, 0.01f),
                //Debug = true,
            });
            ctx.AddObject(new Grid()
            {
                Name = "Grid",
                ModelMatrix = Matrix4.CreateRotationY((float)Math.PI / 2) * Matrix4.CreateTranslation(-10f, 0f, 0.01f),
                //Debug = true,
            });
            ctx.AddObject(new Grid()
            {
                Name = "Grid",
                ModelMatrix = Matrix4.CreateRotationX((float)Math.PI / 2) * Matrix4.CreateTranslation(0f, 10f, 0.01f),
                //Debug = true,
            });
            ctx.AddObject(new Lines()
            {
                Name = "CenterCross",
                ModelMatrix = Matrix4.CreateScale(2.0f) * Matrix4.CreateTranslation(0f, 0f, 0.02f),
                //Debug = true,
            });

            ctx.AddObject(new LightObject()
            {
                Position = new Vector3(0, 2, 2.5f),
                Name = "MovingLight",
                Enabled = false,
            });

            ctx.AddObject(new LightObject()
            {
                Position = new Vector3(2f, 0.5f, 1.25f),
                Name = "StaticLight",
            });

            ctx.AddObject(new TestObject()
            {
                Name = "Box1",
                Rotate = new Vector3(0, 0, (float)Math.PI),
                Scale = new Vector3(1),
                Position = new Vector3(0, 0, 0.5f),
                Debug = true,
                // Enabled = false,
            });

            ctx.AddObject(new TestObject()
            {
                Name = "Box2",
                Scale = new Vector3(1),
                Position = new Vector3(1.5f, 1.5f, 0.5f),
                //Debug = true,
                Enabled = false,
            });

            //CursorVisible = false;

            fb = new FrameBuffer(Width, Height);
            fb.InitNormal();
            fb.CreateRenderBuffer();

            shadowFb = new FrameBuffer(1024, 1024);
            shadowFb.InitDepth();

            shadowCubeFb = new FrameBuffer(1024, 1024);
            shadowCubeFb.InitCubeDepth();

            ctx.AddObject(new ScreenObject(fb.DestinationTexture)
            {
            });

            StartFileListener();

            MovingObject = Camera;
            //MovingObject = ctx.GetObjectByName("Box1") as IPosition;

            base.OnLoad(e);
        }

        private FileSystemWatcher ShaderWatcher;

        private void StartFileListener()
        {
            ShaderWatcher = new FileSystemWatcher(Path.Combine(DirectoryHelper.RootDir, "Shaders"));
            ShaderWatcher.Changed += (sender, e) =>
            {
                // Reload have to be in Main-Thread.
                Dispatch(() => Reload());
            };
            ShaderWatcher.EnableRaisingEvents = true;
        }

        private FrameBuffer fb;
        public static FrameBuffer shadowFb;
        public static FrameBuffer shadowCubeFb;

        private RenderContext ctx;

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            CamAngle -= 0.01;
            var pos = new Vector3((float)(Math.Cos(CamAngle) * 2f), (float)(Math.Sin(CamAngle) * 2f), 1.5f);
            ILightObject light = ctx.LightObjects[0];

            light.Position = pos;

            GL.Enable(EnableCap.DepthTest);

            //--

            GL.Viewport(0, 0, shadowFb.Width, shadowFb.Height);
            shadowFb.Use();
            GL.Clear(ClearBufferMask.DepthBufferBit);

            // Render objects
            foreach (var obj in ctx.ShadowObjects)
                if (obj.Enabled && obj.RenderShadow)
                    obj.OnRenderShadow();

            shadowCubeFb.Use();
            GL.Clear(ClearBufferMask.DepthBufferBit);
            foreach (var obj in ctx.ShadowObjects)
                if (obj.Enabled && obj.RenderShadow)
                    obj.OnRenderCubeShadow();

            //--

            // Configure
            GL.Viewport(0, 0, Width, Height);
            fb.Use();
            GL.Enable(EnableCap.DepthTest);
            GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            // Render objects
            foreach (var obj in ctx.RenderableObjects)
                if (obj.Enabled)
                    obj.OnRender();

            // Render Screen Surface
            foreach (var obj in ctx.RenderableScreenObjects)
                if (obj.Enabled)
                    obj.OnRender();

            //CheckForProgramError();

            // Commit result
            SwapBuffers();

            base.OnRenderFrame(e);
        }

        private void CheckForProgramError()
        {
            var err = LastErrorCode;
            if (err != ErrorCode.NoError)
            {
                var s = "".ToString();
            }
        }

        public static ErrorCode LastErrorCode => GL.GetError();

        private IPosition MovingObject;

        protected override void OnKeyDown(KeyboardKeyEventArgs e)
        {
            var kbState = Keyboard.GetState();
            if (kbState[Key.C])
            {
                MovingObject = Camera;
            }
            if (kbState[Key.B])
            {
                MovingObject = ctx.GetObjectByName("Box1") as IPosition;
            }
            if (kbState[Key.L])
            {
                MovingObject = ctx.GetObjectByName("StaticLight") as IPosition;
            }
            if (kbState[Key.J])
            {
                Camera.Position = MovingObject.Position;
            }
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            ProcessTaskQueue();

            if (!Focused)
            {
                return;
            }

            var input = Keyboard.GetState();

            if (input.IsKeyDown(Key.Escape))
            {
                Exit();
                Environment.Exit(0);
            }

            var kbState = Keyboard.GetState();

            IPosition pos = MovingObject;
            Camera cam = pos as Camera;
            IScaleRotate rot = MovingObject as IScaleRotate;
            bool simpleMove = cam == null;

            if (kbState[Key.W])
            {
                if (simpleMove)
                    pos.Position = new Vector3(
                        pos.Position.X,
                        pos.Position.Y + 0.1f,
                        pos.Position.Z
                    );
                else
                    pos.Position = new Vector3(
                        pos.Position.X + (float)Math.Cos(cam.Facing) * 0.1f,
                        pos.Position.Y + (float)Math.Sin(cam.Facing) * 0.1f,
                        pos.Position.Z
                    );
            }

            if (kbState[Key.S])
            {
                if (simpleMove)
                    pos.Position = new Vector3(
                        pos.Position.X,
                        pos.Position.Y - 0.1f,
                        pos.Position.Z
                    );
                else
                    pos.Position = new Vector3(
                        pos.Position.X - (float)Math.Cos(cam.Facing) * 0.1f,
                        pos.Position.Y - (float)Math.Sin(cam.Facing) * 0.1f,
                        pos.Position.Z
                    );
            }

            if (kbState[Key.A])
            {
                if (simpleMove)
                    pos.Position = new Vector3(
                        pos.Position.X - 0.1f,
                        pos.Position.Y,
                        pos.Position.Z
                    );
                else
                    pos.Position = new Vector3(
                        pos.Position.X + (float)Math.Cos(cam.Facing + Math.PI / 2) * 0.1f,
                        pos.Position.Y + (float)Math.Sin(cam.Facing + Math.PI / 2) * 0.1f,
                        pos.Position.Z
                    );
            }

            if (kbState[Key.D])
            {
                if (simpleMove)
                    pos.Position = new Vector3(
                        pos.Position.X + 0.1f,
                        pos.Position.Y,
                        pos.Position.Z
                    );
                else
                    pos.Position = new Vector3(
                        pos.Position.X - (float)Math.Cos(cam.Facing + Math.PI / 2) * 0.1f,
                        pos.Position.Y - (float)Math.Sin(cam.Facing + Math.PI / 2) * 0.1f,
                        pos.Position.Z
                    );
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

            if (cam != null)
            {
                cam.Facing += MouseSpeed[0] * 2;
                cam.Pitch += MouseSpeed[1] * 2;
            }
            else if (rot != null)
            {
                rot.Rotate = new Vector3(
                    rot.Rotate.X + MouseSpeed[1] * 2,
                    rot.Rotate.Y,
                    rot.Rotate.Z + MouseSpeed[0] * 2
                );
            }
            //Console.WriteLine(Camera.Pitch + " : " + Math.Round(MouseSpeed[1], 3));
            if (simpleMove)
                pos.Position = new Vector3(
                    pos.Position.X,
                    pos.Position.Y,
                    pos.Position.Z + MouseSpeed[2] * 2
                );
            else
                pos.Position = new Vector3(
                    pos.Position.X,
                    pos.Position.Y,
                    pos.Position.Z + MouseSpeed[2] * 2
                );

            if (kbState[Key.Escape])
                Exit();

            if (kbState[Key.F11])
            {
                Reload();
            }

            if (kbState[Key.F12])
            {
                shadowFb.DestinationTexture.GetDepthTexture().Save("test.png");
            }

            base.OnUpdateFrame(e);
        }

        private Queue<Action> TaskQueue = new Queue<Action>();

        public void Dispatch(Action act)
        {
            lock (TaskQueue)
                TaskQueue.Enqueue(act);
        }

        private void ProcessTaskQueue()
        {
            while (TaskQueue.Count > 0)
            {
                Action act;
                lock (TaskQueue)
                    act = TaskQueue.Dequeue();
                if (act != null)
                    act();
            }
        }

        private void Reload()
        {
            foreach (var obj in ctx.AllObjects)
                if (obj is IReloadable reloadable)
                    reloadable.OnReload();
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

            foreach (var obj in ctx.AllObjects)
                obj.Free();

            base.OnUnload(e);
        }
    }
    public class SceneOptions
    {
    }

}