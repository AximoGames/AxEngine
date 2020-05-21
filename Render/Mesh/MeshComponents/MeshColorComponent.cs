// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Aximo.Render;
using Aximo.Render.VertexData;
using OpenToolkit.Mathematics;

namespace Aximo
{
    public class MeshColorComponent : MeshComponent<Vector4>
    {
        public MeshColorComponent()
            : base(MeshComponentType.Color)
        {
        }

        public override MeshComponent CloneEmpty() => new MeshColorComponent();
        public override void AddRange(IEnumerable<IVertex> values)
        {
            foreach (var v in values)
                if (v is IVertexColor p)
                    Add(p.Color);
        }
    }
}
