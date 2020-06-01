// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
//using System.Media;
using NetCoreAudio;

namespace Aximo.Engine.Audio
{
    /// <summary>
    /// Transfers voltages from one Port to another Port.
    /// Flow direction is from <see cref="CableInput"/> to <see cref="CableOutput"/>
    /// </summary>
    public class AudioCable
    {
        public Port CableInput;
        public Port CableOutput;

        public AudioCable(Port cableInput, Port cableOutput)
        {
            CableInput = cableInput;
            CableOutput = cableOutput;
        }

        [MethodImpl(MethodImplOptions.AggressiveOptimization)]
        public void Process()
        {
            CableOutput.SetVoltage(CableInput.GetVoltage());
        }
    }
}
