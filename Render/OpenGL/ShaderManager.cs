// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using OpenToolkit.Graphics.OpenGL4;
using OpenToolkit.Mathematics;

namespace Aximo.Render.OpenGL
{

    internal static class ShaderManager
    {

        private static Dictionary<int, WeakReference<RendererShader>> References = new Dictionary<int, WeakReference<RendererShader>>();

        public static void Register(RendererShader shader)
        {
            var code = shader.GetHashCode();
            References.Remove(code);
            References.Add(code, new WeakReference<RendererShader>(shader));
        }

        public static void Unregister(RendererShader shader)
        {
            var code = shader.GetHashCode();
            References.Remove(code);
        }

        public static void ReloadAll()
        {
            foreach (var weak in References.ToArray())
                if (weak.Value.TryGetTarget(out var shader))
                    shader.Reload();
        }

    }
}
