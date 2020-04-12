// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

// Adapted from this excellent IcoSphere tutorial by Andreas Kahler
// http://blog.andreaskahler.com/2009/06/creating-icosphere-mesh-in-code.html
// Changes Copyright (C) 2014 by David Jeske, and donated to the public domain.

using System;
using System.Collections.Generic;
using Aximo.Render;
using OpenToolkit;
using OpenToolkit.Mathematics;

namespace Aximo.Render.Objects.Util.IcoSphere
{
    public class MeshGeometry3D
    {
        public List<Vector3> Positions = new List<Vector3>();
        public List<int> MeshIndicies = new List<int>();
        public List<TriangleIndices> Faces = new List<TriangleIndices>();
    }
}
