// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
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

        private static CommandLineOptions? _Current;
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
