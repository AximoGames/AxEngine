// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using Aximo.Render;
using OpenToolkit.Mathematics;

namespace Aximo.Engine
{
    public delegate void TweenFinishedDelegate();

    /// <summary>
    /// State of an Tween object
    /// </summary>
    public enum TweenState
    {
        /// <summary>
        /// The tween is running.
        /// </summary>
        Running,

        /// <summary>
        /// The tween is paused.
        /// </summary>
        Paused,

        /// <summary>
        /// The tween is stopped.
        /// </summary>
        Stopped,
    }

    /// <summary>
    /// The behavior to use when manually stopping a tween.
    /// </summary>
    public enum TweenStopBehavior
    {
        /// <summary>
        /// Does not change the current value.
        /// </summary>
        AsIs,

        /// <summary>
        /// Forces the tween progress to the end value.
        /// </summary>
        ForceComplete,
    }

    /// <summary>
    /// Simple Tween caclulation over time.
    /// An Tween is defined by its duration and Tween function.
    /// </summary>
    public abstract class Tween : IUpdateFrame
    {
        public Action Tick;
        public Action OnStart;
        public ScaleFunc ScaleFunc;
        public TweenState State = TweenState.Stopped;
        public TweenStopBehavior StopBehavior = TweenStopBehavior.AsIs;

        public float DefaultScaleFunc(float position) => position;

        public Tween()
        {
            ScaleFunc = DefaultScaleFunc;
        }

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

        public TimeSpan Duration = TimeSpan.FromSeconds(15);
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
            //if (Duration == TimeSpan.Zero)
            //    return;

            _Enabled = true;

            if (OnStart != null)
                OnStart();

            StartTime = DateTime.UtcNow;
            SceneContext.Current.AddUpdateFrameObject(this);
        }

        public void Stop()
        {
            Stop(StopBehavior);
        }

        public void Stop(TweenStopBehavior stopBehavior)
        {
            _Enabled = false;
            SceneContext.Current.RemoveUpdateFrameObject(this);
            if (stopBehavior == TweenStopBehavior.ForceComplete)
            {
                Position = 1;
                if (Tick != null)
                    Tick();
            }
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

        public virtual void OnUpdateFrame()
        {
            if (!_Enabled)
                return;

            if (Duration == TimeSpan.Zero)
            {
                Stop();
            }
            else
            {
                var ts = DateTime.UtcNow - StartTime;
                Position = (float)(1.0 / Duration.TotalMilliseconds * ts.TotalMilliseconds);

                if (Position >= 1.0)
                {
                    if (!Repeat)
                        Stop();
                    else
                        Position = 0;

                    TweenFinished?.Invoke();
                    if (Repeat)
                        StartTime = DateTime.UtcNow;
                }
            }

            if (Tick != null)
                Tick();
        }

        public float Position { get; private set; }
    }
}
