// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

// WIP / Test!

namespace Aximo.Render.GLSL
{
    public struct Vec2
    {
        public static Vec2 operator +(Vec2 a, Vec2 b) => throw new NotImplementedException();
        public static Vec2 operator -(Vec2 a, Vec2 b) => throw new NotImplementedException();
    }
    public struct Vec3
    {
        public static Vec3 operator +(Vec3 a, Vec3 b) => throw new NotImplementedException();
        public static Vec3 operator -(Vec3 a, Vec3 b) => throw new NotImplementedException();
    }

    public struct Vec4
    {
        public static Vec4 operator +(Vec4 a, Vec4 b) => throw new NotImplementedException();
        public static Vec4 operator -(Vec4 a, Vec4 b) => throw new NotImplementedException();
        public static Vec4 operator *(Vec4 a, Vec4 b) => throw new NotImplementedException();
        public static Vec4 operator /(Vec4 a, Vec4 b) => throw new NotImplementedException();
    }

    public struct Sampler2D { }
    public struct Sampler2DArray { }
    public struct SamplerCubeArray { }
}
