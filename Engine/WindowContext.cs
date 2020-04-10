// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.ComponentModel;
using System.IO;
using OpenToolkit.Mathematics;
using OpenToolkit.Windowing.Common;
using OpenToolkit.Windowing.GraphicsLibraryFramework;

namespace Aximo.Engine
{

    public class WindowContext
    {

        private static Serilog.ILogger Log = Aximo.Log.ForContext<RenderApplication>();

        public RenderWindow window { get; private set; }

        internal void CreateWindow()
        {
            window = new RenderWindow(Config)
            {
                WindowBorder = Config.WindowBorder,
                Location = new Vector2i((1920 / 2) + 10, 50),
            };
        }

        private void InitGlfw()
        {
            var glfwLibFileName = Environment.OSVersion.Platform == PlatformID.Win32NT ? "glfw3-x64.dll" : "libglfw.so.3.3";
            var glfwLibFileDest = Path.Combine(DirectoryHelper.BinDir, glfwLibFileName);
            if (!File.Exists(glfwLibFileDest))
            {
                var glfwLibFileSrc = Path.Combine(DirectoryHelper.LibsDir, glfwLibFileName);
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
            Current = new WindowContext();
            Current.InitLocal(config);
        }

        private void InitLocal(RenderApplicationConfig config)
        {
            Config = config;
            InitGlfw();

            Log.Info("Create Window");
            CreateWindow();
        }

        public static WindowContext Current { get; private set; }

    }

}
