using OpenTK;
using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;

namespace ProcEngine
{

    public interface IRenderPipeline
    {
        void Render(RenderContext context, Camera camera);
    }

    public abstract class RenderPipeline : IRenderPipeline
    {
        public virtual void Init() { }
        public abstract void Render(RenderContext context, Camera camera);
    }

    public class PointShadowRenderPipeline : RenderPipeline
    {

        public static FrameBuffer shadowCubeFb;

        public override void Init()
        {
            shadowCubeFb = new FrameBuffer(1024, 1024);
            shadowCubeFb.InitCubeDepth();
        }

        public override void Render(RenderContext context, Camera camera)
        {
        }

    }

    public class DirectionalShadowRenderPipeline : RenderPipeline
    {

        public override void Init()
        {
        }

        public override void Render(RenderContext context, Camera camera)
        {
        }

    }

    public class ForwardRenderPipeline : RenderPipeline
    {

        public override void Init()
        {
        }

        public override void Render(RenderContext context, Camera camera)
        {
        }

    }

    public class DeferredRenderPipeline : RenderPipeline
    {
        public override void Init()
        {
        }

        public override void Render(RenderContext context, Camera camera)
        {
        }
    }

}