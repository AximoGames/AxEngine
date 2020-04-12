// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using OpenToolkit;
using OpenToolkit.Mathematics;

#pragma warning disable SA1003 // Symbols should be spaced correctly
#pragma warning disable SA1137 // Elements should have the same indentation
#pragma warning disable SA1008 // Opening parenthesis should be spaced correctly

namespace Aximo.Render
{
    internal static class DataHelper
    {
        public static VertexDataPosNormalUV[] GetCube()
        {
            var lines = new List<Vector3>();
            lines.Add(new Vector3(0, 0, 1));
            lines.Add(new Vector3(1, 0, 0));
            lines.Add(new Vector3(0, 1, 0));

            var directions = new List<Vector3>();
            foreach (var line in lines)
            {
                directions.Add(line);
                directions.Add(-line);
            }

            var vertices = new List<VertexDataPosNormalUV>();
            foreach (var direction in directions)
            {
                var quad = GetQuad();
                foreach (var vert in quad)
                {
                    var q = Quaternion.FromEulerAngles(direction * MathF.PI / 2);
                    if (direction == -Vector3.UnitY)
                        q = Quaternion.Identity;
                    else if (direction == Vector3.UnitY)
                        q = new Quaternion(0, 0, -1, 0);

                    var v = vert;
                    v.Normal = Vector3.Transform(v.Normal, q);
                    v.Position.Y = -0.5f;
                    v.Position = Vector3.Transform(v.Position, q);
                    vertices.Add(v);
                }
            }

            return vertices.ToArray();
        }

        /// <summary>
        /// Returns a Quad, grounded to XY, facing Z-Up
        /// </summary>
        private static VertexDataPosNormalUV[] GetQuad()
        {
            return new VertexDataPosNormalUV[]
            {
            new VertexDataPosNormalUV(new Vector3(-0.5f, -0.5f, -0.5f),  new Vector3(0.0f, -1.0f,  0.0f),  new Vector2(0.0f, 1.0f)), // Front face
            new VertexDataPosNormalUV(new Vector3( 0.5f, -0.5f, -0.5f),  new Vector3(0.0f, -1.0f,  0.0f),  new Vector2(1.0f, 1.0f)),
            new VertexDataPosNormalUV(new Vector3( 0.5f, -0.5f,  0.5f),  new Vector3(0.0f, -1.0f,  0.0f),  new Vector2(1.0f, 0.0f)),
            new VertexDataPosNormalUV(new Vector3( 0.5f, -0.5f,  0.5f),  new Vector3(0.0f, -1.0f,  0.0f),  new Vector2(1.0f, 0.0f)),
            new VertexDataPosNormalUV(new Vector3(-0.5f, -0.5f,  0.5f),  new Vector3(0.0f, -1.0f,  0.0f),  new Vector2(0.0f, 0.0f)),
            new VertexDataPosNormalUV(new Vector3(-0.5f, -0.5f, -0.5f),  new Vector3(0.0f, -1.0f,  0.0f),  new Vector2(0.0f, 1.0f)),
            };
        }

        public static VertexDataPosNormalUV[] Cube_ => GetCube();

