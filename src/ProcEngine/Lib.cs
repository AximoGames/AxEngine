using Net3dBool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Net3dBoolDemo
{



    public abstract class GameObject : IGameObject
    {
        public readonly int Id;

        public GameObject()
        {
            Id = GetNextGameObjectId();
        }

        private static int CurrentGameObjectId;
        private static int GetNextGameObjectId()
        {
            return Interlocked.Increment(ref CurrentGameObjectId);
        }

    }

    public interface IGameObject
    {
    }

    public interface IRenderableObject : IGameObject
    {
    }

    public interface IMeshObject : IRenderableObject
    {
        Vector3[] GetVertices();
        int[] GetIndices();
    }

    public class MeshObject : GameObject, IMeshObject
    {
        public int[] GetIndices()
        {
            return DefaultCoordinates.DEFAULT_BOX_COORDINATES;
        }

        public Vector3[] GetVertices()
        {
            return DefaultCoordinates.DEFAULT_BOX_VERTICES;
        }
    }

}
