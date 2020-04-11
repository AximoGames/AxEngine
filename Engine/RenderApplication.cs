// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using Aximo.Render;
using OpenToolkit;
using OpenToolkit.Graphics.OpenGL4;
using OpenToolkit.Input;
using OpenToolkit.Mathematics;
using OpenToolkit.Windowing.Common;
using OpenToolkit.Windowing.Common.Input;
using OpenToolkit.Windowing.Desktop;
using OpenToolkit.Windowing.GraphicsLibraryFramework;

namespace Aximo.Engine
{

    public delegate void AfterApplicationInitializedDelegate();

    public class RenderApplication : IDisposable
    {

        private static Serilog.ILogger Log = Aximo.Log.ForContext<RenderApplication>();

        public event AfterApplicationInitializedDelegate AfterApplicationInitialized;

        public static RenderApplication Current { get; private set; }

        public Vector2i ScreenSize => window.Size;

        private RenderApplicationConfig config;

        public bool IsMultiThreaded => config.IsMultiThreaded;

        private float[] MouseSpeed = new float[3];
        private Vector2 MouseDelta;
        private float UpDownDelta;

        public Camera Camera => RenderContext.Camera;

        protected KeyboardState KeyboardState => window.KeyboardState;

        public RenderApplication(RenderApplicationConfig startup)
        {
            config = startup;
        }

        public WindowContext WindowContext => WindowContext.Current;
        private GameWindow window => WindowContext.Current.window;

        private AutoResetEvent RunSyncWaiter;
        public void RunSync()
        {
            RunSyncWaiter = new AutoResetEvent(false);
            Run();
            RunSyncWaiter.WaitOne();
            RunSyncWaiter?.Dispose();
            RunSyncWaiter = null;
        }

        public virtual void Run()
        {
            Current = this;
            Init();
            AfterApplicationInitialized?.Invoke();
            WindowContext.Enabled = true;
        }

        public RenderContext RenderContext { get; private set; }
        public GameContext GameContext { get; private set; }
        public Renderer Renderer { get; private set; }

        public virtual void Init()
        {
            WindowContext.Init(config);
            RegisterWindowEvents();

            Renderer = new Renderer();
            Renderer.Current = Renderer;

            RenderContext = new RenderContext()
            {
                // It's important to take a the size of the new created window instead of the startupConfig,
                // Because they may not be accepted or changed because of other DPI than 100%
                ScreenSize = window.Size,
            };
            RenderContext.Current = RenderContext;

            GameContext = new GameContext
            {
            };
            GameContext.Current = GameContext;

            RenderContext.SceneOpitons = new SceneOptions
            {
            };

            RenderContext.Camera = new PerspectiveFieldOfViewCamera(new Vector3(2f, -5f, 2f), RenderContext.ScreenSize.X / (float)RenderContext.ScreenSize.Y)
            {
                NearPlane = 0.1f,
                FarPlane = 100.0f,
                Facing = 1.88f,
            };

            GameContext.Init();

            //ObjectManager.PushDebugGroup("Setup", "Scene");
            SetupScene();
            //ObjectManager.PopDebugGroup();

            //CursorVisible = false;

            StartFileListener();

            MovingObject = Camera;
            RenderContext.Camera.CameraChangedInternal += () =>
            {
                UpdateMouseWorldPosition();
            };

            Initialized = true;
        }

        private void RegisterWindowEvents()
        {
            // Dont's forget UnregisterWindowEvents!
            WindowContext.RenderFrame += (e) => OnRenderFrameInternal(e);
            WindowContext.UpdateFrame += (e) => OnUpdateFrameInternal(e);
            window.MouseMove += (e) => OnMouseMoveInternal(e);
            window.KeyDown += (e) => OnKeyDownInternal(e);
            window.MouseDown += (e) => OnMouseDownInternal(e);
            window.MouseUp += (e) => OnMouseUpInternal(e);
            window.MouseWheel += (e) => OnMouseWheelInternal(e);
            window.Unload += () => OnUnloadInternal();
            window.Resize += (e) => OnScreenResizeInternal(e);
            window.Closing += (e) => OnClosingInternal(e);
            window.Closed += OnClosed;
        }

        private void UnregisterWindowEvents()
        {
            WindowContext.RenderFrame -= (e) => OnRenderFrameInternal(e);
            WindowContext.UpdateFrame -= (e) => OnUpdateFrameInternal(e);
            window.MouseMove -= (e) => OnMouseMoveInternal(e);
            window.KeyDown -= (e) => OnKeyDownInternal(e);
            window.MouseDown -= (e) => OnMouseDownInternal(e);
            window.MouseUp -= (e) => OnMouseUpInternal(e);
            window.MouseWheel -= (e) => OnMouseWheelInternal(e);
            window.Unload -= () => OnUnloadInternal();
            window.Resize -= (e) => OnScreenResizeInternal(e);
            window.Closing -= (e) => OnClosingInternal(e);
            window.Closed -= OnClosed;
        }

