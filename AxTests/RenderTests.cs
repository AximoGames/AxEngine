// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Aximo.Engine;
using Aximo.Render;
using OpenTK;
using Xunit;

namespace Aximo.AxTests
{

    public class RenderTests : RenderApplicationTests
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
                    DiffuseTexture = GameTexture.GetFromFile("Textures/woodenbox.png"),
                    SpecularTexture = GameTexture.GetFromFile("Textures/woodenbox_specular.png"),
                    Ambient = test.Ambient,
                    ColorBlendMode = test.DiffuseSource == "Texture" ? MaterialColorBlendMode.None : MaterialColorBlendMode.Set,
                    Color = new Vector3(0, 1, 0),
                    PipelineType = test.Pipeline,
                };

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
            var pipelines = new PipelineType[] { PipelineType.Forward, PipelineType.Deferred };

            foreach (var test in GetTestCombinations())
            {
                foreach (var pipe in pipelines)
                {
                    var newTest = test.Clone<TestCase>();
                    newTest.Pipeline = pipe;
                    newTest.ComparisonName = pipe.ToString();
                    yield return new object[] { newTest };
                }
            }

            foreach (var test in GetTestCombinations())
            {
                var test1 = test.Clone<TestCase>();
                test1.Pipeline = PipelineType.Forward;
                test1.ComparisonName = test1.Pipeline.ToString();

                var test2 = test.Clone<TestCase>();
                test2.Pipeline = PipelineType.Deferred;
                test2.ComparisonName = test2.Pipeline.ToString();

                test1.CompareWith = test2;

                yield return new object[] { test1 };
            }
        }

        public static IEnumerable<TestCase> GetTestCombinations()
        {
            var diffuseSources = new string[] { "Texture", "Color" };
            var ambients = new float[] { 0.0f, 0.5f, 1.0f };
            //var ambients = new float[] { 0.5f, 1.0f };

            foreach (var diffSource in diffuseSources)
            {
                foreach (var ambient in ambients)
                {
                    yield return new TestCase
                    {
                        DiffuseSource = diffSource,
                        Ambient = ambient,
                    };
                }
            }
        }

        public class TestCase : TestCaseBase
        {
            public PipelineType Pipeline;
            public string DiffuseSource; // Texture vs Color
            public float Ambient;

            public override TestCaseBase Clone()
            {
                return new TestCase
                {
                    Pipeline = Pipeline,
                    DiffuseSource = DiffuseSource,
                    Ambient = Ambient,
                    ComparisonName = Pipeline.ToString(),
                };
            }

            public override string ToStringWithoutComparison()
            {
                return $"{DiffuseSource}Ambient{Ambient.ToString("F1")}";
            }
        }

    }

}
