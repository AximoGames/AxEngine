using System;
using System.Collections.Generic;
using System.Text;
using OpenToolkit.Graphics.OpenGL;
using OpenToolkit.Mathematics;

namespace Aximo.Engine
{

    public class Component
    {
        public Actor Actor { get; }

        public Component GetComponent<T>()
            where T : Component
        {
            return GetComponent(typeof(T));
        }

        public Component GetComponent(Type componentType)
        {
            throw new NotImplementedException();
        }
    }

}