        protected virtual void SetupScene()
        {
        }



        private FileSystemWatcher ShaderWatcher;

        private void StartFileListener()
        {
            ShaderWatcher = new FileSystemWatcher(Path.Combine(DirectoryHelper.EngineRootDir, "Assets", "Shaders"));
            ShaderWatcher.Changed += (sender, e) =>
            {
                // Reload have to be in Main-Thread.
                DispatchUpdater(() => Reload());
            };
            ShaderWatcher.EnableRaisingEvents = true;
        }

        public bool IsFocused
        {
            get
            {
                return true;
                // return window.IsFocused; // Bug since new OpenTK version
            }
        }

        protected virtual void OnRenderFrame(FrameEventArgs e) { }

        public int UpdateFrameNumber { get; private set; } = 0;
        public int RenderFrameNumber { get; private set; } = 0;

        private bool FirstRenderFrame = true;
        private bool FirstUpdateFrame = true;

        protected virtual void BeforeRenderFrame() { }
        protected virtual void AfterRenderFrame() { }

        protected bool RenderingEnabled { get; set; } = true;

        public EventCounter UpdateCounter = new EventCounter();
        public EventCounter RenderCounter = new EventCounter();

        private bool RenderInitialized = false;

        private void OnRenderFrameInternal(FrameEventArgs e)
        {
            if (!RenderInitialized)
            {
                Renderer.Init();
                RenderInitialized = true;
            }

            if (!RenderingEnabled || UpdateFrameNumber == 0)
                return;

            try
            {
                if (Closing)
                    return;


                if (FirstRenderFrame)
                    FirstRenderFrame = false;
                else
                    RenderFrameNumber++;

                RenderCounter.Tick();

                RenderTasks.ProcessTasks();
                BeforeRenderFrame();

                if (Closing)
                    return;

                if (RenderFrameNumber <= 2)
                    Log.Verbose($"Render Frame #{RenderFrameNumber}");

                OnRenderFrame(e);

                if (Closing)
                    return;

                GameContext.Sync();
                Renderer.Render();
                window.SwapBuffers();
                AfterRenderFrame();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error while rendering.");
                Console.WriteLine(ex.ToString());
                throw;
            }
        }

        private IPosition MovingObject;

        private bool DebugCamera;

        protected virtual void OnKeyDown(KeyboardKeyEventArgs e) { }

        private void OnKeyDownInternal(KeyboardKeyEventArgs e)
        {
            OnKeyDown(e);
            if (DefaultKeyBindings)
            {
                var kbState = window.KeyboardState;
                if (kbState[Key.C])
                {
                    if (e.Shift)
                    {
                        DebugCamera = !DebugCamera;
                        var debugLine = RenderContext.GetObjectByName("DebugLine") as LineObject;
                        debugLine.Enabled = DebugCamera;
                        Console.WriteLine($"DebugCamera: {DebugCamera}");
                    }
                    else
                    {
                        MovingObject = Camera;
                    }
                }
                if (kbState[Key.F])
                {
                    Console.WriteLine("Dump infos:");
                    if (MovingObject != null)
                    {
                        Console.WriteLine($"MovingObject Position: {MovingObject.Position}");
                    }
                    Console.WriteLine($"Camer: Facing {Camera.Facing}, Pitch {Camera.Pitch}");
                }
                if (kbState[Key.B])
                {
                    MovingObject = RenderContext.GetObjectByName("Box1") as IPosition;
                }
                if (kbState[Key.L])
                {
                    MovingObject = RenderContext.GetObjectByName("StaticLight") as IPosition;
                }
                if (kbState[Key.J])
                {
                    Camera.Position = MovingObject.Position;
                }
            }
        }

        protected bool Initialized { get; private set; }

        public WindowBorder WindowBorder => window.WindowBorder;

        protected void OnScreenResizeInternal(ResizeEventArgs e)
        {
            if (!Initialized)
                return;

            if (e.Size == RenderContext.ScreenSize)
                return;

            var eventArgs = new ScreenResizeEventArgs(RenderContext.ScreenSize, e.Size);

            Console.WriteLine("OnScreenResize: " + e.Size.X + "x" + e.Size.Y);
            RenderContext.ScreenSize = e.Size;
            GameContext.OnScreenResize(eventArgs);
            DispatchRender(() => RenderContext.OnScreenResize(eventArgs));
        }

        protected virtual void OnUpdateFrame(FrameEventArgs e) { }

        public bool DefaultKeyBindings = true;

        protected virtual void BeforeUpdateFrame() { }
        protected virtual void AfterUpdateFrame() { }

