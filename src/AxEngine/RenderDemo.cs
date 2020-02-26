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

    public class RenderWindow
    {
        private Window win;


    }

    public class RenderApplicationStartup
    {
        public int WindowWidth { get; set; }
        public int WindowHeight { get; set; }
        public string WindowTitle { get; set; }
    }

    public class RenderApplication
    {

        private RenderApplicationStartup _startup;

        public RenderApplication(RenderApplicationStartup startup)
        {
            _startup = startup;
        }

        private GameWindow window;

        public void Run()
        {
            Toolkit.Init(new ToolkitOptions
            {
                Backend = PlatformBackend.PreferX11,
            });

            Context = new RenderContext();
            window = new Window(_startup.WindowWidth, _startup.WindowHeight, _startup.WindowTitle);
            window.Location = new System.Drawing.Point(1920 / 2 + 10, 10);
            window.Run(60.0);
        }

        public RenderContext Context { get; private set; }

        public virtual void OnInit()
        {
        }

        public virtual void OnRenderFrame()
        {
        }

        public void Dispose()
        {
            window.Dispose();
        }

        public void Close()
        {
            window.Close();
        }

    }

    public class RenderDemo : RenderApplication
    {
        public RenderDemo(RenderApplicationStartup startup) : base(startup)
        {
        }

        public override void OnInit()
        {
        }
    }

}