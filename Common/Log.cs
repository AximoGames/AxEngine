// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Serilog;

namespace Aximo
{
    public static class Log
    {
        private static bool Initialized;
        public static void Init()
        {
            if (Initialized)
                return;

            Serilog.Log.Logger = new LoggerConfiguration()
                .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] <{SourceContext}> {Message:lj}{NewLine}{Exception}")
                .MinimumLevel.Verbose()
                .CreateLogger();

            Initialized = true;
        }

        public static ILogger ForContext<T>()
        {
            Init();
            return Serilog.Log.ForContext("SourceContext", typeof(T).Name);
        }

        public static ILogger ForContext(string sourceContext)
        {
            Init();
            return Serilog.Log.ForContext("SourceContext", sourceContext);
        }
    }
}
