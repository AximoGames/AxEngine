// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;
using OpenToolkit.Graphics.OpenGL4;

namespace Aximo.Render.Pipelines
{
    public class ScreenPipeline : RenderPipeline
    {
        public override void Init(bool reload = false)
        {
        }

        public override void Render(RenderContext context, Camera camera)
        {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            GL.Disable(EnableCap.DepthTest);
            GL.ClearColor(1.0f, 0.0f, 1.0f, 1.0f);
            GL.Clear(ClearBufferMask.ColorBufferBit);

            foreach (var obj in GetRenderObjects(context, camera).OrderBy(o => o.DrawPriority))
                Render(context, camera, obj);
        }

        public override IEnumerable<IRenderableObject> GetRenderObjects(RenderContext context, Camera camera)
        {
            return SortFromFrontToBack(context, camera, base.GetRenderObjects(context, camera));
        }

        protected override IEnumerable<IRenderableObject> SortFromFrontToBack(RenderContext context, Camera camera, IEnumerable<IRenderableObject> objects)
        {
            var list = objects.ToList();
            return list;
            list.Sort(new MeshDepthSorter(camera, false));
            return list;
        }

    }
}
