// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

// Adapted from this excellent IcoSphere tutorial by Andreas Kahler
// http://blog.andreaskahler.com/2009/06/creating-icosphere-mesh-in-code.html
// Changes Copyright (C) 2014 by David Jeske, and donated to the public domain.

using System;
using System.Collections.Generic;
using Aximo.Render;
using OpenToolkit;
using OpenToolkit.Mathematics;

namespace Aximo.Render.Objects.Util.IcoSphere
{
    public class IcoSphereMesh
    {
        public VertexDataPosNormalUV[] Vertices;
        public ushort[] Indicies;

        private MeshGeometry3D geom;

        public IcoSphereMesh(int divisions)
        {
            this.Create(divisions);
        }

        private void ComputeEquirectangularUVForSpherePoint(Vector3 p, out float u, out float v)
        {
            // http://paulbourke.net/geometry/transformationprojection/

            // this is the unit surface normal (and surface point)
            Vector3 vpn = p.Normalized();

            // compute latitute and longitute values from the vector to the normalized sphere surface position
            // using Y-up coordinates

            // compute azimuth (x texture coord)
            float longitude = (float)Math.Atan2(vpn.X, vpn.Z);       // azimuth [-PI  to PI]
            u = (float)((longitude / (2.0 * Math.PI)) + 0.5);     // [0 to 1]

            // compute altitude (y texture coord)
            float latitude = (float)Math.Acos(vpn.Y);               // altitude [0 to PI]
            v = (float)(latitude / Math.PI);                        // [0 to 1]

            // v = (float) ( Math.Log((1.0 + Math.Sin(latitude))/(1.0 - Math.Sin(latitude))) / (4.0 * Math.PI) );
        }

        private void Create(int divisions)
        {
            var icoSphereCreator = new IcoSphereCreator();
            geom = icoSphereCreator.Create(divisions);
            var positions = geom.Positions.ToArray();

            var vertexSoup = new VertexSoup<VertexDataPosNormalUV>();
            var indexList = new List<ushort>();

            // we have to process each face in the IcoSphere, so we can
            // properly "wrap" the texture-coordinates that fall across the left/right border
            foreach (TriangleIndices face in geom.Faces)
            {
                var vp1 = positions[face.V1];
                var vp2 = positions[face.V2];
                var vp3 = positions[face.V3];

                var normal1 = vp1.Normalized();
                var normal2 = vp2.Normalized();
                var normal3 = vp3.Normalized();

                float s1, s2, s3, t1, t2, t3;

                ComputeEquirectangularUVForSpherePoint(normal1, out s1, out t1);
                ComputeEquirectangularUVForSpherePoint(normal2, out s2, out t2);
                ComputeEquirectangularUVForSpherePoint(normal3, out s3, out t3);

                // configure verticies

                var v1 = new VertexDataPosNormalUV
                {
                    Position = vp1,
                    Normal = normal1,
                };
                v1.UV.X = s1;
                v1.UV.Y = t1;

                var v2 = new VertexDataPosNormalUV
                {
                    Position = vp2,
                    Normal = normal2,
                };
                v2.UV.X = s2;
                v2.UV.Y = t2;

                var v3 = new VertexDataPosNormalUV
                {
                    Position = vp3,
                    Normal = normal3,
                };
                v3.UV.X = s3;
                v3.UV.Y = t3;

                // if a triangle spans the left/right seam where U transitions from 1 to -1
                bool v1_left = vp1.X < 0.0f;
                bool v2_left = vp2.X < 0.0f;
                bool v3_left = vp3.X < 0.0f;
                if (vp1.Z < 0.0f && vp2.Z < 0.0f && vp3.Z < 0.0f &&
                    ((v2_left != v1_left) || (v3_left != v1_left)))
                {
                    // we need to "wrap" texture coordinates
                    if (v1.UV.X < 0.5f) { v1.UV.X += 1.0f; }
                    if (v2.UV.X < 0.5f) { v2.UV.X += 1.0f; }
                    if (v3.UV.X < 0.5f) { v3.UV.X += 1.0f; }
                }

                // add configured verticies to mesh..

                ushort idx1 = vertexSoup.DigestVertex(ref v1);
                ushort idx2 = vertexSoup.DigestVertex(ref v2);
                ushort idx3 = vertexSoup.DigestVertex(ref v3);

                indexList.Add(idx1);
                indexList.Add(idx2);
                indexList.Add(idx3);
            }

            Vertices = vertexSoup.Verticies.ToArray();
            Indicies = indexList.ToArray();

            for (var i = 0; i < Vertices.Length; i++)
            {
                Vertices[i].Position *= 0.5f;
            }
        }
    }
}
