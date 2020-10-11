// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Aximo.VertexData;
using OpenToolkit.Mathematics;

namespace Aximo
{
    public class MeshPosition3Component : MeshPositionComponent<Vector3>
    {
        public MeshPosition3Component()
        {
        }

        public MeshPosition3Component(ICollection<IVertexPosition3> values)
        {
            AddRange(values.Select(v => v.Position));
        }

        public override MeshComponent CloneEmpty() => new MeshPosition3Component();

        public void AddRange(IEnumerable<IVertexPosition3> values)
        {
            _Values.AddRange(values.Select(v => v.Position));
        }

        public override void AddRange(IEnumerable<IVertex> values)
        {
            foreach (var v in values)
                if (v is IVertexPosition3 p)
                    Add(p.Position);
        }

        public Box3 Bounds { get; private set; }

        public override void CalculateBounds()
        {
            Bounds = Values.GetBoundingBox();
        }
    }
}
