namespace Aximo.Engine
{

    public interface IScriptBehaviour
    {
        public void Awake();
        public void Start();
        public void Update();
        public void Destroy();
        public void Invoke(string methodName);
    }

}
