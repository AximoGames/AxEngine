using System;
using System.Collections.Generic;
using System.Text;
using OpenToolkit.Graphics.OpenGL;
using OpenToolkit.Mathematics;

namespace Aximo.Engine
{

    public abstract class ScriptBehaviour : Behaviour, IScriptBehaviour
    {
        public virtual void Awake() { }

        public virtual void Destroy()
        {
        }

        public virtual void Invoke(string methodName)
        {
        }

        public virtual void Start()
        {
        }

        public virtual void Update()
        {
        }
    }

}
