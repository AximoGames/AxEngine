// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;

namespace Aximo.Render
{

    public abstract class RenderPipeline : IRenderPipeline
    {
        public virtual void BeforeInit() { }
        public virtual void Init() { }
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
    }

    public interface IRenderPipeline : IDisposable
    {
        void BeforeInit();
        void Init();
        void AfterInit();
        void OnScreenResize(ScreenResizeEventArgs e);
        void InitRender(RenderContext context, Camera camera);
        void Render(RenderContext context, Camera camera);
        IEnumerable<IRenderableObject> GetRenderObjects(RenderContext context, Camera camera);
    }

}
