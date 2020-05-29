// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using OpenToolkit.Mathematics;

namespace Aximo.Engine
{

    public abstract class TweenBuilder
    {
        public List<Tween> Tweens { get; private set; } = new List<Tween>();
        protected Tween CurrentTween = new Tween<int>(); // dummy assignment to receive values
        private protected bool Repat;

        internal TweenBuilder()
        {
            Root = this;
        }

        internal TTween NewTween<TTween>()
            where TTween : Tween, new()
        {
            var tween = new TTween();
            tween.Duration = CurrentTween.Duration;
            tween.ScaleFunc = CurrentTween.ScaleFunc;
            tween.Order = Root.AllTweens.Count() + 1;
            //tween.Repeat = CurrentTween.Repeat;
            CurrentTween = tween;
            Tweens.Add(tween);
            return tween;
        }

        public TweenBuilder NextChain { get; internal set; }
        public TweenBuilder NextTarget { get; internal set; }
        public TweenBuilder Root { get; internal set; }

        public TweenBuilder NextNonEmptyChain => TweenChains.Skip(1).FirstOrDefault();

        protected int ChainLevel = 0;

        public bool IsRootChain => ChainLevel == 0;

        public IEnumerable<TweenBuilder> TweenTargets
        {
            get
            {
                TweenBuilder t = this;
                while (t != null)
                {
                    if (t.Tweens.Count > 0)
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
                    if (t.Tweens.Count > 0)
                        yield return t;
                    t = t.NextChain;
                }
            }
        }

        public IEnumerable<Tween> AllTweens
        {
            get
            {
                foreach (var chain in Root.TweenChains)
                    foreach (var tweenTarget in chain.TweenTargets)
                        foreach (var tween in tweenTarget.Tweens)
                            yield return tween;
            }
        }

        public void Stop()
        {
            foreach (var tween in AllTweens)
                tween.Stop();
        }

        private void InitEvents()
        {
            foreach (var chain in Root.TweenChains)
            {
                var nextChain = chain.NextNonEmptyChain;
                if (nextChain == null)
                {
                    if (!Repat)
                        break;

                    nextChain = Root;
                }

                if (nextChain.Tweens.Count == 0)
                    continue;

                var tweens = new List<Tween>();
                foreach (var tweenTarget in chain.TweenTargets)
                    foreach (var tween in tweenTarget.Tweens)
                        tweens.Add(tween);

                var longestTween = tweens.OrderByDescending(t => t.Duration).ThenByDescending(t => t.Order).FirstOrDefault();
                if (longestTween != null)
                    longestTween.TweenComplete += StartNextChain(nextChain);
            }
        }

        private TweenFinishedDelegate StartNextChain(TweenBuilder nextChain)
        {
            return () =>
            {
                foreach (var tweenTarget in nextChain.TweenTargets)
                    foreach (var tween in tweenTarget.Tweens)
                        tween.Start();
            };
        }

        public TweenBuilder Start()
        {
            Stop();
            InitEvents();
            foreach (var targets in Root.TweenTargets)
                foreach (var tween in targets.Tweens)
                    tween.Start();

            return this;
        }
    }
}
