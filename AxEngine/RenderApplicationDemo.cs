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

        protected override void SetupScene()
        {
            //RenderContext.PrimaryRenderPipeline = RenderContext.GetPipeline<ForwardRenderPipeline>();

            GameContext.AddActor(new Actor(new SphereComponent()
            {
                Name = "CompSphere",
                RelativeTranslation = new Vector3(-1, 0, 0),
                RelativeScale = new Vector3(1.5f),
                Material = new GameMaterial()
                {
                    DiffuseTexture = GameTexture.GetFromFile("Textures/wood.png"),
                    SpecularTexture = GameTexture.GetFromFile("Textures/woodenbox_specular.png"),
                },
            }));

            GameContext.AddActor(new Actor(new CubeComponent()
            {
                Name = "Ground",
                RelativeScale = new Vector3(50, 50, 1),
                RelativeTranslation = new Vector3(0f, 0f, -0.5f),
            }));
            RenderContext.AddObject(new GridObject()
            {
                Name = "Grid",
                ModelMatrix = Matrix4.CreateTranslation(0f, 0f, 0.01f),
            });
            RenderContext.AddObject(new GridObject()
            {
                Name = "Grid",
                ModelMatrix = Matrix4.CreateRotationY((float)Math.PI / 2) * Matrix4.CreateTranslation(-10f, 0f, 0.01f),
            });
            RenderContext.AddObject(new GridObject()
            {
                Name = "Grid",
                ModelMatrix = Matrix4.CreateRotationX((float)Math.PI / 2) * Matrix4.CreateTranslation(0f, 10f, 0.01f),
            });
            RenderContext.AddObject(new CrossLinesObject()
            {
                Name = "CenterCross",
                ModelMatrix = Matrix4.CreateScale(2.0f) * Matrix4.CreateTranslation(0f, 0f, 0.02f),
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
            });

            RenderContext.AddObject(new LightObject()
            {
                Position = new Vector3(0, 2, 2.5f),
                Name = "MovingLight",
                LightType = LightType.Directional,
                ShadowTextureIndex = 0,
            });

            RenderContext.AddObject(new LightObject()
            {
                Position = new Vector3(2f, 0.5f, 3.25f),
                Name = "StaticLight",
                LightType = LightType.Directional,
                ShadowTextureIndex = 1,
            });

            GameContext.AddActor(new Actor(new CubeComponent()
            {
                Name = "GroundCursor",
                RelativeTranslation = new Vector3(0, 1, 0.05f),
                RelativeScale = new Vector3(1.0f, 1.0f, 0.1f),
            }));

            GameContext.AddActor(new Actor(new DebugCubeComponent()
            {
                Name = "Box1",
                RelativeRotation = new Vector3(0, 0, 0.5f).ToQuaternion(),
                RelativeScale = new Vector3(1),
                RelativeTranslation = new Vector3(0, 0, 0.5f),
            }));

            GameContext.AddActor(new Actor(new CubeComponent()
            {
                Name = "Box2",
                RelativeScale = new Vector3(1),
                RelativeTranslation = new Vector3(1.5f, 1.5f, 0.5f),
            }));

            RenderContext.AddObject(new SphereObject()
            {
                Name = "Sphere1",
                Scale = new Vector3(1),
                Position = new Vector3(3f, 3f, 0.5f),
                PrimaryRenderPipeline = RenderContext.GetPipeline<ForwardRenderPipeline>(),
            });
            GameContext.AddActor(new Actor(new CubeComponent()
            {
                Name = "Box4",
                RelativeScale = new Vector3(1),
                RelativeTranslation = new Vector3(4f, 3f, 0.5f),
            }));

            GameContext.AddActor(new Actor(
                new SceneComponent(
                    new CubeComponent
                    {
                        RelativeTranslation = new Vector3(-1, 0, 0),
                    },
                    new SphereComponent
                    {
                        RelativeTranslation = new Vector3(1f, 0, 0),
                    })
                {
                    Name = "CompGroup1",
                    RelativeTranslation = new Vector3(0, 18, 1f),
                    RelativeRotation = new Quaternion(0, 0, MathF.PI / 4),
                    RelativeScale = new Vector3(2f),
                })
            {
                Name = "GroupActor1",
            });

            GameContext.AddActor(new Actor(new DebugCubeComponent()
            {
                Name = "BoxFar1",
                RelativeScale = new Vector3(1),
                RelativeTranslation = new Vector3(18, 0, 0.5f),
            }));
            GameContext.AddActor(new Actor(new DebugCubeComponent()
            {
                Name = "BoxFar2",
                RelativeScale = new Vector3(1),
                RelativeTranslation = new Vector3(16, 1, 0.5f),
                // Enabled = false,
            }));

            // For performance reasons, skybox should rendered as last
            RenderContext.AddObject(new SkyboxObject()
            {
                Name = "Sky",
            });
        }

        private float LightAngle = 0;

        protected override void OnRenderFrame(FrameEventArgs e)
        {
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            if (RenderContext.LightObjects.Count > 0)
            {
                LightAngle -= 0.01f;
                var pos = new Vector3((float)(Math.Cos(LightAngle) * 2f), (float)(Math.Sin(LightAngle) * 2f), 1.5f);
                ILightObject light = RenderContext.LightObjects[0];

                light.Position = pos;
            }

            var actt = GameContext.GetActor("GroupActor1");
            var compp = actt.GetComponent<SceneComponent>("CompGroup1");
            compp.RelativeRotation = new Quaternion(0, 0, LightAngle * 2);

            if (CurrentMouseWorldPositionIsValid)
            {
                var cursor = RenderContext.GetObjectByName<IPosition>("GroundCursor");
                if (cursor != null)
                    cursor.Position = new Vector3(CurrentMouseWorldPosition.X, CurrentMouseWorldPosition.Y, cursor.Position.Z);
            }

        }

    }

}
