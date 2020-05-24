// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Aximo.Render;
using Aximo.VertexData;
using OpenToolkit.Mathematics;

namespace Aximo
{
    public class MeshNormalComponent : MeshComponent<Vector3>
    {
        public MeshNormalComponent()
            : base(MeshComponentType.Normal)
        {
        }

        public override MeshComponent CloneEmpty() => new MeshNormalComponent();

        public override void AddRange(IEnumerable<IVertex> values)
        {
            foreach (var v in values)
                if (v is IVertexNormal p)
                    Add(p.Normal);
        }
    }
}
