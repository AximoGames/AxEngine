// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Aximo.Engine.Components.Geometry;
using Aximo.Render;

namespace Aximo.Engine
{
    public class PrimitiveSceneProxy
    {
        public List<RendererMaterial> Materials = new List<RendererMaterial>();

        public PrimitiveSceneProxy(PrimitiveComponent component) { }
    }
}
