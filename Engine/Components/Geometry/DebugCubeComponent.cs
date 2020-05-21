// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;
using Aximo.Render;
using Aximo.Render.OpenGL;
using OpenToolkit;

namespace Aximo.Engine.Components.Geometry
{
    public class DebugCubeComponent : StaticMeshComponent
    {
        public DebugCubeComponent() : base(MeshDataBuilder.DebugCube(), GameMaterial.Default)
        {
        }
    }
}
