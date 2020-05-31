// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using OpenToolkit.Audio;
using OpenToolkit.Audio.OpenAL;

namespace Aximo.Engine.Audio
{

    public abstract class AudioModul
    {
        public Port[] Ports;
    }

    public class AudioPCMInputModul : AudioModul
    {

        public AudioStream S;

        public AudioPCMInputModul()
        {
        }
    }

    public class Channel
    {
        public int Number;
        public bool Active;
        public float Voltage;
        public Port Port;

        public Channel(int number, Port port)
        {
            Number = number;
            Port = port;
        }
    }

    public class Port
    {
        public Channel[] Channels;
        public const int MaxChannels = 16;

        public Port()
        {
            Channels = new Channel[MaxChannels];
            for (var i = 0; i < MaxChannels; i++)
                Channels[i] = new Channel(i, this);
        }

        public void SetVoltage(float voltage, int channel = 0)
        {
            Channels[channel].Voltage = voltage;
        }

        public float GetVoltage(int channel = 0)
        {
            return Channels[channel].Voltage;
        }
    }

    public interface IDataStream
    {
    }

    public interface IDataStream<T> : IDataStream
    {
    }

    public class AudioStream : IDataStream
    {
        private protected Stream Stream;
        private protected BinaryReader Reader;
        public int Channels;
        public int Bits;
        public int Rate;
        private long DataStartPosition;

        private void ReadHeader()
        {
            Reader = new BinaryReader(Stream);
            // RIFF header
            string signature = new string(Reader.ReadChars(4));
            if (signature != "RIFF")
                throw new NotSupportedException("Specified stream is not a wave file.");

            int riff_chunck_size = Reader.ReadInt32();

            string format = new string(Reader.ReadChars(4));
            if (format != "WAVE")
                throw new NotSupportedException("Specified stream is not a wave file.");

            // WAVE header
            string format_signature = new string(Reader.ReadChars(4));
            if (format_signature != "fmt ")
                throw new NotSupportedException("Specified wave file is not supported.");

            int format_chunk_size = Reader.ReadInt32();
            int audio_format = Reader.ReadInt16();
            int num_channels = Reader.ReadInt16();
            int sample_rate = Reader.ReadInt32();
            int byte_rate = Reader.ReadInt32();
            int block_align = Reader.ReadInt16();
            int bits_per_sample = Reader.ReadInt16();

            string data_signature = new string(Reader.ReadChars(4));
            if (data_signature != "data")
                throw new NotSupportedException("Specified wave file is not supported.");

            int data_chunk_size = Reader.ReadInt32();

            Channels = num_channels;
            Bits = bits_per_sample;
            Rate = sample_rate;

            DataStartPosition = Reader.BaseStream.Position;
        }

        private protected AudioStream(Stream stream)
        {
            Stream = stream;
        }

        public static AudioStream Load(Stream stream)
        {
            var audio = new AudioInt16Stream(stream);
            audio.ReadHeader();
            return audio;
        }

        public static AudioStream Load(string filePath)
        {
            return Load(File.OpenRead(filePath));
        }

        public bool EndOfStream => Stream.Position >= Stream.Length;
        public long Length => Stream.Length - DataStartPosition;
    }

    public abstract class AudioStream<T> : AudioStream, IDataStream<T>
        where T : unmanaged
    {
        public AudioStream(Stream stream)
            : base(stream)
        {
        }

        public abstract T NextSample();

        public T[] ToArray()
        {
            var samples = new List<T>((int)Length);
            while (!EndOfStream)
                samples.Add(NextSample());
            return samples.ToArray();
        }
    }

    public class AudioInt16Stream : AudioStream<short>
    {
        public AudioInt16Stream(Stream stream)
            : base(stream)
        {
        }

        public override short NextSample()
        {
            return Reader.ReadInt16();
        }
    }

    public abstract class ConvertShortToFloatConverter
    {
    }

    public class AudioTest
    {
        //public static readonly string Filename = "/home/sebastian/Downloads/the_ring_that_fell.wav";
        public static readonly string Filename = "c:/users/sebastian/Downloads/the_ring_that_fell.wav";

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

                //reader.ReadBytes((int)reader.BaseStream.Length;
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

        public static void Main_()
        {
            var devices = ALC.GetStringList(GetEnumerationStringList.DeviceSpecifier);
            Console.WriteLine($"Devices: {string.Join(", ", devices)}");

            var device = ALC.OpenDevice(devices.First());
            var con = ALC.CreateContext(device, (int[])null);
            ALC.MakeContextCurrent(con);
            CheckALError();

            int buffer = AL.GenBuffer();
            int source = AL.GenSource();
            int state;

            int channels, bits_per_sample, sample_rate;
            var sound_data = LoadWave(File.Open(Filename, FileMode.Open), out channels, out bits_per_sample, out sample_rate);
            AL.BufferData(buffer, GetSoundFormat(channels, bits_per_sample), sound_data, sound_data.Length, sample_rate);

            //var file = (AudioInt16Stream)AudioStream.Load(Filename);
            //var sound_data = file.ToArray();
            //AL.BufferData(buffer, GetSoundFormat(file.Channels, file.Bits), sound_data, sound_data.Length, file.Rate);

            AL.Source(source, ALSourcei.Buffer, buffer);
            AL.SourcePlay(source);

            Trace.Write("Playing");

            // Query the source to find out when it stops playing.
            do
            {
                Thread.Sleep(250);
                Trace.Write(".");
                AL.GetSource(source, ALGetSourcei.SourceState, out state);
            }
            while ((ALSourceState)state == ALSourceState.Playing);

            Trace.WriteLine("");

            AL.SourceStop(source);
            AL.DeleteSource(source);
            AL.DeleteBuffer(buffer);
        }

        public static void CheckALError()
        {
            ALError error = AL.GetError();
            if (error != ALError.NoError)
            {
                Console.WriteLine($"ALError: {AL.GetErrorString(error)}");
            }
        }
    }
}
