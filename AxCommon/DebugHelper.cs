﻿// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using OpenTK;
//using OpenTK.Graphics.OpenGL4;
using OpenTK.Input;

namespace Aximo
{

    public static class DebugHelper
    {
        public static void LogThreadInfo(string message)
        {
            LogThreadInfo(Thread.CurrentThread, message);
        }
        public static void LogThreadInfo(Thread th, string message)
        {
            Console.WriteLine($"#{th.ManagedThreadId} {th.Name} {message}");
        }
    }

}