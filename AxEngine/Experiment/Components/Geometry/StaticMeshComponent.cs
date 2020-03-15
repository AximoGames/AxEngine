// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;
using Aximo.Render;
using OpenTK;

namespace Aximo.Engine
{

    public class StreamableRenderAsset
    {

        public Stream Stream { get; private set; }

    }

    public class MeshComponent : PrimitiveComponent
    {
    }

    public class StaticMeshComponent : MeshComponent
    {

        public StaticMesh Mesh { get; private set; }

        public void SetMesh(StaticMesh mesh)
        {
            Mesh = mesh;
        }

        public override PrimitiveSceneProxy CreateProxy()
        {
            return new StaticMeshSceneProxy(this);
        }

    }

    public class PrimitiveSceneProxy
    {

        public List<Material> Materials = new List<Material>();

        public PrimitiveSceneProxy(PrimitiveComponent component) { }

    }

    public class StaticMeshSceneProxy : PrimitiveSceneProxy
    {
        public StaticMeshSceneProxy(StaticMeshComponent component) : base(component) { }

        public void DrawStaticElements(StaticPrimitiveDrawInterface pdi)
        {
        }

    }

}
