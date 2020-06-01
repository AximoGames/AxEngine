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
using System.Transactions;
using OpenToolkit.Audio;
using OpenToolkit.Audio.OpenAL;
using OpenToolkit.Graphics.ES20;
using OpenToolkit.Windowing.GraphicsLibraryFramework;

namespace Aximo.Engine.Audio
{

    public class AudioPCMOpenALSinkModule : AudioModule
    {
        private static Serilog.ILogger Log = Aximo.Log.ForContext<AudioPCMOpenALSinkModule>();

        public void SetOutputStream()
        {
        }

        public AudioPCMOpenALSinkModule()
        {
            Name = "PCM Sink";
            ConfigureInput("Left", 0);
            ConfigureInput("Right", 1);
            ConfigureInput("Gate", 2);
            InputChannels = new Port[] { Inputs[0], Inputs[1] };

            Buf1 = new short[BufSize];
            Buf2 = new short[BufSize];
            Buf = Buf1;

            Init();
        }

        private Port[] InputChannels;

        private short[] Buf1;
        private short[] Buf2;
        private short[] Buf;
        private long BufPosition;
        private int SampleSize = sizeof(short) * Channels;
        private long BufSize = 3000 * Channels;

        private const int Channels = 2;

        private void EnsureBuffer()
        {
            if (BufPosition >= BufSize)
            {
                PresentBuffer(Buf);

                if (Buf == Buf1)
                    Buf = Buf2;
                else
                    Buf = Buf1;

                BufPosition = 0;
            }
        }

        private int Bits = 16;
        private int Rate = 44100;

        private void PresentBuffer(short[] buf)
        {
            //Console.WriteLine("Got Buffer. Tick: " + Rack.Tick);
            //if (file.Channels == 1)
            //    len = sound_data.Length;
            //else
            var len = buf.Length * Channels;

            int nextBuf;
            if (buffer == buffer1)
                nextBuf = buffer2;
            else
                nextBuf = buffer1;
            buffer = nextBuf;

            int processed;
            int queued = 2;
            //if (queued < 2)
            //    break;

            while (true)
            {
                AL.GetSource(source, ALGetSourcei.BuffersProcessed, out processed);
                AL.GetSource(source, ALGetSourcei.BuffersQueued, out queued);

                if (queued < 2 || (processed > 0 && queued == 2))
                    break;

                Thread.Sleep(1);
            }

            if (queued == 2 && processed == 2)
                Log.Error("AUDIO BUFFER UNDERRUN");

            if (queued >= 2)
                AL.SourceUnqueueBuffers(source, 1, ref nextBuf);

            CheckALError();
            AL.BufferData(nextBuf, GetSoundFormat(Channels, Bits), buf, len, Rate);
            CheckALError();
            AL.SourceQueueBuffers(source, 1, ref nextBuf);
            CheckALError();

            int state;
            AL.GetSource(source, ALGetSourcei.SourceState, out state);
            if ((ALSourceState)state != ALSourceState.Playing)
                AL.SourcePlay(source);

            AL.GetSource(source, ALGetSourcei.BuffersProcessed, out processed);
            AL.GetSource(source, ALGetSourcei.BuffersQueued, out queued);

            CheckALError();
        }

        public static ALFormat GetSoundFormat(int channels, int bits)
        {
            switch (channels)
            {
                case 1: return bits == 8 ? ALFormat.Mono8 : ALFormat.Mono16;
                case 2: return bits == 8 ? ALFormat.Stereo8 : ALFormat.Stereo16;
                default: throw new NotSupportedException("The specified sound format is not supported.");
            }
        }

        private int buffer1;
        private int buffer2;
        private int buffer;
        private int source;

        private void Init()
        {
            var devices = ALC.GetStringList(GetEnumerationStringList.DeviceSpecifier);
            Console.WriteLine($"Devices: {string.Join(", ", devices)}");

            var device = ALC.OpenDevice(devices.First());
            var con = ALC.CreateContext(device, (int[])null);
            ALC.MakeContextCurrent(con);
            CheckALError();

            buffer1 = AL.GenBuffer();
            buffer2 = AL.GenBuffer();
            buffer = buffer1;
            source = AL.GenSource();
            int state;
        }

        public static void CheckALError()
        {
            ALError error = AL.GetError();
            if (error != ALError.NoError)
            {
                var msg = $"ALError: {AL.GetErrorString(error)}";
                throw new Exception(msg);
            }
        }

        private void Dispose()
        {
            AL.SourceStop(source);
            AL.DeleteSource(source);
            AL.DeleteBuffer(buffer);
        }

        [MethodImpl(MethodImplOptions.AggressiveOptimization)]
        public override void Process()
        {
            EnsureBuffer();

            if (!Inputs[2].IsConnected || Inputs[2].GetVoltage() >= 0.9f)
                for (var i = 0; i < InputChannels.Length; i++)
                    WritePCMSample(PCMConversion.FloatToShort(InputChannels[i].GetVoltage() / 10));
            else
                for (var i = 0; i < InputChannels.Length; i++)
                    WritePCMSample(0);
        }

        private void WritePCMSample(short sample)
        {
            Buf[BufPosition] = sample;
            BufPosition += 1;
        }
    }
}
