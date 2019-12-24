using System.Threading;

namespace ProcEngine
{
    public abstract class GameObject : IGameObject
    {
        public readonly int Id;

        public RenderContext Context;

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
