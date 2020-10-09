// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using System.Security.Policy;
using System.Threading;
using Aximo.Engine.Components.Geometry;
using Aximo.Engine.Windows;
using Aximo.Render.OpenGL;

#nullable disable

namespace Aximo.Engine
{
    public delegate void ExecuteConsoleCommandLineDelegate(ExecuteConsoleCommandLineArgs e);

    public class CommandLineManager
    {
        public event ExecuteConsoleCommandLineDelegate OnExecuteConsoleCommandLine;

        public static CommandLineManager Current { get; private set; } = new CommandLineManager();

        internal void InvokeExecuteConsoleCommandLine(ExecuteConsoleCommandLineArgs e)
        {
            OnExecuteConsoleCommandLine?.Invoke(e);
        }
    }
}
