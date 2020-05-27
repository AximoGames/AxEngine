// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Gtk;
using OpenToolkit.Mathematics;

namespace Aximo.Engine
{

    public static class TweenBuilderExtensions
    {
        public static TweenBuilder<SceneComponent> Move(this TweenBuilder<SceneComponent> tweenTarget, float x, float y)
        {
            Move(tweenTarget, new Vector2(x, y));
            return tweenTarget;
        }

        public static TweenBuilder<SceneComponent> Move(this TweenBuilder<SceneComponent> tweenTarget, Vector2 value)
        {
            var tween = tweenTarget.NewTween<Tween2>();
            var targets = tweenTarget.Targets.ToArray();
            tween.Tick = () =>
            {
                foreach (var comp in targets)
                    comp.RelativeTranslation = new Vector3(value.X, value.Y, comp.RelativeTranslation.Z);
            };
            return tweenTarget;
        }

        public static TweenBuilder<SceneComponent> Scale(this TweenBuilder<SceneComponent> tweenTarget, Vector3 scale)
        {
            var tween = tweenTarget.NewTween<Tween3>();
            Tuple<SceneComponent, Vector3>[] targets = null;
            tween.OnStart = () =>
            {
                targets = tweenTarget.Targets.Select(t => new Tuple<SceneComponent, Vector3>(t, t.RelativeScale)).ToArray();
            };

            //var targets = tweenTarget.Targets.Select(t => new Tuple<SceneComponent, Vector3>(t, t.RelativeScale)).ToArray();
            //var targets = tweenTarget.Targets).ToArray();
            tween.Tick = () =>
            {
                foreach (var comp in targets)
                    comp.Item1.RelativeScale = tween.LerpFunc(comp.Item2, scale, tween.ScaledPosition);
            };
            return tweenTarget;
        }
    }
}
