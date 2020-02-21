using OpenTK;
using System.Collections.Generic;

namespace ProcEngine
{

    public interface IRenderPipeline
    {
        void Init();
        void InitRender(RenderContext context, Camera camera);
        void Render(RenderContext context, Camera camera);
        IEnumerable<IRenderableObject> GetRenderObjects(RenderContext context, Camera camera);
    }

    public abstract class RenderPipeline : IRenderPipeline
    {
        public virtual void Init() { }

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
    }

}