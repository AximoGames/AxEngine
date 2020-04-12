// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Aximo.Engine
{
    public delegate float AnimationFunc(float position);
    public delegate void AnimationFinishedDelegate();

    public class Animation
    {
        public bool Enabled;
        public TimeSpan Duration;
        protected DateTime StartTime;

        public event AnimationFinishedDelegate AnimationFinished;

        public void Start()
        {
            if (this.Duration == TimeSpan.Zero)
                return;

            Enabled = true;
            StartTime = DateTime.UtcNow;
        }

        public void ProcessAnimation()
        {
            if (!Enabled)
                return;

            var pos = Position;
            if (pos >= 1.0)
            {
                Enabled = false;
                AnimationFinished?.Invoke();
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

        public AnimationFunc AnimationFunc;

        public float Value
        {
            get
            {
                if (AnimationFunc == null)
                    return Position;

                return AnimationFunc(Position);
            }
        }
    }

    public static class AnimationFuncs
    {
        public static AnimationFunc Linear()
        {
            return (p) => { return p; };
        }

        public static AnimationFunc LinearReverse()
        {
            return (p) => { return 1 - p; };
        }

        public static AnimationFunc Linear(float scale)
        {
            return (p) => { return p * scale; };
        }

        public static AnimationFunc LinearReverse(float scale)
        {
            return (p) => { return scale - (p * scale); };
        }
    }
}