using System.Collections.Generic;
using Aximo.VertexData;
using OpenToolkit.Mathematics;

// Reference: https://catlikecoding.com/unity/tutorials/rounded-cube/

namespace Aximo.Util.RoundedCube
{

    public class RoundedCubeGenerator
    {

        public int SizeX { get; set; }
        public int SizeY { get; set; }
        public int SizeZ { get; set; }

        public int Roundness { get; set; }

        private Vector3[] vertices;

        public Mesh Mesh { get; set; }

        private Vector3[] normals;

        private Vector4[] cubeUV;

        public void Generate()
        {
            Mesh = new Mesh();
            Mesh.Name = "Procedural Rounded Cube";
            Mesh.AddComponents<VertexDataPosNormalUV>();

            CreateVertices();
            CreateTriangles();
        }

        private void CreateVertices()
        {
            int cornerVertices = 8;
            int edgeVertices = (SizeX + SizeY + SizeZ - 3) * 4;
            int faceVertices = (
                                   ((SizeX - 1) * (SizeY - 1)) +
                                   ((SizeX - 1) * (SizeZ - 1)) +
                                   ((SizeY - 1) * (SizeZ - 1))) * 2;

            vertices = new Vector3[cornerVertices + edgeVertices + faceVertices];
            normals = new Vector3[vertices.Length];
            cubeUV = new Vector4[vertices.Length];

            int v = 0;
            // one layer at a time
            for (int y = 0; y <= SizeY; y++)
            {
                // outer faces and edges
                for (int x = 0; x <= SizeX; x++)
                {
                    SetVertex(v++, x, y, 0);
                }
                for (int z = 1; z <= SizeZ; z++)
                {
                    SetVertex(v++, SizeX, y, z);
                }
                for (int x = SizeX - 1; x >= 0; x--)
                {
                    SetVertex(v++, x, y, SizeZ);
                }
                for (int z = SizeZ - 1; z > 0; z--)
                {
                    SetVertex(v++, 0, y, z);
                }
            }
            // top face fill
            for (int z = 1; z < SizeZ; z++)
            {
                for (int x = 1; x < SizeX; x++)
                {
                    SetVertex(v++, x, SizeY, z);
                }
            }
            // bottom face fill
            for (int z = 1; z < SizeZ; z++)
            {
                for (int x = 1; x < SizeX; x++)
                {
                    SetVertex(v++, x, 0, z);
                }
            }

            var data = new List<VertexDataPosNormalUV>();
            for (var i = 0; i < vertices.Length; i++)
            {
                data.Add(new VertexDataPosNormalUV
                {
                    Position = vertices[i],
                    Normal = normals[i],
                    UV = cubeUV[i].Xy, // TODO: Bug
                });
            }
            Mesh.AddVertices(data);
        }

        private void SetVertex(int i, int x, int y, int z)
        {
            Vector3 inner = vertices[i] = new Vector3(x, y, z);

            if (x < Roundness)
            {
                inner.X = Roundness;
            }
            else if (x > SizeX - Roundness)
            {
                inner.X = SizeX - Roundness;
            }

            if (y < Roundness)
            {
                inner.Y = Roundness;
            }
            else if (y > SizeY - Roundness)
            {
                inner.Y = SizeY - Roundness;
            }

            if (z < Roundness)
            {
                inner.Z = Roundness;
            }
            else if (z > SizeZ - Roundness)
            {
                inner.Z = SizeZ - Roundness;
            }

            normals[i] = (vertices[i] - inner).Normalized();
            vertices[i] = inner + (normals[i] * Roundness);
            cubeUV[i] = new Vector4(x, y, z, 0);
        }

