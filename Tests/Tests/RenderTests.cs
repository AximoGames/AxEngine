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

    public class RenderTests : TestBase
    {

        public RenderTests()
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
                GameMaterial material = new GameMaterial
                {
                    SpecularTexture = GameTexture.GetFromFile("Textures/woodenbox_specular.png"),
                    Ambient = test.Ambient,
                    Color = new Vector3(0, 1, 0),
                    PipelineType = test.Pipeline,
                };
                if (test.DiffuseSource == "Texture")
                    material.DiffuseTexture = GameTexture.GetFromFile("Textures/woodenbox.png");

                GameContext.AddActor(new Actor(new DebugCubeComponent()
                {
                    Name = "Box1",
                    Transform = GetTestTransform(),
                    Material = material,
                }));

                RenderAndCompare(nameof(Box) + test.ToString());
            }
        }

        public static IEnumerable<object[]> GetTestData()
        {
            var diffuseSources = new string[] { "Texture", "Color" };
            var ambients = new float[] { 0.0f, 0.5f, 1.0f };

            foreach (var diffSource in diffuseSources)
            {
                foreach (var ambient in ambients)
                {
                    var test = new TestCase
                    {
                        DiffuseSource = diffSource,
                        Ambient = ambient,
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
        }

        public class TestCase : TestCasePipelineBase
        {
            public string DiffuseSource; // Texture vs Color
            public float Ambient;

            protected override TestCaseBase CloneInternal()
            {
                return new TestCase
                {
                    Pipeline = Pipeline,
                    DiffuseSource = DiffuseSource,
                    Ambient = Ambient,
                };
            }

            public override string ToStringWithoutComparison()
            {
                return $"{DiffuseSource}Ambient{Ambient.ToString("F1", SharedLib.LocaleInvariant)}";
            }
        }

    }

}
