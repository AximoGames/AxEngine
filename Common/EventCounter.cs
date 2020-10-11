// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Diagnostics;

namespace Aximo
{

    public class EventCounter
    {
        private Stopwatch Watch = new Stopwatch();
        private double FrameCount = 0;
        private double DeltaTime = 0.0;
        private double UpdateRate = 0.25;

        public void SetUpdateRate(TimeSpan ts)
        {
            UpdateRate = 1 / ts.TotalSeconds;
        }

        public double EventsPerSecond { get; private set; } = 0.0;

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
            DeltaTime += Watch.Elapsed.TotalSeconds;
            if (DeltaTime > 1.0 / UpdateRate)
            {
                EventsPerSecond = FrameCount / DeltaTime;
                FrameCount = 0;
                DeltaTime -= 1.0 / UpdateRate;
            }
            Watch.Restart();
        }
    }
}
