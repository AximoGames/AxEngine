// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using OpenToolkit;

namespace Aximo.Engine.Windows
{

    public class NativeBindingsContext : IBindingsContext
    {
        private static IBindingsContext _context;

        public NativeBindingsContext()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                _context = new WglBindingsContext();
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                _context = new GlxBindingsContext();
            }
            else
            {
                throw new PlatformNotSupportedException();
            }
        }

        public IntPtr GetProcAddress(string procName)
        {
            return _context.GetProcAddress(procName);
        }
    }
}
