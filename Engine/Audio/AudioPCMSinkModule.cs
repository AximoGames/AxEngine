// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using OpenToolkit.Audio;
using OpenToolkit.Audio.OpenAL;

namespace Aximo.Engine.Audio
{

    public class AudioPCMSinkModule : AudioModule
    {
        public AudioSinkStream OutputStream;

        public void SetOutputStream(AudioSinkStream stream)
        {
            OutputStream = stream;
        }

        public AudioPCMSinkModule()
        {
            SetupInput("Left", 0);
            SetupInput("Right", 1);
            SetupInput("Gate", 2);
            InputChannels = new Port[] { Inputs[0], Inputs[1] };
        }

        private Port[] InputChannels;

        public override void Process()
        {
            if (Inputs[2].GetVoltage() >= 0.9f)
                for (var i = 0; i < InputChannels.Length; i++)
                    OutputStream.Write(FloatToShort(InputChannels[i].GetVoltage() / 10));
        }

        public static short FloatToShort(float data)
        {
            if (data > 1)
                data = 1;
            else if (data < -1)
                data = -1;

            return (short)((data * (0x7FFF + 0.5f)) - 0.5f);
        }
    }
}
