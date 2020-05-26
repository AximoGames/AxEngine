// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using OpenToolkit.Mathematics;

namespace Aximo.Engine
{
    public delegate TValue TweenFunc<TValue>(float position);
    public delegate void TweenFinishedDelegate();

    public static class TweenTargetExtensions
    {
        public static TweenTarget<SceneComponent> Move(this TweenTarget<SceneComponent> tweenTarget, float x, float y)
        {
            Move(tweenTarget, new Vector2(x, y));
            return tweenTarget;
        }

        public static TweenTarget<SceneComponent> Move(this TweenTarget<SceneComponent> tweenTarget, Vector2 value)
        {
            var tween = tweenTarget.NewTween<Tween2>();
            var targets = tweenTarget.Targets.ToArray();
            tween.Tick = (value) =>
            {
                foreach (var comp in targets)
                    comp.RelativeTranslation = new Vector3(value.X, value.Y, comp.RelativeTranslation.Z);
            };
            return tweenTarget;
        }

        public static TweenTarget<SceneComponent> Scale(this TweenTarget<SceneComponent> tweenTarget, Vector3 scale)
        {
            var tween = tweenTarget.NewTween<Tween2>();
            var targets = tweenTarget.Targets.ToArray();
            tween.Tick = (value) =>
            {
                foreach (var comp in targets)
                    comp.RelativeScale = scale;
            };
            return tweenTarget;
        }
    }

    public abstract class TweenTarget
    {
        public List<Tween> Tweens { get; private set; } = new List<Tween>();
        protected Tween CurrentTween = new Tween<int>(); // dummy assignment to receive values

        internal TweenTarget()
        {
            Root = this;
        }

        internal TTween NewTween<TTween>()
            where TTween : Tween, new()
        {
            var tween = new TTween();
            tween.Duration = CurrentTween.Duration;
            tween.Repeat = CurrentTween.Repeat;
            Tweens.Add(tween);
            return tween;
        }

        public TweenTarget NextChain { get; internal set; }
        public TweenTarget NextTarget { get; internal set; }
        public TweenTarget Root { get; internal set; }

        public IEnumerable<TweenTarget> TweenTargets
        {
            get
            {
                TweenTarget t = this;
                while (t != null)
                {
                    yield return t;
                    t = t.NextTarget;
                }
            }
        }

        public IEnumerable<TweenTarget> TweenChains
        {
            get
            {
                TweenTarget t = this;
                while (t != null)
                {
                    yield return t;
                    t = t.NextChain;
                }
            }
        }

        public void Stop()
        {
            foreach (var chain in Root.TweenChains)
                foreach (var tweenTarget in chain.TweenTargets)
                    foreach (var tween in tweenTarget.Tweens)
                        tween.Stop();
        }

        public void Start()
        {
            Stop();
            foreach (var targets in Root.TweenTargets)
                foreach (var tween in targets.Tweens)
                    tween.Start();
        }
    }

    public class TweenTarget<TTarget> : TweenTarget
    {
        public List<TTarget> Targets;

        internal TweenTarget(TTarget target)
        {
            Targets = new List<TTarget>();
            Targets.Add(target);
        }

        internal TweenTarget(IEnumerable<TTarget> targets)
        {
            Targets = new List<TTarget>(targets);
        }

        /// <summary>
        /// Repeat the Tween
        /// </summary>
        public TweenTarget<TTarget> Repeat()
        {
            CurrentTween.Repeat = true;
            return this;
        }

        public TweenTarget<TTarget> Duration(float seconds)
        {
            CurrentTween.Duration = TimeSpan.FromSeconds(seconds);
            return this;
        }

        public TweenTarget<TTarget> Duration(TimeSpan timeSpan)
        {
            CurrentTween.Duration = timeSpan;
            return this;
        }

        /// <summary>
        /// Append a new chain element
        /// </summary>
        public TweenTarget<TTarget> Then()
        {
            var newTarget = new TweenTarget<TTarget>(Targets);
            newTarget.Root = this;
            NextChain = newTarget;
            newTarget.CurrentTween.Duration = CurrentTween.Duration;
            newTarget.CurrentTween.Repeat = CurrentTween.Repeat;
            return newTarget;
        }

        public TweenTarget<TNewTarget> For<TNewTarget>(TNewTarget target)
        {
            var newTarget = new TweenTarget<TNewTarget>(target);
            newTarget.Root = this;
            NextTarget = newTarget;
            newTarget.CurrentTween.Duration = CurrentTween.Duration;
            newTarget.CurrentTween.Repeat = CurrentTween.Repeat;
            return newTarget;
        }

    }

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

        public TweenFinishedDelegate TweenFinished;

        public static TweenTarget<SceneComponent> For(SceneComponent component)
        {
            return new TweenTarget<SceneComponent>(component);
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

            Enabled = true;
            StartTime = DateTime.UtcNow;
        }

        public void Stop()
        {
            Enabled = false;
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
                if (Duration == TimeSpan.Zero)
                    return 0;
                var ts = DateTime.UtcNow - StartTime;
                return (float)(1.0 / Duration.TotalMilliseconds * ts.TotalMilliseconds);
            }
        }
    }
}
