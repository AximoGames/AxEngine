using System;

namespace AxEngine
{
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
                return (float)((1.0 / Duration.TotalMilliseconds) * ts.TotalMilliseconds);
            }
        }

        public float Value
        {
            get
            {
                return 1 - Position;
            }
        }

    }

}