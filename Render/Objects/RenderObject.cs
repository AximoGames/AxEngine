// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Aximo.Render.Pipelines;

namespace Aximo.Render.Objects
{
    public abstract class RenderObject : RenderObjectBase
    {
        public virtual List<IRenderPipeline> RenderPipelines { get; } = new List<IRenderPipeline>();

        public void UsePipeline<T>()
            where T : class, IRenderPipeline
        {
            UsePipeline(Context.GetPipeline<T>());
        }

        public void UsePipeline(IRenderPipeline pipeline)
        {
            if (pipeline == null)
                return;

            if (!RenderPipelines.Contains(pipeline))
                RenderPipelines.Add(pipeline);
        }
    }
}
