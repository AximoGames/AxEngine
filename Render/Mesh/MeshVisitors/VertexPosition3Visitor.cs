// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Aximo.Render;
using OpenToolkit.Mathematics;

namespace Aximo
{
    internal class VertexPosition3Visitor : VertexVisitor<IVertexPosition3>, IVertexPosition3
    {
        private IDynamicArray<Vector3> PositionComponent;

        public VertexPosition3Visitor(Mesh mesh) : base(mesh)
        {
            PositionComponent = mesh.GetComponent<MeshPosition3Component>();
        }

        public override void SetLength(int length)
        {
            PositionComponent.SetLength(length);
        }

        private void EnsureSize()
        {
            SetLength(Index + 1);
        }

        public Vector3 Position
        {
            get => PositionComponent.GetValueWithExpand(Index);
            set => PositionComponent.SetValueWithExpand(Index, value);
        }
        protected override void Set(IVertex vertex)
        {
            if (vertex is IVertexPosition3 position)
                Position = position.Position;
        }

        public void SetPosition(IVertexPosition3 source)
        {
            Position = source.Position;
        }

        public void SetPosition(Vector3 source)
        {
            Position = source;
        }

        public override IVertex Clone() => new VertexDataPos(Position);

        IVertexPosition3 IVertexPosition3.Clone() => new VertexDataPos(Position);
        IVertexPosition<Vector3> IVertexPosition<Vector3>.Clone() => new VertexDataPos(Position);
    }

}