        public static readonly VertexDataPosNormalUV[] Cube =
        {
             // Position          Normal
            new VertexDataPosNormalUV(new Vector3(-0.5f, -0.5f, -0.5f),  new Vector3(0.0f,  0.0f, -1.0f),  new Vector2(0.0f, 0.0f)), // Bottom face
            new VertexDataPosNormalUV(new Vector3( 0.5f,  0.5f, -0.5f),  new Vector3(0.0f,  0.0f, -1.0f),  new Vector2(1.0f, 1.0f)),
            new VertexDataPosNormalUV(new Vector3( 0.5f, -0.5f, -0.5f),  new Vector3(0.0f,  0.0f, -1.0f),  new Vector2(1.0f, 0.0f)),
            new VertexDataPosNormalUV(new Vector3(-0.5f,  0.5f, -0.5f),  new Vector3(0.0f,  0.0f, -1.0f),  new Vector2(0.0f, 1.0f)),
            new VertexDataPosNormalUV(new Vector3( 0.5f,  0.5f, -0.5f),  new Vector3(0.0f,  0.0f, -1.0f),  new Vector2(1.0f, 1.0f)),
            new VertexDataPosNormalUV(new Vector3(-0.5f, -0.5f, -0.5f),  new Vector3(0.0f,  0.0f, -1.0f),  new Vector2(0.0f, 0.0f)),

            new VertexDataPosNormalUV(new Vector3(-0.5f, -0.5f,  0.5f),  new Vector3(0.0f,  0.0f,  1.0f),  new Vector2(0.0f, 1.0f)), // Top face
            new VertexDataPosNormalUV(new Vector3( 0.5f, -0.5f,  0.5f),  new Vector3(0.0f,  0.0f,  1.0f),  new Vector2(1.0f, 1.0f)),
            new VertexDataPosNormalUV(new Vector3( 0.5f,  0.5f,  0.5f),  new Vector3(0.0f,  0.0f,  1.0f),  new Vector2(1.0f, 0.0f)),
            new VertexDataPosNormalUV(new Vector3( 0.5f,  0.5f,  0.5f),  new Vector3(0.0f,  0.0f,  1.0f),  new Vector2(1.0f, 0.0f)),
            new VertexDataPosNormalUV(new Vector3(-0.5f,  0.5f,  0.5f),  new Vector3(0.0f,  0.0f,  1.0f),  new Vector2(0.0f, 0.0f)),
            new VertexDataPosNormalUV(new Vector3(-0.5f, -0.5f,  0.5f),  new Vector3(0.0f,  0.0f,  1.0f),  new Vector2(0.0f, 1.0f)),

            new VertexDataPosNormalUV(new Vector3(-0.5f,  0.5f,  0.5f), -new Vector3(1.0f,  0.0f,  0.0f),  new Vector2(1.0f, 0.0f)), // Left face
            new VertexDataPosNormalUV(new Vector3(-0.5f,  0.5f, -0.5f), -new Vector3(1.0f,  0.0f,  0.0f),  new Vector2(1.0f, 1.0f)),
            new VertexDataPosNormalUV(new Vector3(-0.5f, -0.5f, -0.5f), -new Vector3(1.0f,  0.0f,  0.0f),  new Vector2(0.0f, 1.0f)),
            new VertexDataPosNormalUV(new Vector3(-0.5f, -0.5f, -0.5f), -new Vector3(1.0f,  0.0f,  0.0f),  new Vector2(0.0f, 1.0f)),
            new VertexDataPosNormalUV(new Vector3(-0.5f, -0.5f,  0.5f), -new Vector3(1.0f,  0.0f,  0.0f),  new Vector2(0.0f, 0.0f)),
            new VertexDataPosNormalUV(new Vector3(-0.5f,  0.5f,  0.5f), -new Vector3(1.0f,  0.0f,  0.0f),  new Vector2(1.0f, 0.0f)),

            new VertexDataPosNormalUV(new Vector3( 0.5f, -0.5f, -0.5f),  new Vector3(1.0f,  0.0f,  0.0f),  new Vector2(0.0f, 1.0f)), // Right face
            new VertexDataPosNormalUV(new Vector3( 0.5f,  0.5f, -0.5f),  new Vector3(1.0f,  0.0f,  0.0f),  new Vector2(1.0f, 1.0f)),
            new VertexDataPosNormalUV(new Vector3( 0.5f,  0.5f,  0.5f),  new Vector3(1.0f,  0.0f,  0.0f),  new Vector2(1.0f, 0.0f)),
            new VertexDataPosNormalUV(new Vector3( 0.5f,  0.5f,  0.5f),  new Vector3(1.0f,  0.0f,  0.0f),  new Vector2(1.0f, 0.0f)),
            new VertexDataPosNormalUV(new Vector3( 0.5f, -0.5f,  0.5f),  new Vector3(1.0f,  0.0f,  0.0f),  new Vector2(0.0f, 0.0f)),
            new VertexDataPosNormalUV(new Vector3( 0.5f, -0.5f, -0.5f),  new Vector3(1.0f,  0.0f,  0.0f),  new Vector2(0.0f, 1.0f)),

            new VertexDataPosNormalUV(new Vector3(-0.5f, -0.5f, -0.5f),  new Vector3(0.0f, -1.0f,  0.0f),  new Vector2(0.0f, 1.0f)), // Front face
            new VertexDataPosNormalUV(new Vector3( 0.5f, -0.5f, -0.5f),  new Vector3(0.0f, -1.0f,  0.0f),  new Vector2(1.0f, 1.0f)),
            new VertexDataPosNormalUV(new Vector3( 0.5f, -0.5f,  0.5f),  new Vector3(0.0f, -1.0f,  0.0f),  new Vector2(1.0f, 0.0f)),
            new VertexDataPosNormalUV(new Vector3( 0.5f, -0.5f,  0.5f),  new Vector3(0.0f, -1.0f,  0.0f),  new Vector2(1.0f, 0.0f)),
            new VertexDataPosNormalUV(new Vector3(-0.5f, -0.5f,  0.5f),  new Vector3(0.0f, -1.0f,  0.0f),  new Vector2(0.0f, 0.0f)),
            new VertexDataPosNormalUV(new Vector3(-0.5f, -0.5f, -0.5f),  new Vector3(0.0f, -1.0f,  0.0f),  new Vector2(0.0f, 1.0f)),

            new VertexDataPosNormalUV(new Vector3( 0.5f,  0.5f, -0.5f),  new Vector3(0.0f,  1.0f,  0.0f),  new Vector2(1.0f, 1.0f)), // Back face
            new VertexDataPosNormalUV(new Vector3(-0.5f,  0.5f, -0.5f),  new Vector3(0.0f,  1.0f,  0.0f),  new Vector2(0.0f, 1.0f)),
            new VertexDataPosNormalUV(new Vector3( 0.5f,  0.5f,  0.5f),  new Vector3(0.0f,  1.0f,  0.0f),  new Vector2(1.0f, 0.0f)),
            new VertexDataPosNormalUV(new Vector3(-0.5f,  0.5f,  0.5f),  new Vector3(0.0f,  1.0f,  0.0f),  new Vector2(0.0f, 0.0f)),
            new VertexDataPosNormalUV(new Vector3( 0.5f,  0.5f,  0.5f),  new Vector3(0.0f,  1.0f,  0.0f),  new Vector2(1.0f, 0.0f)),
            new VertexDataPosNormalUV(new Vector3(-0.5f,  0.5f, -0.5f),  new Vector3(0.0f,  1.0f,  0.0f),  new Vector2(0.0f, 1.0f)),
        };

