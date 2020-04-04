// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using OpenToolkit;
using OpenToolkit.Graphics.OpenGL4;
using OpenToolkit.Mathematics;

namespace Aximo.Render
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

        private static Serilog.ILogger Log = Aximo.Log.ForContext<RenderContext>();

        public Matrix4 WorldPositionMatrix = Matrix4.Identity;

        public List<IRenderPipeline> RenderPipelines = new List<IRenderPipeline>();

        public IRenderPipeline CurrentPipeline { get; internal set; }

        public void InitRender()
        {
            foreach (var pipeline in RenderPipelines)
            {
                ObjectManager.PushDebugGroup("InitRender", pipeline);
                CurrentPipeline = pipeline;
                pipeline.InitRender(this, Camera);
                ObjectManager.PopDebugGroup();
            }
        }

        public void Render()
        {
            foreach (var pipeline in RenderPipelines)
            {
                ObjectManager.PushDebugGroup("Render", pipeline);
                CurrentPipeline = pipeline;
                pipeline.Render(this, Camera);
                ObjectManager.PopDebugGroup();
            }
        }

        public void OnWorldRendered()
        {
            foreach (var obj in AllObjects)
                obj.OnWorldRendered();
        }

        public Vector2i _ScreenSize;
        public Vector2i ScreenSize
        {
            get { return _ScreenSize; }
            set
            {
                _ScreenSize = value;
                ScreenAspectRatio = (float)value.X / (float)value.Y;
                PixelToUVFactor = new Vector2(1.0f / _ScreenSize.X, 1.0f / _ScreenSize.Y);
                PixelToNDCFactor = PixelToUVFactor * 2;
            }
        }
        public Vector2 PixelToUVFactor { get; private set; }
        public Vector2 PixelToNDCFactor { get; private set; }
        public float ScreenAspectRatio { get; private set; }

        public T GetPipeline<T>()
            where T : class, IRenderPipeline
        {
            return (T)RenderPipelines.FirstOrDefault(p => p is T);
        }

        public IRenderPipeline PrimaryRenderPipeline { get; set; }

        public static RenderContext Current { get; set; }

        public BindingPoint LightBinding;

        public SceneOptions SceneOpitons;
        public Camera Camera;
        public List<IGameObject> AllObjects = new List<IGameObject>();
        public List<IRenderableObject> RenderableObjects = new List<IRenderableObject>();
        public List<IUpdateFrame> UpdateFrameObjects = new List<IUpdateFrame>();
        public List<IShadowObject> ShadowObjects = new List<IShadowObject>();
        public List<ILightObject> LightObjects = new List<ILightObject>();

        public IGameObject GetObjectByName(string name)
        {
            // TODO: Hash
            return AllObjects.FirstOrDefault(o => o.Name == name);
        }

        public T GetObjectByName<T>(string name)
        {
            var obj = GetObjectByName(name);
            if (obj == null || !(obj is T))
                return default;

            return (T)obj;
        }

        public void AddPipeline(IRenderPipeline pipeline)
        {
            RenderPipelines.Add(pipeline);
        }

        public void AddObject(IGameObject obj)
        {
            obj.AssignContext(this);

            LogInfoMessage($"Init Object {obj.Name}");
            ObjectManager.PushDebugGroup("Init", obj);
            obj.Init();
            ObjectManager.PopDebugGroup();

            AllObjects.Add(obj);

            if (obj is IShadowObject shadowObj)
                ShadowObjects.Add(shadowObj);

            if (obj is IRenderableObject renderableObj)
                RenderableObjects.Add(renderableObj);

            if (obj is IUpdateFrame updateFrameObj)
                UpdateFrameObjects.Add(updateFrameObj);

            if (obj is ILightObject lightObj)
                LightObjects.Add(lightObj);
        }

        public void RemoveObject(IGameObject obj)
        {
            AllObjects.Remove(obj);

            if (obj is IShadowObject shadowObj)
                ShadowObjects.Remove(shadowObj);

            if (obj is IRenderableObject renderableObj)
                RenderableObjects.Remove(renderableObj);

            if (obj is IUpdateFrame updateFrameObj)
                UpdateFrameObjects.Remove(updateFrameObj);

            if (obj is ILightObject lightObj)
                LightObjects.Remove(lightObj);
        }

        private void EmmitLogMessage(DebugType type, DebugSeverity severity, string message)
        {
            var handle = GCHandle.Alloc(message, GCHandleType.Pinned);
            GL.DebugMessageInsert(DebugSourceExternal.DebugSourceApplication, type, 0, severity, message.Length, message);
            handle.Free();
        }

        public void LogInfoMessage(string message)
        {
            EmmitLogMessage(DebugType.DebugTypeOther, DebugSeverity.DebugSeverityNotification, message);
        }

        public void OnScreenResize()
        {
            GL.Viewport(0, 0, ScreenSize.X, ScreenSize.Y);

            // GL.Scissor(0, 0, ScreenSize.X, ScreenSize.Y);
            // GL.Enable(EnableCap.ScissorTest);
            Camera.SetAspectRatio(ScreenSize.X, ScreenSize.Y);

            foreach (var pipe in RenderPipelines)
                pipe.OnScreenResize();

            foreach (var obj in AllObjects)
                obj.OnScreenResize();
        }

        public Vector4 BackgroundColor { get; set; } = new Vector4(0.2f, 0.3f, 0.3f, 1.0f);

        public void Free()
        {
            LightBinding?.Free();
            LightBinding = null;
        }

        public void DumpInfo(bool list)
        {
            Log.Info("Objects: {ObjectCount}", AllObjects.Count);
            if (list)
                lock (AllObjects)
                    foreach (var obj in AllObjects)
                        Log.Info("{Id} {Type} {Name}", obj.Id, obj.GetType().Name, obj.Name);

            InternalTextureManager.DumpInfo(list);
        }

        public void DeleteOrphaned()
        {
            for (var i = AllObjects.Count - 1; i >= 0; i--)
            {
                var obj = AllObjects[i];
                if (obj.Orphaned)
                {
                    Log.Verbose("Delete Orhpaned Object {Id} {Type} {name}", obj.Id, obj.GetType().Name, obj.Name);
                    obj.Free();
                    RemoveObject(obj);
                }
            }

            InternalTextureManager.DeleteOrphaned();
        }

    }

    public class SceneOptions
    {
    }

}
