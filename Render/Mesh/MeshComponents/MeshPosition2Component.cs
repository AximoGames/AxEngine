// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using OpenToolkit.Mathematics;
using Aximo.Render;
using System.Linq;

namespace Aximo
{
    public class MeshPosition2Component : MeshComponent<Vector2>
    {
        public MeshPosition2Component()
            : base(MeshComponentType.Position)
        {
        }

        public override MeshComponent CloneEmpty() => new MeshPositionComponent();

        public MeshPosition2Component(ICollection<IVertexPosition2> values)
            : this()
        {
            AddRange(values.Select(v => v.Position));
        }

        public override void AddRange(IEnumerable<IVertex> values)
        {
            foreach (var v in values)
                if (v is IVertexPosition2 p)
                    Add(p.Position);
        }

    }

}
