// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using OpenToolkit.Audio;
using OpenToolkit.Audio.OpenAL;

namespace Aximo.Engine.Audio
{

    public class Port
    {
        public Channel[] Channels;
        public const int MaxChannels = 16;
        public string Name;

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
}
