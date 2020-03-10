// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;
using OpenTK;

namespace Aximo.Engine
{


    public class GMaterialInterface
    {
    }

    public enum GTextureFilter
    {
        Default,
        Nearest,
    }

    public class GTextureSource
    {

        public int Width => Size.X;
        public int Height => Size.Y;

        public Vector2i Size { get; private set; }

    }

    public class GTexture
    {

        public GTextureFilter Filter { get; set; }

        public GTextureSource Source { get; private set; }

    }

    public class GMaterial : GMaterialInterface
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

    public class StaticMeshSourceModel
    {
        public StaticMesh Owner { get; private set; }

    }

    public class StaticMesh : StreamableRenderAsset
    {

        public StaticMeshSourceModel SourceModel { get; private set; }

        private List<GMaterialInterface> _Materials;
        public ICollection<GMaterialInterface> Materials { get; private set; }

        public SceneComponent Parent { get; private set; }

        public StaticMesh()
        {
            _Materials = new List<GMaterialInterface>();
            Materials = new ReadOnlyCollection<GMaterialInterface>(_Materials);
        }

        public void AddMaterial(GMaterialInterface material)
        {
            _Materials.Add(material);
        }

        public void RemoveMaterial(GMaterialInterface material)
        {
            _Materials.Remove(material);
        }

        public int GetNumVertices(int lod)
        {
            throw new NotImplementedException();
        }

    }

    public class StaticMeshComponent : PrimitiveComponent
    {


        public StaticMesh Mesh { get; private set; }

        public void SetMesh(StaticMesh mesh)
        {
            Mesh = mesh;
        }

    }

}