        private void CreateTriangles()
        {
            int[] trianglesZ = new int[(SizeX * SizeY) * 12];
            int[] trianglesX = new int[(SizeY * SizeZ) * 12];
            int[] trianglesY = new int[(SizeX * SizeZ) * 12];
            int ring = (SizeX + SizeZ) * 2;
            int tZ = 0,
                tX = 0,
                tY = 0,
                v = 0;

            for (int y = 0; y < SizeY; y++, v++)
            {
                for (int q = 0; q < SizeX; q++, v++)
                {
                    tZ = SetQuad(trianglesZ, tZ, v, v + 1, v + ring, v + ring + 1);
                }
                for (int q = 0; q < SizeZ; q++, v++)
                {
                    tX = SetQuad(trianglesX, tX, v, v + 1, v + ring, v + ring + 1);
                }
                for (int q = 0; q < SizeX; q++, v++)
                {
                    tZ = SetQuad(trianglesZ, tZ, v, v + 1, v + ring, v + ring + 1);
                }
                for (int q = 0; q < SizeZ - 1; q++, v++)
                {
                    tX = SetQuad(trianglesX, tX, v, v + 1, v + ring, v + ring + 1);
                }
                tX = SetQuad(trianglesX, tX, v, v - ring + 1, v + ring, v + 1);
            }
            tY = CreateTopFace(trianglesY, tY, ring);
            tY = CreateBottomFace(trianglesY, tY, ring);

            //mesh.subMeshCount = 3;
            //mesh.SetTriangles(trianglesZ, 0);
            //mesh.SetTriangles(trianglesX, 1);
            //mesh.SetTriangles(trianglesY, 2);
            Mesh.AddFaceAndMaterial(0, trianglesZ);
            Mesh.AddFaceAndMaterial(1, trianglesX);
            Mesh.AddFaceAndMaterial(2, trianglesY);
        }

        private int CreateTopFace(int[] triangles, int t, int ring)
        {
            int v = ring * SizeY;
            for (int x = 0; x < SizeX - 1; x++, v++)
            {
                t = SetQuad(triangles, t, v, v + 1, v + ring - 1, v + ring);
            }
            t = SetQuad(triangles, t, v, v + 1, v + ring - 1, v + 2);

            int vMin = (ring * (SizeY + 1)) - 1;
            int vMid = vMin + 1;
            int vMax = v + 2;

            for (int z = 1; z < SizeZ - 1; z++, vMin--, vMid++, vMax++)
            {
                t = SetQuad(triangles, t, vMin, vMid, vMin - 1, vMid + SizeX - 1);
                for (int x = 1; x < SizeX - 1; x++, vMid++)
                {
                    t = SetQuad(triangles, t, vMid, vMid + 1, vMid + SizeX - 1, vMid + SizeX);
                }
                t = SetQuad(triangles, t, vMid, vMax, vMid + SizeX - 1, vMax + 1);
            }

            int vTop = vMin - 2;
            t = SetQuad(triangles, t, vMin, vMid, vTop + 1, vTop);
            for (int x = 1; x < SizeX - 1; x++, vTop--, vMid++)
            {
                t = SetQuad(triangles, t, vMid, vMid + 1, vTop, vTop - 1);
            }
            t = SetQuad(triangles, t, vMid, vTop - 2, vTop, vTop - 1);

            return t;
        }

        private int CreateBottomFace(int[] triangles, int t, int ring)
        {
            int v = 1;
            int vMid = vertices.Length - ((SizeX - 1) * (SizeZ - 1));
            t = SetQuad(triangles, t, ring - 1, vMid, 0, 1);
            for (int x = 1; x < SizeX - 1; x++, v++, vMid++)
            {
                t = SetQuad(triangles, t, vMid, vMid + 1, v, v + 1);
            }
            t = SetQuad(triangles, t, vMid, v + 2, v, v + 1);

            int vMin = ring - 2;
            vMid -= SizeX - 2;
            int vMax = v + 2;

            for (int z = 1; z < SizeZ - 1; z++, vMin--, vMid++, vMax++)
            {
                t = SetQuad(triangles, t, vMin, vMid + SizeX - 1, vMin + 1, vMid);
                for (int x = 1; x < SizeX - 1; x++, vMid++)
                {
                    t = SetQuad(triangles, t, vMid + SizeX - 1, vMid + SizeX, vMid, vMid + 1);
                }
                t = SetQuad(triangles, t, vMid + SizeX - 1, vMax + 1, vMid, vMax);
            }

            int vTop = vMin - 1;
            t = SetQuad(triangles, t, vTop + 1, vTop, vTop + 2, vMid);
            for (int x = 1; x < SizeX - 1; x++, vTop--, vMid++)
            {
                t = SetQuad(triangles, t, vTop, vTop - 1, vMid, vMid + 1);
            }
            t = SetQuad(triangles, t, vTop, vTop - 1, vMid, vTop - 2);

            return t;
        }

        private static int SetQuad(int[] triangles, int i, int v00, int v10, int v01, int v11)
        {
            triangles[i] = v00;
            triangles[i + 1] = triangles[i + 4] = v01;
            triangles[i + 2] = triangles[i + 3] = v10;
            triangles[i + 5] = v11;
            return i + 6;
        }

    }
}
