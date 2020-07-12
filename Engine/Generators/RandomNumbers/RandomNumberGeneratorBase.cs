#region usings

using System;

#endregion

namespace Aximo.Generators.RandomNumbers
{
    public abstract class RandomNumberGeneratorBase
    {
        public abstract int Next(int maxValue);
        public abstract int Next(int minValue, int maxValue);
        public abstract bool NextBool();
        public abstract void NextBytes(byte[] buffer);
        public abstract void NextBytes(byte[] buffer, int start, int count);
        public abstract uint NextUInt();
        public abstract int Next();
        public abstract double NextDouble();

        public virtual double NextDouble(double minValue, double maxValue)
        {
            //maxValue += 1;
            if (minValue > maxValue)
                throw new ArgumentOutOfRangeException(nameof(maxValue), maxValue, "upperBound must be >=lowerBound");

            var num = maxValue - minValue;
            if (num < 0)
            {
                var num2 = maxValue - minValue;
                return (NextDouble() * num2) + minValue;
            }
            return (NextDouble() * num) + minValue;
        }

        public virtual double NextDouble(double maxValue)
        {
            //maxValue += 1;
            return NextDouble() * maxValue;
        }

        private bool use_last_result = false; // flag for NextGaussian3()
        private double y2 = 0.0;  // secondary result for NextGaussian3()

        public double NextGaussian(double mean, double sd)
        {
            double x1, x2, w, y1 = 0.0;

            if (use_last_result) // use answer from previous call?
            {
                y1 = y2;
                use_last_result = false;
            }
            else
            {
                do
                {
                    x1 = (2.0 * NextDouble()) - 1.0;
                    x2 = (2.0 * NextDouble()) - 1.0;
                    w = (x1 * x1) + (x2 * x2);
                }
                while (w >= 1.0); // are x1 and x2 inside unit circle?

                w = Math.Sqrt(-2.0 * Math.Log(w) / w);
                y1 = x1 * w;
                y2 = x2 * w;
                use_last_result = true;
            }

            return mean + (y1 * sd);
        }

        public double NextGaussian()
        {
            double x1, x2, w, y1 = 0.0;

            if (use_last_result) // use answer from previous call?
            {
                y1 = y2;
                use_last_result = false;
            }
            else
            {
                do
                {
                    x1 = (2.0 * NextDouble()) - 1.0;
                    x2 = (2.0 * NextDouble()) - 1.0;
                    w = (x1 * x1) + (x2 * x2);
                }
                while (w >= 1.0); // are x1 and x2 inside unit circle?

                w = Math.Sqrt(-2.0 * Math.Log(w) / w);
                y1 = x1 * w;
                y2 = x2 * w;
                use_last_result = true;
            }

            return y1;
        }

        //(5, 7, 15, 20) --> zufallszahl zwischen 5 und 20, zwischen 5 und 7 jedoch seltener, als auch zwischen 15 und 20. Die Kurve ist per Kreisfunktion abgerundet.
        public double NextCircleRange(double min, double middleMin, double middleMax, double max)
        {
            while (true)
            {
                var value = NextDouble(min, max);
                if (middleMin <= value && value <= middleMax) return value;
                if (value < middleMin)
                {
                    var range = middleMin - min;
                    var rangePos = middleMin - value;
                    if (RandomInCircleBow(rangePos / range)) return value;
                }
                if (value > middleMax)
                {
                    var range = max - middleMax;
                    var rangePos = value - middleMax;
                    if (RandomInCircleBow(rangePos / range)) return value;
                }
            }
        }

        public bool RandomInCircleBow(double v)
        { //0..1
            if (v < 0 || v > 1) throw new Exception("0..1");
            return Math.Sin(v * (Math.PI / 2)) < NextDouble();
        }

    }

}
