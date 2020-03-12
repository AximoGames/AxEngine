﻿// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using Aximo.Render;
using OpenTK;
using OpenTK.Input;

namespace Aximo.Engine
{

    public class RenderApplication
    {

        public static RenderApplication Current { get; private set; }

        private RenderApplicationStartup _startup;

        private float[] MouseSpeed = new float[3];
        private Vector2 MouseDelta;
        private float UpDownDelta;

        public Camera Camera => RenderContext.Camera;

        public RenderApplication(RenderApplicationStartup startup)
        {
            _startup = startup;
        }

        private GameWindow window;

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

        public RenderContext RenderContext { get; private set; }
        public GameContext GameContext { get; private set; }
        public Renderer Renderer { get; private set; }

        public virtual void Init()
        {
            RenderContext = new RenderContext()
            {
                ScreenSize = _startup.WindowSize,
            };
            RenderContext.Current = RenderContext;

            GameContext = new GameContext
            {
            };
            GameContext.Current = GameContext;

            window = new Window(_startup.WindowSize.X, _startup.WindowSize.Y, _startup.WindowTitle)
            {
                WindowBorder = _startup.WindowBorder,
                Location = new System.Drawing.Point((1920 / 2) + 10, 10),

            };
            window.RenderFrame += (s, e) => OnRenderFrameInternal(e);
            window.UpdateFrame += (s, e) => OnUpdateFrameInternal(e);
            window.MouseMove += (s, e) => OnMouseMoveInternal(e);
            window.KeyDown += (s, e) => OnKeyDownInternal(e);
            window.MouseDown += (s, e) => OnMouseDownInternal(e);
            window.MouseUp += (s, e) => OnMouseUpInternal(e);
            window.MouseWheel += (s, e) => OnMouseWheelInternal(e);
            window.Unload += (s, e) => OnUnloadInternal(e);
            window.Resize += (s, e) => OnScreenResizeInternal();

            //PrintExtensions();

            RenderContext.SceneOpitons = new SceneOptions
            {
            };

            RenderContext.LogInfoMessage("Window.OnLoad");

            Renderer = new Renderer();
            Renderer.Current = Renderer;
            Renderer.Init();

            RenderContext.LightBinding = new BindingPoint();
            Console.WriteLine("LightBinding: " + RenderContext.LightBinding.Number);

            RenderContext.Camera = new PerspectiveFieldOfViewCamera(new Vector3(1f, -5f, 2f), RenderContext.ScreenSize.X / (float)RenderContext.ScreenSize.Y)
            {
                NearPlane = 0.1f,
                FarPlane = 100.0f,
            };
            // ctx.Camera = new OrthographicCamera(new Vector3(1f, -5f, 2f))
            // {
            //     NearPlane = 0.01f,
            //     FarPlane = 100.0f,
            // };

            RenderContext.AddObject(new ScreenSceneObject()
            {
            });

            ObjectManager.PushDebugGroup("Setup", "Scene");
            SetupScene();
            ObjectManager.PopDebugGroup();

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

        protected virtual void OnRenderFrame(FrameEventArgs e) { }

        private void OnRenderFrameInternal(FrameEventArgs e)
        {
            OnRenderFrame(e);

            //--
            var ubo = new UniformBufferObject();
            ubo.Create();
            if (RenderContext.LightObjects.Count >= 2)
            {
                var lightsData = new GlslLight[2];
                lightsData[0].Position = RenderContext.LightObjects[0].Position;
                lightsData[0].Color = new Vector3(0.5f, 0.5f, 0.5f);
                lightsData[0].ShadowLayer = RenderContext.LightObjects[0].ShadowTextureIndex;
                lightsData[0].DirectionalLight = RenderContext.LightObjects[0].LightType == LightType.Directional ? 1 : 0;
                lightsData[0].LightSpaceMatrix = Matrix4.Transpose(RenderContext.LightObjects[0].LightCamera.ViewMatrix * RenderContext.LightObjects[0].LightCamera.ProjectionMatrix);
                lightsData[0].Linear = 0.1f;
                lightsData[0].Quadric = 0f;

                lightsData[1].Position = RenderContext.LightObjects[1].Position;
                lightsData[1].Color = new Vector3(0.5f, 0.5f, 0.5f);
                lightsData[1].ShadowLayer = RenderContext.LightObjects[1].ShadowTextureIndex;
                lightsData[1].DirectionalLight = RenderContext.LightObjects[1].LightType == LightType.Directional ? 1 : 0;
                lightsData[1].LightSpaceMatrix = Matrix4.Transpose(RenderContext.LightObjects[1].LightCamera.ViewMatrix * RenderContext.LightObjects[1].LightCamera.ProjectionMatrix);
                lightsData[1].Linear = 0.1f;
                lightsData[1].Quadric = 0f;
                ubo.SetData(lightsData);

                ubo.SetBindingPoint(RenderContext.LightBinding);
            }

            //--

            //--

            Renderer.InitRender();
            Renderer.Render();

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

        private IPosition MovingObject;

        private bool DebugCamera;

        protected virtual void OnKeyDown(KeyboardKeyEventArgs e) { }

        private void OnKeyDownInternal(KeyboardKeyEventArgs e)
        {
            OnKeyDown(e);
            if (DefaultKeyBindings)
            {
                var kbState = Keyboard.GetState();
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

            Console.WriteLine("OnScreenResize: " + window.Width + "x" + window.Height);
            Renderer.OnScreenResize(ScreenSize);
        }

        protected virtual void OnUpdateFrame(FrameEventArgs e) { }

        public bool DefaultKeyBindings = true;

        private void OnUpdateFrameInternal(FrameEventArgs e)
        {
            foreach (var anim in GameContext.Animations)
                anim.ProcessAnimation();

            foreach (var obj in RenderContext.UpdateFrameObjects)
                obj.OnUpdateFrame();

            OnUpdateFrame(e);

            ProcessTaskQueue();

            if (!window.Focused)
            {
                return;
            }

            if (DefaultKeyBindings)
            {

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
                else if (rot != null)
                {
                    rot.Rotate = new Vector3(
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
            foreach (var obj in RenderContext.AllObjects)
                if (obj is IReloadable reloadable)
                    reloadable.OnReload();
        }

        protected virtual void OnMouseMove(MouseMoveEventArgs e) { }

        private void OnMouseMoveInternal(MouseMoveEventArgs e)
        {
            OnMouseMove(e);

            if (e.Mouse.LeftButton == ButtonState.Pressed)
                MouseDelta = new Vector2(e.XDelta, e.YDelta);

            var x = (float)(((double)e.X / (double)ScreenSize.X * 2.0) - 1.0);
            var y = (float)(((double)e.Y / (double)ScreenSize.Y * 2.0) - 1.0);

            CurrentMousePosition = new Vector2(x, y);
            // Console.WriteLine(CurrentMouseWorldPosition.ToString());
            // Console.WriteLine(CurrentMousePosition.ToString());
        }

        public Vector2i ScreenSize => new Vector2i(window.Width, window.Height);

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
            OnMouseWheel(e);
            if (DefaultKeyBindings)
                Camera.Fov -= e.DeltaPrecise;
        }
 
        protected virtual void OnUnload(EventArgs e) { }

        private void OnUnloadInternal(EventArgs e)
        {
            OnUnload(e);
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

    }

    public class RenderApplicationStartup
    {
        public Vector2i WindowSize { get; set; }
        public string WindowTitle { get; set; }
        public WindowBorder WindowBorder { get; set; } = WindowBorder.Resizable;
    }

}
