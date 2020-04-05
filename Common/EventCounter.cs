// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using OpenToolkit;
using OpenToolkit.GraphicsLibraryFramework;
using OpenToolkit.Input;
using OpenToolkit.Mathematics;
using OpenToolkit.Windowing.Common;
using OpenToolkit.Windowing.Common.Input;
using OpenToolkit.Windowing.Desktop;

namespace Aximo
{

    public class EventCounter
    {
        private Stopwatch Watch = new Stopwatch();
        private double FrameCount = 0;
        private double DeltaTime = 0.0;
        private double UpdateRate = 4.0;  // 4 updates per sec.        

        private double _Fps = 0.0;
        public double EventsPerSecond => _Fps;

        public TimeSpan Elapsed { get; private set; }

        public void Reset()
        {
            Watch.Restart();
        }

        public void Tick()
        {
            Watch.Stop();
            Elapsed = Watch.Elapsed;
            FrameCount++;
            DeltaTime += Watch.ElapsedMilliseconds / 1000.0;
            if (DeltaTime > 1.0 / UpdateRate)
            {
                _Fps = FrameCount / DeltaTime;
                FrameCount = 0;
                DeltaTime -= 1.0 / UpdateRate;
            }
            Watch.Restart();
        }
    }

}
