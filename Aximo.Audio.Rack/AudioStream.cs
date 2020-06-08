// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.IO;

namespace Aximo.Engine.Audio
{
    public class AudioStream : IDataStream
    {
        public string Name;
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

            DataLength = Reader.ReadInt32();

            Channels = num_channels;
            Bits = bits_per_sample;
            Rate = sample_rate;

            SampleSizeAllChannels = (bits_per_sample / 8) * Channels;
            DataStartPosition = Reader.BaseStream.Position; // normally 44
        }

        public int SampleSizeAllChannels;

        private protected AudioStream(Stream stream)
        {
            Stream = stream;
            BaseLength = stream.Length; // TODO: If real stream, set Length to 0 (or -1)
        }

        public void PreloadAll()
        {
            if (Stream is MemoryStream)
                return;

            var oldPosition = Stream.Position;
            Stream.Position = 0;
            var stream = new MemoryStream(Stream.ReadToEnd());
            stream.Position = oldPosition;
            Stream.Dispose();
            Stream = stream;
            Reader = new BinaryReader(stream);
        }

        public static AudioStream Load(Stream stream)
        {
            var audio = new AudioInt16Stream(stream);
            audio.ReadHeader();
            return audio;
        }

        public static AudioStream Load(string filePath)
        {
            var stream = Load(File.OpenRead(filePath));
            stream.Name = Path.GetFileNameWithoutExtension(filePath);
            return stream;
        }

        public bool EndOfStream => DataPosition + (SampleSizeAllChannels - 1) >= DataLength;
        public long DataPosition => Stream.Position - DataStartPosition;

        private long BaseLength;
        public long DataLength;

        public void SetPosition(long position)
        {
            Stream.Position = DataStartPosition + position;
        }
    }
}
