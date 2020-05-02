// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using OpenToolkit.Mathematics;

namespace Aximo
{
    public abstract class MeshComponent
    {
        public virtual MeshComponentType Type { get; protected set; }

        public static MeshComponent Create(MeshComponentType componentType)
        {
            switch (componentType)
            {
                case MeshComponentType.Position:
                    return new MeshComponent<Vector3>(componentType);
                case MeshComponentType.Normal:
                    return new MeshComponent<Vector3>(componentType);
                case MeshComponentType.Color:
                    return new MeshComponent<Vector4>(componentType);
                case MeshComponentType.UV:
                    return new MeshComponent<Vector2>(componentType);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public abstract int Count { get; }

        public abstract void AddRange(MeshComponent src, int start, int count);

        public abstract MeshComponent CloneEmpty();
    }
}
