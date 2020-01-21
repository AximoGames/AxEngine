using System.Collections.Generic;

namespace ProcEngine
{
    //public class MeshObject : GameObject, IMeshObject
    //{
    //    public int[] GetIndices()
    //    {
    //        return DefaultCoordinates.DEFAULT_BOX_COORDINATES;
    //    }

    //    public Vector3[] GetVertices()
    //    {
    //        return DefaultCoordinates.DEFAULT_BOX_VERTICES;
    //    }
    //}

    public class RenderContext
    {

        public Camera Camera;
        public List<IGameObject> AllObjects = new List<IGameObject>();
        public List<IRenderableObject> RenderableObjects = new List<IRenderableObject>();
        public List<IRenderableObject> RenderableScreenObjects = new List<IRenderableObject>();
        public List<IShadowObject> ShadowObjects = new List<IShadowObject>();
        public List<ILightObject> LightObjects = new List<ILightObject>();

        public void AddObject(IGameObject obj)
        {
            obj.AssignContext(this);

            obj.Init();

            AllObjects.Add(obj);

            if (obj is IShadowObject shadowObj)
                ShadowObjects.Add(shadowObj);

            if (obj is IRenderableObject renderableObj)
            {
                if (renderableObj.RenderPosition == RenderPosition.Scene)
                    RenderableObjects.Add(renderableObj);
                if (renderableObj.RenderPosition == RenderPosition.Screen)
                    RenderableScreenObjects.Add(renderableObj);
            }

            if (obj is ILightObject lightObj)
                LightObjects.Add(lightObj);
        }
    }

}
