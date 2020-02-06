using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using OpenTK.Graphics.OpenGL4;

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
        public BindingPoint LightBinding;

        public SceneOptions SceneOpitons;
        public Camera Camera;
        public List<IGameObject> AllObjects = new List<IGameObject>();
        public List<IRenderableObject> RenderableObjects = new List<IRenderableObject>();
        public List<IRenderableObject> RenderableScreenObjects = new List<IRenderableObject>();
        public List<IShadowObject> ShadowObjects = new List<IShadowObject>();
        public List<ILightObject> LightObjects = new List<ILightObject>();

        public IGameObject GetObjectByName(string name)
        {
            // TODO: Hash
            return AllObjects.FirstOrDefault(o => o.Name == name);
        }

        public void AddObject(IGameObject obj)
        {
            obj.AssignContext(this);

            LogInfoMessage($"Init Object {obj.Name}");
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

        private void EmmitLogMessage(DebugType type, DebugSeverity severity, string message)
        {
            var handle = GCHandle.Alloc(message, GCHandleType.Pinned);
            GL.DebugMessageInsert(DebugSourceExternal.DebugSourceApplication, type, 0, severity, message.Length, message);
            handle.Free();
        }

        public void LogInfoMessage(string message)
        {
            EmmitLogMessage(DebugType.DebugTypeError, DebugSeverity.DebugSeverityNotification, message);
        }
    }

}
