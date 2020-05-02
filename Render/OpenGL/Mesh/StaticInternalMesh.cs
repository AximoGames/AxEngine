// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;

namespace Aximo.Render
{
    public class StaticInternalMesh : InternalMesh
    {
        public StaticInternalMesh() : base() { }
        public StaticInternalMesh(Mesh meshData) : base(meshData) { }
        public StaticInternalMesh(Mesh meshData, Material material) : base(meshData, material) { }
    }
}
