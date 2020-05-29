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
        public static TweenBuilder<TTarget> Value<TTarget, TValue>(TweenBuilder<TTarget> tweenTarget, TValue value, Func<TTarget, TValue> getValue, Action<TTarget, TValue> setValue)
        {
            var tween = tweenTarget.NewTween<Tween<TValue>>();
            Tuple<TTarget, TValue>[] targets = null;
            tween.OnStart = () =>
            {
                targets = tweenTarget.Targets.Select(t => new Tuple<TTarget, TValue>(t, getValue(t))).ToArray();
            };
            tween.Tick = () =>
            {
                foreach (var comp in targets)
                {
                    var v = tween.LerpFunc(comp.Item2, value, tween.ScaledPosition);
                    setValue(comp.Item1, v);
                }
            };
            return tweenTarget;
        }

        public static TweenBuilder<SceneComponent> Translate(this TweenBuilder<SceneComponent> tweenTarget, float x, float y)
        {
            return Translate(tweenTarget, new Vector2(x, y));
        }

        public static TweenBuilder<SceneComponent> Translate(this TweenBuilder<SceneComponent> tweenTarget, Vector2 translation)
        {
            return Value(
                tweenTarget,
                translation,
                t => t.RelativeTranslation.Xy,
                (t, v) => t.RelativeTranslation = new Vector3(v.X, v.Y, t.RelativeTranslation.Z));
        }

        public static TweenBuilder<SceneComponent> Translate(this TweenBuilder<SceneComponent> tweenTarget, float x, float y, float z)
        {
            return Translate(tweenTarget, new Vector3(x, y, z));
        }

        public static TweenBuilder<SceneComponent> Translate(this TweenBuilder<SceneComponent> tweenTarget, Vector3 translation)
        {
            return Value(
                tweenTarget,
                translation,
                t => t.RelativeTranslation,
                (t, v) => t.RelativeTranslation = new Vector3(v.X, v.Y, v.Z));
        }

        public static TweenBuilder<SceneComponent> Scale(this TweenBuilder<SceneComponent> tweenTarget, float scale)
        {
            return Scale(tweenTarget, new Vector3(scale));
        }

        public static TweenBuilder<SceneComponent> Scale(this TweenBuilder<SceneComponent> tweenTarget, float x, float y, float z)
        {
            return Scale(tweenTarget, new Vector3(x, y, z));
        }

        public static TweenBuilder<SceneComponent> Scale(this TweenBuilder<SceneComponent> tweenTarget, Vector3 scale)
        {
            return Value(
                tweenTarget,
                scale,
                t => t.RelativeScale,
                (t, v) => t.RelativeScale = v);
        }

        public static TweenBuilder<SceneComponent> Rotate(this TweenBuilder<SceneComponent> tweenTarget, Quaternion quaternion)
        {
            return Value(
                tweenTarget,
                quaternion,
                t => t.RelativeRotation,
                (t, v) => t.RelativeRotation = v);
        }
    }
}
