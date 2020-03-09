using System;
using System.Collections.Generic;
using System.Linq;

namespace Aximo.Engine
{
    public class CommandLineOptions
    {

        public bool Mute { get; set; }

        public CommandLineOptions(string[] commandLineArgs)
        {
            Mute = commandLineArgs.Contains("--mute");
        }

        private static CommandLineOptions _Current;
        public static CommandLineOptions Current
        {
            get
            {
                if (_Current == null)
                    _Current = new CommandLineOptions(Environment.GetCommandLineArgs());

                return _Current;
            }
        }

    }
}