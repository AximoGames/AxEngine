// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Linq;
using Aximo.Render;
using Aximo.Render.Objects;
using Aximo.Render.OpenGL;

namespace Aximo.Engine.Components.Geometry
{
    /// <inheritdoc/>
    public class StaticMeshComponent : MeshComponent
    {
        private static Serilog.ILogger Log = Aximo.Log.ForContext<StaticMeshComponent>();

        public StaticMeshComponent()
        {
        }

        public StaticMeshComponent(Mesh mesh)
        {
            SetMesh(mesh);
        }

        public StaticMeshComponent(Mesh mesh, Material material)
        {
            SetMesh(mesh);
            Material = material;
        }

        /// <summary>
        /// Holds a the Mesh.
        /// </summary>
        public Mesh Mesh { get; private set; }
        private StaticInternalMesh InternalMesh;

        /// <summary>
        /// Sets the Mesh.
        /// </summary>
        /// <param name="mesh">The mesh data.</param>
        public void SetMesh(Mesh mesh)
        {
            if (mesh == null)
            {
                Mesh = null;
                UpdateMesh();
                return;
            }

            Mesh = mesh;
            mesh.CalculateBounds();
            UpdateMesh();
        }

        private protected bool MeshChanged;
        private protected void UpdateMesh()
        {
            MeshChanged = true;
            PropertyChanged();
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
                    InternalMesh = new StaticInternalMesh(Mesh);
                    obj.Name = Name;
                }
                else
                {
                    InternalMesh.SetMesh(Mesh);
                }
                InternalMesh.Materials.Clear();
                foreach (var gameMat in Materials)
                {
                    InternalMesh.Materials.Add(gameMat.RendererMaterial);
                }

                obj.UseTransparency = Materials.Any(m => m.UseTransparency);

                obj.SetVertices(InternalMesh);
                MeshChanged = false;
            }

            if (TransformChanged)
            {
                var trans = LocalToWorld();
                UpdateWorldBounds(trans);
                obj.PositionMatrix = trans;
                obj.WorldBounds = WorldBounds;
                TransformChanged = false;
            }

            obj.Enabled = Visible;
            obj.DrawPriority = DrawPriority;

            if (created)
                RenderContext.Current.AddObject(RenderableObject);
        }
    }
}
