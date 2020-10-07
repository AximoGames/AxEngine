using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using McMaster.NETCore.Plugins;
using OpenToolkit.Graphics.OpenGL;
using OpenToolkit.Mathematics;

namespace Aximo.Engine
{

    internal class ScriptBehaviourWrapper : ScriptBehaviour
    {

        public override void Awake()
        {
            Script.Awake();
        }

        public override void Start()
        {
            Script.Start();
        }

        public override void Destroy()
        {
            Script.Destroy();
        }

        internal IScriptBehaviour Script { get; private set; }

        public ScriptBehaviourWrapper(IScriptBehaviour script)
        {
            Script = script;
        }

        public override void Invoke(string methodName)
        {
            Script.Invoke(methodName);
        }

        internal void ReloadStage1()
        {
            Destroy();
        }

        internal void ReloadStage2(IScriptBehaviour script)
        {
            Script = script;
        }

        internal void ReloadStage3()
        {
            Start();
        }

    }

}
