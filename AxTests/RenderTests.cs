// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Threading;
using Aximo.Engine;
using Aximo.Render;
using OpenTK;
using Xunit;

namespace Aximo.AxDemo
{

    public class RenderTests : RenderApplicationTests
    {

        public RenderTests() : base(new RenderApplicationStartup
        {
            WindowTitle = "AxTests",
            WindowSize = new Vector2i(160, 120),
            WindowBorder = WindowBorder.Fixed,
        })
        {

        }

        [Fact]
        public void ForwardBox1()
        {
            var mat2 = new GameMaterial()
            {
                DiffuseTexture = GameTexture.GetFromFile("Textures/woodenbox.png"),
                SpecularTexture = GameTexture.GetFromFile("Textures/woodenbox_specular.png"),
                Ambient = 1f,
                ColorBlendMode = MaterialColorBlendMode.Set,
                Color = new Vector3(0, 1, 0),
                PipelineType = PipelineType.Forward,
            };

            GameContext.AddActor(new Actor(new DebugCubeComponent()
            {
                Name = "Box2",
                RelativeRotation = new Vector3(0, 0, 0.5f).ToQuaternion(),
                RelativeScale = new Vector3(1),
                RelativeTranslation = new Vector3(0f, 0, 0.5f),

                // RelativeRotation = new Vector3(0, 0, 0.5f).ToQuaternion(),
                // RelativeScale = new Vector3(8, 1, 8),
                // RelativeTranslation = new Vector3(0, 0, 0.5f),

                Material = mat2,
            }));

            RenderAndCompare(nameof(ForwardBox1));

            //RenderSingleFrameSync();
            Thread.Sleep(4000);
        }

        [Fact(Skip = "skip")]
        public void test2()
        {
            var mat = new GameMaterial()
            {
                DiffuseTexture = GameTexture.GetFromFile("Textures/woodenbox.png"),
                SpecularTexture = GameTexture.GetFromFile("Textures/woodenbox_specular.png"),
                Ambient = 1f,
                ColorBlendMode = MaterialColorBlendMode.Set,
                Color = new Vector3(0, 1, 0),
                PipelineType = PipelineType.Forward,
            };

            GameContext.AddActor(new Actor(new DebugCubeComponent()
            {
                Name = "Box1",
                RelativeRotation = new Vector3(0, 0.25f, 0.5f).ToQuaternion(),
                RelativeScale = new Vector3(1),
                RelativeTranslation = new Vector3(1f, 0, 0.5f),
                Material = mat,
            }));
            RenderSingleFrameSync();
            Thread.Sleep(4000);
        }

    }

}
