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
        public Quaternion Rotation { get; set; } = Quaternion.Identity;
        public Vector3 Scale { get; set; } = Vector3.One;
        public Matrix4 LocalTransform { get; set; }

        public Vector3 WorldPosition { get; private set; }
        public Quaternion WorldRotation { get; private set; } = Quaternion.Identity;
        public Vector3 WorldScale { get; private set; } = Vector3.One;
        public Matrix4 WorldTransform { get; private set; }

        //public Matrix4 GetMatrix()
        //{
        //    //return Matrix4.CreateScale(Scale)
        //    //    * Matrix4.CreateRotationX(Rotate.X * (MathF.PI * 2))
        //    //    * Matrix4.CreateRotationY(Rotate.Y * (MathF.PI * 2))
        //    //    * Matrix4.CreateRotationZ(Rotate.Z * (MathF.PI * 2))
        //    //    * Matrix4.CreateTranslation((new Vector4(Position, 1.0f) * PositionMatrix).Xyz);

        //    return Matrix4.CreateScale(Scale) * Matrix4.CreateTranslation(Position);
        //}

        public void UpdateTransform()
        {
            Matrix4 s, r, t;

            // cache local/world transforms
            s = Matrix4.CreateScale(Scale);
            r = Matrix4.CreateFromQuaternion(Rotation);
            t = Matrix4.CreateTranslation(Position);
            LocalTransform = s * r * t;

            var parent = Parent;
            if (parent != null)
            {
                WorldRotation = parent.WorldRotation * Rotation;
                WorldScale = parent.WorldScale * Scale;
                WorldPosition = parent.WorldPosition + (parent.WorldRotation * (parent.WorldScale * Position));

                s = Matrix4.CreateScale(WorldScale);
                r = Matrix4.CreateFromQuaternion(WorldRotation);
                t = Matrix4.CreateTranslation(WorldPosition);
                WorldTransform = s * r * t;
            }
            else
            {
                //derivedPosition = relativePosition;
                //derivedRotation = relativeRotation;
                //derivedScale = relativeScale;

                WorldTransform = LocalTransform;
            }
        }

    }

}
