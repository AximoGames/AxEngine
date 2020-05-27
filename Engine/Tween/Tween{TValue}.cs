// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Aximo.Engine
{
    /// <summary>
    /// Takes in progress which is the percentage of the tween complete and returns
    /// the interpolation value that is fed into the lerp function for the tween.
    /// </summary>
    /// <remarks>
    /// Scale functions are used to define how the tween should occur. Examples would be linear,
    /// easing in quadratic, or easing out circular. You can implement your own scale function
    /// or use one of the many defined in the ScaleFuncs static class.
    /// </remarks>
    /// <param name="position">The percentage of the tween complete in the range [0, 1].</param>
    /// <returns>The scale value used to lerp between the tween's start and end values</returns>
    public delegate float ScaleFunc(float position);

    /// <summary>
    /// Standard linear interpolation function: "start + (end - start) * position"
    /// </summary>
    /// <returns>The interpolated value, generally using "start + (end - start) * position"</returns>
    public delegate TValue LerpFunc<TValue>(TValue start, TValue end, float position);

    /// <inheritdoc/>
    public class Tween<TValue> : Tween
    {
        public LerpFunc<TValue> LerpFunc;

        public TValue StartValue;
        public TValue EndValue;

        public float ScaledPosition
        {
            get
            {
                if (ScaleFunc == null)
                    return default;

                return ScaleFunc(Position);
            }
        }

        public TValue Value
        {
            get
            {
                return LerpFunc(StartValue, EndValue, ScaledPosition);
            }
        }
    }
}
