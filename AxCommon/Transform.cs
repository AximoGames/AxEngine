// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using OpenTK;

namespace Aximo
{
    public struct Transform
    {
        public Quaternion Rotation;
        public Vector3 Scale;
        public Vector3 Translation;

        public Transform(Quaternion rotation, Vector3 scale, Vector3 translation)
        {
            Rotation = rotation;
            Scale = scale;
            Translation = translation;
        }
    }

}
