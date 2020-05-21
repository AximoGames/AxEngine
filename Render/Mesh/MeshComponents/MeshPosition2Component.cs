// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Aximo.Render;
using Aximo.Render.VertexData;
using OpenToolkit.Mathematics;

namespace Aximo
{
    public class MeshPosition2Component : MeshPositionComponent<Vector2>
    {
        public MeshPosition2Component()
        {
        }

        public override MeshComponent CloneEmpty() => new MeshPosition2Component();

        public MeshPosition2Component(ICollection<IVertexPosition2> values)
        {
            AddRange(values.Select(v => v.Position));
        }

        public override void AddRange(IEnumerable<IVertex> values)
        {
            foreach (var v in values)
                if (v is IVertexPosition2 p)
                    Add(p.Position);
        }

        public Box2 Bounds { get; private set; }

        public override void CalculateBounds()
        {
            float minX = 0;
            float minY = 0;

            float maxX = 0;
            float maxY = 0;

            for (var i = 0; i < Values.Count; i++)
            {
                var pos = Values[i];

                minX = MathF.Min(minX, pos.X);
                minY = MathF.Min(minY, pos.Y);

                maxX = MathF.Min(maxX, pos.X);
                maxY = MathF.Min(maxY, pos.Y);
            }

            Bounds = new Box2(minX, minY, maxX, maxY);
        }
    }
}
