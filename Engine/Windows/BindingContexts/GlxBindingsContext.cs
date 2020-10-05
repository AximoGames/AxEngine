// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using OpenToolkit;

namespace Aximo.Engine.Windows
{

    public class GlxBindingsContext : IBindingsContext
    {
        [DllImport("libGL", CharSet = CharSet.Ansi)]
        private static extern IntPtr glXGetProcAddress(string procName);
        public IntPtr GetProcAddress(string procName)
        {
            return glXGetProcAddress(procName);
        }
    }
}
