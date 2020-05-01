// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Aximo.Engine;
using Aximo.Engine.Mesh2;
using Aximo.Render;
using OpenToolkit;
using OpenToolkit.Mathematics;
using Xunit;

namespace Aximo.AxTests
{

    public class MeshTests
    {

        private Engine.Mesh2.Mesh CreateEmptyMesh()
        {
            var tmp = new Engine.Mesh2.Mesh();

            var compPosition = new MeshPositionComponent();
            var compNormal = new MeshNormalComponent();
            var compUV = new MeshUVComponent();

            tmp.AddComponent(compPosition);
            tmp.AddComponent(compNormal);
            tmp.AddComponent(compUV);

            return tmp;
        }

        private Engine.Mesh2.Mesh CreateMesh()
        {
            var data = (BufferData1D<VertexDataPosNormalUV>)MeshDataBuilder.Cube().Data;
            var span = data.Span;

            var tmp = CreateEmptyMesh();
            var vv = tmp.View<IVertexPosNormalUV>();
            for (var i = 0; i < span.Length; i++)
            {
                vv.Add(span[i]);
            }
            return tmp;
        }

        [Fact]
        public void Test1()
        {
            var data = (BufferData1D<VertexDataPosNormalUV>)MeshDataBuilder.Cube().Data;
            var span = data.Span;

            var tmp = CreateEmptyMesh();
            var vv = tmp.View<IVertexPosNormalUV>();
            for (var i = 0; i < span.Length; i++)
            {
                vv.Add(span[i]);
            }

            Assert.Equal(span.ToArray().Select(v => v.Position), vv.Select(v => v.Position));
        }

        [Fact]
        public void Test2()
        {
            var tmp = CreateMesh();

            //tmp.Visit<IVertexPosNormalUV>((m, i) => m.Position = Vector3.UnitZ);

            var view = tmp.View<IVertexPosition3>();
            var view2 = tmp.View<IVertexPosNormalUV>();

            var p = new Vector3(2, 1, 3);
            view[0].Position = p;
            Assert.Equal(p, view[0].Position);
            Assert.Equal(p, view2[0].Position);

            // tmp.AddFace(new int[] { 0, 1, 2, 3 });
            // var faces = tmp.FaceView<IVertexPosNormalUV>();
            // var f = faces[0];
            // f[0].Position = new Vector3();
        }

        [Fact]
        public void Test3()
        {
            var tmp = CreateEmptyMesh();

            IVertexPosition3 v = new VertexDataPosNormalUV();
            IVertexPosNormalUV v2 = new VertexDataPosNormalUV();

            var view = tmp.View<IVertexPosNormalUV>();
            view.AddRange(
                new VertexDataPosNormalUV
                {
                    Position = new Vector3(11, 12, 13),
                    Normal = Vector3.UnitZ,
                    UV = new Vector2(0, 1)
                }, new VertexDataPosNormalUV
                {
                    Position = new Vector3(21, 22, 23),
                    Normal = Vector3.UnitZ,
                    UV = new Vector2(0, 1)
                }, new VertexDataPosNormalUV
                {
                    Position = new Vector3(31, 32, 33),
                    Normal = Vector3.UnitZ,
                    UV = new Vector2(0, 1)
                }, new VertexDataPosNormalUV
                {
                    Position = new Vector3(41, 42, 43),
                    Normal = Vector3.UnitZ,
                    UV = new Vector2(0, 1)
                }, new VertexDataPosNormalUV
                {
                    Position = new Vector3(51, 52, 53),
                    Normal = Vector3.UnitZ,
                    UV = new Vector2(0, 1)
                }, new VertexDataPosNormalUV
                {
                    Position = new Vector3(61, 62, 73),
                    Normal = Vector3.UnitZ,
                    UV = new Vector2(0, 1)
                });

            tmp.AddFace(3, 4, 5);
            tmp.AddFace(3, 0, 1);

            var vertices = tmp.View<IVertexPosNormalUV>();
            var faces = tmp.FaceView<IVertexPosNormalUV>();

            var face = faces[0];
            var vv = face[0];

            Assert.Equal(2, faces.Count);
            Assert.Equal(vertices[3].Position, faces[0][0].Position);
        }
    }

}