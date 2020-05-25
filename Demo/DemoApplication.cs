// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Aximo.Engine;
using Aximo.Engine.Components.Geometry;
using Aximo.Engine.Components.Lights;
using Aximo.Engine.Components.UI;
using OpenToolkit;
using OpenToolkit.Graphics.ES11;
using OpenToolkit.Mathematics;
using OpenToolkit.Windowing.Common;
using SixLabors.ImageSharp;

namespace Aximo.AxDemo
{
    public class DemoApplication : Application
    {
        protected override void SetupScene()
        {
            // GameMaterial material = new GameMaterial
            // {
            //     DiffuseTexture = GameTexture.GetFromFile("Textures/woodenbox.png"),
            //     SpecularTexture = GameTexture.GetFromFile("Textures/woodenbox_specular.png"),
            //     Ambient = 0.2f,
            //     Shininess = 32f,
            //     PipelineType = PipelineType.Forward,
            //     CastShadow = true,
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
            //     PipelineType = PipelineType.Deferred,
            //     CastShadow = true,
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

            // GameContext.AddActor(new Actor(new PointLightComponent()
            // {
            //     RelativeTranslation = new Vector3(-0.2f, -2.1f, 1.85f),
            //     Name = "StaticLight",
            // }));

            // GameContext.AddActor(new Actor(new CubeComponent()
            // {
            //     Name = "Ground",
            //     RelativeScale = new Vector3(50, 50, 1),
            //     RelativeTranslation = new Vector3(0f, 0f, -0.5f),
            //     Material = material,
            // }));

            // return;
            //---

            //RenderContext.PrimaryRenderPipeline = RenderContext.GetPipeline<ForwardRenderPipeline>();

            var materialWood1 = new Material()
            {
                DiffuseTexture = Texture.GetFromFile("Textures/woodenbox.png"),
                SpecularTexture = Texture.GetFromFile("Textures/woodenbox_specular.png"),
                Ambient = 0.3f,
                Shininess = 32.0f,
                SpecularStrength = 0.5f,
                CastShadow = true,
            };

            var materialWood2 = new Material()
            {
                DiffuseTexture = Texture.GetFromFile("Textures/wood.png"),
                SpecularTexture = Texture.GetFromFile("Textures/woodenbox_specular.png"),
                Ambient = 0.3f,
                Shininess = 32.0f,
                SpecularStrength = 0.5f,
                CastShadow = true,
            };

            SceneContext.AddActor(new Actor(new SphereComponent()
            {
                Name = "CompSphere",
                RelativeTranslation = new Vector3(-3, 0, 0),
                RelativeScale = new Vector3(1.5f),
                Material = materialWood2,
            }));

            SceneContext.AddActor(new Actor(new CubeComponent()
            {
                Name = "Ground",
                RelativeScale = new Vector3(50, 50, 1),
                RelativeTranslation = new Vector3(0f, 0f, -0.5f),
                Material = materialWood1,
            }));

            SceneContext.AddActor(new Actor(new GridPlaneComponent(10, true)
            {
                Name = "GridPlaneXY",
                RelativeTranslation = new Vector3(0f, 0f, 0.01f),
            }));
            SceneContext.AddActor(new Actor(new GridPlaneComponent(10, true)
            {
                Name = "GridPlaneYZ",
                RelativeTranslation = new Vector3(-10f, 0f, 0.01f),
                RelativeRotation = new Vector3(0, 0.25f, 0).ToQuaternion(),
            }));
            SceneContext.AddActor(new Actor(new GridPlaneComponent(10, true)
            {
                Name = "GridPlaneXZ",
                RelativeTranslation = new Vector3(0f, 10f, 0.01f),
                RelativeRotation = new Vector3(0.25f, 0, 0).ToQuaternion(),
            }));
            SceneContext.AddActor(new Actor(new CrossLineComponent(10, true)
            {
                Name = "CenterCross",
                RelativeTranslation = new Vector3(0f, 0f, 0.02f),
                RelativeScale = new Vector3(2.0f),
            }));
            // GameContext.AddActor(new Actor(new UIImage("Textures/wood.png")
            // {
            //     Name = "UI-Image-Alone",
            //     RectanglePixels = new RectangleF(0, 0, 30f, 30f),
            // }));

            // -- UI

            var flowContainer = new UIFlowContainer()
            {
                DefaultChildSizes = new Vector2(0, 50),
                ExtraChildMargin = new UIAnchors(10, 10, 10, 0),
            };
            SceneContext.AddActor(new Actor(flowContainer));

            //flowContainer.AddComponent(new UIImage("Textures/woodenbox_specular.png")
            //{
            //    Name = "ScreenTexture1.1",
            //    //Size = new Vector2(100, 100),
            //    //Margin = new UIAnchors(20, 20, 20, 20),
            //});

            //flowContainer.AddComponent(new UIImage("Textures/woodenbox_specular.png")
            //{
            //    Name = "ScreenTexture1.1",
            //    //Size = new Vector2(100, 100),
            //    //Margin = new UIAnchors(20, 20, 20, 20),
            //});

            //flowContainer.AddComponent(new UIButton()
            //{
            //    Name = "bt1",
            //    //Size = new Vector2(100, 100),
            //    //Margin = new UIAnchors(20, 20, 20, 20),
            //});

            // --

            SceneContext.AddActor(new Actor(new StatsComponent()
            {
                Name = "Stats",
                CustomOrder = 10,
            }));

            SceneContext.AddActor(new Actor(new LineComponent(new Vector3(0, 0, 0), new Vector3(2, 2, 2))
            {
                Name = "DebugLine",
            }));

            SceneContext.AddActor(new Actor(new DirectionalLightComponent()
            {
                RelativeTranslation = new Vector3(0, 2, 2.5f),
                Name = "MovingLight",
            }));
            SceneContext.AddActor(new Actor(new DirectionalLightComponent()
            {
                RelativeTranslation = new Vector3(2f, 0.5f, 3.25f),
                Name = "StaticLight",
            }));

            SceneContext.AddActor(new Actor(new CubeComponent()
            {
                Name = "GroundCursor",
                RelativeTranslation = new Vector3(0, 1, 0.05f),
                RelativeScale = new Vector3(1.0f, 1.0f, 0.1f),
                Material = materialWood1,
            }));

            SceneContext.AddActor(new Actor(new DebugCubeComponent()
            {
                Name = "Box1",
                RelativeRotation = new Vector3(0, 0, 0.5f).ToQuaternion(),
                RelativeScale = new Vector3(1),
                RelativeTranslation = new Vector3(0, 0, 0.5f),
                Material = materialWood1,
            }));

            SceneContext.AddActor(new Actor(new CubeComponent()
            {
                Name = "Box2",
                RelativeScale = new Vector3(1),
                RelativeTranslation = new Vector3(1.5f, 1.5f, 0.5f),
                Material = materialWood1,
            }));

            SceneContext.AddActor(new Actor(new SphereComponent()
            {
                Name = "Sphere1",
                RelativeTranslation = new Vector3(3f, 3f, 0.5f),
                Material = materialWood1,
            }));
            SceneContext.AddActor(new Actor(new CubeComponent()
            {
                Name = "Box4",
                RelativeScale = new Vector3(1),
                RelativeTranslation = new Vector3(4f, 3f, 0.5f),
                Material = materialWood1,
            }));

            SceneContext.AddActor(new Actor(
                new SceneComponent(
                    new CubeComponent
                    {
                        RelativeTranslation = new Vector3(-1, 0, 0),
                        Material = materialWood1,
                    },
                    new SphereComponent
                    {
                        RelativeTranslation = new Vector3(1f, 0, 0),
                        Material = materialWood1,
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

            SceneContext.AddActor(new Actor(new DebugCubeComponent()
            {
                Name = "BoxFar1",
                RelativeScale = new Vector3(1),
                RelativeTranslation = new Vector3(18, 0, 0.5f),
                Material = materialWood1,
            }));
            SceneContext.AddActor(new Actor(new DebugCubeComponent()
            {
                Name = "BoxFar2",
                RelativeScale = new Vector3(1),
                RelativeTranslation = new Vector3(16, 1, 0.5f),
                Material = materialWood1,
                // Enabled = false,
            }));

            // For performance reasons, skybox should rendered as last
            SceneContext.AddActor(new Actor(new SkyBoxComponent()
            {
                Name = "Sky",
            }));

            var materialWoodRemoveTest = new Material()
            {
                DiffuseTexture = Texture.GetFromFile("Textures/woodenbox.png"),
                SpecularTexture = Texture.GetFromFile("Textures/woodenbox_specular.png"),
                Ambient = 0.3f,
                Shininess = 32.0f,
                SpecularStrength = 0.5f,
                CastShadow = true,
            };

            SceneContext.AddActor(new Actor(new DebugCubeComponent()
            {
                Name = "RemoveTets",
                RelativeScale = new Vector3(1),
                RelativeTranslation = new Vector3(18, 0, 0.5f),
                Material = materialWoodRemoveTest,
            }));
            SceneContext.AddActor(new Actor(new DebugCubeComponent()
            {
                Name = "RemoveTets",
                RelativeScale = new Vector3(1),
                RelativeTranslation = new Vector3(18, 0, 0.5f),
                Material = materialWoodRemoveTest,
            }));

            SceneContext.AddTween(LightTween = new Tween2
            {
                TweenFunc = Tween2.Circle(),
                Duration = TimeSpan.FromSeconds(8),
                Repeat = true,
                Enabled = true,
            });
            SceneContext.AddTween(BoxTween = new Tween1
            {
                TweenFunc = Tween1.Linear(AxMath.Norm2Rad),
                Duration = TimeSpan.FromSeconds(6),
                Repeat = true,
                Enabled = true,
            });
        }

        private Tween2 LightTween;
        private Tween1 BoxTween;

        protected override void OnRenderFrame(FrameEventArgs e)
        {
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            var movingLight = SceneContext.GetActor("MovingLight")?.GetComponent<LightComponent>();
            if (movingLight != null)
                movingLight.RelativeTranslation = new Vector3(LightTween.Value.X, LightTween.Value.Y, 1.5f);

            var actt = SceneContext.GetActor("GroupActor1");
            if (actt != null)
            {
                var compp = actt.GetComponent<SceneComponent>("CompGroup1");
                compp.RelativeRotation = new Quaternion(0, 0, BoxTween.Value);
            }

            if (CurrentMouseWorldPositionIsValid)
            {
                var cursor = SceneContext.GetActor("GroundCursor")?.RootComponent;
                if (cursor != null)
                    cursor.RelativeTranslation = new Vector3(CurrentMouseWorldPosition.X, CurrentMouseWorldPosition.Y, cursor.RelativeTranslation.Z);
            }
        }
    }
}
