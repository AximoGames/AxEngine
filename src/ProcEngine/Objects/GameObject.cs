using System.Threading;
using System.Collections.Generic;

namespace ProcEngine
{

    public abstract class GameObject : IGameObject
    {
        public int Id { get; }
        public string Name { get; set; }
        public bool Enabled { get; set; } = true;

        public RenderContext Context { get; private set; }

        public virtual List<IRenderPipeline> RenderPipelines { get; set; } = new List<IRenderPipeline>();

        public void UsePipeline<T>()
            where T : class, IRenderPipeline
        {
            RenderPipelines.Add(Context.GetPipeline<T>());
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
    }

}
