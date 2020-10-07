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

    public class Actor
    {
        public Scene Scene { get; internal set; }
        public int Layer { get; set; }

        /// <summary>
        /// True if this and their parents are active
        /// </summary>
        public bool Enabled => !ForceDisable && LocalActive;

        internal bool LocalActive { get; set; } = true;
        internal bool ForceDisable { get; set; } = true;

        public void SetActive(bool state)
        {
            if (state)
                Activate();
            else
                Deactivate();
        }

        internal void OnAddedToScene()
        {
            TryActivate();
        }

        internal void OnSceneActivated()
        {
            TryActivate();
        }

        private void TryActivate()
        {
            if (Scene.Enabled && LocalActive)
            {
                if (ForceDisable)
                {
                    ForceDisable = false;
                    DoActivateScripts();
                }
            }
        }

        private void TryDeActivate()
        {
            if (!Scene.Enabled || !LocalActive)
            {
                if (!Scene.Enabled)
                    ForceDisable = true;

                DoDeactivateScripts();
            }
        }

        private void DoActivateScripts()
        {
            var scripts = GetScripts();
            foreach (var script in scripts)
                script.Awake();

            foreach (var script in scripts)
                script.Start();
        }

        private void DoDeactivateScripts()
        {
            var scripts = GetScripts();
            foreach (var script in scripts)
                script.Destroy();
        }

        private void Activate()
        {
            var state = Enabled;
            LocalActive = true;
            if (Enabled == state)
                return;

            DoActivateScripts();
        }

        private void Deactivate()
        {
            var state = Enabled;
            LocalActive = false;
            if (Enabled == state)
                return;

            DoDeactivateScripts();
        }

        private IList<ScriptBehaviour> GetScripts()
        {
            return GetComponents<ScriptBehaviour>();
        }

        public TransformComponent Transform { get; }

        public Component GetComponent<T>()
            where T : Component
        {
            return GetComponent(typeof(T));
        }

        public Component GetComponent(Type componentType)
        {
            return Components.Where(c => c.GetType().Is(componentType)).FirstOrDefault();
        }

        public Component AddComponent(string typeName)
        {
            return AddComponent(TypeManager.GetType(typeName));
        }

        public T AddComponent<T>()
            where T : Component
        {
            return (T)AddComponent(typeof(T));
        }

        public Component AddComponent(Type componentType)
        {
            var c = CreateComponent(componentType);
            lock (Components)
                Components.Add(c);
            return c;
        }

        private IList<Component> Components { get; } = new List<Component>();

        public IList<Component> GetComponents() => Components;
        public IList<T> GetComponents<T>()
            where T : Component
        {
            return Components.Where(c => c is T).Cast<T>().ToList();
        }

        private Component CreateComponent(Type componentType)
        {
            if (componentType.Is<IScriptBehaviour>() && !TypeManager.OwnedByDefaultLoadContext(componentType))
            {
                var script = new ScriptBehaviourWrapper(PluginManager.GetScriptBehaviour(componentType));
                if (Enabled)
                {
                    script.Awake();
                    script.Start();
                }
                return script;
            }

            return (Component)Activator.CreateInstance(componentType);
        }

        public void AddComponent(Component component)
        {
            throw new NotImplementedException();
        }
    }

}
