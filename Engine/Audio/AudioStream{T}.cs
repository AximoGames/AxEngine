// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.IO;

namespace Aximo.Engine.Audio
{
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
}
