// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.IO;

namespace Aximo.Engine.Audio
{
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
            Length = stream.Length; // TODO: If real stream, set Length to 0 (or -1)
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

        public bool EndOfStream => Stream.Position >= Length;
        public long Position => Stream.Position - DataStartPosition;

        public long Length;

        public void SetPosition(long position)
        {
            Stream.Position = DataStartPosition + position;
        }
    }
}
