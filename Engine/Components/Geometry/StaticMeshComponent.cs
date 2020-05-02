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
    public class StaticMeshComponent : MeshComponent
    {
        private static Serilog.ILogger Log = Aximo.Log.ForContext<StaticMeshComponent>();

        public StaticMeshComponent()
        {
        }

        public StaticMeshComponent(Mesh3 mesh)
        {
            SetMesh(mesh);
        }

        public StaticMeshComponent(Mesh3 mesh, GameMaterial material)
        {
            SetMesh(mesh);
            Material = material;
        }

        public Mesh3 Mesh { get; private set; }
        private StaticMesh InternalMesh;

        public void SetMesh(Mesh3 mesh)
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

        internal override void DoDeallocation()
        {
            if (!HasDeallocation)
                return;

            if (RenderableObject == null)
                return;

            Log.Verbose("Set RenderableObject.Orphaned");

            RenderableObject.Orphaned = true;
            RenderableObject = null;

            base.DoDeallocation();
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

            obj.Enabled = Visible;

            if (created)
                RenderContext.Current.AddObject(RenderableObject);
        }
    }
}
