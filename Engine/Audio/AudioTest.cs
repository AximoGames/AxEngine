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

        public static float[] ShortToFloat(short[] data)
        {
            var len = data.Length;
            var output = new float[len];
            for (var i = 0; i < len; i++)
            {
                output[i] = (data[i] + .5f) / (0x7FFF + .5f);
            }
            return output;
        }

        public static short[] FloatToShort(float[] data)
        {
            var len = data.Length;
            var output = new short[len];
            for (var i = 0; i < len; i++)
            {
                output[i] = (short)((data[i] * (0x7FFF + .5f)) - .5f);
            }
            return output;
        }

        // Loads a wave/riff audio file.
        public static short[] LoadWave(Stream stream, out int channels, out int bits, out int rate)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            using (BinaryReader reader = new BinaryReader(stream))
            {
                // RIFF header
                string signature = new string(reader.ReadChars(4));
                if (signature != "RIFF")
                    throw new NotSupportedException("Specified stream is not a wave file.");

                int riff_chunck_size = reader.ReadInt32();

                string format = new string(reader.ReadChars(4));
                if (format != "WAVE")
                    throw new NotSupportedException("Specified stream is not a wave file.");

                // WAVE header
                string format_signature = new string(reader.ReadChars(4));
                if (format_signature != "fmt ")
                    throw new NotSupportedException("Specified wave file is not supported.");

                int format_chunk_size = reader.ReadInt32();
                int audio_format = reader.ReadInt16();
                int num_channels = reader.ReadInt16();
                int sample_rate = reader.ReadInt32();
                int byte_rate = reader.ReadInt32();
                int block_align = reader.ReadInt16();
                int bits_per_sample = reader.ReadInt16();

                string data_signature = new string(reader.ReadChars(4));
                if (data_signature != "data")
                    throw new NotSupportedException("Specified wave file is not supported.");

                int data_chunk_size = reader.ReadInt32();

                channels = num_channels;
                bits = bits_per_sample;
                rate = sample_rate;

                var len = (reader.BaseStream.Length - reader.BaseStream.Position) / 2;
                short[] data = new short[len];

                for (var i = 0; i < len; i++)
                    data[i] = reader.ReadInt16();

                return FloatToShort(ShortToFloat(data));
                //return data;

                //return reader.ReadBytes((int)reader.BaseStream.Length);
            }
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

        private class ALBuffer
        {
            public int Handle;
        }

        public static void Main_()
        {
            //var devices = ALC.GetStringList(GetEnumerationStringList.DeviceSpecifier);
            //Console.WriteLine($"Devices: {string.Join(", ", devices)}");

            //var device = ALC.OpenDevice(devices.First());
            //var con = ALC.CreateContext(device, (int[])null);
            //ALC.MakeContextCurrent(con);
            //CheckALError();

            //int buffer = AL.GenBuffer();
            //int source = AL.GenSource();
            //int state;

            //int channels, bits_per_sample, sample_rate;
            //var sound_data = LoadWave(File.Open(Filename, FileMode.Open), out channels, out bits_per_sample, out sample_rate);
            //AL.BufferData(buffer, GetSoundFormat(channels, bits_per_sample), sound_data, sound_data.Length * 2, sample_rate);
            //CheckALError();

            var file = (AudioInt16Stream)AudioStream.Load(Filename);
            var rack = new AudioRack();

            var inMod = new AudioPCMSourceModule();
            inMod.SetInput(file);
            //inMod.OnEndOfStream += () => rack.Stop();

            //var outMod = new AudioPCMFileSinkModule();
            var outMod = new AudioPCMOpenALSinkModule();
            var outFile = new AudioSinkStream();
            //outMod.SetOutputStream(outFile);

            var ampMod = new AudioAmplifierModule();

            rack.AddModule(inMod);
            rack.AddModule(outMod);
            rack.AddModule(ampMod);

            rack.AddCable(inMod.GetOutput("Left"), ampMod.GetInput("Left"));
            rack.AddCable(inMod.GetOutput("Right"), ampMod.GetInput("Right"));
            rack.AddCable(ampMod.GetOutput("Left"), outMod.GetInput("Left"));
            rack.AddCable(ampMod.GetOutput("Right"), outMod.GetInput("Right"));
            rack.AddCable(inMod.GetOutput("Gate"), outMod.GetInput("Gate"));

            rack.StartThread();

            //var d = outFile.ToArray();

            //var file = (AudioInt16Stream)AudioStream.Load(Filename);
            //var sound_data = file.ToArray();
            //var sound_data = d;
            //int len;
            //if (file.Channels == 1)
            //    len = sound_data.Length;
            //else
            //    len = sound_data.Length * 2;

            //AL.BufferData(buffer, GetSoundFormat(file.Channels, file.Bits), sound_data, len, file.Rate);

            //AL.Source(source, ALSourcei.Buffer, buffer);
            //CheckALError();
            //AL.SourcePlay(source);
            //CheckALError();

            //Trace.Write("Playing");

            //var time = DateTime.UtcNow;

            //// Query the source to find out when it stops playing.
            //do
            //{
            //    Thread.Sleep(250);
            //    Trace.Write(".");
            //    AL.GetSource(source, ALGetSourcei.SourceState, out state);
            //}
            //while ((ALSourceState)state == ALSourceState.Playing);
            //var duration = DateTime.UtcNow - time;

            //Trace.WriteLine($"Duration {duration}");

            //AL.SourceStop(source);
            //AL.DeleteSource(source);
            //AL.DeleteBuffer(buffer);
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
    }
}