        private const float DX = -0.5f;
        private const float DY = -0.5f;
        private const float DZ = 0.7f;

        private const float EX = 0.7f;
        private const float EY = -0.5f;
        private const float EZ = 0.5f;
        public static readonly float[] DebugCube =
        {
             // Position          Normal
            -0.5f, -0.5f, -0.5f,  0.0f,  0.0f, -1.0f,  0.0f, 0.0f, // Bottom face
             0.5f,  0.5f, -0.5f,  0.0f,  0.0f, -1.0f,  1.0f, 1.0f,
             0.5f, -0.5f, -0.5f,  0.0f,  0.0f, -1.0f,  1.0f, 0.0f,
            -0.5f,  0.5f, -0.5f,  0.0f,  0.0f, -1.0f,  0.0f, 1.0f,
             0.5f,  0.5f, -0.5f,  0.0f,  0.0f, -1.0f,  1.0f, 1.0f,
            -0.5f, -0.5f, -0.5f,  0.0f,  0.0f, -1.0f,  0.0f, 0.0f,

            -0.6f, -0.6f,  0.5f,  0.0f,  0.0f,  1.0f,  0.0f, 1.0f, // Top face
             0.6f, -0.6f,  0.5f,  0.0f,  0.0f,  1.0f,  1.0f, 1.0f,
             0.6f,  0.6f,  0.5f,  0.0f,  0.0f,  1.0f,  1.0f, 0.0f,
             0.6f,  0.6f,  0.5f,  0.0f,  0.0f,  1.0f,  1.0f, 0.0f,
            -0.6f,  0.6f,  0.5f,  0.0f,  0.0f,  1.0f,  0.0f, 0.0f,
            -0.6f, -0.6f,  0.5f,  0.0f,  0.0f,  1.0f,  0.0f, 1.0f,

            -0.5f,  0.5f,  0.5f, -1.0f,  0.0f,  0.0f,  1.0f, 0.0f, // Left face
            -0.5f,  0.5f, -0.5f, -1.0f,  0.0f,  0.0f,  1.0f, 1.0f,
            -0.5f, -0.5f, -0.5f, -1.0f,  0.0f,  0.0f,  0.0f, 1.0f,
            -0.5f, -0.5f, -0.5f, -1.0f,  0.0f,  0.0f,  0.0f, 1.0f,
            -0.5f, -0.5f,  0.5f, -1.0f,  0.0f,  0.0f,  0.0f, 0.0f,
            -0.5f,  0.5f,  0.5f, -1.0f,  0.0f,  0.0f,  1.0f, 0.0f,

             0.5f, -0.5f, -0.5f,  1.0f,  0.0f,  0.0f,  0.0f, 1.0f, // Right face
             0.5f,  0.5f, -0.5f,  1.0f,  0.0f,  0.0f,  1.0f, 1.0f,
             0.5f,  0.5f,  0.5f,  1.0f,  0.0f,  0.0f,  1.0f, 0.0f,
             0.5f,  0.5f,  0.5f,  1.0f,  0.0f,  0.0f,  1.0f, 0.0f,
             0.5f, -0.5f,  0.5f,  1.0f,  0.0f,  0.0f,  0.0f, 0.0f,
             0.5f, -0.5f, -0.5f,  1.0f,  0.0f,  0.0f,  0.0f, 1.0f,

            -0.5f, -0.5f, -0.5f,  0.0f, -1.0f,  0.0f,  0.0f, 1.0f, // Front face
             0.5f, -0.5f, -0.5f,  0.0f, -1.0f,  0.0f,  1.0f, 1.0f,
             0.5f, -0.5f,  0.5f,  0.0f, -1.0f,  0.0f,  1.0f, 0.0f,
             0.5f, -0.5f,  0.5f,  0.0f, -1.0f,  0.0f,  1.0f, 0.0f,
            -0.5f, -0.5f,  0.5f,  0.0f, -1.0f,  0.0f,  0.0f, 0.0f,
            -0.5f, -0.5f, -0.5f,  0.0f, -1.0f,  0.0f,  0.0f, 1.0f,

             0.5f,  0.5f, -0.5f,  0.0f,  1.0f,  0.0f,  1.0f, 1.0f, // Back face
            -0.5f,  0.5f, -0.5f,  0.0f,  1.0f,  0.0f,  0.0f, 1.0f,
             0.5f,  0.5f,  0.5f,  0.0f,  1.0f,  0.0f,  1.0f, 0.0f,
            -0.5f,  0.5f,  0.5f,  0.0f,  1.0f,  0.0f,  0.0f, 0.0f,
             0.5f,  0.5f,  0.5f,  0.0f,  1.0f,  0.0f,  1.0f, 0.0f,
            -0.5f,  0.5f, -0.5f,  0.0f,  1.0f,  0.0f,  0.0f, 1.0f,

//--
            -0.1f+DX, -0.1f+DY, -0.3f+DZ,  0.0f,  0.0f, -1.0f,  0.0f, 0.0f, // Bottom face
             0.1f+DX,  0.1f+DY, -0.3f+DZ,  0.0f,  0.0f, -1.0f,  1.0f, 1.0f,
             0.1f+DX, -0.1f+DY, -0.3f+DZ,  0.0f,  0.0f, -1.0f,  1.0f, 0.0f,
            -0.1f+DX,  0.1f+DY, -0.3f+DZ,  0.0f,  0.0f, -1.0f,  0.0f, 1.0f,
             0.1f+DX,  0.1f+DY, -0.3f+DZ,  0.0f,  0.0f, -1.0f,  1.0f, 1.0f,
            -0.1f+DX, -0.1f+DY, -0.3f+DZ,  0.0f,  0.0f, -1.0f,  0.0f, 0.0f,

            -0.1f+DX, -0.1f+DY,  0.3f+DZ,  0.0f,  0.0f,  1.0f,  0.0f, 1.0f, // Top face
             0.1f+DX, -0.1f+DY,  0.3f+DZ,  0.0f,  0.0f,  1.0f,  1.0f, 1.0f,
             0.1f+DX,  0.1f+DY,  0.3f+DZ,  0.0f,  0.0f,  1.0f,  1.0f, 0.0f,
             0.1f+DX,  0.1f+DY,  0.3f+DZ,  0.0f,  0.0f,  1.0f,  1.0f, 0.0f,
            -0.1f+DX,  0.1f+DY,  0.3f+DZ,  0.0f,  0.0f,  1.0f,  0.0f, 0.0f,
            -0.1f+DX, -0.1f+DY,  0.3f+DZ,  0.0f,  0.0f,  1.0f,  0.0f, 1.0f,

            -0.1f+DX,  0.1f+DY,  0.3f+DZ, -1.0f,  0.0f,  0.0f,  1.0f, 0.0f, // Left face
            -0.1f+DX,  0.1f+DY, -0.3f+DZ, -1.0f,  0.0f,  0.0f,  1.0f, 1.0f,
            -0.1f+DX, -0.1f+DY, -0.3f+DZ, -1.0f,  0.0f,  0.0f,  0.0f, 1.0f,
            -0.1f+DX, -0.1f+DY, -0.3f+DZ, -1.0f,  0.0f,  0.0f,  0.0f, 1.0f,
            -0.1f+DX, -0.1f+DY,  0.3f+DZ, -1.0f,  0.0f,  0.0f,  0.0f, 0.0f,
            -0.1f+DX,  0.1f+DY,  0.3f+DZ, -1.0f,  0.0f,  0.0f,  1.0f, 0.0f,

             0.1f+DX, -0.1f+DY, -0.3f+DZ,  1.0f,  0.0f,  0.0f,  0.0f, 1.0f, // Right face
             0.1f+DX,  0.1f+DY, -0.3f+DZ,  1.0f,  0.0f,  0.0f,  1.0f, 1.0f,
             0.1f+DX,  0.1f+DY,  0.3f+DZ,  1.0f,  0.0f,  0.0f,  1.0f, 0.0f,
             0.1f+DX,  0.1f+DY,  0.3f+DZ,  1.0f,  0.0f,  0.0f,  1.0f, 0.0f,
             0.1f+DX, -0.1f+DY,  0.3f+DZ,  1.0f,  0.0f,  0.0f,  0.0f, 0.0f,
             0.1f+DX, -0.1f+DY, -0.3f+DZ,  1.0f,  0.0f,  0.0f,  0.0f, 1.0f,

            -0.1f+DX, -0.1f+DY, -0.3f+DZ,  0.0f, -1.0f,  0.0f,  0.0f, 1.0f, // Front face
             0.1f+DX, -0.1f+DY, -0.3f+DZ,  0.0f, -1.0f,  0.0f,  1.0f, 1.0f,
             0.1f+DX, -0.1f+DY,  0.3f+DZ,  0.0f, -1.0f,  0.0f,  1.0f, 0.0f,
             0.1f+DX, -0.1f+DY,  0.3f+DZ,  0.0f, -1.0f,  0.0f,  1.0f, 0.0f,
            -0.1f+DX, -0.1f+DY,  0.3f+DZ,  0.0f, -1.0f,  0.0f,  0.0f, 0.0f,
            -0.1f+DX, -0.1f+DY, -0.3f+DZ,  0.0f, -1.0f,  0.0f,  0.0f, 1.0f,

             0.1f+DX,  0.1f+DY, -0.3f+DZ,  0.0f,  1.0f,  0.0f,  1.0f, 1.0f, // Back face
            -0.1f+DX,  0.1f+DY, -0.3f+DZ,  0.0f,  1.0f,  0.0f,  0.0f, 1.0f,
             0.1f+DX,  0.1f+DY,  0.3f+DZ,  0.0f,  1.0f,  0.0f,  1.0f, 0.0f,
            -0.1f+DX,  0.1f+DY,  0.3f+DZ,  0.0f,  1.0f,  0.0f,  0.0f, 0.0f,
             0.1f+DX,  0.1f+DY,  0.3f+DZ,  0.0f,  1.0f,  0.0f,  1.0f, 0.0f,
            -0.1f+DX,  0.1f+DY, -0.3f+DZ,  0.0f,  1.0f,  0.0f,  0.0f, 1.0f,
//--
            -0.3f+EX, -0.1f+EY, -0.1f+EZ,  0.0f,  0.0f, -1.0f,  0.0f, 0.0f, // Bottom face
             0.3f+EX,  0.1f+EY, -0.1f+EZ,  0.0f,  0.0f, -1.0f,  1.0f, 1.0f,
             0.3f+EX, -0.1f+EY, -0.1f+EZ,  0.0f,  0.0f, -1.0f,  1.0f, 0.0f,
            -0.3f+EX,  0.1f+EY, -0.1f+EZ,  0.0f,  0.0f, -1.0f,  0.0f, 1.0f,
             0.3f+EX,  0.1f+EY, -0.1f+EZ,  0.0f,  0.0f, -1.0f,  1.0f, 1.0f,
            -0.3f+EX, -0.1f+EY, -0.1f+EZ,  0.0f,  0.0f, -1.0f,  0.0f, 0.0f,

            -0.3f+EX, -0.1f+EY,  0.1f+EZ,  0.0f,  0.0f,  1.0f,  0.0f, 1.0f, // Top face
             0.3f+EX, -0.1f+EY,  0.1f+EZ,  0.0f,  0.0f,  1.0f,  1.0f, 1.0f,
             0.3f+EX,  0.1f+EY,  0.1f+EZ,  0.0f,  0.0f,  1.0f,  1.0f, 0.0f,
             0.3f+EX,  0.1f+EY,  0.1f+EZ,  0.0f,  0.0f,  1.0f,  1.0f, 0.0f,
            -0.3f+EX,  0.1f+EY,  0.1f+EZ,  0.0f,  0.0f,  1.0f,  0.0f, 0.0f,
            -0.3f+EX, -0.1f+EY,  0.1f+EZ,  0.0f,  0.0f,  1.0f,  0.0f, 1.0f,

            -0.3f+EX,  0.1f+EY,  0.1f+EZ, -1.0f,  0.0f,  0.0f,  1.0f, 0.0f, // Left face
            -0.3f+EX,  0.1f+EY, -0.1f+EZ, -1.0f,  0.0f,  0.0f,  1.0f, 1.0f,
            -0.3f+EX, -0.1f+EY, -0.1f+EZ, -1.0f,  0.0f,  0.0f,  0.0f, 1.0f,
            -0.3f+EX, -0.1f+EY, -0.1f+EZ, -1.0f,  0.0f,  0.0f,  0.0f, 1.0f,
            -0.3f+EX, -0.1f+EY,  0.1f+EZ, -1.0f,  0.0f,  0.0f,  0.0f, 0.0f,
            -0.3f+EX,  0.1f+EY,  0.1f+EZ, -1.0f,  0.0f,  0.0f,  1.0f, 0.0f,

             0.3f+EX, -0.1f+EY, -0.1f+EZ,  1.0f,  0.0f,  0.0f,  0.0f, 1.0f, // Right face
             0.3f+EX,  0.1f+EY, -0.1f+EZ,  1.0f,  0.0f,  0.0f,  1.0f, 1.0f,
             0.3f+EX,  0.1f+EY,  0.1f+EZ,  1.0f,  0.0f,  0.0f,  1.0f, 0.0f,
             0.3f+EX,  0.1f+EY,  0.1f+EZ,  1.0f,  0.0f,  0.0f,  1.0f, 0.0f,
             0.3f+EX, -0.1f+EY,  0.1f+EZ,  1.0f,  0.0f,  0.0f,  0.0f, 0.0f,
             0.3f+EX, -0.1f+EY, -0.1f+EZ,  1.0f,  0.0f,  0.0f,  0.0f, 1.0f,

            -0.3f+EX, -0.1f+EY, -0.1f+EZ,  0.0f, -1.0f,  0.0f,  0.0f, 1.0f, // Front face
             0.3f+EX, -0.1f+EY, -0.1f+EZ,  0.0f, -1.0f,  0.0f,  1.0f, 1.0f,
             0.3f+EX, -0.1f+EY,  0.1f+EZ,  0.0f, -1.0f,  0.0f,  1.0f, 0.0f,
             0.3f+EX, -0.1f+EY,  0.1f+EZ,  0.0f, -1.0f,  0.0f,  1.0f, 0.0f,
            -0.3f+EX, -0.1f+EY,  0.1f+EZ,  0.0f, -1.0f,  0.0f,  0.0f, 0.0f,
            -0.3f+EX, -0.1f+EY, -0.1f+EZ,  0.0f, -1.0f,  0.0f,  0.0f, 1.0f,

             0.3f+EX,  0.1f+EY, -0.1f+EZ,  0.0f,  1.0f,  0.0f,  1.0f, 1.0f, // Back face
            -0.3f+EX,  0.1f+EY, -0.1f+EZ,  0.0f,  1.0f,  0.0f,  0.0f, 1.0f,
             0.3f+EX,  0.1f+EY,  0.1f+EZ,  0.0f,  1.0f,  0.0f,  1.0f, 0.0f,
            -0.3f+EX,  0.1f+EY,  0.1f+EZ,  0.0f,  1.0f,  0.0f,  0.0f, 0.0f,
             0.3f+EX,  0.1f+EY,  0.1f+EZ,  0.0f,  1.0f,  0.0f,  1.0f, 0.0f,
            -0.3f+EX,  0.1f+EY, -0.1f+EZ,  0.0f,  1.0f,  0.0f,  0.0f, 1.0f,
        };

