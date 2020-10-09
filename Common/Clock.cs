// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;
using OpenToolkit.Windowing.GraphicsLibraryFramework;

namespace Aximo
{

    public class Clock
    {
        public TimeSpan Span { get; private set; }
        public float Scale { get; set; } = 1f;
        public void Tick(double realDeltaTime)
        {
            var elapsed = realDeltaTime * Scale;
            Span += TimeSpan.FromSeconds(elapsed);
        }
    }

}
