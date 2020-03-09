namespace System
{
    public static class TimeSpanExtensions
    {

        public static TimeSpan Multiply(this TimeSpan ts, double value)
        {
            // TODO: dotnet core has a build in Operator.
            return new TimeSpan((long)(ts.Ticks * value));
        }

    }


}
