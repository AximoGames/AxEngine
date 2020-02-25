using System.Threading;
using System.Collections.Generic;

namespace AxEngine
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

        public virtual List<IRenderPipeline> RenderPipelines { get; set; } = new List<IRenderPipeline>();

        public void UsePipeline<T>()
            where T : class, IRenderPipeline
        {
            UsePipeline(Context.GetPipeline<T>());
        }

        public void UsePipeline(IRenderPipeline pipeline)
        {
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
    }

}
