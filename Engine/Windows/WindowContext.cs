// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using OpenToolkit.Graphics.OpenGL4;
using OpenToolkit.Mathematics;
using OpenToolkit.Windowing.Common;
using OpenToolkit.Windowing.GraphicsLibraryFramework;

namespace Aximo.Engine.Windows
{
    /// <summary>
    /// Holds the Window and OpenGL Context.
    /// </summary>
    /// <remarks>
    /// Usefull when keeping the window over multiple unit tests.
    /// </remarks>
    public class WindowContext
    {
        private static Serilog.ILogger Log = Aximo.Log.ForContext<RenderApplication>();

        public RenderWindow Window { get; private set; }

        private Thread UpdateThread;
        private Thread RenderThread;

        internal unsafe void CreateWindow()
        {
            Window = new RenderWindow(Config)
            {
                WindowBorder = Config.WindowBorder,
                Location = new Vector2i((1920 / 2) + 10, 50),
            };
            if (Config.IsMultiThreaded && Environment.OSVersion.Platform == PlatformID.Win32NT)
                GLFW.MakeContextCurrent(null);

            Window.RenderFrame += OnRenderFrameInternal;
            Window.UpdateFrame += OnUpdateFrameInternal;
            Window.FocusedChanged += OnFocusedChangedInternal;
        }

        public event Action<FrameEventArgs> UpdateFrame;
        public event Action<FrameEventArgs> RenderFrame;

        [ThreadStatic]
        internal static bool IsRenderThread = false;

        [ThreadStatic]
        internal static bool IsUpdateThread = false;

        public bool IsFocused { get; private set; }

        private static DebugProc _debugProcCallback;
        private static GCHandle _debugProcCallbackHandle;
        private static Serilog.ILogger OpenGlLog = Aximo.Log.ForContext("OpenGL");
        private static void DebugCallback(DebugSource source, DebugType type, int id, DebugSeverity severity, int length, IntPtr message, IntPtr userParam)
        {
            string messageString = Marshal.PtrToStringAnsi(message, length);

            if (type == DebugType.DebugTypeError)
                OpenGlLog.Error(messageString);
        }

        private void EnableDebugCallback()
        {
            _debugProcCallback = DebugCallback;
            _debugProcCallbackHandle = GCHandle.Alloc(_debugProcCallback);
            GL.DebugMessageCallback(_debugProcCallback, IntPtr.Zero);
            GL.Enable(EnableCap.DebugOutput);
        }

        private void OnUpdateFrameInternal(FrameEventArgs e)
        {
            UpdateReady = true;

            if (Enabled)
                UpdateFrame?.Invoke(e);
        }

        private bool RenderInitialized = false;

        private void OnRenderFrameInternal(FrameEventArgs e)
        {
            if (!RenderInitialized)
            {
                Log.Verbose("Init OpenGL Bindings");
                Log.Verbose("Grab Context");
                Window.MakeCurrent();

                Log.Verbose("Load OpenGL Bindings");
                GL.LoadBindings(new GLFWBindingsContext());
                var vendor = GL.GetString(StringName.Vendor);
                var version = GL.GetString(StringName.Version);
                var shadingLanguageVersion = GL.GetString(StringName.ShadingLanguageVersion);
                var renderer = GL.GetString(StringName.Renderer);

                Log.Info($"Vendor: {vendor}, version: {version}, shadinglangVersion: {shadingLanguageVersion}, renderer: {renderer}");

                EnableDebugCallback();

                RenderInitialized = true;
            }

            if (!RenderThreadHasContext)
            {
                Window.MakeCurrent();
                RenderThreadHasContext = true;
            }

            SetRenderThread();
            RenderReady = true;

            if (Enabled)
                RenderFrame?.Invoke(e);
        }

        private void OnFocusedChangedInternal(FocusedChangedEventArgs e)
        {
            IsFocused = e.IsFocused;
        }

        private bool RenderReady = false;
        private bool UpdateReady = false;

        private bool RenderThreadHasContext = false;

        private void InitGlfw()
        {
            var glfwLibFileName = Environment.OSVersion.Platform == PlatformID.Win32NT ? "glfw3-x64.dll" : "libglfw.so.3.3";
            var glfwLibFileDest = Path.Combine(DirectoryHelper.BinDir, glfwLibFileName);
            if (!File.Exists(glfwLibFileDest))
            {
                var glfwLibFileSrc = Path.Combine(DirectoryHelper.LibsDir, glfwLibFileName);
                Log.Verbose("glfwLibFileSrc: " + glfwLibFileSrc);
                if (!File.Exists(glfwLibFileSrc))
                    glfwLibFileSrc = Path.Combine(DirectoryHelper.NativeRuntimeDir, glfwLibFileName);
                Log.Verbose("glfwLibFileSrc: " + glfwLibFileSrc);

                File.Copy(glfwLibFileSrc, glfwLibFileDest);
            }

            // On some unix systems, the we need to the major version, too.
            if (Environment.OSVersion.Platform == PlatformID.Unix)
            {
                // .so.x.x --> .so.x
                var glfwLibFilePathMajor = Path.Combine(DirectoryHelper.BinDir, Path.GetFileNameWithoutExtension(glfwLibFileName));

                // .so.x --> .so
                var glfwLibFilePathDefault = Path.Combine(DirectoryHelper.BinDir, Path.GetFileNameWithoutExtension(glfwLibFilePathMajor));

                if (!File.Exists(glfwLibFilePathMajor))
                    File.Copy(glfwLibFileDest, glfwLibFilePathMajor);

                if (!File.Exists(glfwLibFilePathDefault))
                    File.Copy(glfwLibFileDest, glfwLibFilePathDefault);
            }

            Log.Info("Init Glfw");
            GLFW.Init();
            Log.Info("GLFW Version: {Version}", GLFW.GetVersionString());
        }

        private RenderApplicationConfig Config;

        public static void Init(RenderApplicationConfig config)
        {
            if (Current != null)
                return;

            Current = new WindowContext();
            Current.InitLocal(config);
        }

        private void InitLocal(RenderApplicationConfig config)
        {
            Config = config;

            UpdateThread = new Thread(UIThread);
            UpdateThread.Start();
            while (!UpdateReady || !RenderReady)
                Thread.Sleep(50);
        }

        public bool Enabled = false;

        private void UIThread()
        {
            if (Thread.CurrentThread.Name == null)
                Thread.CurrentThread.Name = Config.IsMultiThreaded ? "Update Thread" : "Update+Render Thread";
            IsUpdateThread = true;

            DebugHelper.LogThreadInfo(Thread.CurrentThread.Name);
            UpdateThread = Thread.CurrentThread;

            InitGlfw();

            Log.Info("Create Window");
            CreateWindow();
            IsFocused = true;
            Window.Run();
        }

        public static WindowContext Current { get; private set; }

        private bool RenderThreadChecked;

        private void SetRenderThread()
        {
            if (!RenderThreadChecked)
            {
                RenderThreadChecked = true;

                var currentThread = Thread.CurrentThread;
                if (Config.IsMultiThreaded && currentThread != UpdateThread)
                {
                    RenderThread = currentThread;
                    RenderThread.Name = "Render Thread";
                    IsRenderThread = true;
                }
            }
        }
    }
}
