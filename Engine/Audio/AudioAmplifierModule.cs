// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using GLib;
using OpenToolkit.Audio;
using OpenToolkit.Audio.OpenAL;

namespace Aximo.Engine.Audio
{

    public class AudioAmplifierModule : AudioModule
    {
        private Port[] InputChannels;
        private Port[] OutputChannels;

        public AudioAmplifierModule()
        {
            SetupInput("Left", 0);
            SetupInput("Right", 1);
            SetupOutput("Left", 0);
            SetupOutput("Right", 1);
            InputChannels = new Port[] { Inputs[0], Inputs[1] };
            OutputChannels = new Port[] { Outputs[0], Outputs[1] };
        }

        public float Volume = 0.5f;

        public override void Process()
        {
            for (var i = 0; i < InputChannels.Length; i++)
                OutputChannels[i].SetVoltage(InputChannels[i].GetVoltage() * Volume);
        }
    }
}
