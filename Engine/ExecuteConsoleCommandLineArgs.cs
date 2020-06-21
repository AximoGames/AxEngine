// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using System.Security.Policy;
using System.Threading;
using Aximo.Engine.Components.Geometry;
using Aximo.Engine.Windows;
using Aximo.Render.OpenGL;

namespace Aximo.Engine
{
    public class ExecuteConsoleCommandLineArgs
    {
        public string CommandLine { get; private set; }
        public bool Handled { get; set; }

        public ExecuteConsoleCommandLineArgs(string commandLine)
        {
            CommandLine = commandLine;
        }
    }
}
