using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using OpenToolkit.Graphics.OpenGL;
using OpenToolkit.Mathematics;

namespace Aximo.Engine
{

    public class Component : BaseObject
    {

        public Actor? Actor { get; }

        public Component GetComponent<T>()
            where T : Component
        {
            return GetComponent(typeof(T));
        }

        public Component GetComponent(Type componentType)
        {
            return Actor.GetComponent(componentType);
        }

    }

}
