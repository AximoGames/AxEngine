﻿// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using OpenToolkit.Mathematics;

namespace Aximo.Engine
{

    public class TweenBuilder<TTarget> : TweenBuilder
    {
        public List<TTarget> Targets;

        internal TweenBuilder(TTarget target)
        {
            Targets = new List<TTarget>();
            Targets.Add(target);
        }

        internal TweenBuilder(IEnumerable<TTarget> targets)
        {
            Targets = new List<TTarget>(targets);
        }

        /// <summary>
        /// Repeat the Tween
        /// </summary>
        public TweenBuilder<TTarget> Repeat()
        {
            CurrentTween.Repeat = true;
            return this;
        }

        public TweenBuilder<TTarget> Duration(float seconds)
        {
            CurrentTween.Duration = TimeSpan.FromSeconds(seconds);
            return this;
        }

        public TweenBuilder<TTarget> Duration(TimeSpan timeSpan)
        {
            CurrentTween.Duration = timeSpan;
            return this;
        }

        /// <summary>
        /// Append a new chain element
        /// </summary>
        public TweenBuilder<TTarget> Then()
        {
            var newTarget = new TweenBuilder<TTarget>(Targets);
            newTarget.Root = this;
            NextChain = newTarget;
            newTarget.CurrentTween.Duration = CurrentTween.Duration;
            newTarget.CurrentTween.Repeat = CurrentTween.Repeat;
            return newTarget;
        }

        public TweenBuilder<TNewTarget> For<TNewTarget>(TNewTarget target)
        {
            var newTarget = new TweenBuilder<TNewTarget>(target);
            newTarget.Root = this;
            NextTarget = newTarget;
            newTarget.CurrentTween.Duration = CurrentTween.Duration;
            newTarget.CurrentTween.Repeat = CurrentTween.Repeat;
            newTarget.ChainLevel = ChainLevel + 1;
            return newTarget;
        }

    }
}
