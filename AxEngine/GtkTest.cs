using System.Threading;
using Gdk;
using Gtk;

namespace Aximo.Engine
{
    public class GtkUI
    {

        private Thread th;

        public void Start()
        {
            th = new Thread(Run);
            th.Start();
        }

        private Gtk.Window win;
        private Application app;

        private void Run()
        {
            Application.Init();
            app = new Application("", GLib.ApplicationFlags.None);
            app.Register(GLib.Cancellable.Current);

            win = new Gtk.Window("");
            app.AddWindow(win);

            Fill();

            win.ShowAll();
            Application.Run();
        }

        private void Fill()
        {
            var menu = new GLib.Menu();
            menu.AppendItem(new GLib.MenuItem("Help", "app.help"));
            menu.AppendItem(new GLib.MenuItem("About", "app.about"));
            menu.AppendItem(new GLib.MenuItem("Quit", "app.quit"));
            app.AppMenu = menu;

            win.DefaultSize = new Size(300, 300);

            var s = new Scale(Orientation.Horizontal, 0, 100, 1);
            s.ShowAll();
            win.Add(s);
        }

    }
}