using System;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Input;
using AxEngine;
using System.IO;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace AxEngine
{

    public class RenderApplication
    {

        public static RenderApplication Current { get; private set; }

        public int ScreenWidth => window.Width;
        public int ScreenHeight => window.Height;

        private RenderApplicationStartup _startup;

        private float[] MouseSpeed = new float[3];
        private Vector2 MouseDelta;
        private float UpDownDelta;

        public Camera Camera => ctx.Camera;

        private double CamAngle = 0;

        public RenderApplication(RenderApplicationStartup startup)
        {
            _startup = startup;
        }

        private static void PrintExtensions()
        {
            int numExtensions;
            GL.GetInteger(GetPName.NumExtensions, out numExtensions);
            for (var i = 0; i < numExtensions; i++)
            {
                var extName = GL.GetString(StringNameIndexed.Extensions, i);
                Console.WriteLine(extName);
            }
        }

        private GameWindow window;

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

        public void Run()
        {
            Current = this;

            Toolkit.Init(new ToolkitOptions
            {
                Backend = PlatformBackend.PreferX11,
            });

            Init();
            window.Run(60.0);
        }

        public RenderContext ctx { get; private set; }

        public virtual void Init()
        {
            ctx = new RenderContext()
            {
                ScreenWidth = _startup.WindowWidth,
                ScreenHeight = _startup.WindowHeight,
            };
            RenderContext.Current = ctx;

            window = new Window(_startup.WindowWidth, _startup.WindowHeight, _startup.WindowTitle);
            window.Location = new System.Drawing.Point(1920 / 2 + 10, 10);
            window.RenderFrame += (s, e) => OnRenderFrame(e);
            window.UpdateFrame += (s, e) => OnUpdateFrame(e);
            window.MouseMove += (s, e) => OnMouseMove(e);
            window.KeyDown += (s, e) => OnKeyDown(e);
            window.MouseWheel += (s, e) => OnMouseWheel(e);
            window.Unload += (s, e) => OnUnload(e);
            window.Resize += (s, e) => OnScreenResize();

            var vendor = GL.GetString(StringName.Vendor);
            var version = GL.GetString(StringName.Version);
            var shadingLanguageVersion = GL.GetString(StringName.ShadingLanguageVersion);
            var renderer = GL.GetString(StringName.Renderer);

            Console.WriteLine($"Vendor: {vendor}, version: {version}, shadinglangVersion: {shadingLanguageVersion}, renderer: {renderer}");

            //PrintExtensions();

            GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);

            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.CullFace);

            ctx.SceneOpitons = new SceneOptions
            {
            };

            ctx.LogInfoMessage("Window.OnLoad");

            ObjectManager.PushDebugGroup("Setup", "Pipelines");
            SetupPipelines();
            ObjectManager.PopDebugGroup();

            ctx.LightBinding = new BindingPoint();
            Console.WriteLine("LightBinding: " + ctx.LightBinding.Number);

            ctx.Camera = new PerspectiveFieldOfViewCamera(new Vector3(1f, -5f, 2f), ctx.ScreenWidth / (float)ctx.ScreenHeight)
            {
                NearPlane = 0.1f,
                FarPlane = 100.0f,
            };
            // ctx.Camera = new OrthographicCamera(new Vector3(1f, -5f, 2f))
            // {
            //     NearPlane = 0.01f,
            //     FarPlane = 100.0f,
            // };
            ctx.Camera.CameraChangedInternal += () =>
            {
                UpdateMouseWorldPosition();
            };


            ObjectManager.PushDebugGroup("Setup", "Scene");
            SetupScene();
            ObjectManager.PopDebugGroup();

            //CursorVisible = false;

            ctx.AddObject(new ScreenSceneObject()
            {
            });

            StartFileListener();

            MovingObject = Camera;
        }

        public void SetupPipelines()
        {
            ctx.AddPipeline(new DirectionalShadowRenderPipeline());
            ctx.AddPipeline(new PointShadowRenderPipeline());
            ctx.AddPipeline(new DeferredRenderPipeline());
            ctx.AddPipeline(new ForwardRenderPipeline());
            ctx.AddPipeline(new ScreenPipeline());

            ObjectManager.PushDebugGroup("BeforeInit", "Pipelines");
            foreach (var pipe in ctx.RenderPipelines)
            {
                ObjectManager.PushDebugGroup("BeforeInit", pipe);
                pipe.BeforeInit();
                ObjectManager.PopDebugGroup();
            }
            ObjectManager.PopDebugGroup();

            ObjectManager.PushDebugGroup("Init", "Pipelines");
            foreach (var pipe in ctx.RenderPipelines)
            {
                ObjectManager.PushDebugGroup("Init", pipe);
                pipe.Init();
                ObjectManager.PopDebugGroup();
            }
            ObjectManager.PopDebugGroup();

            ObjectManager.PushDebugGroup("AfterInit", "Pipelines");
            foreach (var pipe in ctx.RenderPipelines)
            {
                ObjectManager.PushDebugGroup("AfterInit", pipe);
                pipe.AfterInit();
                ObjectManager.PopDebugGroup();
            }
            ObjectManager.PopDebugGroup();
        }

        protected virtual void SetupScene()
        {
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

        public virtual void OnRenderFrame(FrameEventArgs e)
        {
            CamAngle -= 0.01;
            var pos = new Vector3((float)(Math.Cos(CamAngle) * 2f), (float)(Math.Sin(CamAngle) * 2f), 1.5f);
            ILightObject light = ctx.LightObjects[0];

            light.Position = pos;

            //--
            var ubo = new UniformBufferObject();
            ubo.Create();
            var lightsData = new GlslLight[2];
            lightsData[0].Position = ctx.LightObjects[0].Position;
            lightsData[0].Color = new Vector3(0.5f, 0.5f, 0.5f);
            lightsData[0].ShadowLayer = ctx.LightObjects[0].ShadowTextureIndex;
            lightsData[0].DirectionalLight = ctx.LightObjects[0].LightType == LightType.Directional ? 1 : 0;
            lightsData[0].LightSpaceMatrix = Matrix4.Transpose(ctx.LightObjects[0].LightCamera.ViewMatrix * ctx.LightObjects[0].LightCamera.ProjectionMatrix);
            lightsData[0].Linear = 0.1f;
            lightsData[0].Quadric = 0f;

            lightsData[1].Position = ctx.LightObjects[1].Position;
            lightsData[1].Color = new Vector3(0.5f, 0.5f, 0.5f);
            lightsData[1].ShadowLayer = ctx.LightObjects[1].ShadowTextureIndex;
            lightsData[1].DirectionalLight = ctx.LightObjects[1].LightType == LightType.Directional ? 1 : 0;
            lightsData[1].LightSpaceMatrix = Matrix4.Transpose(ctx.LightObjects[1].LightCamera.ViewMatrix * ctx.LightObjects[1].LightCamera.ProjectionMatrix);
            lightsData[1].Linear = 0.1f;
            lightsData[1].Quadric = 0f;
            ubo.SetData(lightsData);

            ubo.SetBindingPoint(ctx.LightBinding);

            //--
            GL.Enable(EnableCap.DepthTest);

            //--

            foreach (var pipeline in ctx.RenderPipelines)
            {
                ObjectManager.PushDebugGroup("InitRender", pipeline);
                ctx.CurrentPipeline = pipeline;
                pipeline.InitRender(ctx, ctx.Camera);
                ObjectManager.PopDebugGroup();
            }

            foreach (var pipeline in ctx.RenderPipelines)
            {
                ObjectManager.PushDebugGroup("Render", pipeline);
                ctx.CurrentPipeline = pipeline;
                pipeline.Render(ctx, ctx.Camera);
                ObjectManager.PopDebugGroup();
            }

            //--

            // Configure

            // Render objects

            // Render Screen Surface

            //CheckForProgramError();

            // Commit result
            window.SwapBuffers();

            ubo.Free();
        }

        [Serializable]
        [StructLayout(LayoutKind.Explicit, Size = 112)]
        private struct GlslLight
        {
            [FieldOffset(0)]
            public Vector3 Position;

            [FieldOffset(16)]
            public Vector3 Color;

            [FieldOffset(32)]
            public Matrix4 LightSpaceMatrix;

            [FieldOffset(96)]
            public int ShadowLayer;

            [FieldOffset(100)]
            public int DirectionalLight; // Bool

            [FieldOffset(104)]
            public float Linear;

            [FieldOffset(108)]
            public float Quadric;
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

        private bool DebugCamera;
        protected void OnKeyDown(KeyboardKeyEventArgs e)
        {
            var kbState = Keyboard.GetState();
            if (kbState[Key.C])
            {
                if (e.Shift)
                {
                    DebugCamera = !DebugCamera;
                    var debugLine = ctx.GetObjectByName("DebugLine") as LineObject;
                    debugLine.Enabled = DebugCamera;
                    Console.WriteLine($"DebugCamera: {DebugCamera}");
                }
                else
                {
                    MovingObject = Camera;
                }
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

        protected void OnScreenResize()
        {
            ctx.OnScreenResize();
        }

        protected void OnUpdateFrame(FrameEventArgs e)
        {
            ProcessTaskQueue();

            if (!window.Focused)
            {
                return;
            }

            var input = Keyboard.GetState();

            if (input.IsKeyDown(Key.Escape))
            {
                window.Exit();
                Environment.Exit(0);
                return;
            }

            var kbState = Keyboard.GetState();

            IPosition pos = MovingObject;
            Camera cam = pos as Camera;
            IScaleRotate rot = MovingObject as IScaleRotate;
            bool simpleMove = cam == null;

            var stepSize = 0.1f;
            if (kbState[Key.ControlLeft])
                stepSize = 0.01f;

            if (kbState[Key.W])
            {
                if (simpleMove)
                    pos.Position = new Vector3(
                        pos.Position.X,
                        pos.Position.Y + stepSize,
                        pos.Position.Z
                    );
                else
                    pos.Position = new Vector3(
                        pos.Position.X + (float)Math.Cos(cam.Facing) * stepSize,
                        pos.Position.Y + (float)Math.Sin(cam.Facing) * stepSize,
                        pos.Position.Z
                    );
            }

            if (kbState[Key.S])
            {
                if (simpleMove)
                    pos.Position = new Vector3(
                        pos.Position.X,
                        pos.Position.Y - stepSize,
                        pos.Position.Z
                    );
                else
                    pos.Position = new Vector3(
                        pos.Position.X - (float)Math.Cos(cam.Facing) * stepSize,
                        pos.Position.Y - (float)Math.Sin(cam.Facing) * stepSize,
                        pos.Position.Z
                    );
            }

            if (kbState[Key.A])
            {
                if (simpleMove)
                    pos.Position = new Vector3(
                        pos.Position.X - stepSize,
                        pos.Position.Y,
                        pos.Position.Z
                    );
                else
                    pos.Position = new Vector3(
                        pos.Position.X + (float)Math.Cos(cam.Facing + Math.PI / 2) * stepSize,
                        pos.Position.Y + (float)Math.Sin(cam.Facing + Math.PI / 2) * stepSize,
                        pos.Position.Z
                    );
            }

            if (kbState[Key.D])
            {
                if (simpleMove)
                    pos.Position = new Vector3(
                        pos.Position.X + stepSize,
                        pos.Position.Y,
                        pos.Position.Z
                    );
                else
                    pos.Position = new Vector3(
                        pos.Position.X - (float)Math.Cos(cam.Facing + Math.PI / 2) * stepSize,
                        pos.Position.Y - (float)Math.Sin(cam.Facing + Math.PI / 2) * stepSize,
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

            if (kbState[Key.F11])
            {
                Reload();
            }

            // if (kbState[Key.F12])
            // {
            //     shadowFb.DestinationTexture.GetDepthTexture().Save("test.png");
            // }
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

        protected void OnMouseMove(MouseMoveEventArgs e)
        {
            if (e.Mouse.LeftButton == ButtonState.Pressed)
                MouseDelta = new Vector2(e.XDelta, e.YDelta);

            var x = (float)((((double)e.X / (double)ScreenWidth) * 2.0) - 1.0);
            var y = (float)((((double)e.Y / (double)ScreenHeight) * 2.0) - 1.0);

            CurrentMousePosition = new Vector2(x, y);
            // Console.WriteLine(CurrentMouseWorldPosition.ToString());
            // Console.WriteLine(CurrentMousePosition.ToString());
        }

        protected void OnMouseWheel(MouseWheelEventArgs e)
        {
            Camera.Fov -= e.DeltaPrecise;
        }

        protected void OnResize(EventArgs e)
        {
            GL.Viewport(0, 0, ctx.ScreenWidth, ctx.ScreenHeight);
            Camera.AspectRatio = ctx.ScreenWidth / (float)ctx.ScreenHeight;
        }

        protected void OnUnload(EventArgs e)
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);
            GL.UseProgram(0);

            foreach (var obj in ctx.AllObjects)
                obj.Free();
        }

        public void Dispose()
        {
            window.Dispose();
        }

        public void Close()
        {
            window.Close();
        }

        private Vector2 _CurrentMousePosition;
        public Vector2 CurrentMousePosition
        {
            get
            {
                return _CurrentMousePosition;
            }
            set
            {
                if (_CurrentMousePosition == value)
                    return;
                _CurrentMousePosition = value;
                UpdateMouseWorldPosition();
            }
        }

        public bool CurrentMouseWorldPositionIsValid { get; private set; }

        internal void UpdateMouseWorldPosition()
        {
            var pos = ScreenPositionToWorldPosition(CurrentMousePosition);
            if (pos != null)
            {
                _CurrentMouseWorldPosition = (Vector3)pos;
                CurrentMouseWorldPositionIsValid = true;
                //Console.WriteLine(_CurrentMouseWorldPosition);
            }
            else
            {
                CurrentMouseWorldPositionIsValid = false;
            }
        }

        private Vector3 _CurrentMouseWorldPosition;
        public Vector3 CurrentMouseWorldPosition
        {
            get { return _CurrentMouseWorldPosition; }
        }

        protected Vector3? ScreenPositionToWorldPosition(Vector2 normalizedScreenCoordinates)
        {
            // FUTURE: Read Dept-Buffer to get the avoid ray-Cast and get adaptive Z-Position
            // Currently, it's fixed to 0 (Plane is at Z=0).
            var plane = new Plane(new Vector3(0, 0, 1), new Vector3(0, 0, 0));

            var pos1 = UnProject(normalizedScreenCoordinates, -1); // -1 requied for ortho. With z=0, perspective will not work, but no ortho
            var pos2 = UnProject(normalizedScreenCoordinates, 1);

            // if (!DebugCamera)
            // {
            //     var debugLine = ctx.GetObjectByName("DebugLine") as Line;
            //     // debugLine.SetPoint1(new Vector3(0, 0, 1));
            //     // debugLine.SetPoint2(new Vector3(3, 3, 1));
            //     debugLine.SetPoint1(pos1);
            //     debugLine.SetPoint2(pos2);
            //     debugLine.UpdateData();
            // }

            //Console.WriteLine($"{pos1} + {pos2}");

            var ray = Ray.FromPoints(pos1, pos2);
            if (plane.Raycast(ray, out float result))
                return ray.GetPoint(result);

            return null;
        }

        public Vector3 UnProject(Vector2 mouse, float z)
        {
            Vector4 vec;

            vec.X = mouse.X;
            vec.Y = -mouse.Y;
            vec.Z = z;
            vec.W = 1.0f;

            vec = Vector4.Transform(vec, Camera.InvertedViewProjectionMatrix);

            return vec.Xyz / vec.W;
        }
    }

    public class RenderApplicationStartup
    {
        public int WindowWidth { get; set; }
        public int WindowHeight { get; set; }
        public string WindowTitle { get; set; }
    }

    public class SceneOptions
    {
    }

}