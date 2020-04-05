using Serilog;

namespace Aximo
{
    public static class Log
    {
        public static ILogger ForContext<T>()
        {
            return Serilog.Log.ForContext("SourceContext", typeof(T).Name);
        }

        public static ILogger ForContext(string sourceContext)
        {
            return Serilog.Log.ForContext("SourceContext", sourceContext);
        }
    }
}
