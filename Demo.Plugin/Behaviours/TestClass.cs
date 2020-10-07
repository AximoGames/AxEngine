// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Reflection;
using System.Runtime.Loader;
using System.Threading;
using Aximo.Engine;
using McMaster.NETCore.Plugins;
using OpenToolkit.Mathematics;
using OpenToolkit.Windowing.Common;

namespace Aximo.AxDemo
{
    public class TestClass : ScriptBehaviour
    {

        public static int Counter = 0;

        public TestClass()
        {
        }

        private int TCounter;

        private Timer Timer;

        public override void Start()
        {
            Console.WriteLine($"P:{Counter++}");

            Timer = new Timer(c =>
             {
                 Console.WriteLine($"TK:{TCounter++}");
             }, null, TimeSpan.Zero, TimeSpan.FromSeconds(1));

        }

        public override void Destroy()
        {
            Timer?.Dispose();
            Timer = null;
        }

    }
}
