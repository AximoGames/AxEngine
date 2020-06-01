// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using OpenToolkit.Audio.OpenAL;

namespace Aximo.Engine.Audio
{
    public class AudioTest
    {
        //public static readonly string Filename = "/home/sebastian/Downloads/the_ring_that_fell.wav";
        public static readonly string Filename = "c:/users/sebastian/Downloads/275416__georcduboe__ambient-naturept2-squarepurity-2-135bpm.wav";
        //public static readonly string Filename = "c:/users/sebastian/Downloads/the_ring_that_fell.wav";

        public static void Main_()
        {
            var rack = new AudioRack();

            var inMod = new AudioPCMSourceModule();
            inMod.SetInput(AudioStream.Load(Filename));
            inMod.OnEndOfStream += () => inMod.Play();

            var inMod2 = new AudioPCMSourceModule();
            inMod2.SetInput(AudioStream.Load(Filename));
            inMod2.OnEndOfStream += () => inMod2.Play();

            var outMod = new AudioPCMOpenALSinkModule();
            var outFile = new AudioSinkStream();

            var mixMod = new AudioMix4Module();
            mixMod.GetParameter("Volume1").SetValue(0.5f);

            rack.AddModule(inMod);
            rack.AddModule(inMod2);
            rack.AddModule(outMod);
            rack.AddModule(mixMod);

            rack.AddCable(inMod.GetOutput("Left"), mixMod.GetInput("Left1"));
            rack.AddCable(inMod.GetOutput("Right"), mixMod.GetInput("Right1"));
            rack.AddCable(inMod2.GetOutput("Left"), mixMod.GetInput("Left2"));
            rack.AddCable(inMod2.GetOutput("Right"), mixMod.GetInput("Right2"));
            rack.AddCable(mixMod.GetOutput("Left"), outMod.GetInput("Left"));
            rack.AddCable(mixMod.GetOutput("Right"), outMod.GetInput("Right"));

            //rack.AddCable(inMod.GetOutput("Gate"), outMod.GetInput("Gate"));

            rack.StartThread();
            Thread.Sleep(1000);
            inMod2.Play();
        }
    }
}
