// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using OpenToolkit;
using OpenToolkit.Graphics.OpenGL4;
using OpenToolkit.Mathematics;

namespace Aximo.Render
{
    public static class StructHelper
    {
        public static VertexAttribPointerType GetVertexAttribPointerType(Type type)
        {
            if (type == typeof(float))
                return VertexAttribPointerType.Float;
            return VertexAttribPointerType.Float;
        }

        public static int GetFieldSizeOf(Type type)
        {
            if (type == typeof(float))
                return 4;

            return 4;
        }

        public static int GetFieldsOf(Type type)
        {
            if (type == typeof(float))
                return 1;
            if (type == typeof(Vector4))
                return 4;
            if (type == typeof(Vector3))
                return 3;
            if (type == typeof(Vector2))
                return 2;

            return type.GetFields().Length;
        }
    }
}
