// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using Aximo.Render;
using OpenToolkit.Mathematics;

namespace Aximo.Engine
{
    public delegate TValue TweenFunc<TValue>(float position);
    public delegate void TweenFinishedDelegate();

    /// <summary>
    /// Simple Tween caclulation over time.
    /// An Tween is defined by its duration and Tween function.
    /// </summary>
    public abstract class Tween : IUpdateFrame
    {
        private bool _Enabled;
        public bool Enabled
        {
            get => _Enabled;
            set
            {
                if (_Enabled == value)
                    return;

                if (value)
                    Start();
                else
                    Stop();
            }
        }
        public TimeSpan Duration;
        protected DateTime StartTime;
        public bool Repeat;

        public TweenFinishedDelegate TweenFinished;

        public static TweenBuilder<SceneComponent> For(SceneComponent component)
        {
            return new TweenBuilder<SceneComponent>(component);
        }

        private void Test()
        {
            //Tween.For(null).Move(0, 3).Repeat();
            //Tween.For(null).Move(0, 3).For(null).Move(0, -3).Duration(2).Repeat();
            Tween.For(null).Move(0, 3).Then().Move(0, -3).Duration(2).Duration(1).Repeat();
            //Tween.For(null).Move(0, 3).Duration(1);
        }

        public void Start()
        {
            if (Duration == TimeSpan.Zero)
                return;

            _Enabled = true;
            StartTime = DateTime.UtcNow;
            SceneContext.Current.AddUpdateFrameObject(this);
        }

        public void Stop()
        {
            _Enabled = false;
            SceneContext.Current.RemoveUpdateFrameObject(this);
        }

        public void End()
        {
            Stop();
            // TODO: End-Position
        }

        public void Pause()
        {
            throw new NotImplementedException();
        }

        public void Resume()
        {
            throw new NotImplementedException();
        }

        public void OnUpdateFrame()
        {
            if (!_Enabled)
                return;

            var pos = Position;
            if (pos >= 1.0)
            {
                if (!Repeat)
                    Stop();
                TweenFinished?.Invoke();
                if (Repeat)
                    StartTime = DateTime.UtcNow;
            }
        }

        public float Position
        {
            get
            {
                if (!_Enabled)
                    return 0;
                if (Duration == TimeSpan.Zero)
                    return 0;
                var ts = DateTime.UtcNow - StartTime;
                return (float)(1.0 / Duration.TotalMilliseconds * ts.TotalMilliseconds);
            }
        }
    }
}
