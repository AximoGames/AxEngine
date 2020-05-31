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

    public class AudioPCMInputModule : AudioModule
    {
        public AudioStream InputStream;
        private AudioInt16Stream Stream16;

        private bool Playing => !Stream16.EndOfStream; // TODO

        public event Action OnEndOfStream;
        private bool OnEndOfStreamRaised = false;

        public void SetInput(AudioStream stream)
        {
            InputStream = stream;
            Stream16 = (AudioInt16Stream)stream;
        }

        public AudioPCMInputModule()
        {
            SetupOutput("Left", 0);
            SetupOutput("Right", 1);
            SetupOutput("Gate", 2);
        }



        public override void Process()
        {
            if (InputStream.Position >= 314540)
            {
                var s = "";
            }

            if (InputStream.EndOfStream)
            {
                if (!OnEndOfStreamRaised)
                {
                    OnEndOfStreamRaised = true;
                    OnEndOfStream?.Invoke();
                }
                Outputs[0].SetVoltage(0);
                Outputs[1].SetVoltage(0);
            }
            else
            {
                for (var i = 0; i < InputStream.Channels; i++)
                {
                    Outputs[i].SetVoltage(ShortToFloat(Stream16.NextSample()) * 10);
                }
            }
            Outputs[2].SetVoltage(Playing ? 1 : 0);
        }

        // http://blog.bjornroche.com/2009/12/int-float-int-its-jungle-out-there.html

        private static float ShortToFloat(short data)
        {
            return (data + 0.5f) / (0x7FFF + 0.5f);
        }
    }
}
