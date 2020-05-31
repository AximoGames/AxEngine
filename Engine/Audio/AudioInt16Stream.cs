// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.IO;

namespace Aximo.Engine.Audio
{
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
}
