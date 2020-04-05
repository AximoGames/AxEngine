// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Threading;

namespace Aximo.Render
{

    public abstract class RenderObjectBase : IRenderObject
    {

        public int Id { get; }
        public string Name { get; set; }
        public bool Enabled { get; set; } = true;

        public RenderObjectBase()
        {
            Id = GetNextGameObjectId();
        }

        private static int CurrentGameObjectId;
        private static int GetNextGameObjectId()
        {
            return Interlocked.Increment(ref CurrentGameObjectId);
        }

        public void AssignContext(RenderContext ctx)
        {
            Context = ctx;
        }

        private Dictionary<string, object> ExtraData = new Dictionary<string, object>();

        public T GetExtraData<T>(string name, T defaultValue = default)
        {
            return IDataHelper.GetData(ExtraData, name, defaultValue);
        }

        public bool HasExtraData(string name)
        {
            return IDataHelper.HasData(ExtraData, name);
        }

        public bool SetExraData<T>(string name, T value, T defaultValue = default)
        {
            return IDataHelper.SetData(ExtraData, name, value, defaultValue);
        }

        public RenderContext Context { get; private set; }
        public virtual bool Orphaned { get; set; }

        public abstract void Init();

        public abstract void Free();

        public virtual void OnScreenResize()
        {
        }

        public virtual void OnWorldRendered()
        {
        }

    }

}
