// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using OpenTK;

namespace Aximo
{
    public struct Transform
    {
        public Vector3 Scale;
        public Quaternion Rotation;
        public Vector3 Translation;

        public static readonly Transform Identity = new Transform(Vector3.One, Quaternion.Identity, Vector3.Zero);

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

    }

}
