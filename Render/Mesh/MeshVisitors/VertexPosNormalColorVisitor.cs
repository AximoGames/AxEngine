// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Aximo.Render;
using Aximo.Render.VertexData;
using OpenToolkit.Mathematics;

namespace Aximo
{
    internal class VertexPosNormalColorVisitor : VertexVisitor<IVertexPosNormalColor>, IVertexPosNormalColor
    {
        private IDynamicArray<Vector3> PositionComponent;
        private IDynamicArray<Vector3> NormalComponent;
        private IDynamicArray<Vector4> ColorComponent;

        public VertexPosNormalColorVisitor(Mesh mesh) : base(mesh)
        {
            PositionComponent = mesh.GetComponent<MeshPosition3Component>();
            NormalComponent = mesh.GetComponent<MeshNormalComponent>();
            ColorComponent = mesh.GetComponent<MeshColorComponent>();
        }

        public Vector3 Position
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

        public Vector3 Normal
        {
            get
            {
                EnsureSize();
                return NormalComponent[Index];
            }

            set
            {
                EnsureSize();
                NormalComponent[Index] = value;
            }
        }

        public Vector4 Color
        {
            get
            {
                EnsureSize();
                return ColorComponent[Index];
            }

            set
            {
                EnsureSize();
                ColorComponent[Index] = value;
            }
        }

        public override void SetLength(int length)
        {
            PositionComponent.SetLength(length);
            NormalComponent.SetLength(length);
            ColorComponent.SetLength(length);
        }

        private void EnsureSize()
        {
            SetLength(Index + 1);
        }

        protected override void Set(IVertex vertex)
        {
            EnsureSize();
            if (vertex is IVertexPosition3 position)
                Position = position.Position;
            if (vertex is IVertexNormal normal)
                Normal = normal.Normal;
            if (vertex is IVertexColor color)
                Color = color.Color;
        }

        public void Set(IVertexPosNormalColor source)
        {
            Position = source.Position;
            Normal = source.Normal;
            Color = source.Color;
        }

        public void Set(VertexDataPosNormalColor source)
        {
            Position = source.Position;
            Normal = source.Normal;
            Color = source.Color;
        }

        public void SetPosition(IVertexPosition3 source)
        {
            Position = source.Position;
        }

        public void SetPosition(Vector3 source)
        {
            Position = source;
        }

        public override IVertex Clone()
        {
            return new VertexDataPosNormalColor(Position, Normal, Color);
        }

        VertexDataPosNormalColor IVertexPosNormalColor.Clone()
        {
            return new VertexDataPosNormalColor(Position, Normal, Color);
        }

        IVertexPosition3 IVertexPosition3.Clone()
        {
            return new VertexDataPosNormalColor(Position, Normal, Color);
        }

        IVertexPosition<Vector3> IVertexPosition<Vector3>.Clone()
        {
            return new VertexDataPosNormalColor(Position, Normal, Color);
        }

        IVertexNormal IVertexNormal.Clone()
        {
            return new VertexDataPosNormalColor(Position, Normal, Color);
        }

        IVertexColor IVertexColor.Clone()
        {
            return new VertexDataPosNormalColor(Position, Normal, Color);
        }
    }
}
