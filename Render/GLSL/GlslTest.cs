// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

//using System.Runtime.InteropServices;

//// WIP / Test!

//#pragma warning disable SA1134 // Attributes should not share line
//#pragma warning disable SA1400 // Access modifier should be declared

//namespace Aximo.Render.GLSL
//{
//    public struct SMaterial
//    {
//        public Vec4 DiffuseColor;
//        public Sampler2D DiffuseMap;
//    }

//    [StructLayout(LayoutKind.Explicit, Pack = 1, Size = 4)]
//    public struct SLight
//    {
//    }

//    public struct LightsArray
//    {
//        public const int MAX_NUM_TOTAL_LIGHTS = 100;
//        [ArraySize(MAX_NUM_TOTAL_LIGHTS)] public SLight Lights;
//    }

//    public abstract class GlslTest : GlslBase
//    {
//        [OutParam] Vec4 FragColor;

//        [Uniform] SMaterial Material;
//        [Uniform] Vec3 ViewPos;
//        [Uniform] Sampler2DArray DirectionalShadowMap;
//        [Uniform] SamplerCubeArray PointShadowMap;
//        [InParam] Vec2 TexCoords;

//        [InParam] Vec3 Normal;
//        [InParam] Vec3 NormalTransposed;
//        [InParam] Vec3 FragPos;

//        [GlslLayout(GlslLayoutType.Std140)] [Uniform] LightsArray Lights;

//        public override void Main()
//        {
//            Vec3 viewDir = Normalize(ViewPos - FragPos);
//            Vec4 matDiffuse = Material.DiffuseColor;
//            matDiffuse = Texture(Material.DiffuseMap, TexCoords) * Material.DiffuseColor;
//        }
//    }
//}
