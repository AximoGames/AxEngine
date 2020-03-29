// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Drawing;
using Aximo.Engine;
using OpenTK;

namespace Aximo.AxDemo
{

    public class RenderApplicationDemo : RenderApplication
    {
        public RenderApplicationDemo(RenderApplicationStartup startup) : base(startup)
        {
        }

        protected override void SetupScene()
        {
            // GameMaterial material = new GameMaterial
            // {
            //     DiffuseTexture = GameTexture.GetFromFile("Textures/woodenbox.png"),
            //     SpecularTexture = GameTexture.GetFromFile("Textures/woodenbox_specular.png"),
            //     Ambient = 0.2f,
            //     Shininess = 32f,
            //     PipelineType = PipelineType.Deferred,
            // };

            // GameContext.AddActor(new Actor(new DebugCubeComponent()
            // {
            //     Name = "Box1",
            //     Transform = new Transform
            //     {
            //         Scale = new Vector3(1),
            //         Rotation = new Vector3(0, 0, 0.5f).ToQuaternion(),
            //         Translation = new Vector3(0f, 0, 0.5f),
            //     },
            //     Material = material,
            // }));

            // GameMaterial material2 = new GameMaterial
            // {
            //     DiffuseTexture = GameTexture.GetFromFile("Textures/woodenbox.png"),
            //     SpecularTexture = GameTexture.GetFromFile("Textures/woodenbox_specular.png"),
            //     Shininess = 32f,
            //     Ambient = 0.2f,
            //     PipelineType = PipelineType.Forward,
            // };

            // GameContext.AddActor(new Actor(new DebugCubeComponent()
            // {
            //     Name = "Box2",
            //     Transform = new Transform
            //     {
            //         Scale = new Vector3(1),
            //         Rotation = new Vector3(0, 0, 0.5f).ToQuaternion(),
            //         Translation = new Vector3(1.0f, 0, 0.5f),
            //     },
            //     Material = material2,
            // }));

            // GameContext.AddActor(new Actor(new DirectionalLightComponent()
            // {
            //     RelativeTranslation = new Vector3(-0.2f, -2.1f, 1.85f),
            //     Name = "StaticLight",
            // }));

            // return;
            //---

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
                    CastShadow = true,
                },
            }));

            GameContext.AddActor(new Actor(new CubeComponent()
            {
                Name = "Ground",
                RelativeScale = new Vector3(50, 50, 1),
                RelativeTranslation = new Vector3(0f, 0f, -0.5f),
            }));

            GameContext.AddActor(new Actor(new GridPlaneComponent(10, true)
            {
                Name = "GridPlaneXY",
                RelativeTranslation = new Vector3(0f, 0f, 0.01f),
            }));
            GameContext.AddActor(new Actor(new GridPlaneComponent(10, true)
            {
                Name = "GridPlaneYZ",
                RelativeTranslation = new Vector3(-10f, 0f, 0.01f),
                RelativeRotation = new Vector3(0, 0.25f, 0).ToQuaternion(),
            }));
            GameContext.AddActor(new Actor(new GridPlaneComponent(10, true)
            {
                Name = "GridPlaneXZ",
                RelativeTranslation = new Vector3(0f, 10f, 0.01f),
                RelativeRotation = new Vector3(0.25f, 0, 0).ToQuaternion(),
            }));
            GameContext.AddActor(new Actor(new CrossLineComponent(10, true)
            {
                Name = "CenterCross",
                RelativeTranslation = new Vector3(0f, 0f, 0.02f),
                RelativeScale = new Vector3(2.0f),
            }));
            GameContext.AddActor(new Actor(new ScreenTextureComponent("Textures/wood.png")
            {
                Name = "ScreenTexture1.1",
                RectanglePixels = new RectangleF(0, 0, 30f, 30f),
            }));

            GameContext.AddActor(new Actor(new StatsComponent()
            {
                Name = "Stats",
                RectanglePixels = new RectangleF(40, 40, 100f, 100f),
            }));

            GameContext.AddActor(new Actor(new LineComponent(new Vector3(0, 0, 0), new Vector3(2, 2, 2))
            {
                Name = "DebugLine",
            }));

            GameContext.AddActor(new Actor(new DirectionalLightComponent()
            {
                RelativeTranslation = new Vector3(0, 2, 2.5f),
                Name = "MovingLight",
            }));
            GameContext.AddActor(new Actor(new DirectionalLightComponent()
            {
                RelativeTranslation = new Vector3(2f, 0.5f, 3.25f),
                Name = "StaticLight",
            }));

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

            GameContext.AddActor(new Actor(new SphereComponent()
            {
                Name = "Sphere1",
                RelativeTranslation = new Vector3(3f, 3f, 0.5f),
            }));
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
            GameContext.AddActor(new Actor(new SkyBoxComponent()
            {
                Name = "Sky",
            }));
        }

        private float LightAngle = 0;

        protected override void OnRenderFrame(FrameEventArgs e)
        {
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            var movingLight = GameContext.GetActor("MovingLight")?.GetComponent<LightComponent>();
            if (movingLight != null)
            {
                LightAngle -= 0.01f;
                var pos = new Vector3((float)(Math.Cos(LightAngle) * 2f), (float)(Math.Sin(LightAngle) * 2f), 1.5f);

                movingLight.RelativeTranslation = pos;
            }

            var actt = GameContext.GetActor("GroupActor1");
            if (actt != null)
            {
                var compp = actt.GetComponent<SceneComponent>("CompGroup1");
                compp.RelativeRotation = new Quaternion(0, 0, LightAngle * 2);
            }

            if (CurrentMouseWorldPositionIsValid)
            {
                var cursor = GameContext.GetActor("GroundCursor")?.RootComponent;
                if (cursor != null)
                    cursor.RelativeTranslation = new Vector3(CurrentMouseWorldPosition.X, CurrentMouseWorldPosition.Y, cursor.RelativeTranslation.Z);
            }

        }

    }

}
