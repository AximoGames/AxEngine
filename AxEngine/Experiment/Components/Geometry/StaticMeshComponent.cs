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
        public CubeComponent() : base(MeshDataBuilder.Cube(), GameMaterial.Default)
        {
        }
    }

    public class DebugCubeComponent : StaticMeshComponent
    {
        public DebugCubeComponent() : base(MeshDataBuilder.DebugCube(), GameMaterial.Default)
        {
        }
    }

    public class SphereComponent : StaticMeshComponent
    {
        public SphereComponent(int divisions = 2) : base(MeshDataBuilder.Sphere(2), GameMaterial.Default)
        {
        }
    }

    public class StaticMeshComponent : MeshComponent
    {

        public StaticMeshComponent()
        {

        }

        public StaticMeshComponent(MeshData mesh)
        {
            SetMesh(mesh);
        }

        public StaticMeshComponent(MeshData mesh, GameMaterial material)
        {
            SetMesh(mesh);
            Material = material;
        }

        public MeshData Mesh { get; private set; }
        private StaticMesh InternalMesh;

        public void SetMesh(MeshData mesh)
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

            base.SyncChanges();

            bool created = false;
            if (RenderableObject == null)
            {
                created = true;
                RenderableObject = new SimpleVertexObject();
            }

            var obj = (SimpleVertexObject)RenderableObject;
            if (MeshChanged)
            {
                if (InternalMesh == null)
                {
                    InternalMesh = new StaticMesh(Mesh);
                    obj.Name = Name;
                }
                InternalMesh.Materials.Clear();
                foreach (var gameMat in Materials)
                {
                    InternalMesh.Materials.Add(gameMat.InternalMaterial);
                }

                obj.SetVertices(InternalMesh);
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
