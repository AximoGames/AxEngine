// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using OpenToolkit.Windowing.GraphicsLibraryFramework;

#nullable enable

namespace Aximo
{

    public class LoopTimerController
    {
        public readonly Clock Clock;
        public LoopTimerController(Clock clock)
        {
            Clock = clock;
        }

        private Dictionary<int, WeakReference<LoopTimer>> Timers = new Dictionary<int, WeakReference<LoopTimer>>();
        private List<WeakReference<LoopTimer>> TimerList = new List<WeakReference<LoopTimer>>();

        public void Tick()
        {
            lock (Timers)
            {
                for (var i = TimerList.Count - 1; i >= 0; i--)
                {
                    var weak = TimerList.TryGet(i);
                    if (weak == null)
                        continue;

                    if (weak.TryGetTarget(out var t))
                        t.Tick();
                }
            }
        }

        internal void RegisterTimer(LoopTimer timer)
        {
            lock (Timers)
            {
                if (!Timers.ContainsKey(timer.GetHashCode()))
                {
                    if (!Timers.ContainsKey(timer.GetHashCode()))
                    {
                        var weak = new WeakReference<LoopTimer>(timer);
                        Timers.Add(timer.GetHashCode(), weak);
                        TimerList.Add(weak);
                    }
                }

                // remove old entries
                foreach (var entry in Timers.ToArray())
                {
                    if (!entry.Value.TryGetTarget(out var t))
                    {
                        Timers.Remove(entry.Key);
                        TimerList.Remove(entry.Value);
                    }
                }
            }
        }

        internal void UnregisterTimer(LoopTimer timer)
        {
            lock (Timers)
            {
                if (Timers.TryGetValue(timer.GetHashCode(), out var weak))
                {
                    Timers.Remove(timer.GetHashCode());
                    TimerList.Remove(weak);
                }
            }
        }

    }

}
