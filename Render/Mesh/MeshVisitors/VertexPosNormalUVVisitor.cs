// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Aximo.Render;
using Aximo.Render.VertexData;
using OpenToolkit.Mathematics;

namespace Aximo
{
    internal class VertexPosNormalUVVisitor : VertexVisitor<IVertexPosNormalUV>, IVertexPosNormalUV
    {
        private IDynamicArray<Vector3> PositionComponent;
        private IDynamicArray<Vector3> NormalComponent;
        private IDynamicArray<Vector2> UVComponent;

        public VertexPosNormalUVVisitor(Mesh mesh) : base(mesh)
        {
            PositionComponent = mesh.GetComponent<MeshPosition3Component>();
            NormalComponent = mesh.GetComponent<MeshNormalComponent>();
            UVComponent = mesh.GetComponent<MeshUVComponent>();
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
            NormalComponent.SetLength(length);
            UVComponent.SetLength(length);
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
            if (vertex is IVertexUV uv)
                UV = uv.UV;
        }

        public void Set(IVertexPosNormalUV source)
        {
            Position = source.Position;
            Normal = source.Normal;
            UV = source.UV;
        }

        public void Set(VertexDataPosNormalUV source)
        {
            Position = source.Position;
            Normal = source.Normal;
            UV = source.UV;
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
            return new VertexDataPosNormalUV(Position, Normal, UV);
        }

        VertexDataPosNormalUV IVertexPosNormalUV.Clone()
        {
            return new VertexDataPosNormalUV(Position, Normal, UV);
        }

        IVertexPosition3 IVertexPosition3.Clone()
        {
            return new VertexDataPosNormalUV(Position, Normal, UV);
        }

        IVertexPosition<Vector3> IVertexPosition<Vector3>.Clone()
        {
            return new VertexDataPosNormalUV(Position, Normal, UV);
        }

        IVertexNormal IVertexNormal.Clone()
        {
            return new VertexDataPosNormalUV(Position, Normal, UV);
        }
    }
}
