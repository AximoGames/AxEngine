// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Aximo.Render;
using OpenToolkit.Mathematics;

namespace Aximo
{
    public class MeshUVComponent : MeshComponent<Vector2>
    {
        public MeshUVComponent()
            : base(MeshComponentType.UV)
        {
        }

        public override MeshComponent CloneEmpty() => new MeshUVComponent();

        public override void AddRange(IEnumerable<IVertex> values)
        {
            foreach (var v in values)
                if (v is IVertexUV p)
                    Add(p.UV);
        }
    }
}
