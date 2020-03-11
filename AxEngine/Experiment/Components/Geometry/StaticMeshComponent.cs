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

    public class GMaterial : IMaterialInterface
    {

        public Vector3 BaseColor { get; set; }
        public bool DisableDepthTest { get; set; }

        public Vector3 WorldPositionOffset { get; set; }

        private List<GTexture> _Textures;
        public ICollection<GTexture> Textures { get; private set; }

        public GMaterial()
        {
            _Textures = new List<GTexture>();
            Textures = new ReadOnlyCollection<GTexture>(_Textures);
        }

        public void AddTexture(GTexture texture)
        {
            _Textures.Add(texture);
        }

        public void RemoveTexture(GTexture texture)
        {
            _Textures.Remove(texture);
        }

    }

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

}
