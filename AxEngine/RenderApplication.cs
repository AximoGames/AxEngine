﻿// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using Aximo.Render;
using OpenToolkit;
using OpenToolkit.GraphicsLibraryFramework;
using OpenToolkit.Input;
using OpenToolkit.Mathematics;
using OpenToolkit.Windowing.Common;
using OpenToolkit.Windowing.Common.Input;
using OpenToolkit.Windowing.Desktop;

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

        private GameWindow window;

        public void Run()
        {
            if (Thread.CurrentThread.Name == null)
                Thread.CurrentThread.Name = config.IsMultiThreaded ? "Update Thread" : "Update+Render Thread";
            DebugHelper.LogThreadInfo(Thread.CurrentThread.Name);
            UpdateThread = Thread.CurrentThread;
            Current = this;

            Init();
            AfterApplicationInitialized?.Invoke();
            window.Run();
            Console.WriteLine("Exited Run()");
            WindowExited = true;
        }
        protected bool WindowExited;

        public RenderContext RenderContext { get; private set; }
        public GameContext GameContext { get; private set; }
        public Renderer Renderer { get; private set; }

        private Thread UpdateThread;
        private Thread RenderThread;

        private void InitGlfw()
        {
            var glfwLibFileName = Environment.OSVersion.Platform == PlatformID.Win32NT ? "glfw3-x64.dll" : "libglfw.so.3.3";
            var glfwLibFileDest = Path.Combine(DirectoryHelper.BinDir, glfwLibFileName);
            if (!File.Exists(glfwLibFileDest))
            {
                var glfwLibFileSrc = Path.Combine(DirectoryHelper.LibsDir, glfwLibFileName);
                File.Copy(glfwLibFileSrc, glfwLibFileDest);
            }

            GLFW.Init();
            Log.Info("GLFW Version: {Version}", GLFW.GetVersionString());
        }

        public virtual void Init()
        {
            InitGlfw();

            window = new RenderWindow(config)
            {
                WindowBorder = config.WindowBorder,
                Location = new Vector2i((1920 / 2) + 10, 50),
            };
            window.RenderFrame += (e) => OnRenderFrameInternal(e);
            window.UpdateFrame += (e) => OnUpdateFrameInternal(e);
            window.MouseMove += (e) => OnMouseMoveInternal(e);
            window.KeyDown += (e) => OnKeyDownInternal(e);
            window.MouseDown += (e) => OnMouseDownInternal(e);
            window.MouseUp += (e) => OnMouseUpInternal(e);
            window.MouseWheel += (e) => OnMouseWheelInternal(e);
            window.Unload += () => OnUnloadInternal();
            window.Resize += (e) => OnScreenResizeInternal();

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
                Dispatch(() => Reload());
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

        private bool RenderThreadChecked;

        private void SetRenderThread()
        {
            if (!RenderThreadChecked)
            {
                RenderThreadChecked = true;

                var currentThread = Thread.CurrentThread;
                if (config.IsMultiThreaded && currentThread != UpdateThread)
                {
                    RenderThread = currentThread;
                    RenderThread.Name = "Render Thread";
                }
            }
        }

        private bool RenderThreadHasContext = false;

        public EventCounter UpdateCounter = new EventCounter();
        public EventCounter RenderCounter = new EventCounter();

        private bool RenderInitialized = false;

        private void OnRenderFrameInternal(FrameEventArgs e)
        {

            if (!RenderInitialized)
            {
                if (Environment.OSVersion.Platform != PlatformID.Win32NT) // Crash on mswin!
                    window.MakeCurrent();
                Renderer.Init(new GLFWBindingsContext());
                //window.SwapBuffers();
                RenderInitialized = true;
            }

            if (!RenderingEnabled || UpdateFrameNumber == 0)
                return;

            try
            {
                if (Exiting)
                    return;

                if (!RenderThreadHasContext)
                {
                    window.MakeCurrent();
                    RenderThreadHasContext = true;
                }

                if (FirstRenderFrame)
                    FirstRenderFrame = false;
                else
                    RenderFrameNumber++;

                RenderCounter.Tick();

                SetRenderThread();
                BeforeRenderFrame();

                if (Exiting)
                    return;

                if (RenderFrameNumber <= 2)
                    Log.Verbose($"Render Frame #{RenderFrameNumber}");

                OnRenderFrame(e);

                if (Exiting)
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

        protected void OnScreenResizeInternal()
        {
            if (!Initialized)
                return;

            if (WindowBorder != WindowBorder.Resizable)
                return;

            Console.WriteLine("OnScreenResize: " + window.Size.X + "x" + window.Size.Y);
            RenderContext.OnScreenResize();
        }

        protected virtual void OnUpdateFrame(FrameEventArgs e) { }

        public bool DefaultKeyBindings = true;

        protected virtual void BeforeUpdateFrame() { }
        protected virtual void AfterUpdateFrame() { }

        private void OnUpdateFrameInternal(FrameEventArgs e)
        {
            if (Exiting)
                return;

            if (FirstUpdateFrame)
                FirstUpdateFrame = false;
            else
                UpdateFrameNumber++;

            UpdateCounter.Tick();

            BeforeUpdateFrame();

            if (Exiting)
                return;

            if (UpdateFrameNumber <= 2)
                Log.Verbose($"Update Frame #{UpdateFrameNumber}");

            foreach (var anim in GameContext.Animations)
                anim.ProcessAnimation();

            GameContext.OnUpdateFrame();
            if (Exiting)
                return;

            foreach (var obj in RenderContext.UpdateFrameObjects)
                obj.OnUpdateFrame();

            OnUpdateFrame(e);

            if (Exiting)
                return;

            ProcessTaskQueue();

            if (!IsFocused)
            {
                return;
            }

            if (DefaultKeyBindings)
            {
                var input = window.KeyboardState;

                if (input.IsKeyDown(Key.Escape))
                {
                    window.Close();
                    return;
                }

                var kbState = window.KeyboardState;

                IPosition pos = MovingObject;
                Camera cam = pos as Camera;
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
                else if (MovingObject is IScaleRotate rot)
                {
                    rot.Rotate = new Quaternion(
                        rot.Rotate.X + (MouseSpeed[1] * 2),
                        rot.Rotate.Y,
                        rot.Rotate.Z + (MouseSpeed[0] * 2));
                }
                //Console.WriteLine(Camera.Pitch + " : " + Math.Round(MouseSpeed[1], 3));
                if (simpleMove)
                    pos.Position = new Vector3(
                        pos.Position.X,
                        pos.Position.Y,
                        pos.Position.Z + (MouseSpeed[2] * 2));
                else
                    pos.Position = new Vector3(
                        pos.Position.X,
                        pos.Position.Y,
                        pos.Position.Z + (MouseSpeed[2] * 2));

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

                act?.Invoke();
            }
        }

        private void Reload()
        {
            foreach (var obj in RenderContext.AllObjects)
                if (obj is IReloadable reloadable)
                    reloadable.OnReload();
        }

        protected virtual void OnMouseMove(MouseMoveEventArgs e) { }

        private void OnMouseMoveInternal(MouseMoveEventArgs e)
        {
            OnMouseMove(e);

            // TODO: MIG
            // if (e.Mouse.LeftButton == ButtonState.Pressed)
            //     MouseDelta = new Vector2(e.XDelta, e.YDelta);

            var x = (float)(((double)e.X / (double)ScreenSize.X * 2.0) - 1.0);
            var y = (float)(((double)e.Y / (double)ScreenSize.Y * 2.0) - 1.0);

            CurrentMousePosition = new Vector2(x, y);
            // Console.WriteLine(CurrentMouseWorldPosition.ToString());
            // Console.WriteLine(CurrentMousePosition.ToString());
        }

        protected virtual void OnMouseDown(MouseButtonEventArgs e) { }

        private void OnMouseDownInternal(MouseButtonEventArgs e)
        {
            OnMouseDown(e);
        }

        protected virtual void OnMouseUp(MouseButtonEventArgs e) { }

        private void OnMouseUpInternal(MouseButtonEventArgs e)
        {
            OnMouseUp(e);
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
            RenderContext.Free();
        }

        public virtual void Dispose()
        {
            SignalShutdown();
            window?.Close();

            Thread.Sleep(200);

            window?.Dispose();
            window = null;
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

        private bool _Exiting;
        public bool Exiting => _Exiting || window == null || window.IsExiting;

        // Foreign Thread
        protected void SignalShutdown()
        {
            _Exiting = true;
        }

        // UI Thread
        public void Close()
        {
            SignalShutdown();
            window.Close();
        }

    }

}