        private void OnUpdateFrameInternal(FrameEventArgs e)
        {
            if (Closing)
                return;

            if (FirstUpdateFrame)
                FirstUpdateFrame = false;
            else
                UpdateFrameNumber++;

            UpdateCounter.Tick();
            GameContext.UpdateTime();

            BeforeUpdateFrame();

            if (Closing)
                return;

            if (UpdateFrameNumber <= 2)
                Log.Verbose($"Update Frame #{UpdateFrameNumber}");

            foreach (var anim in GameContext.Animations)
                anim.ProcessAnimation();

            GameContext.OnUpdateFrame();
            if (Closing)
                return;

            foreach (var obj in RenderContext.UpdateFrameObjects)
                obj.OnUpdateFrame();

            OnUpdateFrame(e);

            if (Closing)
                return;

            UpdaterTasks.ProcessTasks();

            if (!IsFocused)
            {
                return;
            }

            if (DefaultKeyBindings)
            {
                var input = window.KeyboardState;

                if (input.IsKeyDown(Key.Escape))
                {
                    Stop();
                    return;
                }

                var kbState = window.KeyboardState;

                IPosition pos = MovingObject;
                Camera cam = pos as Camera;
                bool simpleMove = cam == null;

                var stepSize = (float)(0.0025f * e.Time);
                if (kbState[Key.ControlLeft])
                    stepSize *= 0.1f;

                if (kbState[Key.W])
                {
                    if (simpleMove)
                        pos.Position = new Vector3(
                            pos.Position.X,
                            pos.Position.Y + stepSize,
                            pos.Position.Z);
                    else
                        pos.Position = new Vector3(
                            pos.Position.X + ((float)Math.Cos(cam.Facing) * stepSize),
                            pos.Position.Y + ((float)Math.Sin(cam.Facing) * stepSize),
                            pos.Position.Z);
                }

                if (kbState[Key.S])
                {
                    if (simpleMove)
                        pos.Position = new Vector3(
                            pos.Position.X,
                            pos.Position.Y - stepSize,
                            pos.Position.Z);
                    else
                        pos.Position = new Vector3(
                            pos.Position.X - ((float)Math.Cos(cam.Facing) * stepSize),
                            pos.Position.Y - ((float)Math.Sin(cam.Facing) * stepSize),
                            pos.Position.Z);
                }

                if (kbState[Key.A])
                {
                    if (simpleMove)
                        pos.Position = new Vector3(
                            pos.Position.X - stepSize,
                            pos.Position.Y,
                            pos.Position.Z);
                    else
                        pos.Position = new Vector3(
                            pos.Position.X + ((float)Math.Cos(cam.Facing + (Math.PI / 2)) * stepSize),
                            pos.Position.Y + ((float)Math.Sin(cam.Facing + (Math.PI / 2)) * stepSize),
                            pos.Position.Z);
                }

                if (kbState[Key.D])
                {
                    if (simpleMove)
                        pos.Position = new Vector3(
                            pos.Position.X + stepSize,
                            pos.Position.Y,
                            pos.Position.Z);
                    else
                        pos.Position = new Vector3(
                            pos.Position.X - ((float)Math.Cos(cam.Facing + (Math.PI / 2)) * stepSize),
                            pos.Position.Y - ((float)Math.Sin(cam.Facing + (Math.PI / 2)) * stepSize),
                            pos.Position.Z);
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

                float reduce = 0.0035f * (float)e.Time;
                float reduceFactor = 1f - reduce;

                MouseSpeed[0] *= reduceFactor;
                MouseSpeed[1] *= reduceFactor;
                MouseSpeed[2] *= reduceFactor;

                MouseSpeed[0] = -(MouseDelta.X / (4000f) * (float)e.Time);
                MouseSpeed[1] = -(MouseDelta.Y / 4000f) * (float)e.Time;
                MouseSpeed[2] = -(UpDownDelta / 4000f) * (float)e.Time;
                MouseDelta = new Vector2();
                UpDownDelta = 0;

                if (cam != null)
                {
                    //Console.WriteLine(MouseSpeed[0]);
                    cam.Facing += MouseSpeed[0];
                    cam.Pitch += MouseSpeed[1];
                }
                else if (MovingObject is IScaleRotate rot)
                {
                    rot.Rotate = new Quaternion(
                        rot.Rotate.X + (MouseSpeed[1]),
                        rot.Rotate.Y,
                        rot.Rotate.Z + (MouseSpeed[0]));
                }
                //Console.WriteLine(Camera.Pitch + " : " + Math.Round(MouseSpeed[1], 3));
                if (simpleMove)
                    pos.Position = new Vector3(
                        pos.Position.X,
                        pos.Position.Y,
                        pos.Position.Z + (MouseSpeed[2]));
                else
                    pos.Position = new Vector3(
                        pos.Position.X,
                        pos.Position.Y,
                        pos.Position.Z + (MouseSpeed[2]));

                if (kbState[Key.F11])
                {
                    Reload();
                }
            }

            // if (kbState[Key.F12])
            // {
            //     shadowFb.DestinationTexture.GetDepthTexture().Save("test.png");
            // }

            AfterUpdateFrame();
        }

        private TaskQueue UpdaterTasks = new TaskQueue();
        public void DispatchUpdater(Action task)
        {
            UpdaterTasks.Dispatch(task);
        }

        private TaskQueue RenderTasks = new TaskQueue();
        public void DispatchRender(Action task)
        {
            RenderTasks.Dispatch(task);
        }

        private void Reload()
        {
            foreach (var obj in RenderContext.AllObjects)
                if (obj is IReloadable reloadable)
                    reloadable.OnReload();
        }

        protected virtual void OnMouseMove(MouseMoveArgs e) { }

        private void OnMouseMoveInternal(MouseMoveEventArgs e)
        {
            var args = new MouseMoveArgs(e);

            // TODO: MIG
            // if (e.Mouse.LeftButton == ButtonState.Pressed)
            //     MouseDelta = new Vector2(e.XDelta, e.YDelta);

            var x = (float)(((double)e.X / (double)ScreenSize.X * 2.0) - 1.0);
            var y = (float)(((double)e.Y / (double)ScreenSize.Y * 2.0) - 1.0);

            CurrentMousePosition = new Vector2(x, y);


            OnMouseMove(args);
            if (args.Handled)
                return;

            GameContext.OnScreenMouseMove(args);

            // Console.WriteLine(CurrentMouseWorldPosition.ToString());
            // Console.WriteLine(CurrentMousePosition.ToString());
        }

        protected virtual void OnMouseDown(MouseButtonArgs e)
        {
        }

        private void OnMouseDownInternal(MouseButtonEventArgs e)
        {
            var args = new MouseButtonArgs(OldMouseButtonPos, CurrentMousePosition, e);
            OldMouseButtonPos = CurrentMousePosition;

            OnMouseDown(args);
            if (args.Handled)
                return;

            GameContext.OnScreenMouseDown(args);
        }

        private Vector2 OldMouseButtonPos;
        protected virtual void OnMouseUp(MouseButtonArgs e)
        {
        }

        private void OnMouseUpInternal(MouseButtonEventArgs e)
        {
            var args = new MouseButtonArgs(OldMouseButtonPos, CurrentMousePosition, e);
            OldMouseButtonPos = CurrentMousePosition;

            OnMouseUp(args);
            if (args.Handled)
                return;

            GameContext.OnScreenMouseUp(args);
        }

        protected virtual void OnMouseWheel(MouseWheelEventArgs e) { }

        private void OnMouseWheelInternal(MouseWheelEventArgs e)
        {
            // TODO: MIG
            // OnMouseWheel(e);
            // if (DefaultKeyBindings)
            //     Camera.Fov -= e.DeltaPrecise;
        }

        protected void OnResize(EventArgs e)
        {
            // GL.Viewport(0, 0, RenderContext.ScreenSize.X, RenderContext.ScreenSize.Y);
            // Camera.AspectRatio = RenderContext.ScreenSize.X / (float)RenderContext.ScreenSize.Y;
        }

        protected virtual void OnUnload() { }

        private void OnUnloadInternal()
        {
            OnUnload();
        }

        protected virtual void OnClosing(CancelEventArgs e) { }

        private void OnClosingInternal(CancelEventArgs e)
        {
            OnClosing(e);
            if (e.Cancel)
                return;
            Stop();
        }

        protected virtual void OnClosed() { }

        private void OnClosedInternal()
        {
            OnClosed();
            Stop();
        }

        public virtual void Dispose()
        {
            SignalShutdown();
            UnregisterWindowEvents();

            Thread.Sleep(200);

            RenderContext.Free();
            RenderContext = null;

            ShaderWatcher?.Dispose();
            ShaderWatcher = null;
            Current = null;
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
                return (new Vector4(ray.GetPoint(result), 1) * RenderContext.WorldPositionMatrix).Xyz;

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

        public double RenderFrequency => window.RenderFrequency;
        public double UpdateFrequency => window.UpdateFrequency;

        private bool _Closing;
        public bool Closing => _Closing || window == null || window.IsExiting;

        // Foreign Thread
        protected void SignalShutdown()
        {
            WindowContext.Enabled = false;
            _Closing = true;
        }

        public bool Stopped { get; private set; }

        // UI Thread
        public virtual void Stop()
        {
            if (_Closing || Stopped)
                return;

            SignalShutdown();
            Stopped = true;
            RunSyncWaiter?.Set();
        }

    }

}
