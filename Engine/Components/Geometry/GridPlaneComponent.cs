// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;
using Aximo.Render;
using OpenToolkit;

namespace Aximo.Engine
{
    public class GridPlaneComponent : StaticMeshComponent
    {
        public GridPlaneComponent(int size, bool center) : base(MeshDataBuilder.Grid(size, center), MaterialManager.DefaultLineMaterial)
        {
        }
    }
}
