// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using OpenToolkit.Mathematics;

namespace Aximo.Engine
{

    public abstract class TweenBuilder
    {
        public List<Tween> Tweens { get; private set; } = new List<Tween>();
        protected Tween CurrentTween = new Tween<int>(); // dummy assignment to receive values

        internal TweenBuilder()
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

        public TweenBuilder NextChain { get; internal set; }
        public TweenBuilder NextTarget { get; internal set; }
        public TweenBuilder Root { get; internal set; }

        protected int ChainLevel = 0;

        public bool IsRootChain => ChainLevel == 0;

        public IEnumerable<TweenBuilder> TweenTargets
        {
            get
            {
                TweenBuilder t = this;
                while (t != null)
                {
                    yield return t;
                    t = t.NextTarget;
                }
            }
        }

        public IEnumerable<TweenBuilder> TweenChains
        {
            get
            {
                TweenBuilder t = this;
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
}
