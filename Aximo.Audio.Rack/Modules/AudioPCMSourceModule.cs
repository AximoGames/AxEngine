// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Runtime.CompilerServices;

namespace Aximo.Engine.Audio
{

    public class AudioPCMSourceModule : AudioModule
    {
        private static Serilog.ILogger Log = Aximo.Log.ForContext<AudioPCMSourceModule>();

        public AudioStream InputStream;
        private AudioInt16Stream Stream16;

        public bool Playing = false;

        public event Action OnEndOfStream;
        private bool OnEndOfStreamRaised = false;

        public void Play()
        {
            Playing = true;
            OnEndOfStreamRaised = false;
            InputStream.SetPosition(0);
        }

        public void SetInput(AudioStream stream)
        {
            Log.Verbose("Play {path}", stream.Name);

            InputStream = stream;
            Stream16 = (AudioInt16Stream)stream;

            for (var i = 0; i < Outputs.Length; i++)
                Outputs[i].SetVoltage(0);
        }

        public AudioPCMSourceModule()
        {
            Name = "PCM Source";
            ConfigureOutput("Left", 0);
            ConfigureOutput("Right", 1);
            ConfigureOutput("Gate", 2);
            ConfigureOutput("Progress", 3);
        }

        [MethodImpl(MethodImplOptions.AggressiveOptimization)]
        public override void Process()
        {
            if (Rack.Tick == 73888)
            {
                var s = "";
            }

            if (Playing)
            {
                var outputs = Outputs;
                if (InputStream.EndOfStream)
                {
                    Playing = false;
                    if (!OnEndOfStreamRaised)
                    {
                        OnEndOfStreamRaised = true;
                        OnEndOfStream?.Invoke();

                        if (Playing && InputStream.EndOfStream)
                            Playing = false;
                    }

                    if (!Playing)
                    {
                        outputs[0].SetVoltage(0);
                        outputs[1].SetVoltage(0);
                    }
                }

                if (Playing)
                {
                    var inputStreamChannels = InputStream.Channels;
                    for (var i = 0; i < inputStreamChannels; i++)
                    {
                        short sample = Stream16.NextSample();
                        float voltage = PCMConversion.ShortToFloat(sample) * 5f;
                        outputs[i].SetVoltage(voltage);
                        if (inputStreamChannels == 1) // mono
                            outputs[i].SetVoltage(voltage);
                    }

                    outputs[2].SetVoltage(1);

                    if (outputs[3].IsConnected)
                        outputs[3].SetVoltage((float)(InputStream.DataPosition / (double)InputStream.DataLength));
                }
                else
                {
                    outputs[2].SetVoltage(0);
                    outputs[3].SetVoltage(1);
                }
            }
        }
    }
}
