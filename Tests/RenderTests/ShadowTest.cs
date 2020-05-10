// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Aximo.Engine;
using Aximo.Render;
using OpenToolkit;
using OpenToolkit.Mathematics;
using Xunit;

namespace Aximo.AxTests
{
    public class ShadowTypeTests : TestBase
    {
        public ShadowTypeTests()
        {
        }

        [Theory]
        [MemberData(nameof(GetTestData))]
        public void Box(TestCase test)
        {
            if (test.CompareWith != null)
            {
                Compare(nameof(Box), test, test.CompareWith);
            }
            else
            {
                GameMaterial mat1 = new GameMaterial
                {
                    Color = new Vector4(0.5f, 0.5f, 0, 1),
                    Ambient = 0.3f,
                    Shininess = 64.0f,
                    SpecularStrength = 0.3f,
                    PipelineType = test.Pipeline,
                    CastShadow = true,
                };

                var mat2 = new GameMaterial()
                {
                    Color = new Vector4(0.5f, 0, 0.5f, 1),
                    Ambient = 0.3f,
                    Shininess = 64.0f,
                    SpecularStrength = 0.3f,
                    PipelineType = test.Pipeline,
                    CastShadow = true,
                };

                GameContext.AddActor(new Actor(new CubeComponent()
                {
                    Name = "Ground",
                    RelativeScale = new Vector3(50, 50, 1),
                    RelativeTranslation = new Vector3(0f, 0f, -0.5f),
                    Material = mat1,
                }));

                GameContext.AddActor(new Actor(new CubeComponent()
                {
                    Name = "Box",
                    RelativeTranslation = new Vector3(0f, 0f, 0.5f),
                    Material = mat2,
                }));

                switch (test.LightType)
                {
                    case LightType.Point:
                        GameContext.AddActor(new Actor(new PointLightComponent()
                        {
                            RelativeTranslation = new Vector3(1f, 2, 2.5f),
                            Name = "MovingLight",
                        }));
                        break;
                    case LightType.Directional:
                        GameContext.AddActor(new Actor(new DirectionalLightComponent()
                        {
                            RelativeTranslation = new Vector3(1f, 2, 2.5f),
                            Name = "MovingLight",
                        }));
                        break;
                }

                RenderAndCompare(nameof(Box) + test.ToString());
            }
        }

        public static IEnumerable<object[]> GetTestData()
        {
            var lightTypes = new LightType[] { LightType.Point, LightType.Directional };

            foreach (var lightType in lightTypes)
            {
                var test = new TestCase
                {
                    LightType = lightType,
                };

                foreach (var pipe in Pipelines)
                {
                    test.Pipeline = pipe;
                    test.ComparisonName = pipe.ToString();
                    yield return TestDataResult(test);
                }

                foreach (var t in GetComparePipelineTests(test))
                    yield return t;
            }
        }

        public class TestCase : TestCasePipelineBase
        {
            public LightType LightType;

            protected override TestCaseBase CloneInternal()
            {
                return new TestCase
                {
                    Pipeline = Pipeline,
                    LightType = LightType,
                };
            }

            public override string ToStringWithoutComparison()
            {
                return $"{LightType}";
            }
        }
    }
}
