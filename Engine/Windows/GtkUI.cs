// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using Gdk;
using Gtk;
using OpenToolkit.Graphics.OpenGL4;
using OpenToolkit.Mathematics;

namespace Aximo.Engine.Windows
{
    public class GtkUI : IDisposable
    {
        private Thread th;

        public void Start()
        {
            th = new Thread(Run);
            th.Start();
        }

        private Gtk.Window win;
        private Gtk.Application app;

        private void Run()
        {
            Gtk.Application.Init();
            app = new Gtk.Application("", GLib.ApplicationFlags.None);
            app.Register(GLib.Cancellable.Current);

            win = new Gtk.Window("");
            app.AddWindow(win);

            Fill();

            win.ShowAll();
            Gtk.Application.Run();
        }

        public GLArea area;
        public Label label;

        private void Fill()
        {
            var menu = new GLib.Menu();
            menu.AppendItem(new GLib.MenuItem("Help", "app.help"));
            menu.AppendItem(new GLib.MenuItem("About", "app.about"));
            menu.AppendItem(new GLib.MenuItem("Quit", "app.quit"));
            app.AppMenu = menu;

            win.DefaultSize = new Size(300, 300);

            var fix = new Fixed();
            win.Add(fix);

            area = new GLArea();

            area.Realized += AreaOnRealized;
            area.Render += AreaOnRender;
            area.SetSizeRequest(200, 200);
            fix.Put(area, 50, 50);

            var s = new Scale(Orientation.Horizontal, 0, 100, 1);
            s.SetSizeRequest(100, 100);
            fix.Put(s, 0, 0);

            label = new Label("Blubb");
            label.SetSizeRequest(100, 100);
            fix.Put(label, 0, 0);

            win.ShowAll();
        }

        private void AreaOnRealized(object sender, EventArgs e)
        {
            area.MakeCurrent();
            GL.LoadBindings(new NativeBindingsContext());
            GL.ClearColor(Color4.DarkRed);
        }

        private DateTime LastLog = DateTime.UtcNow;
        private EventCounter TmpCounter = new EventCounter();
        private void AreaOnRender(object o, RenderArgs args)
        {
            TmpCounter.Tick();
            if ((DateTime.UtcNow - LastLog).TotalSeconds > 4)
            {
                LastLog = DateTime.UtcNow;
                //Console.WriteLine($"FPS: {TmpCounter.EventsPerSecond.ToString("F1")}");
                label.Text = $"FPS: {TmpCounter.EventsPerSecond:F1}";
            }
            GL.Clear(ClearBufferMask.ColorBufferBit);
            //Console.Write(".");
            area.QueueDraw();
        }

        public void Dispose()
        {
            win?.Dispose();
            win = null;
            app?.Dispose();
            app = null;
        }
    }
}
