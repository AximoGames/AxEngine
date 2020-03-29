// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using Aximo.Engine;
using OpenTK;

using Xunit;

namespace Aximo.AxTests
{
    internal class Program
    {

        public static void Main(string[] args)
        {
            var tester = new RenderTests();
            tester.ForwardBox1();
            tester.Dispose();
            //Console.ReadLine();
            Environment.Exit(0);
        }

    }
}
