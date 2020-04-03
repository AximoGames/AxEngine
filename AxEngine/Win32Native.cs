// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Runtime.InteropServices;
using OpenToolkit;
using OpenToolkit.Graphics;
using OpenToolkit.Input;
using OpenToolkit.Mathematics;
using OpenToolkit.Windowing.Common;
using OpenToolkit.Windowing.Desktop;

namespace Aximo.Engine
{

    public static class Win32Native
    {

        [DllImport("user32.dll")]
        private static extern IntPtr GetActiveWindow();

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
        private const int GWL_STYLE = -16;

        public static void HideTitleBar()
        {
            IntPtr ptr = GetActiveWindow();
            SetWindowLong(ptr, GWL_STYLE, 0);
        }
    }
}
