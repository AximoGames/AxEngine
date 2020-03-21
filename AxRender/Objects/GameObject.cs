// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Threading;

namespace Aximo.Render
{

    public abstract class GameObject : IGameObject
    {
        public int Id { get; }
        public string Name { get; set; }
        public bool Enabled { get; set; } = true;

        private Dictionary<string, object> Data = new Dictionary<string, object>();

        public T GetData<T>(string name, T defaultValue = default)
        {
            return IDataHelper.GetData(Data, name, defaultValue);
        }

        public bool HasData(string name)
        {
            return IDataHelper.HasData(Data, name);
        }

        public bool SetData<T>(string name, T value, T defaultValue = default)
        {
            return IDataHelper.SetData(Data, name, value, defaultValue);
        }

        public RenderContext Context { get; private set; }

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

        public void AssignContext(RenderContext ctx)
        {
            Context = ctx;
        }

        public GameObject()
        {
            Id = GetNextGameObjectId();
        }

        private static int CurrentGameObjectId;
        private static int GetNextGameObjectId()
        {
            return Interlocked.Increment(ref CurrentGameObjectId);
        }

        public abstract void Init();

        public abstract void Free();

        public virtual void OnScreenResize()
        {
        }
    }

}
