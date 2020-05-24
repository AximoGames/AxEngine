// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Aximo
{
    public abstract class MeshPositionComponent<T> : MeshComponent<T>
        where T : unmanaged
    {
        public MeshPositionComponent()
            : base(MeshComponentType.Position)
        {
        }

        public abstract void CalculateBounds();
    }
}
