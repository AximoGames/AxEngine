// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using OpenToolkit;
using OpenToolkit.Mathematics;

namespace Aximo
{
    public struct Transform
    {
        public Vector3 Scale;
        public Quaternion Rotation;
        public Vector3 Translation;

        public static readonly Transform Identity = new Transform(Vector3.One, Quaternion.Identity, Vector3.Zero);
        //public static readonly Transform One = new Transform(Vector3.One, Quaternion.Identity, Vector3.One);

        public Transform(Vector3 scale, Quaternion rotation, Vector3 translation)
        {
            Scale = scale;
            Rotation = rotation;
            Translation = translation;
        }

        public Matrix4 GetMatrix()
        {
            return Matrix4.CreateScale(Scale) * Matrix4.CreateFromQuaternion(Rotation) * Matrix4.CreateTranslation(Translation);
        }

        public static Transform CreateScale(Vector3 scale)
        {
            return new Transform(scale, Quaternion.Identity, Vector3.Zero);
        }

        public static Transform CreateScale(float x, float y, float z)
        {
            return new Transform(new Vector3(x, y, z), Quaternion.Identity, Vector3.Zero);
        }

        public static Transform CreateScale(float scale)
        {
            return new Transform(new Vector3(scale), Quaternion.Identity, Vector3.Zero);
        }
    }
}
