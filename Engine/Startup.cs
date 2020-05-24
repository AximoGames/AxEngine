// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using System.Threading;
using Aximo.Engine.Components.Geometry;
using Aximo.Engine.Windows;
using Aximo.Render.OpenGL;

namespace Aximo.Engine
{
    /// <summary>
    /// Wrapper to start the <see cref="Application"/>. Takes care about thread ownership.
    /// </summary>
    public abstract class Startup : IDisposable
    {
        protected abstract void Dispose(bool disposing);

        public void Dispose()
        {
            Dispose(true);
        }

        public static void Start<TApplication>()
            where TApplication : Application, new()
        {
            new Startup<TApplication>().Start();
        }

        public static void Start<TApplication>(ApplicationConfig config)
            where TApplication : Application, new()
        {
            new Startup<TApplication>(config).Start();
        }

        internal static void Start<TApplication>(ApplicationConfig config, TApplication instance)
            where TApplication : Application, new()
        {
            new Startup<TApplication>(config).Start(instance);
        }
    }
}
