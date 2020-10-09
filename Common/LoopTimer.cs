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

    public class LoopTimer : IDisposable
    {
        private LoopTimerController Controller;
        public LoopTimer(LoopTimerController controller, TimeSpan interval)
        {
            Controller = controller;
            Interval = interval;
            ScheduleNext();
            Controller.RegisterTimer(this);
        }

        public TimeSpan _Interval;
        public TimeSpan Interval
        {
            get => _Interval;
            set
            {
                _Interval = value;
                ScheduleNext();
            }
        }

        private TimeSpan NextInvoke;

        public event Action? OnTimer;

        private void ScheduleNext()
        {
            NextInvoke = Controller.Clock.Span + Interval;
        }

        public void Tick()
        {
            if (Controller.Clock.Span >= NextInvoke)
            {
                ScheduleNext();
                OnTimer?.Invoke();
            }
        }

        public void Dispose()
        {
            Controller.UnregisterTimer(this);
        }
    }

}
