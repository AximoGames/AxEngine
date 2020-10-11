using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using Aximo.Render;
using Aximo.Render.Objects;
using Aximo.Render.OpenGL;
using McMaster.NETCore.Plugins;
using OpenToolkit.Graphics.OpenGL;
using OpenToolkit.Mathematics;

namespace Aximo.Engine
{

    public class MeshC : Component
    {
        private Mesh? _Mesh;

        public Mesh Mesh
        {
            get => _Mesh;
            set => SetMesh(value);
        }

        public void SetMesh(Mesh mesh)
        {
            _Mesh = mesh;
            NeedSyncRenderer();
        }

        public void UpdateMesh()
        {
            NeedSyncRenderer();
        }

        private SimpleVertexObject? obj;

        private protected override void OnSyncRendererInternal()
        {
            base.OnSyncRendererInternal();
            if (obj == null)
            {
                obj = new SimpleVertexObject();
                obj.SetVertices(new StaticInternalMesh(Mesh));
                obj.Name = ToString();
                RenderContext.Current.AddObject(obj);
                obj.PositionMatrix = Actor.Transform.GetMatrix();
                Mesh.CalculateBounds();
                obj.LocalBounds = Mesh.Bounds;
            }
        }

    }

}
