// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.IO;

namespace Aximo.Engine.Audio
{
    public class AudioSinkStream : IDataStream<short>
    {

        private List<short> Samples = new List<short>();

        public AudioSinkStream()
        {
        }

        public void Write(short sample)
        {
            Samples.Add(sample);
        }

        public short[] ToArray()
        {
            return Samples.ToArray();
        }
    }
}
