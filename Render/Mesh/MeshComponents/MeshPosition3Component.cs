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
            float minX = 0;
            float minY = 0;
            float minZ = 0;

            float maxX = 0;
            float maxY = 0;
            float maxZ = 0;

            for (var i = 0; i < Values.Count; i++)
            {
                var pos = Values[i];

                minX = MathF.Min(minX, pos.X);
                minY = MathF.Min(minY, pos.Y);
                minZ = MathF.Min(minZ, pos.Z);

                maxX = MathF.Min(maxX, pos.X);
                maxY = MathF.Min(maxY, pos.Y);
                maxZ = MathF.Min(maxZ, pos.Z);
            }

            Bounds = new Box3(minX, minY, minZ, maxX, maxY, maxZ);
        }
    }
}