        // Here we now have added the normals of the vertices
        // Remember to define the layouts to the VAO's
        public static readonly float[] Quad =
        {
            // vertex attributes for a quad that fills the entire screen in Normalized Device Coordinates.
            // positions   // texCoords
            -1.0f,  1.0f,  0.0f, 1.0f,
            -1.0f, -1.0f,  0.0f, 0.0f,
             1.0f, -1.0f,  1.0f, 0.0f,

            -1.0f,  1.0f,  0.0f, 1.0f,
             1.0f, -1.0f,  1.0f, 0.0f,
             1.0f,  1.0f,  1.0f, 1.0f,
        };

        public static void GetData<T>(BufferData2D<T> target, Action<IntPtr> getPixels)
        {
            var handle = target.CreateHandle();
            try
            {
                getPixels(handle.AddrOfPinnedObject());
            }
            finally
            {
                handle.Free();
            }
        }

        public static void GetDepthData(BufferData2D<float> target, Action<IntPtr> getPixels)
        {
            GetData(target, getPixels);
        }

        public static Matrix4 CoordinateSystemMatrix = new Matrix4(
            new Vector4(1, 0, 0, 0),
            new Vector4(0, 0, 1, 0),
            new Vector4(0, -1, 0, 0),
            new Vector4(0, 0, 0, 1));

