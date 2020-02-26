using System;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Input;
using AxEngine;
using System.IO;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace AxEngine
{
    public class RenderDemo : RenderApplication
    {
        public RenderDemo(RenderApplicationStartup startup) : base(startup)
        {
        }

        protected override void SetupScene()
        {
            ctx.AddObject(new TestObject()
            {
                Name = "Ground",
                Scale = new Vector3(50, 50, 1),
                Position = new Vector3(0f, 0f, -0.5f),
                // RenderShadow = false,
                //PrimaryRenderPipeline = ctx.GetPipeline<ForwardRenderPipeline>(),
            });
            ctx.AddObject(new Grid()
            {
                Name = "Grid",
                ModelMatrix = Matrix4.CreateTranslation(0f, 0f, 0.01f),
                //Debug = true,
            });
            ctx.AddObject(new Grid()
            {
                Name = "Grid",
                ModelMatrix = Matrix4.CreateRotationY((float)Math.PI / 2) * Matrix4.CreateTranslation(-10f, 0f, 0.01f),
                //Debug = true,
            });
            ctx.AddObject(new Grid()
            {
                Name = "Grid",
                ModelMatrix = Matrix4.CreateRotationX((float)Math.PI / 2) * Matrix4.CreateTranslation(0f, 10f, 0.01f),
                //Debug = true,
            });
            ctx.AddObject(new Lines()
            {
                Name = "CenterCross",
                ModelMatrix = Matrix4.CreateScale(2.0f) * Matrix4.CreateTranslation(0f, 0f, 0.02f),
                //Debug = true,
            });

            ctx.AddObject(new LightObject()
            {
                Position = new Vector3(0, 2, 2.5f),
                Name = "MovingLight",
                LightType = LightType.Directional,
                ShadowTextureIndex = 0,
                //Enabled = false,
            });

            ctx.AddObject(new LightObject()
            {
                Position = new Vector3(2f, 0.5f, 3.25f),
                Name = "StaticLight",
                LightType = LightType.Directional,
                ShadowTextureIndex = 1,
            });

            ctx.AddObject(new TestObject()
            {
                Name = "Box1",
                Rotate = new Vector3(0, 0, (float)Math.PI),
                Scale = new Vector3(1),
                Position = new Vector3(0, 0, 0.5f),
                Debug = true,
                // Enabled = false,
            });

            ctx.AddObject(new TestObject()
            {
                Name = "Box2",
                Scale = new Vector3(1),
                Position = new Vector3(1.5f, 1.5f, 0.5f),
                //Debug = true,
                //Enabled = false,
            });

            ctx.AddObject(new TestObject()
            {
                Name = "Box3",
                Scale = new Vector3(1),
                Position = new Vector3(3f, 3f, 0.5f),
                //Debug = true,
                //Enabled = false,
                PrimaryRenderPipeline = ctx.GetPipeline<ForwardRenderPipeline>(),
            });
            ctx.AddObject(new TestObject()
            {
                Name = "Box4",
                Scale = new Vector3(1),
                Position = new Vector3(4f, 3f, 0.5f),
                //Debug = true,
                //Enabled = false,
                //PrimaryRenderPipeline = ctx.GetPipeline<ForwardRenderPipeline>(),
            });

            ctx.AddObject(new TestObject()
            {
                Name = "BoxFar1",
                Scale = new Vector3(1),
                Position = new Vector3(18, 0, 0.5f),
                Debug = true,
                // Enabled = false,
            });
            ctx.AddObject(new TestObject()
            {
                Name = "BoxFar2",
                Scale = new Vector3(1),
                Position = new Vector3(16, 1, 0.5f),
                Debug = true,
                // Enabled = false,
            });
            ctx.AddObject(new TestObject()
            {
                Name = "BoxFar3",
                Scale = new Vector3(1),
                Position = new Vector3(0, 18, 0.5f),
                Debug = true,
                // Enabled = false,
            });
            ctx.AddObject(new TestObject()
            {
                Name = "BoxFar4",
                Scale = new Vector3(1),
                Position = new Vector3(-1, 16, 0.5f),
                Debug = true,
                // Enabled = false,
            });

            // For performance reasons, skybox should rendered as last
            ctx.AddObject(new Skybox()
            {
                Name = "Sky",
                // RenderShadow = false,
            });
        }

    }

}