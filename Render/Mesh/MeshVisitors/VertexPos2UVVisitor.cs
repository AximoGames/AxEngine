// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Aximo.Render;
using OpenToolkit.Mathematics;

namespace Aximo
{
    internal class VertexPos2UVVisitor : VertexVisitor<IVertexPos2UV>, IVertexPos2UV
    {
        private IDynamicArray<Vector2> PositionComponent;
        private IDynamicArray<Vector2> UVComponent;

        public VertexPos2UVVisitor(Mesh mesh) : base(mesh)
        {
            PositionComponent = mesh.GetComponent<MeshPosition2Component>();
            UVComponent = mesh.GetComponent<MeshUVComponent>();
        }

        public Vector2 Position
        {
            get
            {
                EnsureSize();
                return PositionComponent[Index];
            }

            set
            {
                EnsureSize();
                PositionComponent[Index] = value;
            }
        }

        public Vector2 UV
        {
            get
            {
                EnsureSize();
                return UVComponent[Index];
            }

            set
            {
                EnsureSize();
                UVComponent[Index] = value;
            }
        }

        public override void SetLength(int length)
        {
            PositionComponent.SetLength(length);
            UVComponent.SetLength(length);
        }

        private void EnsureSize()
        {
            SetLength(Index + 1);
        }

        protected override void Set(IVertex vertex)
        {
            EnsureSize();
            if (vertex is IVertexPosition2 position)
                Position = position.Position;
            if (vertex is IVertexUV color)
                UV = color.UV;
        }

        public void Set(IVertexPos2UV source)
        {
            Position = source.Position;
            UV = source.UV;
        }

        public void Set(VertexDataPos2UV source)
        {
            Position = source.Position;
            UV = source.UV;
        }

        public void SetPosition(IVertexPosition2 source)
        {
            Position = source.Position;
        }

        public void SetPosition(Vector2 source)
        {
            Position = source;
        }

        public override IVertex Clone()
        {
            return new VertexDataPos2UV(Position, UV);
        }

        IVertexPosition2 IVertexPosition2.Clone()
        {
            return new VertexDataPos2UV(Position, UV);
        }

        IVertexPosition<Vector2> IVertexPosition<Vector2>.Clone()
        {
            return new VertexDataPos2UV(Position, UV);
        }

        VertexDataPos2UV IVertexPos2UV.Clone()
        {
            return new VertexDataPos2UV(Position, UV);
        }
    }
}
