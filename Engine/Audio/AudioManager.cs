// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using System.Threading;
using System.Xml;
//using System.Media;
using NetCoreAudio;

namespace Aximo.Engine.Audio
{

    public class AudioManager
    {
        private static Serilog.ILogger Log;

        public bool Mute => CommandLineOptions.Current.Mute;
        //public bool Mute => true;

        public static AudioManager Default { get; private set; }

        static AudioManager()
        {
            Log = Aximo.Log.ForContext<AudioManager>();
            Default = new AudioManager();
        }

        public AudioMainRack Rack;

        public AudioManager()
        {
            if (Mute)
                return;

            AudioModuleManager.Current.RegisterModules(typeof(AudioPCMSourceModule).Assembly);

            var rack = new AudioMainRack();
            Rack = rack;

            var inMod = new AudioPCMSourceModule();
            var inMod2 = new AudioPCMSourceModule();
            var inMod3 = new AudioPCMSourceModule();
            var inMod4 = new AudioPCMSourceModule();

            var outMod = new AudioPCMOpenALSinkModule();
            var outFile = new AudioSinkStream();

            var mixMod = new AudioMix4Module();
            mixMod.GetParameter("Volume1").SetValue(1f);

            rack.AddModule(inMod);
            rack.AddModule(inMod2);
            rack.AddModule(inMod3);
            rack.AddModule(inMod4);

            rack.AddModule(outMod);
            rack.AddModule(mixMod);

            rack.AddCable(inMod.GetOutput("Left"), mixMod.GetInput("Left1"));
            rack.AddCable(inMod.GetOutput("Right"), mixMod.GetInput("Right1"));

            rack.AddCable(inMod2.GetOutput("Left"), mixMod.GetInput("Left2"));
            rack.AddCable(inMod2.GetOutput("Right"), mixMod.GetInput("Right2"));

            rack.AddCable(inMod3.GetOutput("Left"), mixMod.GetInput("Left3"));
            rack.AddCable(inMod3.GetOutput("Right"), mixMod.GetInput("Right3"));

            rack.AddCable(inMod4.GetOutput("Left"), mixMod.GetInput("Left4"));
            rack.AddCable(inMod4.GetOutput("Right"), mixMod.GetInput("Right4"));

            rack.AddCable(mixMod.GetOutput("Left"), outMod.GetInput("Left"));
            rack.AddCable(mixMod.GetOutput("Right"), outMod.GetInput("Right"));

            rack.StartThread();
        }

        private string GetPath(string path)
        {
            return AssetManager.GetAssetsPath(path);
        }

        public void PlayAsync(string path)
        {
            if (Mute) return;

            ThreadPool.QueueUserWorkItem((state) =>
            {
                try
                {
                    var stream = AudioStream.Load(GetPath(path));
                    stream.PreloadAll();
                    //Log.Verbose("Play {path}", path);
                    var mod = Rack.GetModules<AudioPCMSourceModule>().OrderBy(m => m.GetOutput("Gate").GetVoltage()).ThenByDescending(m => m.GetOutput("Progress").GetVoltage()).FirstOrDefault();
                    Rack.Dispatch(() =>
                    {
                        mod.SetInput(stream);
                        mod.Play();
                    });
                }
                catch (Exception ex)
                {
                    Log.Error(ex);
                }
            });
        }

        public void PlaySync(string path)
        {
            if (Mute) return;

            // Player.Stop();
            // Player.SoundLocation = GetPath(path);
            // Player.PlaySync();

            throw new NotSupportedException();
        }
    }
}
