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
        [MemberData(nameof(GetData))]
        public void Box(TestCase test)
        {

            if (test.ComparePipelines)
            {
                test.Pipeline = PipelineType.Forward;
                var testName1 = test.ToString();
                test.Pipeline = PipelineType.Deferred;
                var testName2 = test.ToString();

                //...
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

        public static IEnumerable<object[]> GetData()
        {
            var pipelines = new PipelineType[] { PipelineType.Forward, PipelineType.Deferred };

            foreach (var test in GetData_())
            {
                foreach (var pipe in pipelines)
                {
                    var newTest = test.Clone();
                    newTest.Pipeline = pipe;
                    yield return new object[] { newTest };
                }
            }

            foreach (var test in GetData_())
            {
                test.ComparePipelines = true;
                yield return new object[] { test };
            }
        }

        public static IEnumerable<TestCase> GetData_()
        {
            var diffuseSources = new string[] { "Texture", "Color" };
            var ambients = new float[] { 0.5f, 1.0f };

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

        public class TestCase
        {
            public PipelineType Pipeline;
            public string DiffuseSource; // Texture vs Color
            public float Ambient;

            public bool ComparePipelines = false;

            public TestCase Clone()
            {
                return new TestCase
                {
                    Pipeline = Pipeline,
                    DiffuseSource = DiffuseSource,
                    Ambient = Ambient,
                    ComparePipelines = ComparePipelines,
                };
            }

            public override string ToString()
            {
                return $"{Pipeline}{DiffuseSource}Ambient{Ambient}";
            }

        }

    }

}
