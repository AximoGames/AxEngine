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

    public class CubeComponent : StaticMeshComponent
    {
        public CubeComponent() : base(MeshBuilder.Cube())
        {
        }
    }

    public class SphereComponent : StaticMeshComponent
    {
        public SphereComponent(int divisions = 2) : base(MeshBuilder.Sphere(2))
        {
        }
    }

    public class StaticMeshComponent : MeshComponent
    {

        public StaticMeshComponent()
        {

        }

        public StaticMeshComponent(StaticMesh mesh)
        {
            SetMesh(mesh);
        }

        public StaticMesh Mesh { get; private set; }

        public void SetMesh(StaticMesh mesh)
        {
            Mesh = mesh;
            UpdateMesh();
        }

        protected internal bool MeshChanged;
        protected internal void UpdateMesh()
        {
            MeshChanged = true;
            Update();
        }

        public override PrimitiveSceneProxy CreateProxy()
        {
            return new StaticMeshSceneProxy(this);
        }

        internal override void SyncChanges()
        {
            if (!HasChanges)
                return;

            bool created = false;
            if (RenderableObject == null)
            {
                created = true;
                RenderableObject = new SimpleVertexObject();
            }

            var obj = (SimpleVertexObject)RenderableObject;
            if (MeshChanged)
            {
                obj.SetVertices(Mesh);
                MeshChanged = false;
            }

            if (TransformChanged)
            {
                var trans = LocalToWorld();
                obj.PositionMatrix = trans;
                TransformChanged = false;
            }

            if (created)
                RenderContext.Current.AddObject(RenderableObject);

            base.SyncChanges();
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
