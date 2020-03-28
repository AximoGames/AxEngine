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

        private GameMaterial GetMaterial(PipelineType pipelineType, Vector3 color)
        {
            return new GameMaterial()
            {
                DiffuseTexture = GameTexture.GetFromFile("Textures/woodenbox.png"),
                SpecularTexture = GameTexture.GetFromFile("Textures/woodenbox_specular.png"),
                Ambient = 1f,
                ColorBlendMode = MaterialColorBlendMode.Set,
                Color = color,
                PipelineType = pipelineType,
            };
        }

        private Transform GetTransform()
        {
            return new Transform
            {
                Scale = new Vector3(1),
                Rotation = new Vector3(0, 0, 0.5f).ToQuaternion(),
                Translation = new Vector3(0f, 0, 0.5f),
            };
        }

        [Fact]
        public void ForwardBox1()
        {
            GameContext.AddActor(new Actor(new DebugCubeComponent()
            {
                Name = "Box2",
                Transform = GetTransform(),
                Material = GetMaterial(PipelineType.Forward, new Vector3(0, 1, 0)),
            }));
            RenderAndCompare(nameof(ForwardBox1));
        }

        [Fact]
        public void DeferredBox1()
        {
            GameContext.AddActor(new Actor(new DebugCubeComponent()
            {
                Name = "Box2",
                Transform = GetTransform(),
                Material = GetMaterial(PipelineType.Deferred, new Vector3(0, 1, 0)),
            }));
            RenderAndCompare(nameof(DeferredBox1));
        }

    }

}
