// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Aximo.Render.OpenGL;

namespace Aximo.Render.Pipelines
{
    public abstract class RenderPipeline : IRenderPipeline, IReloadable
    {
        public virtual void BeforeInit() { }
        public virtual void Init(bool reload = false) { }
        public virtual void AfterInit() { }

        public virtual void InitRender(RenderContext context, Camera camera)
        {
        }

        public abstract void Render(RenderContext context, Camera camera);
        protected virtual void Render(RenderContext context, Camera camera, IRenderableObject obj)
        {
            ObjectManager.PushDebugGroup("OnRender", obj);
            obj.OnRender();
            ObjectManager.PopDebugGroup();
        }
        public virtual IEnumerable<IRenderableObject> GetRenderObjects(RenderContext context, Camera camera)
        {
            foreach (var obj in context.RenderableObjects)
                if (obj.Enabled)
                    if (obj.RenderPipelines.Contains(this))
                        yield return obj;
        }

        protected virtual IEnumerable<IRenderableObject> SortFromFrontToBack(RenderContext context, Camera camera, IEnumerable<IRenderableObject> objects)
        {
            var list = objects.ToList();
            list.Sort(new MeshDepthSorter(camera));
            return list;
        }

        public virtual void OnScreenResize(ScreenResizeEventArgs e)
        {
        }

        #region IDisposable Support
        protected bool Disposed { get; private set; } // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (Disposed)
                return;

            if (disposing)
            {
            }

            Disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
        }

        #endregion
        public void OnReload()
        {
            Init(true);
        }
    }

    public interface IRenderPipeline : IDisposable
    {
        void BeforeInit();
        void Init(bool reload = false);
        void AfterInit();
        void OnScreenResize(ScreenResizeEventArgs e);
        void InitRender(RenderContext context, Camera camera);
        void Render(RenderContext context, Camera camera);
        IEnumerable<IRenderableObject> GetRenderObjects(RenderContext context, Camera camera);
    }
}
