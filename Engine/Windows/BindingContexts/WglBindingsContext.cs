// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using Microsoft.Win32.SafeHandles;
using OpenToolkit;

namespace Aximo.Engine.Windows
{

    public class WglBindingsContext : IBindingsContext
    {
        [DllImport("opengl32.dll", CharSet = CharSet.Ansi)]
        private static extern IntPtr wglGetProcAddress(string procName);

        private readonly ModuleSafeHandle _openGlHandle;

        public WglBindingsContext()
        {
            _openGlHandle = Kernel32.LoadLibrary("opengl32.dll");
        }

        public IntPtr GetProcAddress(string procName)
        {
            IntPtr addr = wglGetProcAddress(procName);
            return addr != IntPtr.Zero ? addr : Kernel32.GetProcAddress(_openGlHandle, procName);
        }

        private static class Kernel32
        {
            [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Ansi)]
            public static extern ModuleSafeHandle LoadLibrary([MarshalAs(UnmanagedType.LPStr)] string lpFileName);

            [DllImport("kernel32", SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool FreeLibrary(IntPtr hModule);

            [DllImport("kernel32", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
            public static extern IntPtr GetProcAddress(ModuleSafeHandle hModule, string procName);
        }

        [SecurityPermission(SecurityAction.InheritanceDemand, UnmanagedCode = true)]
        [SecurityPermission(SecurityAction.Demand, UnmanagedCode = true)]
        // ReSharper disable once ClassNeverInstantiated.Local
        private class ModuleSafeHandle : SafeHandleZeroOrMinusOneIsInvalid
        {
            public ModuleSafeHandle() : base(true)
            {
            }

            [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
            protected override bool ReleaseHandle()
            {
                return Kernel32.FreeLibrary(handle);
            }
        }
    }
}
