// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using OpenToolkit.Mathematics;

namespace Aximo.Engine
{
    public delegate TValue TweenFunc<TValue>(float position);
    public delegate void TweenFinishedDelegate();

    /// <summary>
    /// Simple Tween caclulation over time.
    /// An Tween is defined by its duration and Tween function.
    /// </summary>
    public abstract class Tween
    {
        public bool Enabled;
        public TimeSpan Duration;
        protected DateTime StartTime;
        public bool Repeat;

        public event TweenFinishedDelegate TweenFinished;

        public void Start()
        {
            if (this.Duration == TimeSpan.Zero)
                return;

            Enabled = true;
            StartTime = DateTime.UtcNow;
        }

        public void ProcessTween()
        {
            if (!Enabled)
                return;

            var pos = Position;
            if (pos >= 1.0)
            {
                if (!Repeat)
                    Enabled = false;
                TweenFinished?.Invoke();
                if (Repeat)
                    StartTime = DateTime.UtcNow;
            }
        }

        public float Position
        {
            get
            {
                if (!Enabled)
                    return 0;
                if (this.Duration == TimeSpan.Zero)
                    return 0;
                var ts = DateTime.UtcNow - StartTime;
                return (float)(1.0 / Duration.TotalMilliseconds * ts.TotalMilliseconds);
            }
        }
    }
}
