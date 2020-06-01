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
            var file = AudioStream.Load(Filename);
            var rack = new AudioRack();

            var inMod = new AudioPCMSourceModule();
            inMod.SetInput(file);
            inMod.OnEndOfStream += () => inMod.Play();

            var outMod = new AudioPCMOpenALSinkModule();
            var outFile = new AudioSinkStream();

            var ampMod = new AudioAmplifierModule();
            ampMod.GetParameter("Volume").SetValue(0.5f);

            var ampMix = new AudioMix4Module();

            rack.AddModule(inMod);
            rack.AddModule(outMod);
            rack.AddModule(ampMod);

            rack.AddCable(inMod.GetOutput("Left"), ampMod.GetInput("Left"));
            rack.AddCable(inMod.GetOutput("Right"), ampMod.GetInput("Right"));
            rack.AddCable(ampMod.GetOutput("Left"), outMod.GetInput("Left"));
            rack.AddCable(ampMod.GetOutput("Right"), outMod.GetInput("Right"));
            rack.AddCable(inMod.GetOutput("Gate"), outMod.GetInput("Gate"));

            rack.StartThread();
        }
    }
}
