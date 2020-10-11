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
        public Vector3 Scale { get; set; }

        public Matrix4 GetMatrix()
        {
            //return Matrix4.CreateScale(Scale)
            //    * Matrix4.CreateRotationX(Rotate.X * (MathF.PI * 2))
            //    * Matrix4.CreateRotationY(Rotate.Y * (MathF.PI * 2))
            //    * Matrix4.CreateRotationZ(Rotate.Z * (MathF.PI * 2))
            //    * Matrix4.CreateTranslation((new Vector4(Position, 1.0f) * PositionMatrix).Xyz);

            return Matrix4.CreateScale(Scale) * Matrix4.CreateTranslation(Position);
        }

    }

}
