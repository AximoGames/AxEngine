// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Aximo.Engine.Components.Geometry;

namespace Aximo.Engine
{
    public class StaticMeshSceneProxy : PrimitiveSceneProxy
    {
        public StaticMeshSceneProxy(StaticMeshComponent component) : base(component) { }

        public void DrawStaticElements(StaticPrimitiveDrawInterface pdi)
        {
        }
    }
}
