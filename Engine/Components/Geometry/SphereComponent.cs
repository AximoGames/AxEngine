// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Aximo.Engine.Components.Geometry
{
    public class SphereComponent : StaticMeshComponent
    {
        public SphereComponent(int divisions = 2) : base(Mesh.CreateSphere(divisions), Material.Default)
        {
        }
    }
}
