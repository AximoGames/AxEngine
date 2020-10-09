using System;
using System.Collections.Generic;
using System.Text;
using OpenToolkit.Graphics.OpenGL;
using OpenToolkit.Mathematics;

namespace Aximo.Engine
{
    public class TransformComponent : Component
    {
        public TransformComponent? Parent { get; }
        public TransformComponent? Root { get; }

        public Vector3 Position { get; set; }
        public Quaternion Rotation { get; set; }
    }

}
