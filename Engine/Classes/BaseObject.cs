using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using OpenToolkit.Graphics.OpenGL;
using OpenToolkit.Mathematics;

namespace Aximo.Engine
{

    public class BaseObject
    {

        private static int LastComponentID;

        internal BaseObject()
        {
            InstanceID = Interlocked.Increment(ref LastComponentID);
            ObjectManager.RegisterObject(this);
            Name = GetType().Name;
        }

        ~BaseObject()
        {
            ObjectManager.UnregisterObject(this);
        }

        public readonly int InstanceID;

        public string Name { get; set; }

        public override string ToString()
        {
            return $"[{InstanceID}] {Name}";
        }

        internal void CallOnSyncRenderer()
        {
            OnSyncRendererInternal();
        }

        private protected virtual void OnSyncRendererInternal()
        {
        }

        internal bool _NeedSyncRenderer = true;
        private protected void NeedSyncRenderer()
        {
            _NeedSyncRenderer = true;
        }

    }

}
