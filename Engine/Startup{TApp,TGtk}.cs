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
    /// <inheritdoc/>
    /// </summary>
    public class Startup<TApp, TGtk> : Startup<TApp>
        where TApp : Application
        where TGtk : GtkUI
    {
        public Startup(ApplicationConfig config) : base(config)
        {
        }

        private protected override GtkUI CreateGtkUI() => (TGtk)Activator.CreateInstance(typeof(TGtk));
    }
}
