// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Aximo.Engine
{
    public delegate TValue TweenFunc<TValue>(float position);
    public delegate void TweenFinishedDelegate();

    //public class TweenTarget
    //{
    //    public List<SceneComponent> Components;
    //    private List<Action> Actions = new List<Action>();
    //    private Tween Tween;
    //    private Tween Tween;

    //    public TweenTarget(SceneComponent component)
    //    {
    //        Components = new List<SceneComponent>();
    //        Components.Add(component);
    //    }

    //    public TweenTarget Move(float x, float y)
    //    {
    //        Actions.Add(()=>Components.ForEach(c=>c.Transform));
    //    }

    //    public TweenTarget Move(Vector2 target)
    //    {
    //    }

    //    /// <summary>
    //    /// Repeat the Tween
    //    /// </summary>
    //    public TweenTarget Repeat()
    //    {
    //    }

    //    /// <summary>
    //    /// Append a new chain element
    //    /// </summary>
    //    public TweenTarget Then()
    //    {
    //    }

    //    public TweenTarget For(SceneObject obj)
    //    {
    //    }

    //    public void ProcessActions()
    //    {
    //        foreach (var action in Actions)
    //            action();
    //    }

    //}

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

        public event TweenFinishedDelegate TweenFinished;

        //public static TweenTarget For(SceneComponent component)
        //{
        //    return new TweenTarget(component);
        //}

        //private void Test()
        //{
        //    //Tween.For(null).Move(0, 3).Repeat();
        //    Tween.For(null).Move(0, 3).For(null).Move(0, -3).Repeat();
        //}

        public void Start()
        {
            if (this.Duration == TimeSpan.Zero)
                return;

            Enabled = true;
            StartTime = DateTime.UtcNow;
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
                if (this.Duration == TimeSpan.Zero)
                    return 0;
                var ts = DateTime.UtcNow - StartTime;
                return (float)(1.0 / Duration.TotalMilliseconds * ts.TotalMilliseconds);
            }
        }
    }
}
