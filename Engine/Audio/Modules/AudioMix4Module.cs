// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using GLib;
using OpenToolkit.Audio;
using OpenToolkit.Audio.OpenAL;

namespace Aximo.Engine.Audio
{

    public class AudioMix4Module : AudioModule
    {
        private Port[] InputChannels;
        private Port[] OutputChannels;

        private AudioParameter[] VolumeParam;

        public AudioMix4Module()
        {
            Name = "Amplifier";

            ConfigureParameter("Volume1", 0, 0, 1, 1);
            ConfigureParameter("Volume2", 1, 0, 1, 1);
            ConfigureParameter("Volume3", 2, 0, 1, 1);
            ConfigureParameter("Volume4", 3, 0, 1, 1);

            ConfigureInput("Left1", 0);
            ConfigureInput("Right1", 1);
            ConfigureInput("Left1", 2);
            ConfigureInput("Right1", 3);
            ConfigureInput("Left1", 4);
            ConfigureInput("Right1", 5);
            ConfigureInput("Left1", 6);
            ConfigureInput("Right1", 7);

            ConfigureOutput("Left", 0);
            ConfigureOutput("Right", 1);

            InputChannels = new Port[] { Inputs[0], Inputs[1], Inputs[2], Inputs[3], Inputs[4], Inputs[5], Inputs[6], Inputs[7] };
            OutputChannels = new Port[] { Outputs[0], Outputs[1] };
        }

        private float[] TmpOut = new float[2];

        [MethodImpl(MethodImplOptions.AggressiveOptimization)]
        public override void Process()
        {
            var inputChannels = InputChannels;
            var len = InputChannels.Length / 2;
            var outputChannels = OutputChannels;
            var tmpOut = TmpOut;
            tmpOut[0] = 0;
            tmpOut[1] = 0;
            var parameters = Parameters;

            for (var leftIdx = 0; leftIdx < len; leftIdx += 2)
            {
                var rightIdx = leftIdx + 1;
                tmpOut[leftIdx] += inputChannels[leftIdx].GetVoltage() * parameters[leftIdx].Value;
                tmpOut[rightIdx] += inputChannels[rightIdx].GetVoltage() * parameters[rightIdx].Value;
            }

            outputChannels[0].SetVoltage(tmpOut[0]);
            outputChannels[1].SetVoltage(tmpOut[1]);
        }
    }
}
