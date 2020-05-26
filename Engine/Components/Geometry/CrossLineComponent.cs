// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Aximo.Render.OpenGL;

namespace Aximo.Engine.Components.Geometry
{
    public class CrossLineComponent : StaticMeshComponent
    {
        public CrossLineComponent(int size, bool center) : base(MeshDataBuilder.CrossLine(), MaterialManager.DefaultLineMaterial)
        {
        }
    }
}
