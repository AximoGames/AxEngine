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

    public class IcoSphereCreator
    {
        private MeshGeometry3D geometry;
        private int index;
        private Dictionary<long, int> middlePointIndexCache;

        // add vertex to mesh, fix position to be on unit sphere, return index
        private int AddVertex(Vector3 p)
        {
            float length = (float)Math.Sqrt((p.X * p.X) + (p.Y * p.Y) + (p.Z * p.Z));
            geometry.Positions.Add(new Vector3(p.X / length, p.Y / length, p.Z / length));
            return index++;
        }

        // return index of point in the middle of p1 and p2
        private int GetMiddlePoint(int p1, int p2)
        {
            // first check if we have it already
            bool firstIsSmaller = p1 < p2;
            long smallerIndex = firstIsSmaller ? p1 : p2;
            long greaterIndex = firstIsSmaller ? p2 : p1;
            long key = (smallerIndex << 32) + greaterIndex;

            int ret;
            if (this.middlePointIndexCache.TryGetValue(key, out ret))
            {
                return ret;
            }

            // not in cache, calculate it
            Vector3 point1 = this.geometry.Positions[p1];
            Vector3 point2 = this.geometry.Positions[p2];
            Vector3 middle = new Vector3(
                                 (point1.X + point2.X) / 2.0f,
                                 (point1.Y + point2.Y) / 2.0f,
                                 (point1.Z + point2.Z) / 2.0f);

            // add vertex makes sure point is on unit sphere
            int i = AddVertex(middle);

            // store it, return index
            this.middlePointIndexCache.Add(key, i);
            return i;
        }

        public MeshGeometry3D Create(int recursionLevel)
        {
            this.geometry = new MeshGeometry3D();
            this.middlePointIndexCache = new Dictionary<long, int>();
            this.index = 0;

            // create 12 vertices of a icosahedron
            float t = (float)((1.0f + Math.Sqrt(5.0)) / 2.0);

            AddVertex(new Vector3(-1, t, 0f));
            AddVertex(new Vector3(1, t, 0f));
            AddVertex(new Vector3(-1, -t, 0f));
            AddVertex(new Vector3(1, -t, 0f));

            AddVertex(new Vector3(0, -1, t));
            AddVertex(new Vector3(0, 1, t));
            AddVertex(new Vector3(0, -1, -t));
            AddVertex(new Vector3(0, 1, -t));

            AddVertex(new Vector3(t, 0, -1));
            AddVertex(new Vector3(t, 0, 1));
            AddVertex(new Vector3(-t, 0, -1));
            AddVertex(new Vector3(-t, 0, 1));

            // create 20 triangles of the icosahedron
            var faces = new List<TriangleIndices>();

            // 5 faces around point 0
            faces.Add(new TriangleIndices(0, 11, 5));
            faces.Add(new TriangleIndices(0, 5, 1));
            faces.Add(new TriangleIndices(0, 1, 7));
            faces.Add(new TriangleIndices(0, 7, 10));
            faces.Add(new TriangleIndices(0, 10, 11));

            // 5 adjacent faces
            faces.Add(new TriangleIndices(1, 5, 9));
            faces.Add(new TriangleIndices(5, 11, 4));
            faces.Add(new TriangleIndices(11, 10, 2));
            faces.Add(new TriangleIndices(10, 7, 6));
            faces.Add(new TriangleIndices(7, 1, 8));

            // 5 faces around point 3
            faces.Add(new TriangleIndices(3, 9, 4));
            faces.Add(new TriangleIndices(3, 4, 2));
            faces.Add(new TriangleIndices(3, 2, 6));
            faces.Add(new TriangleIndices(3, 6, 8));
            faces.Add(new TriangleIndices(3, 8, 9));

            // 5 adjacent faces
            faces.Add(new TriangleIndices(4, 9, 5));
            faces.Add(new TriangleIndices(2, 4, 11));
            faces.Add(new TriangleIndices(6, 2, 10));
            faces.Add(new TriangleIndices(8, 6, 7));
            faces.Add(new TriangleIndices(9, 8, 1));

            // refine triangles
            for (int i = 0; i < recursionLevel; i++)
            {
                var faces2 = new List<TriangleIndices>();
                foreach (var tri in faces)
                {
                    // replace triangle by 4 triangles
                    int a = GetMiddlePoint(tri.V1, tri.V2);
                    int b = GetMiddlePoint(tri.V2, tri.V3);
                    int c = GetMiddlePoint(tri.V3, tri.V1);

                    faces2.Add(new TriangleIndices(tri.V1, a, c));
                    faces2.Add(new TriangleIndices(tri.V2, b, a));
                    faces2.Add(new TriangleIndices(tri.V3, c, b));
                    faces2.Add(new TriangleIndices(a, b, c));
                }
                faces = faces2;
            }

            this.geometry.Faces = faces;

            // done, now add triangles to mesh
            foreach (var tri in faces)
            {
                this.geometry.MeshIndicies.Add(tri.V1);
                this.geometry.MeshIndicies.Add(tri.V2);
                this.geometry.MeshIndicies.Add(tri.V3);
            }

            return this.geometry;
        }
    }

    public class VertexSoup<TVertexStruct>
    {
        private Dictionary<TVertexStruct, ushort> vertexToIndexMap = new Dictionary<TVertexStruct, ushort>();
        public List<TVertexStruct> Verticies = new List<TVertexStruct>();
        private readonly bool deDup;

        public ushort DigestVertex(ref TVertexStruct vertex)
        {
            ushort retval;
            if (deDup && vertexToIndexMap.ContainsKey(vertex))
            {
                retval = vertexToIndexMap[vertex];
            }
            else
            {
                ushort nextIndex = (ushort)Verticies.Count;
                vertexToIndexMap[vertex] = nextIndex;
                Verticies.Add(vertex);
                retval = nextIndex;
            }

            return retval;
        }

        public ushort[] DigestVerticies(TVertexStruct[] vertex_list)
        {
            ushort[] retval = new ushort[vertex_list.Length];

            for (int x = 0; x < vertex_list.Length; x++)
            {
                retval[x] = DigestVertex(ref vertex_list[x]);
            }
            return retval;
        }

        public VertexSoup(bool deDup = true)
        {
            this.deDup = deDup;
        }
    }

    public struct TriangleIndices
    {
        public int V1;
        public int V2;
        public int V3;

        public TriangleIndices(int v1, int v2, int v3)
        {
            this.V1 = v1;
            this.V2 = v2;
            this.V3 = v3;
        }
    }

    public class MeshGeometry3D
    {
        public List<Vector3> Positions = new List<Vector3>();
        public List<int> MeshIndicies = new List<int>();
        public List<TriangleIndices> Faces = new List<TriangleIndices>();
    }
}
