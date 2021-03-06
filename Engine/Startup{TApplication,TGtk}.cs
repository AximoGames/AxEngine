﻿// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Aximo.Engine.Windows;

namespace Aximo.Engine
{
    /// <inheritdoc/>
    /// </summary>
    public class Startup<TApplication, TGtk> : Startup<TApplication>
        where TApplication : Application, new()
        where TGtk : GtkUI, new()
    {
        public Startup(ApplicationConfig config) : base(config)
        {
        }

        private protected override GtkUI CreateGtkUI() => (TGtk)Activator.CreateInstance(typeof(TGtk));
    }
}
