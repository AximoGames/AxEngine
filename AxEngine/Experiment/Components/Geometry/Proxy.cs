// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using OpenTK;

namespace Aximo.Engine
{

    public enum GPrimitiveType
    {
        Triangles,
        Lines,
    }

    public class MeshBatch
    {
        public bool CastShadowget { get; set; }
        public GPrimitiveType Type { get; set; }

        // Data
    }

    public struct BoxSphereBounds
    {
        public Vector3 BoxExtent;
        public Vector3 Origin;
        public float SphereRadius;
    }

    public class StaticPrimitiveDrawInterface
    {
        public void DrawMesh(MeshBatch mesh)
        {
        }
    }

    // Always called from Render Thread
    public class PrimitiveSceneProxy
    {
        public BoxSphereBounds Bounds { get; private set; }
        public void DrawStaticElements(StaticPrimitiveDrawInterface meshInterface) { }
        public bool IsStatic { get; private set; }
    }

    public class StaticMeshSceneProxy : PrimitiveSceneProxy
    {

        public bool CastShadow { get; private set; }

    }

}
