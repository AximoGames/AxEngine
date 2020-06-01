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
using OpenToolkit.Audio;
using OpenToolkit.Audio.OpenAL;

namespace Aximo.Engine.Audio
{

    public class AudioPCMSourceModule : AudioModule
    {
        public AudioStream InputStream;
        private AudioInt16Stream Stream16;

        private bool Playing
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return !Stream16.EndOfStream; // TODO
            }
        }

        public event Action OnEndOfStream;
        private bool OnEndOfStreamRaised = false;

        public void SetInput(AudioStream stream)
        {
            InputStream = stream;
            Stream16 = (AudioInt16Stream)stream;
        }

        public AudioPCMSourceModule()
        {
            Name = "PCM Source";
            ConfigureOutput("Left", 0);
            ConfigureOutput("Right", 1);
            ConfigureOutput("Gate", 2);
        }

        [MethodImpl(MethodImplOptions.AggressiveOptimization)]
        public override void Process()
        {
            var outputs = Outputs;
            if (InputStream.EndOfStream)
            {
                if (!OnEndOfStreamRaised)
                {
                    OnEndOfStreamRaised = true;
                    OnEndOfStream?.Invoke();
                }
                outputs[0].SetVoltage(0);
                outputs[1].SetVoltage(0);
            }
            else
            {
                var inputStreamChannels = InputStream.Channels;
                for (var i = 0; i < inputStreamChannels; i++)
                    outputs[i].SetVoltage(PCMConversion.ShortToFloat(Stream16.NextSample()) * 10);
            }
            outputs[2].SetVoltage(Playing ? 1 : 0);
        }
    }
}
