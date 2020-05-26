// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;

namespace Aximo
{
    public static class DebugHelper
    {
        private static Serilog.ILogger Log = Aximo.Log.ForContext(nameof(DebugHelper));

        public static void LogThreadInfo(string message)
        {
            LogThreadInfo(Thread.CurrentThread, message);
        }
        public static void LogThreadInfo(Thread th, string message)
        {
            if (message == th.Name)
                message = "";
            Log.Info("Thread #{ThreadId} {ThreadName} {Message}", th.ManagedThreadId, th.Name, message);
        }
    }
}
