using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK;

namespace ProcEngine
{

    public interface IGameObject
    {
        void Init();
        void Free();
    }

    public interface IRenderableObject : IGameObject
    {
        void OnRender();
    }

    public interface IShadowObject : IGameObject
    {
        void OnRenderShadow();
    }

    public interface IRenderTarget : IRenderableObject
    {
    }

    public interface IMeshObject : IRenderableObject
    {
        Vector3[] GetVertices();
        int[] GetIndices();
    }

    public interface IPosition : IGameObject
    {
        Vector3 Position { get; set; }
    }

    public interface ILightObject : IPosition, IGameObject
    {
    }

}
