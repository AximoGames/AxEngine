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
using OpenToolkit.Audio;
using OpenToolkit.Audio.OpenAL;

namespace Aximo.Engine.Audio
{

    public static class PCMConversion
    {
        // http://blog.bjornroche.com/2009/12/int-float-int-its-jungle-out-there.html
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static float ShortToFloat(short data)
        {
            return (data + 0.5f) / (0x7FFF + 0.5f);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static short FloatToShort(float data)
        {
            if (data > 1)
                data = 1;
            else if (data < -1)
                data = -1;

            return (short)((data * (0x7FFF + 0.5f)) - 0.5f);
        }
    }

}
