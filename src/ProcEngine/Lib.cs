using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK;
using System.IO;

namespace ProcEngine
{


    public static class DirectoryHelper
    {
        public static string RootDir
        {
            get
            {
                return new DirectoryInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..")).FullName;
            }
        }
    }

    public interface IGameObject
    {
        int Id { get; }
        string Name { get; set; }
        bool Enabled { get; set; }
        RenderContext Context { get; }
        void Init();
        void Free();
        void AssignContext(RenderContext ctx);
    }

    public interface IRenderableObject : IGameObject
    {
        void OnRender();
        RenderPosition RenderPosition { get; }
    }

    public interface ILightTarget
    {
        List<ILightObject> Lights { get; }
    }

    public enum RenderPosition
    {
        Scene,
        Screen,
    }

    public interface IReloadable : IGameObject
    {
        void OnReload();
    }

    public interface IShadowObject : IGameObject
    {
        void OnRenderShadow();
        void OnRenderCubeShadow();
        bool RenderShadow { get; set; }
    }

    public interface IRenderTarget : IRenderableObject
    {
    }

    public interface IMeshObject : IRenderableObject
    {
        Vector3[] GetVertices();
        int[] GetIndices();
    }

    public interface IPosition
    {
        Vector3 Position { get; set; }
    }

    public interface IScaleRotate : IPosition
    {
        Vector3 Scale { get; set; }
        Vector3 Rotate { get; set; }
    }

    public interface ILightObject : IPosition, IGameObject
    {
    }

}
