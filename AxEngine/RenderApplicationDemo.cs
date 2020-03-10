// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Drawing;
using Aximo.Render;
using OpenTK;

namespace Aximo.Engine
{
    public class RenderApplicationDemo : RenderApplication
    {
        public RenderApplicationDemo(RenderApplicationStartup startup) : base(startup)
        {
        }

        //public override IRenderPipeline PrimaryRenderPipeline => ctx.GetPipeline<ForwardRenderPipeline>();

        protected override void SetupScene()
        {
            RenderContext.AddObject(new TestObject()
            {
                Name = "Ground",
                Scale = new Vector3(50, 50, 1),
                Position = new Vector3(0f, 0f, -0.5f),
                // RenderShadow = false,
                //PrimaryRenderPipeline = ctx.GetPipeline<ForwardRenderPipeline>(),
            });
            RenderContext.AddObject(new GridObject()
            {
                Name = "Grid",
                ModelMatrix = Matrix4.CreateTranslation(0f, 0f, 0.01f),
                //Debug = true,
            });
            RenderContext.AddObject(new GridObject()
            {
                Name = "Grid",
                ModelMatrix = Matrix4.CreateRotationY((float)Math.PI / 2) * Matrix4.CreateTranslation(-10f, 0f, 0.01f),
                //Debug = true,
            });
            RenderContext.AddObject(new GridObject()
            {
                Name = "Grid",
                ModelMatrix = Matrix4.CreateRotationX((float)Math.PI / 2) * Matrix4.CreateTranslation(0f, 10f, 0.01f),
                //Debug = true,
            });
            RenderContext.AddObject(new CrossLinesObject()
            {
                Name = "CenterCross",
                ModelMatrix = Matrix4.CreateScale(2.0f) * Matrix4.CreateTranslation(0f, 0f, 0.02f),
                //Debug = true,
            });
            RenderContext.AddObject(new ScreenTextureObject()
            {
                Name = "ScreenTexture1",
                TexturePath = "Textures/woodenbox.png",
                RectanglePixels = new RectangleF(000, 0, 30f, 30f),
            });

            RenderContext.AddObject(new StatsObject()
            {
                Name = "Stats",
                RectanglePixels = new RectangleF(40, 40, 100f, 100f),
            });

            RenderContext.AddObject(new LineObject()
            {
                Name = "DebugLine",
                //Enabled = false,
            });

            RenderContext.AddObject(new LightObject()
            {
                Position = new Vector3(0, 2, 2.5f),
                Name = "MovingLight",
                LightType = LightType.Directional,
                ShadowTextureIndex = 0,
                //Enabled = false,
            });

            RenderContext.AddObject(new LightObject()
            {
                Position = new Vector3(2f, 0.5f, 3.25f),
                Name = "StaticLight",
                LightType = LightType.Directional,
                ShadowTextureIndex = 1,
            });

            RenderContext.AddObject(new TestObject()
            {
                Name = "GroundCursor",
                Position = new Vector3(0, 1, 0.05f),
                Scale = new Vector3(1.0f, 1.0f, 0.1f),
                // Enabled = false,
            });

            RenderContext.AddObject(new TestObject()
            {
                Name = "Box1",
                Rotate = new Vector3(0, 0, 0.5f),
                Scale = new Vector3(1),
                Position = new Vector3(0, 0, 0.5f),
                Debug = true,
                // Enabled = false,
            });

            RenderContext.AddObject(new TestObject()
            {
                Name = "Box2",
                Scale = new Vector3(1),
                Position = new Vector3(1.5f, 1.5f, 0.5f),
                //Debug = true,
                //Enabled = false,
            });

            RenderContext.AddObject(new SphereObject()
            {
                Name = "Sphere1",
                Scale = new Vector3(1),
                Position = new Vector3(3f, 3f, 0.5f),
                //Debug = true,
                //Enabled = false,
                PrimaryRenderPipeline = RenderContext.GetPipeline<ForwardRenderPipeline>(),
            });
            RenderContext.AddObject(new TestObject()
            {
                Name = "Box4",
                Scale = new Vector3(1),
                Position = new Vector3(4f, 3f, 0.5f),
                //Debug = true,
                //Enabled = false,
                //PrimaryRenderPipeline = ctx.GetPipeline<ForwardRenderPipeline>(),
            });

            RenderContext.AddObject(new TestObject()
            {
                Name = "BoxFar1",
                Scale = new Vector3(1),
                Position = new Vector3(18, 0, 0.5f),
                Debug = true,
                // Enabled = false,
            });
            RenderContext.AddObject(new TestObject()
            {
                Name = "BoxFar2",
                Scale = new Vector3(1),
                Position = new Vector3(16, 1, 0.5f),
                Debug = true,
                // Enabled = false,
            });
            RenderContext.AddObject(new TestObject()
            {
                Name = "BoxFar3",
                Scale = new Vector3(1),
                Position = new Vector3(0, 18, 0.5f),
                Debug = true,
                // Enabled = false,
            });
            RenderContext.AddObject(new TestObject()
            {
                Name = "BoxFar4",
                Scale = new Vector3(1),
                Position = new Vector3(-1, 16, 0.5f),
                Debug = true,
                // Enabled = false,
            });

            // For performance reasons, skybox should rendered as last
            RenderContext.AddObject(new SkyboxObject()
            {
                Name = "Sky",
                // RenderShadow = false,
            });
        }

        private double LightAngle = 0;

        protected override void OnRenderFrame(FrameEventArgs e)
        {
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            if (RenderContext.LightObjects.Count > 0)
            {
                LightAngle -= 0.01;
                var pos = new Vector3((float)(Math.Cos(LightAngle) * 2f), (float)(Math.Sin(LightAngle) * 2f), 1.5f);
                ILightObject light = RenderContext.LightObjects[0];

                light.Position = pos;
            }

            if (CurrentMouseWorldPositionIsValid)
            {
                var cursor = RenderContext.GetObjectByName<IPosition>("GroundCursor");
                if (cursor != null)
                    cursor.Position = new Vector3(CurrentMouseWorldPosition.X, CurrentMouseWorldPosition.Y, cursor.Position.Z);
            }

        }

    }

}
