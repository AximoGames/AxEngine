// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Drawing;
using System.Threading;
using Aximo.Engine;
using Aximo.Render;
using OpenTK;
using Xunit;

namespace Aximo.AxDemo
{

    public class Tester : RenderApplicationTests
    {

        public Tester() : base(new RenderApplicationStartup
        {
            WindowTitle = "AxEngineDemo",
            WindowSize = new Vector2i(800, 600),
            WindowBorder = WindowBorder.Fixed,
        })
        {

        }

        [Fact]
        public void test()
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
                RelativeRotation = new Vector3(0, 0, 0.5f).ToQuaternion(),
                RelativeScale = new Vector3(1),
                RelativeTranslation = new Vector3(0, 0, 0.5f),
                Material = mat,
            }));
            //GameContext.AddActor(new BufferActor());
            RenderSingleFrameSync();
            Thread.Sleep(4000);
        }

        [Fact]
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
