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

        public static IEnumerable<object[]> GetData()
        {
            var pipelines = new PipelineType[] { PipelineType.Forward, PipelineType.Deferred };
            var diffuseSources = new string[] { "Texture", "Color" };
            var ambients = new float[] { 0.5f, 1.0f };

            foreach (var pipe in pipelines)
                foreach (var diffSource in diffuseSources)
                    foreach (var ambient in ambients)
                        yield return new object[]
                        { new TestCase
                            {
                                Pipeline = pipe,
                                DiffuseSource = diffSource,
                                Ambient = ambient,
                            }
                        };
        }

        public class TestCase
        {
            public PipelineType Pipeline;
            public string DiffuseSource; // Texture vs Color
            public float Ambient;

            public string ToString()
            {
                return $"{Pipeline}{DiffuseSource}Ambient{Ambient}";
            }
        }

    }

}
