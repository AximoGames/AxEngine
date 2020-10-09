using System;
using System.Collections.Generic;
using System.Linq;

namespace Aximo.Engine
{

    public static class ObjectManager
    {

        private static Dictionary<int, BaseObject> Hash = new Dictionary<int, BaseObject>();
        private static object ObjectsLock = new object();

        public static IEnumerable<BaseObject> GetAllObjectsSlow()
        {
            lock (ObjectsLock)
                return Hash.Values.ToArray();
        }

        private static IEnumerable<BaseObject> GetAllObjectsUnsafe()
        {
            return Hash.Values;
        }

        internal static void GetAllObjectsUnsafe(Action<IEnumerable<BaseObject>> callback)
        {
            lock (ObjectsLock)
                callback(GetAllObjectsUnsafe());
        }

        internal static void RegisterObject(BaseObject obj)
        {
            lock (ObjectsLock)
                Hash.Add(obj.InstanceID, obj);
        }

        internal static void UnregisterObject(BaseObject obj)
        {
            lock (ObjectsLock)
                Hash.Remove(obj.InstanceID);
        }
    }

    public static class SceneManager
    {
        private static Scene? CurrentScene;

        public static void SetActiveScene(Scene scene)
        {
            CurrentScene = scene;
            scene.Enabled = true;
            foreach (var act in scene.GetActors())
                act.OnSceneActivated();
        }

        public static Scene GetCurrentScene()
        {
            return CurrentScene;
        }

    }

}
