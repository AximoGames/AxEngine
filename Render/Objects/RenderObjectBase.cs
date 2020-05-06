// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Threading;

namespace Aximo.Render
{
    public abstract class RenderObjectBase : IRenderObject
    {
        public int Id { get; }
        public string Name { get; set; }
        public bool Enabled { get; set; } = true;
        public int DrawPriority { get; set; }

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

        public virtual void OnScreenResize(ScreenResizeEventArgs e)
        {
        }

        public virtual void OnWorldRendered()
        {
        }

        #region IDisposable Support
        protected bool Disposed { get; private set; }

        protected virtual void Dispose(bool disposing)
        {
            if (!Disposed)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                Disposed = true;
            }
        }

        ~RenderObjectBase()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
