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
            return GetCube(Vector3.One, Vector3.Zero);
        }

        public static VertexDataPosNormalUV[] GetCube(Vector3 size, Vector3 translate)
        {
            var lines = new List<Vector3>();
            lines.Add(Vector3.UnitX);
            lines.Add(Vector3.UnitY);
            lines.Add(Vector3.UnitZ);

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

                Translate(quad, new Vector3(0, 0, 1f));

                Rotate(quad, new Rotor3(Vector3.UnitZ, -Vector3.UnitY));

                Rotor3 r;
                if (direction == -Vector3.UnitY)
                    r = Rotor3.Identity;
                else if (direction == Vector3.UnitY)
                    r = new Rotor3(0, 1, 0, 0);
                else
                    r = new Rotor3(-Vector3.UnitY, direction);

                Rotate(quad, r);
                Scale(quad, new Vector3(0.5f) * size);
                if (translate != Vector3.Zero)
                    Translate(quad, translate);

                RoundSmooth(quad);

                vertices.AddRange(quad);
                //break;
            }

            return vertices.ToArray();
        }

        public static VertexDataPosNormalUV[] GetDebugCube()
        {
            var list = new List<VertexDataPosNormalUV>();

            const float DX = -0.5f;
            const float DY = -0.5f;
            const float DZ = 0.7f;

            const float EX = 0.7f;
            const float EY = -0.5f;
            const float EZ = 0.5f;

            var baseCube = GetCube();
            for (var i = 24; i < 30; i++)
                baseCube[i].Position = baseCube[i].Position * new Vector3(1.2f, 1.2f, 1f);

            list.AddRange(baseCube);
            list.AddRange(GetCube(new Vector3(0.2f, 0.2f, 0.6f), new Vector3(DX, DY, DZ)));
            list.AddRange(GetCube(new Vector3(0.6f, 0.2f, 0.2f), new Vector3(EX, EY, EZ)));
            //list.AddRange(GetCube());


            //for (var i = 0; i < list.Count; i++)
            //{
            //    if (list[i].Position != DebugCube_[i].Position)
            //    {
            //        var s = "";
            //    }
            //}

            return list.ToArray();
        }

        private static void Rotate(VertexDataPosNormalUV[] vertices, Quaternion q)
        {
            for (var i = 0; i < vertices.Length; i++)
            {
                var v = vertices[i];
                v.Normal = Vector3.Transform(v.Normal, q);
                v.Position = Vector3.Transform(v.Position, q);
                vertices[i] = v;
            }
        }

        private static void Rotate(VertexDataPosNormalUV[] vertices, Rotor3 q)
        {
            for (var i = 0; i < vertices.Length; i++)
            {
                //var r = new Rotor3(Vector3.UnitZ, -Vector3.UnitY);
                ////r.YZ = -1;
                //var pos = Vector3.UnitZ;
                //var newPos = Rotor3.Rotate(r, pos);

                var v = vertices[i];
                v.Normal = Rotor3.Rotate(q, v.Normal);
                v.Position = Rotor3.Rotate(q, v.Position);
                vertices[i] = v;
            }
        }

        private static void Round(VertexDataPosNormalUV[] vertices, int digits)
        {
            for (var i = 0; i < vertices.Length; i++)
            {
                var v = vertices[i];
                v.Normal = v.Normal.Round(digits);
                v.Position = v.Position.Round(digits);
                vertices[i] = v;
            }
        }

        private static void RoundSmooth(VertexDataPosNormalUV[] vertices)
        {
            Round(vertices, 6);
        }

        private static void Scale(VertexDataPosNormalUV[] vertices, Vector3 scale)
        {
            for (var i = 0; i < vertices.Length; i++)
                vertices[i].Position *= scale;
        }

        private static void Translate(VertexDataPosNormalUV[] vertices, Vector3 value)
        {
            for (var i = 0; i < vertices.Length; i++)
                vertices[i].Position += value;
        }

        /// <summary>
        /// Returns a Quad, grounded to XY, facing Z-Up
        /// </summary>
        private static VertexDataPosNormalUV[] GetQuad()
        {
            return new VertexDataPosNormalUV[]
            {
                new VertexDataPosNormalUV(new Vector3(-1f, -1f,  0.0f),  Vector3.UnitZ,  new Vector2(0.0f, 1.0f)), // Top face
                new VertexDataPosNormalUV(new Vector3( 1f, -1f,  0.0f),  Vector3.UnitZ,  new Vector2(1.0f, 1.0f)),
                new VertexDataPosNormalUV(new Vector3( 1f,  1f,  0.0f),  Vector3.UnitZ,  new Vector2(1.0f, 0.0f)),
                new VertexDataPosNormalUV(new Vector3( 1f,  1f,  0.0f),  Vector3.UnitZ,  new Vector2(1.0f, 0.0f)),
                new VertexDataPosNormalUV(new Vector3(-1f,  1f,  0.0f),  Vector3.UnitZ,  new Vector2(0.0f, 0.0f)),
                new VertexDataPosNormalUV(new Vector3(-1f, -1f,  0.0f),  Vector3.UnitZ,  new Vector2(0.0f, 1.0f)),
            };
        }

        public static VertexDataPosNormalUV[] DefaultCube => GetCube();

        private const float DX = -0.5f;
        private const float DY = -0.5f;
        private const float DZ = 0.7f;

        private const float EX = 0.7f;
        private const float EY = -0.5f;
        private const float EZ = 0.5f;

        public static VertexDataPosNormalUV[] DebugCube => GetDebugCube();

        public static readonly VertexDataPosNormalUV[] DebugCube_ =
        {
             // Position          Normal
            new VertexDataPosNormalUV(new Vector3(-0.5f, -0.5f, -0.5f), new Vector3(0.0f,  0.0f, -1.0f), new Vector2(0.0f, 0.0f)), // Bottom face
            new VertexDataPosNormalUV(new Vector3( 0.5f,  0.5f, -0.5f), new Vector3(0.0f,  0.0f, -1.0f), new Vector2(1.0f, 1.0f)),
            new VertexDataPosNormalUV(new Vector3( 0.5f, -0.5f, -0.5f), new Vector3(0.0f,  0.0f, -1.0f), new Vector2(1.0f, 0.0f)),
            new VertexDataPosNormalUV(new Vector3(-0.5f,  0.5f, -0.5f), new Vector3(0.0f,  0.0f, -1.0f), new Vector2(0.0f, 1.0f)),
            new VertexDataPosNormalUV(new Vector3( 0.5f,  0.5f, -0.5f), new Vector3(0.0f,  0.0f, -1.0f), new Vector2(1.0f, 1.0f)),
            new VertexDataPosNormalUV(new Vector3(-0.5f, -0.5f, -0.5f), new Vector3(0.0f,  0.0f, -1.0f), new Vector2(0.0f, 0.0f)),

            new VertexDataPosNormalUV(new Vector3(-0.6f, -0.6f,  0.5f), new Vector3(0.0f,  0.0f,  1.0f), new Vector2(0.0f, 1.0f)), // Top face
            new VertexDataPosNormalUV(new Vector3( 0.6f, -0.6f,  0.5f), new Vector3(0.0f,  0.0f,  1.0f), new Vector2(1.0f, 1.0f)),
            new VertexDataPosNormalUV(new Vector3( 0.6f,  0.6f,  0.5f), new Vector3(0.0f,  0.0f,  1.0f), new Vector2(1.0f, 0.0f)),
            new VertexDataPosNormalUV(new Vector3( 0.6f,  0.6f,  0.5f), new Vector3(0.0f,  0.0f,  1.0f), new Vector2(1.0f, 0.0f)),
            new VertexDataPosNormalUV(new Vector3(-0.6f,  0.6f,  0.5f), new Vector3(0.0f,  0.0f,  1.0f), new Vector2(0.0f, 0.0f)),
            new VertexDataPosNormalUV(new Vector3(-0.6f, -0.6f,  0.5f), new Vector3(0.0f,  0.0f,  1.0f), new Vector2(0.0f, 1.0f)),

            new VertexDataPosNormalUV(new Vector3(-0.5f,  0.5f,  0.5f), new Vector3(1.0f,  0.0f,  0.0f), new Vector2(1.0f, 0.0f)), // Left face
            new VertexDataPosNormalUV(new Vector3(-0.5f,  0.5f, -0.5f), new Vector3(1.0f,  0.0f,  0.0f), new Vector2(1.0f, 1.0f)),
            new VertexDataPosNormalUV(new Vector3(-0.5f, -0.5f, -0.5f), new Vector3(1.0f,  0.0f,  0.0f), new Vector2(0.0f, 1.0f)),
            new VertexDataPosNormalUV(new Vector3(-0.5f, -0.5f, -0.5f), new Vector3(1.0f,  0.0f,  0.0f), new Vector2(0.0f, 1.0f)),
            new VertexDataPosNormalUV(new Vector3(-0.5f, -0.5f,  0.5f), new Vector3(1.0f,  0.0f,  0.0f), new Vector2(0.0f, 0.0f)),
            new VertexDataPosNormalUV(new Vector3(-0.5f,  0.5f,  0.5f), new Vector3(1.0f,  0.0f,  0.0f), new Vector2(1.0f, 0.0f)),

            new VertexDataPosNormalUV(new Vector3( 0.5f, -0.5f, -0.5f), new Vector3(1.0f,  0.0f,  0.0f), new Vector2(0.0f, 1.0f)), // Right face
            new VertexDataPosNormalUV(new Vector3( 0.5f,  0.5f, -0.5f), new Vector3(1.0f,  0.0f,  0.0f), new Vector2(1.0f, 1.0f)),
            new VertexDataPosNormalUV(new Vector3( 0.5f,  0.5f,  0.5f), new Vector3(1.0f,  0.0f,  0.0f), new Vector2(1.0f, 0.0f)),
            new VertexDataPosNormalUV(new Vector3( 0.5f,  0.5f,  0.5f), new Vector3(1.0f,  0.0f,  0.0f), new Vector2(1.0f, 0.0f)),
            new VertexDataPosNormalUV(new Vector3( 0.5f, -0.5f,  0.5f), new Vector3(1.0f,  0.0f,  0.0f), new Vector2(0.0f, 0.0f)),
            new VertexDataPosNormalUV(new Vector3( 0.5f, -0.5f, -0.5f), new Vector3(1.0f,  0.0f,  0.0f), new Vector2(0.0f, 1.0f)),

            new VertexDataPosNormalUV(new Vector3(-0.5f, -0.5f, -0.5f), new Vector3(0.0f, -1.0f,  0.0f), new Vector2(0.0f, 1.0f)), // Front face
            new VertexDataPosNormalUV(new Vector3( 0.5f, -0.5f, -0.5f), new Vector3(0.0f, -1.0f,  0.0f), new Vector2(1.0f, 1.0f)),
            new VertexDataPosNormalUV(new Vector3( 0.5f, -0.5f,  0.5f), new Vector3(0.0f, -1.0f,  0.0f), new Vector2(1.0f, 0.0f)),
            new VertexDataPosNormalUV(new Vector3( 0.5f, -0.5f,  0.5f), new Vector3(0.0f, -1.0f,  0.0f), new Vector2(1.0f, 0.0f)),
            new VertexDataPosNormalUV(new Vector3(-0.5f, -0.5f,  0.5f), new Vector3(0.0f, -1.0f,  0.0f), new Vector2(0.0f, 0.0f)),
            new VertexDataPosNormalUV(new Vector3(-0.5f, -0.5f, -0.5f), new Vector3(0.0f, -1.0f,  0.0f), new Vector2(0.0f, 1.0f)),

            new VertexDataPosNormalUV(new Vector3( 0.5f,  0.5f, -0.5f), new Vector3(0.0f,  1.0f,  0.0f), new Vector2(1.0f, 1.0f)), // Back face
            new VertexDataPosNormalUV(new Vector3(-0.5f,  0.5f, -0.5f), new Vector3(0.0f,  1.0f,  0.0f), new Vector2(0.0f, 1.0f)),
            new VertexDataPosNormalUV(new Vector3( 0.5f,  0.5f,  0.5f), new Vector3(0.0f,  1.0f,  0.0f), new Vector2(1.0f, 0.0f)),
            new VertexDataPosNormalUV(new Vector3(-0.5f,  0.5f,  0.5f), new Vector3(0.0f,  1.0f,  0.0f), new Vector2(0.0f, 0.0f)),
            new VertexDataPosNormalUV(new Vector3( 0.5f,  0.5f,  0.5f), new Vector3(0.0f,  1.0f,  0.0f), new Vector2(1.0f, 0.0f)),
            new VertexDataPosNormalUV(new Vector3(-0.5f,  0.5f, -0.5f), new Vector3(0.0f,  1.0f,  0.0f), new Vector2(0.0f, 1.0f)),

            new VertexDataPosNormalUV(new Vector3(-0.1f+DX, -0.1f+DY, -0.3f+DZ), new Vector3(0.0f,  0.0f, -1.0f), new Vector2(0.0f, 0.0f)), // Bottom face
            new VertexDataPosNormalUV(new Vector3( 0.1f+DX,  0.1f+DY, -0.3f+DZ), new Vector3(0.0f,  0.0f, -1.0f), new Vector2(1.0f, 1.0f)),
            new VertexDataPosNormalUV(new Vector3( 0.1f+DX, -0.1f+DY, -0.3f+DZ), new Vector3(0.0f,  0.0f, -1.0f), new Vector2(1.0f, 0.0f)),
            new VertexDataPosNormalUV(new Vector3(-0.1f+DX,  0.1f+DY, -0.3f+DZ), new Vector3(0.0f,  0.0f, -1.0f), new Vector2(0.0f, 1.0f)),
            new VertexDataPosNormalUV(new Vector3( 0.1f+DX,  0.1f+DY, -0.3f+DZ), new Vector3(0.0f,  0.0f, -1.0f), new Vector2(1.0f, 1.0f)),
            new VertexDataPosNormalUV(new Vector3(-0.1f+DX, -0.1f+DY, -0.3f+DZ), new Vector3(0.0f,  0.0f, -1.0f), new Vector2(0.0f, 0.0f)),

            new VertexDataPosNormalUV(new Vector3(-0.1f+DX, -0.1f+DY,  0.3f+DZ), new Vector3(0.0f,  0.0f,  1.0f), new Vector2(0.0f, 1.0f)), // Top face
            new VertexDataPosNormalUV(new Vector3( 0.1f+DX, -0.1f+DY,  0.3f+DZ), new Vector3(0.0f,  0.0f,  1.0f), new Vector2(1.0f, 1.0f)),
            new VertexDataPosNormalUV(new Vector3( 0.1f+DX,  0.1f+DY,  0.3f+DZ), new Vector3(0.0f,  0.0f,  1.0f), new Vector2(1.0f, 0.0f)),
            new VertexDataPosNormalUV(new Vector3( 0.1f+DX,  0.1f+DY,  0.3f+DZ), new Vector3(0.0f,  0.0f,  1.0f), new Vector2(1.0f, 0.0f)),
            new VertexDataPosNormalUV(new Vector3(-0.1f+DX,  0.1f+DY,  0.3f+DZ), new Vector3(0.0f,  0.0f,  1.0f), new Vector2(0.0f, 0.0f)),
            new VertexDataPosNormalUV(new Vector3(-0.1f+DX, -0.1f+DY,  0.3f+DZ), new Vector3(0.0f,  0.0f,  1.0f), new Vector2(0.0f, 1.0f)),

            new VertexDataPosNormalUV(new Vector3(-0.1f+DX,  0.1f+DY,  0.3f+DZ), new Vector3(1.0f,  0.0f,  0.0f), new Vector2(1.0f, 0.0f)), // Left face
            new VertexDataPosNormalUV(new Vector3(-0.1f+DX,  0.1f+DY, -0.3f+DZ), new Vector3(1.0f,  0.0f,  0.0f), new Vector2(1.0f, 1.0f)),
            new VertexDataPosNormalUV(new Vector3(-0.1f+DX, -0.1f+DY, -0.3f+DZ), new Vector3(1.0f,  0.0f,  0.0f), new Vector2(0.0f, 1.0f)),
            new VertexDataPosNormalUV(new Vector3(-0.1f+DX, -0.1f+DY, -0.3f+DZ), new Vector3(1.0f,  0.0f,  0.0f), new Vector2(0.0f, 1.0f)),
            new VertexDataPosNormalUV(new Vector3(-0.1f+DX, -0.1f+DY,  0.3f+DZ), new Vector3(1.0f,  0.0f,  0.0f), new Vector2(0.0f, 0.0f)),
            new VertexDataPosNormalUV(new Vector3(-0.1f+DX,  0.1f+DY,  0.3f+DZ), new Vector3(1.0f,  0.0f,  0.0f), new Vector2(1.0f, 0.0f)),

            new VertexDataPosNormalUV(new Vector3( 0.1f+DX, -0.1f+DY, -0.3f+DZ), new Vector3(1.0f,  0.0f,  0.0f), new Vector2(0.0f, 1.0f)), // Right face
            new VertexDataPosNormalUV(new Vector3( 0.1f+DX,  0.1f+DY, -0.3f+DZ), new Vector3(1.0f,  0.0f,  0.0f), new Vector2(1.0f, 1.0f)),
            new VertexDataPosNormalUV(new Vector3( 0.1f+DX,  0.1f+DY,  0.3f+DZ), new Vector3(1.0f,  0.0f,  0.0f), new Vector2(1.0f, 0.0f)),
            new VertexDataPosNormalUV(new Vector3( 0.1f+DX,  0.1f+DY,  0.3f+DZ), new Vector3(1.0f,  0.0f,  0.0f), new Vector2(1.0f, 0.0f)),
            new VertexDataPosNormalUV(new Vector3( 0.1f+DX, -0.1f+DY,  0.3f+DZ), new Vector3(1.0f,  0.0f,  0.0f), new Vector2(0.0f, 0.0f)),
            new VertexDataPosNormalUV(new Vector3( 0.1f+DX, -0.1f+DY, -0.3f+DZ), new Vector3(1.0f,  0.0f,  0.0f), new Vector2(0.0f, 1.0f)),

            new VertexDataPosNormalUV(new Vector3(-0.1f+DX, -0.1f+DY, -0.3f+DZ), new Vector3(0.0f, -1.0f,  0.0f), new Vector2(0.0f, 1.0f)), // Front face
            new VertexDataPosNormalUV(new Vector3( 0.1f+DX, -0.1f+DY, -0.3f+DZ), new Vector3(0.0f, -1.0f,  0.0f), new Vector2(1.0f, 1.0f)),
            new VertexDataPosNormalUV(new Vector3( 0.1f+DX, -0.1f+DY,  0.3f+DZ), new Vector3(0.0f, -1.0f,  0.0f), new Vector2(1.0f, 0.0f)),
            new VertexDataPosNormalUV(new Vector3( 0.1f+DX, -0.1f+DY,  0.3f+DZ), new Vector3(0.0f, -1.0f,  0.0f), new Vector2(1.0f, 0.0f)),
            new VertexDataPosNormalUV(new Vector3(-0.1f+DX, -0.1f+DY,  0.3f+DZ), new Vector3(0.0f, -1.0f,  0.0f), new Vector2(0.0f, 0.0f)),
            new VertexDataPosNormalUV(new Vector3(-0.1f+DX, -0.1f+DY, -0.3f+DZ), new Vector3(0.0f, -1.0f,  0.0f), new Vector2(0.0f, 1.0f)),

            new VertexDataPosNormalUV(new Vector3( 0.1f+DX,  0.1f+DY, -0.3f+DZ), new Vector3(0.0f,  1.0f,  0.0f), new Vector2(1.0f, 1.0f)), // Back face
            new VertexDataPosNormalUV(new Vector3(-0.1f+DX,  0.1f+DY, -0.3f+DZ), new Vector3(0.0f,  1.0f,  0.0f), new Vector2(0.0f, 1.0f)),
            new VertexDataPosNormalUV(new Vector3( 0.1f+DX,  0.1f+DY,  0.3f+DZ), new Vector3(0.0f,  1.0f,  0.0f), new Vector2(1.0f, 0.0f)),
            new VertexDataPosNormalUV(new Vector3(-0.1f+DX,  0.1f+DY,  0.3f+DZ), new Vector3(0.0f,  1.0f,  0.0f), new Vector2(0.0f, 0.0f)),
            new VertexDataPosNormalUV(new Vector3( 0.1f+DX,  0.1f+DY,  0.3f+DZ), new Vector3(0.0f,  1.0f,  0.0f), new Vector2(1.0f, 0.0f)),
            new VertexDataPosNormalUV(new Vector3(-0.1f+DX,  0.1f+DY, -0.3f+DZ), new Vector3(0.0f,  1.0f,  0.0f), new Vector2(0.0f, 1.0f)),

            new VertexDataPosNormalUV(new Vector3(-0.3f+EX, -0.1f+EY, -0.1f+EZ), new Vector3(0.0f,  0.0f, -1.0f), new Vector2(0.0f, 0.0f)), // Bottom face
            new VertexDataPosNormalUV(new Vector3( 0.3f+EX,  0.1f+EY, -0.1f+EZ), new Vector3(0.0f,  0.0f, -1.0f), new Vector2(1.0f, 1.0f)),
            new VertexDataPosNormalUV(new Vector3( 0.3f+EX, -0.1f+EY, -0.1f+EZ), new Vector3(0.0f,  0.0f, -1.0f), new Vector2(1.0f, 0.0f)),
            new VertexDataPosNormalUV(new Vector3(-0.3f+EX,  0.1f+EY, -0.1f+EZ), new Vector3(0.0f,  0.0f, -1.0f), new Vector2(0.0f, 1.0f)),
            new VertexDataPosNormalUV(new Vector3( 0.3f+EX,  0.1f+EY, -0.1f+EZ), new Vector3(0.0f,  0.0f, -1.0f), new Vector2(1.0f, 1.0f)),
            new VertexDataPosNormalUV(new Vector3(-0.3f+EX, -0.1f+EY, -0.1f+EZ), new Vector3(0.0f,  0.0f, -1.0f), new Vector2(0.0f, 0.0f)),

            new VertexDataPosNormalUV(new Vector3(-0.3f+EX, -0.1f+EY,  0.1f+EZ), new Vector3(0.0f,  0.0f,  1.0f), new Vector2(0.0f, 1.0f)), // Top face
            new VertexDataPosNormalUV(new Vector3( 0.3f+EX, -0.1f+EY,  0.1f+EZ), new Vector3(0.0f,  0.0f,  1.0f), new Vector2(1.0f, 1.0f)),
            new VertexDataPosNormalUV(new Vector3( 0.3f+EX,  0.1f+EY,  0.1f+EZ), new Vector3(0.0f,  0.0f,  1.0f), new Vector2(1.0f, 0.0f)),
            new VertexDataPosNormalUV(new Vector3( 0.3f+EX,  0.1f+EY,  0.1f+EZ), new Vector3(0.0f,  0.0f,  1.0f), new Vector2(1.0f, 0.0f)),
            new VertexDataPosNormalUV(new Vector3(-0.3f+EX,  0.1f+EY,  0.1f+EZ), new Vector3(0.0f,  0.0f,  1.0f), new Vector2(0.0f, 0.0f)),
            new VertexDataPosNormalUV(new Vector3(-0.3f+EX, -0.1f+EY,  0.1f+EZ), new Vector3(0.0f,  0.0f,  1.0f), new Vector2(0.0f, 1.0f)),

            new VertexDataPosNormalUV(new Vector3(-0.3f+EX,  0.1f+EY,  0.1f+EZ), new Vector3(1.0f,  0.0f,  0.0f), new Vector2(1.0f, 0.0f)), // Left face
            new VertexDataPosNormalUV(new Vector3(-0.3f+EX,  0.1f+EY, -0.1f+EZ), new Vector3(1.0f,  0.0f,  0.0f), new Vector2(1.0f, 1.0f)),
            new VertexDataPosNormalUV(new Vector3(-0.3f+EX, -0.1f+EY, -0.1f+EZ), new Vector3(1.0f,  0.0f,  0.0f), new Vector2(0.0f, 1.0f)),
            new VertexDataPosNormalUV(new Vector3(-0.3f+EX, -0.1f+EY, -0.1f+EZ), new Vector3(1.0f,  0.0f,  0.0f), new Vector2(0.0f, 1.0f)),
            new VertexDataPosNormalUV(new Vector3(-0.3f+EX, -0.1f+EY,  0.1f+EZ), new Vector3(1.0f,  0.0f,  0.0f), new Vector2(0.0f, 0.0f)),
            new VertexDataPosNormalUV(new Vector3(-0.3f+EX,  0.1f+EY,  0.1f+EZ), new Vector3(1.0f,  0.0f,  0.0f), new Vector2(1.0f, 0.0f)),

            new VertexDataPosNormalUV(new Vector3( 0.3f+EX, -0.1f+EY, -0.1f+EZ), new Vector3(1.0f,  0.0f,  0.0f), new Vector2(0.0f, 1.0f)), // Right face
            new VertexDataPosNormalUV(new Vector3( 0.3f+EX,  0.1f+EY, -0.1f+EZ), new Vector3(1.0f,  0.0f,  0.0f), new Vector2(1.0f, 1.0f)),
            new VertexDataPosNormalUV(new Vector3( 0.3f+EX,  0.1f+EY,  0.1f+EZ), new Vector3(1.0f,  0.0f,  0.0f), new Vector2(1.0f, 0.0f)),
            new VertexDataPosNormalUV(new Vector3( 0.3f+EX,  0.1f+EY,  0.1f+EZ), new Vector3(1.0f,  0.0f,  0.0f), new Vector2(1.0f, 0.0f)),
            new VertexDataPosNormalUV(new Vector3( 0.3f+EX, -0.1f+EY,  0.1f+EZ), new Vector3(1.0f,  0.0f,  0.0f), new Vector2(0.0f, 0.0f)),
            new VertexDataPosNormalUV(new Vector3( 0.3f+EX, -0.1f+EY, -0.1f+EZ), new Vector3(1.0f,  0.0f,  0.0f), new Vector2(0.0f, 1.0f)),

            new VertexDataPosNormalUV(new Vector3(-0.3f+EX, -0.1f+EY, -0.1f+EZ), new Vector3(0.0f, -1.0f,  0.0f), new Vector2(0.0f, 1.0f)), // Front face
            new VertexDataPosNormalUV(new Vector3( 0.3f+EX, -0.1f+EY, -0.1f+EZ), new Vector3(0.0f, -1.0f,  0.0f), new Vector2(1.0f, 1.0f)),
            new VertexDataPosNormalUV(new Vector3( 0.3f+EX, -0.1f+EY,  0.1f+EZ), new Vector3(0.0f, -1.0f,  0.0f), new Vector2(1.0f, 0.0f)),
            new VertexDataPosNormalUV(new Vector3( 0.3f+EX, -0.1f+EY,  0.1f+EZ), new Vector3(0.0f, -1.0f,  0.0f), new Vector2(1.0f, 0.0f)),
            new VertexDataPosNormalUV(new Vector3(-0.3f+EX, -0.1f+EY,  0.1f+EZ), new Vector3(0.0f, -1.0f,  0.0f), new Vector2(0.0f, 0.0f)),
            new VertexDataPosNormalUV(new Vector3(-0.3f+EX, -0.1f+EY, -0.1f+EZ), new Vector3(0.0f, -1.0f,  0.0f), new Vector2(0.0f, 1.0f)),

            new VertexDataPosNormalUV(new Vector3( 0.3f+EX,  0.1f+EY, -0.1f+EZ), new Vector3(0.0f,  1.0f,  0.0f), new Vector2(1.0f, 1.0f)), // Back face
            new VertexDataPosNormalUV(new Vector3(-0.3f+EX,  0.1f+EY, -0.1f+EZ), new Vector3(0.0f,  1.0f,  0.0f), new Vector2(0.0f, 1.0f)),
            new VertexDataPosNormalUV(new Vector3( 0.3f+EX,  0.1f+EY,  0.1f+EZ), new Vector3(0.0f,  1.0f,  0.0f), new Vector2(1.0f, 0.0f)),
            new VertexDataPosNormalUV(new Vector3(-0.3f+EX,  0.1f+EY,  0.1f+EZ), new Vector3(0.0f,  1.0f,  0.0f), new Vector2(0.0f, 0.0f)),
            new VertexDataPosNormalUV(new Vector3( 0.3f+EX,  0.1f+EY,  0.1f+EZ), new Vector3(0.0f,  1.0f,  0.0f), new Vector2(1.0f, 0.0f)),
            new VertexDataPosNormalUV(new Vector3(-0.3f+EX,  0.1f+EY, -0.1f+EZ), new Vector3(0.0f,  1.0f,  0.0f), new Vector2(0.0f, 1.0f)),
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