        public static Matrix3 CoordinateSystemMatrix3 = new Matrix3(
            new Vector3(1, 0, 0),
            new Vector3(0, 0, 1),
            new Vector3(0, -1, 0));

        //private const float LineGrayTone = 0.65f;
        public static readonly float[] Cross =
        {
             // Position          Color
             -1f, 0f, 0.0f,  0.5f, 0f, 0f,  1.0f, // Line X
             1.0f,  0.0f, 0.0f,  1f, 0, 0,  1.0f,

             0f, -1f, 0.0f,  0f, 0.5f, 0f,  1.0f, // Line Y
             0.0f,  1.0f, 0.0f,  0f, 1f, 0f,  1.0f,

             0f, 0f, -1.0f,  0f, 0f, 0.5f,  1.0f, // Line Z
             0.0f,  0.0f, 1.0f,  0f, 0f, 1f,  1.0f,
        };

        public static readonly float[] Line =
        {
             // Position          Color
             -1.0f,  0.0f, 0.0f,  0.6f, 0.05f, 0.6f, 1.0f, // X-Aligned line
              1.0f,  0.0f, 0.0f,  1.0f, 0, 0f, 1.0f, 1.0f,
        };

        public static readonly float[] SkyBox =
        {
            // positions
            -1.0f, -1.0f,  1.0f, // Left
            -1.0f, -1.0f, -1.0f,
            -1.0f,  1.0f, -1.0f,
            -1.0f,  1.0f, -1.0f,
            -1.0f,  1.0f,  1.0f,
            -1.0f, -1.0f,  1.0f,

            1.0f, -1.0f, -1.0f, // Right
            1.0f, -1.0f,  1.0f,
            1.0f,  1.0f,  1.0f,
            1.0f,  1.0f,  1.0f,
            1.0f,  1.0f, -1.0f,
            1.0f, -1.0f, -1.0f,

            -1.0f, -1.0f,  1.0f, // Top
            -1.0f,  1.0f,  1.0f,
            1.0f,  1.0f,  1.0f,
            1.0f,  1.0f,  1.0f,
            1.0f, -1.0f,  1.0f,
            -1.0f, -1.0f,  1.0f,

            -1.0f,  1.0f, -1.0f, // Bottom
            -1.0f, -1.0f, -1.0f,
            1.0f, -1.0f, -1.0f,
            1.0f, -1.0f, -1.0f,
            1.0f,  1.0f, -1.0f,
            -1.0f,  1.0f, -1.0f,

            -1.0f,  1.0f, -1.0f, // Front
            1.0f,  1.0f, -1.0f,
            1.0f,  1.0f,  1.0f,
            1.0f,  1.0f,  1.0f,
            -1.0f,  1.0f,  1.0f,
            -1.0f,  1.0f, -1.0f,

            -1.0f, -1.0f, -1.0f, // Back
            -1.0f, -1.0f,  1.0f,
            1.0f, -1.0f, -1.0f,
            1.0f, -1.0f, -1.0f,
            -1.0f, -1.0f,  1.0f,
            1.0f, -1.0f,  1.0f,
        };
    }
}
