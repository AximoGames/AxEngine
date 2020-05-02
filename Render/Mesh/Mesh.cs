﻿// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using Aximo.Render;
using OpenToolkit.Mathematics;

namespace Aximo
{
    public class Mesh
    {
        public MeshFaceType PrimitiveType = MeshFaceType.Triangle;

        public IList<MeshComponent> Components { get; private set; } = new List<MeshComponent>();

        public MeshComponent AddComponent(MeshComponentType componentType)
        {
            var comp = MeshComponent.Create(componentType);
            Components.Add(comp);
            return comp;
        }

        public T AddComponent<T>()
            where T : MeshComponent, new()
        {
            var comp = new T();
            AddComponent(comp);
            return comp;
        }

        public void AddComponent(MeshComponent component)
        {
            Components.Add(component);
        }

        public void AddComponents<T>()
            where T : IVertex
        {
            var type = typeof(T);
            if (typeof(IVertexPosition3).IsAssignableFrom(type))
            {
                AddComponent(new MeshPosition3Component());
            }
            if (typeof(IVertexPosition2).IsAssignableFrom(type))
            {
                AddComponent(new MeshPosition2Component());
            }
            if (typeof(IVertexNormal).IsAssignableFrom(type))
            {
                AddComponent(new MeshNormalComponent());
            }
            if (typeof(IVertexUV).IsAssignableFrom(type))
            {
                AddComponent(new MeshUVComponent());
            }
            if (typeof(IVertexColor).IsAssignableFrom(type))
            {
                AddComponent(new MeshColorComponent());
            }
        }

        // public static void CreateFromVertices<VertexDataPosNormalColor>()
        // {
        // }

        // public static void CreateFromVertices(VertexDataPos2UV[] vertices)
        // {
        // }

        public static Mesh CreateSphere(int divisions = 2)
        {
            var ico = new Render.Objects.Util.IcoSphere.IcoSphereMesh(divisions);
            return CreateFromVertices(ico.Vertices, ico.Indicies);
        }

        public static Mesh CreateCube()
        {
            return CreateFromVertices(DataHelper.DefaultCube);
        }

        public static Mesh CreateFromVertices<T>(T[] vertices, int[] indicies = null, MeshFaceType primitiveType = MeshFaceType.Triangle)
            where T : IVertex
        {
            var mesh = new Mesh();
            mesh.PrimitiveType = primitiveType;
            mesh.AddComponents<T>();
            mesh.AddVertices(vertices);

            if (indicies != null)
            {
                var faceCount = indicies.Length / 3;
                for (var faceIndex = 0; faceIndex < faceCount; faceIndex++)
                {
                    var i = faceIndex * 3;
                    mesh.AddFace(indicies[i + 0], indicies[i + 1], indicies[i + 2]);
                }
            }

            return mesh;
        }

        public Mesh Clone()
        {
            var newMesh = CloneEmpty();
            newMesh.PrimitiveType = PrimitiveType;
            var newFace = new List<int>();
            foreach (var comp in Components)
            {
                var destComp = newMesh.GetComponent(comp.Type);
                destComp.AddRange(comp, 0, comp.Count);
            }
            newMesh.Indicies.AddRange(Indicies);
            newMesh.InternalMeshFaces.AddRange(InternalMeshFaces);
            foreach (var matId in MaterialIds)
                newMesh.MaterialIds.Add(matId);
            return newMesh;
        }

        public void Expand()
        {
            ReplaceInternal(Expanded());
        }

        private void ReplaceInternal(Mesh src)
        {
            Components = src.Components;
            InternalMeshFaces = src.InternalMeshFaces;
            Indicies = src.Indicies;
            MaterialIds = src.MaterialIds;
            PrimitiveType = src.PrimitiveType;
        }

        public Mesh Expanded()
        {
            if (Indicies.Count == 0)
                return Clone();

            var newMesh = CloneEmpty();
            newMesh.PrimitiveType = PrimitiveType;
            var newFace = new List<int>();
            foreach (var face in InternalMeshFaces)
            {
                for (var i = 0; i < face.Count; i++)
                {
                    newFace.Add(newMesh.AddVertex(this, Indicies[face[i]]));
                }
                newMesh.AddFace(newFace);
                newFace.Clear();
            }
            foreach (var matId in MaterialIds)
                newMesh.MaterialIds.Add(matId);

            return newMesh;
        }

        public T GetComponent<T>()
            where T : MeshComponent
        {
            foreach (var comp in Components)
                if (comp is T)
                    return (T)comp;
            return null;
        }

        public T GetComponent<T>(MeshComponentType componentType)
            where T : MeshComponent
        {
            foreach (var comp in Components)
                if (comp.Type == componentType && comp is T)
                    return (T)comp;
            return default;
        }

        public MeshComponent GetComponent(MeshComponentType componentType)
        {
            foreach (var comp in Components)
                if (comp.Type == componentType)
                    return comp;
            return null;
        }

        public bool HasComponent<T>()
            where T : MeshComponent
        {
            return GetComponent<T>() != null;
        }

        public bool HasComponent(MeshComponentType componentType)
        {
            return GetComponent(componentType) != null;
        }

        public bool HasComponents<T1, T2>()
            where T1 : MeshComponent
            where T2 : MeshComponent
        {
            return HasComponent<T1>() && HasComponent<T2>();
        }

        public bool HasComponents<T1, T2, T3>()
            where T1 : MeshComponent
            where T2 : MeshComponent
            where T3 : MeshComponent
        {
            return HasComponent<T1>() && HasComponent<T2>() && HasComponent<T3>();
        }

        public bool IsComponents<T1, T2>()
            where T1 : MeshComponent
            where T2 : MeshComponent
        {
            return Components.Count == 2 && HasComponents<T1, T1>();
        }

        public bool IsComponents<T1, T2, T3>()
            where T1 : MeshComponent
            where T2 : MeshComponent
            where T3 : MeshComponent
        {
            return Components.Count == 3 && HasComponents<T1, T1, T3>();
        }

        internal IList<InternalMeshFace> InternalMeshFaces = new List<InternalMeshFace>();

        /// <summary>
        /// Mixed Face Types (Poly, quad, ...)
        /// </summary>
        internal IList<int> Indicies = new List<int>();

        public void AddFace(params int[] indicies)
        {
            var face = new InternalMeshFace()
            {
                StartIndex = Indicies.Count,
                Count = indicies.Length,
            };
            InternalMeshFaces.Add(face);
            for (var i = 0; i < indicies.Length; i++)
                Indicies.Add(indicies[i]);
        }

        public void AddFace(IList<int> indicies)
        {
            var face = new InternalMeshFace()
            {
                StartIndex = Indicies.Count,
                Count = indicies.Count,
            };
            InternalMeshFaces.Add(face);
            for (var i = 0; i < indicies.Count; i++)
                Indicies.Add(indicies[i]);
        }

        public void AddFaceAndMaterial(int materialId, params int[] indicies)
        {
            var face = new InternalMeshFace()
            {
                StartIndex = Indicies.Count,
                Count = indicies.Length,
                MaterialId = materialId,
            };

            // TODO: Improve. Track every matrialID.
            MaterialIds.Add(materialId);

            InternalMeshFaces.Add(face);
            for (var i = 0; i < indicies.Length; i++)
                Indicies.Add(indicies[i]);
        }

        public void AddFaceFromVerticesTail()
        {
            AddFaceFromVerticesTail(PrimitiveType);
        }

        public void AddFaceFromVerticesTail(MeshFaceType faceType)
        {
            switch (faceType)
            {
                case MeshFaceType.Point:
                    AddFaceFromVerticesPosition(VertexCount - 1, faceType);
                    break;
                case MeshFaceType.Line:
                    AddFaceFromVerticesPosition(VertexCount - 2, faceType);
                    break;
                case MeshFaceType.Triangle:
                    AddFaceFromVerticesPosition(VertexCount - 3, faceType);
                    break;
                case MeshFaceType.Quad:
                    AddFaceFromVerticesPosition(VertexCount - 4, faceType);
                    break;
                default:
                    throw new NotSupportedException(PrimitiveType.ToString());
            }
        }

        public void AddFaceFromVerticesPosition(int vertexStartIndex)
        {
            AddFaceFromVerticesPosition(vertexStartIndex, PrimitiveType);
        }

        public void AddFaceFromVerticesPosition(int vertexStartIndex, MeshFaceType faceType)
        {
            switch (faceType)
            {
                case MeshFaceType.Point:
                    AddFace(vertexStartIndex);
                    break;
                case MeshFaceType.Line:
                    AddFace(vertexStartIndex, vertexStartIndex + 1);
                    break;
                case MeshFaceType.Triangle:
                    AddFace(vertexStartIndex, vertexStartIndex + 1, vertexStartIndex + 2);
                    break;
                case MeshFaceType.Quad:
                    AddFace(vertexStartIndex, vertexStartIndex + 1, vertexStartIndex + 2, vertexStartIndex + 3);
                    break;
                default:
                    throw new NotSupportedException(PrimitiveType.ToString());
            }
        }

        public void CreateFacesFromIndicies()
        {
            CreateFacesFromIndicies(PrimitiveType);
        }

        public void CreateFacesFromIndicies(MeshFaceType type)
        {
            InternalMeshFaces.Clear();
            var n = (int)type;
            var faceCount = Indicies.Count / n;
            for (var i = 0; i < faceCount; i++)
                InternalMeshFaces.Add(new InternalMeshFace()
                {
                    StartIndex = i * n,
                    Count = n,
                    MaterialId = 0,
                });

            MaterialIds.Add(0);
        }

        public void CreateFacesAndIndicies()
        {
            CreateFacesAndIndicies(PrimitiveType);
        }

        public void CreateFacesAndIndicies(MeshFaceType type)
        {
            Indicies.Clear();
            InternalMeshFaces.Clear();
            var n = (int)type;
            var faceCount = VertexCount / n;
            for (var i = 0; i < faceCount; i++)
                AddFaceFromVerticesPosition(i * n, type);
        }

        public void SetMaterial(int faceIndex, int materialId)
        {
            var face = InternalMeshFaces[faceIndex];
            face.MaterialId = materialId;
            InternalMeshFaces[faceIndex] = face;
            MaterialIds.Add(materialId);
        }

        public void ReplaceMaterial(int oldMaterialId, int newMaterialid)
        {
            for (var i = 0; i < InternalMeshFaces.Count; i++)
            {
                var face = InternalMeshFaces[i];
                if (face.MaterialId == oldMaterialId)
                {
                    face.MaterialId = newMaterialid;
                    InternalMeshFaces[i] = face;
                }
            }
            MaterialIds.Remove(oldMaterialId);
            MaterialIds.Add(newMaterialid);
        }

        public void SetNormals(IEnumerable<Vector3> normals)
        {
            var comp = GetComponent<MeshNormalComponent>();
            if (comp == null)
                comp = AddComponent<MeshNormalComponent>();

            comp.Clear();
            comp.AddRange(normals);
        }

        public void RecalculateNormals(float angle)
        {
            NormalSolver.RecalculateNormals(this, angle);
        }

        public void AddMesh(Mesh mesh)
        {
            AddMeshInternal(mesh, -1);
        }

        public void AddMesh(Mesh mesh, int filterMaterialId)
        {
            AddMeshInternal(mesh, filterMaterialId);
        }

        public void AddMesh(Mesh mesh, int filterMaterialId, int newMaterialId)
        {
            AddMeshInternal(mesh, filterMaterialId, newMaterialId);
        }

        internal void AddMeshInternal(Mesh mesh, int filterMaterialId = -1, int newMaterialId = -1)
        {
            if (mesh.FaceCount == 0)
                mesh.CreateFacesAndIndicies();

            var newFace = new List<int>();
            foreach (var face in mesh.InternalMeshFaces)
            {
                if (filterMaterialId > -1 && face.MaterialId != filterMaterialId)
                    continue;

                for (var i = 0; i < face.Count; i++)
                {
                    newFace.Add(AddVertex(mesh, mesh.Indicies[face[i]]));
                }
                AddFace(newFace);
                newFace.Clear();
            }
        }

        public Mesh CloneEmpty()
        {
            var mesh = new Mesh();
            foreach (var c in Components)
                mesh.AddComponent(c.CloneEmpty());
            return mesh;
        }

        public void Translate(Vector3 direction)
        {
            var comp = GetComponent<MeshPosition3Component>();
            for (var i = 0; i < comp.Count; i++)
                comp[i] += direction;
        }

        public void Translate(Vector2 direction)
        {
            var comp = GetComponent<MeshPosition2Component>();
            for (var i = 0; i < comp.Count; i++)
                comp[i] += direction;
        }

        public int VertexCount => Components == null ? 0 : Components[0].Count;

        public int FaceCount => InternalMeshFaces.Count;

        public Mesh ToPrimitive()
        {
            return ToPrimitive(PrimitiveType, 0);
        }

        public Mesh ToPrimitive(MeshFaceType faceType, int materialId)
        {
            var newMesh = CloneEmpty();
            newMesh.PrimitiveType = faceType;

            if (FaceCount == 0)
                CreateFacesAndIndicies();

            var newFace = new List<int>();
            foreach (var face in InternalMeshFaces)
            {
                if (face.MaterialId != materialId)
                    continue;

                if (face.IsPoint && faceType == MeshFaceType.Point)
                {
                    newFace.Add(newMesh.AddVertex(this, Indicies[face[0]]));
                    newMesh.AddFace(newFace);
                    newFace.Clear();
                }
                else if (face.IsLine && faceType == MeshFaceType.Line)
                {
                    newFace.Add(newMesh.AddVertex(this, Indicies[face[0]]));
                    newFace.Add(newMesh.AddVertex(this, Indicies[face[1]]));
                    newMesh.AddFace(newFace);
                    newFace.Clear();
                }
                else if (face.IsTriangle && faceType == MeshFaceType.Triangle)
                {
                    newFace.Add(newMesh.AddVertex(this, Indicies[face[0]]));
                    newFace.Add(newMesh.AddVertex(this, Indicies[face[1]]));
                    newFace.Add(newMesh.AddVertex(this, Indicies[face[2]]));
                    newMesh.AddFace(newFace);
                    newFace.Clear();
                }
                else if (face.IsQuad && (faceType == MeshFaceType.Triangle))
                {
                    newFace.Add(newMesh.AddVertex(this, Indicies[face[0]]));
                    newFace.Add(newMesh.AddVertex(this, Indicies[face[1]]));
                    newFace.Add(newMesh.AddVertex(this, Indicies[face[2]]));
                    newMesh.AddFace(newFace);
                    newFace.Clear();

                    newFace.Add(newMesh.AddVertex(this, Indicies[face[2]]));
                    newFace.Add(newMesh.AddVertex(this, Indicies[face[3]]));
                    newFace.Add(newMesh.AddVertex(this, Indicies[face[1]]));
                    newMesh.AddFace(newFace);
                    newFace.Clear();
                }
                else
                {
                    throw new NotSupportedException(face.Type.ToString());
                }
            }
            return newMesh;
        }

        public IList<MeshFace<T>> FaceView<T>()
            where T : IVertex
        {
            return new FaceList<T>(this);
        }

        public int AddVertex(Mesh src, int index)
        {
            AddVertices(src, index, 1);
            return VertexCount - 1;
        }

        public void AddVertices(Mesh src, int start, int count)
        {
            foreach (var c in Components)
            {
                var srcComp = src.GetComponent(c.Type);
                if (srcComp == null)
                    continue;

                c.AddRange(srcComp, start, count);
            }
        }

        public void AddVertices<T>(ICollection<T> values)
            where T : IVertex
        {
            var type = typeof(T);
            if (type.IsInterface)
            {
                View<T>().AddRange(values);
                return;
            }

            if (typeof(IVertexPosition3).IsAssignableFrom(type))
                GetComponent<MeshPosition3Component>()?.AddRange(values);
            if (typeof(IVertexPosition2).IsAssignableFrom(type))
                GetComponent<MeshPosition2Component>()?.AddRange(values);
            if (typeof(IVertexNormal).IsAssignableFrom(type))
                GetComponent<MeshNormalComponent>()?.AddRange(values);
            if (typeof(IVertexUV).IsAssignableFrom(type))
                GetComponent<MeshUVComponent>()?.AddRange(values);
            if (typeof(IVertexColor).IsAssignableFrom(type))
                GetComponent<MeshColorComponent>()?.AddRange(values);
        }

        public MeshData<T> ToMeshData<T>()
            where T : struct, IVertex
        {
            var polyMesh = ToPrimitive();
            var data = new T[polyMesh.VertexCount];
            for (var i = 0; i < polyMesh.VertexCount; i++)
                data[i] = polyMesh.ToMeshData<T>(i);
            var indicies = polyMesh.Indicies.Select(index => (ushort)index).ToArray();
            return new MeshData<T>(new BufferData1D<T>(data), new BufferData1D<ushort>(indicies));
        }

        private T ToMeshData<T>(int index)
            where T : struct, IVertex
        {
            Vector3 pos = Vector3.Zero;
            Vector3 normal = Vector3.UnitZ;
            Vector2 uv = Vector2.Zero;

            foreach (var c in Components)
            {
                if (c is MeshPosition3Component c1)
                    pos = c1.Values[index];
            }

            if (typeof(T) == typeof(VertexDataPosNormalUV))
                return (T)(object)new VertexDataPosNormalUV(pos, normal, uv);

            throw new NotSupportedException();
        }

        public void Visit<T>(Action<T> callback)
            where T : IVertex
        {
            var enu = new VertexEnumerator<T>(this);
            while (enu.MoveNext())
                callback(enu.Current);
        }

        public void Visit<T>(Action<T, int> callback)
            where T : IVertex
        {
            var enu = new VertexEnumerator<T>(this);
            while (enu.MoveNext())
                callback(enu.Current, enu.Visitor.Index);
        }

        public VertexList<T> View<T>()
            where T : IVertex
        {
            return new VertexList<T>(VertexVisitor<T>.CreateVisitor(this));
        }

        public T[] GetVertexArray<T>()
        {
            if (typeof(T) == typeof(VertexDataPosNormalUV))
                return View<IVertexPosNormalUV>().ToArray<T>();
            if (typeof(T) == typeof(VertexDataPosNormalColor))
                return View<IVertexPosNormalColor>().ToArray<T>();
            if (typeof(T) == typeof(VertexDataPos2UV))
                return View<IVertexPos2UV>().ToArray<T>();

            throw new NotSupportedException(typeof(T).Name);
        }

        public int[] GetIndiciesArray()
        {
            return Indicies.ToArray();
        }

        public int[] GetIndiciesArray(int materialId)
        {
            var indicies = new List<int>();
            foreach (var face in InternalMeshFaces)
            {
                if (face.MaterialId == materialId)
                    for (var i = 0; i < face.Count; i++)
                        indicies.Add(Indicies[face[i]]);
            }
            return indicies.ToArray();
        }

        public T[] GetIndiciesArray<T>()
            where T : unmanaged
        {
            return GetIndiciesArray().Select(v => (T)Convert.ChangeType(v, typeof(T))).ToArray();
        }

        public BufferData1D<T> GetIndiciesBuffer<T>()
            where T : unmanaged
        {
            if (Indicies.Count == 0)
                return null;
            return new BufferData1D<T>(GetIndiciesArray<T>());
        }

        public bool IsCompatible<T>()
        where T : IVertex
        {
            if (typeof(T) == typeof(IVertexPosNormalUV))
                return IsComponents<MeshPosition3Component, MeshNormalComponent, MeshUVComponent>();
            if (typeof(T) == typeof(IVertexPosNormalColor))
                return IsComponents<MeshPosition3Component, MeshNormalComponent, MeshColorComponent>();
            if (typeof(T) == typeof(IVertexPosColor))
                return IsComponents<MeshPosition3Component, MeshColorComponent>();
            if (typeof(T) == typeof(IVertexPos2UV))
                return IsComponents<MeshPosition2Component, MeshUVComponent>();

            return false;
        }

        public MeshData GetMeshData(int materialId)
        {
            //return ToPolygons().GetMeshData();
            return ToPrimitive(PrimitiveType, materialId).GetMeshData();
            //return GetMeshData();
        }

        private MeshData GetMeshData()
        {
            if (IsCompatible<IVertexPosNormalUV>())
                return new MeshData<VertexDataPosNormalUV>(View<IVertexPosNormalUV>().ToBuffer<VertexDataPosNormalUV>(), GetIndiciesBuffer<ushort>(), GetPrimitiveType());
            if (IsCompatible<IVertexPosNormalColor>())
                return new MeshData<VertexDataPosNormalColor>(View<IVertexPosNormalColor>().ToBuffer<VertexDataPosNormalColor>(), GetIndiciesBuffer<ushort>(), GetPrimitiveType());
            if (IsCompatible<IVertexPosColor>())
                return new MeshData<VertexDataPosColor>(View<IVertexPosColor>().ToBuffer<VertexDataPosColor>(), GetIndiciesBuffer<ushort>(), GetPrimitiveType());
            if (IsCompatible<IVertexPos2UV>())
                return new MeshData<VertexDataPos2UV>(View<IVertexPos2UV>().ToBuffer<VertexDataPos2UV>(), GetIndiciesBuffer<ushort>(), GetPrimitiveType());

            throw new NotSupportedException();
        }

        private AxPrimitiveType GetPrimitiveType()
        {
            switch (PrimitiveType)
            {
                case MeshFaceType.Line:
                    return AxPrimitiveType.Lines;
                case MeshFaceType.Triangle:
                    return AxPrimitiveType.Triangles;
                default:
                    throw new NotSupportedException(PrimitiveType.ToString());
            }
        }

        public int MaterialCount => MaterialIds.Count;

        public HashSet<int> MaterialIds { get; private set; } = new HashSet<int>(new int[] { 0 });

        public class VertexList<T> : IList<T>
            where T : IVertex
        {
            // public void AddRange(params T[] items)
            // {

            // }

            public VertexList(IDynamicArray<T> innerList)
            {
                InnerList = innerList;
            }

            private IDynamicArray<T> InnerList;

            public T this[int index]
            {
                get => InnerList[index];
                set => InnerList[index] = value;
            }

            public int Count => InnerList.Count;

            public bool IsReadOnly => false;

            public void Add(T item)
            {
                InnerList.SetValueWithExpand(InnerList.Count, item);
            }

            public void Clear()
            {
                InnerList.Clear();
            }

            public bool Contains(T item)
            {
                return InnerList.Contains(item);
            }

            public void CopyTo(T[] array, int arrayIndex)
            {
                throw new NotImplementedException();
            }

            public IEnumerator<T> GetEnumerator()
            {
                return new ArrayEnumerator<T>(InnerList);
            }

            public int IndexOf(T item)
            {
                throw new NotImplementedException();
            }

            public void Insert(int index, T item)
            {
                throw new NotImplementedException();
            }

            public bool Remove(T item)
            {
                return InnerList.Remove(item);
            }

            public void RemoveAt(int index)
            {
                InnerList.RemoveAt(index);
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                throw new NotImplementedException();
            }

            public T[] ToArray()
            {
                var array = new T[Count];
                for (var i = 0; i < Count; i++)
                    array[i] = (T)this[i].Clone();
                return array;
            }

            public BufferData1D<T> ToBuffer()
            {
                return new BufferData1D<T>(ToArray());
            }

            public BufferData1D<TDestination> ToBuffer<TDestination>()
            {
                return new BufferData1D<TDestination>(ToArray<TDestination>());
            }

            public TDestination[] ToArray<TDestination>()
            {
                if (typeof(TDestination) == typeof(T))
                    return (TDestination[])(object)ToArray();

                if (typeof(TDestination) == typeof(VertexDataPosNormalUV))
                {
                    var array = new VertexDataPosNormalUV[Count];
                    for (var i = 0; i < Count; i++)
                        array[i].Set((IVertexPosNormalUV)this[i]);
                    return (TDestination[])(object)array;
                }

                if (typeof(TDestination) == typeof(VertexDataPosNormalColor))
                {
                    var array = new VertexDataPosNormalColor[Count];
                    for (var i = 0; i < Count; i++)
                        array[i].Set((IVertexPosNormalColor)this[i]);
                    return (TDestination[])(object)array;
                }

                if (typeof(TDestination) == typeof(VertexDataPosColor))
                {
                    var array = new VertexDataPosColor[Count];
                    for (var i = 0; i < Count; i++)
                        array[i].Set((IVertexPosColor)this[i]);
                    return (TDestination[])(object)array;
                }

                if (typeof(TDestination) == typeof(VertexDataPos2UV))
                {
                    var array = new VertexDataPos2UV[Count];
                    for (var i = 0; i < Count; i++)
                        array[i].Set((IVertexPos2UV)this[i]);
                    return (TDestination[])(object)array;
                }

                throw new NotSupportedException(typeof(TDestination).Name);
            }
        }

        private class FaceList<T> : IList<MeshFace<T>>
            where T : IVertex
        {
            private Mesh Mesh;
            public FaceList(Mesh mesh)
            {
                Mesh = mesh;
                VertexView = Mesh.View<T>();
            }

            private IList<T> VertexView;
            private IList<InternalMeshFace> Faces => Mesh.InternalMeshFaces;

            public int Count => Faces.Count;

            public bool IsReadOnly => false;

            public MeshFace<T> this[int index]
            {
                get => GetFace(index);
                set => throw new NotImplementedException();
            }

            private MeshFace<T> GetFace(InternalMeshFace face)
            {
                return new MeshFace<T>(VertexView, Mesh.Indicies, face);
            }

            private MeshFace<T> GetFace(int faceIndex)
            {
                return GetFace(Faces[faceIndex]);
            }

            public int IndexOf(MeshFace<T> item)
            {
                throw new NotImplementedException();
            }

            public void Insert(int index, MeshFace<T> item)
            {
                throw new NotImplementedException();
            }

            public void RemoveAt(int index)
            {
                throw new NotImplementedException();
            }

            public void Add(MeshFace<T> item)
            {
                throw new NotImplementedException();
            }

            public void Clear()
            {
                throw new NotImplementedException();
            }

            public bool Contains(MeshFace<T> item)
            {
                throw new NotImplementedException();
            }

            public void CopyTo(MeshFace<T>[] array, int arrayIndex)
            {
                throw new NotImplementedException();
            }

            public bool Remove(MeshFace<T> item)
            {
                throw new NotImplementedException();
            }

            public IEnumerator<MeshFace<T>> GetEnumerator()
            {
                return new FaceEnumerator(this);
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return new FaceEnumerator(this);
            }

            private class FaceEnumerator : IEnumerator<MeshFace<T>>
            {
                private FaceList<T> Faces;
                private int Index = -1;
                public FaceEnumerator(FaceList<T> faces)
                {
                    Faces = faces;
                }

                public MeshFace<T> Current => Faces[Index];

                object IEnumerator.Current => Current;

                public void Dispose()
                {
                }

                public bool MoveNext()
                {
                    if (Index >= Faces.Count - 1)
                        return false;

                    Index++;
                    return true;
                }

                public void Reset()
                {
                    Index = -1;
                }
            }
        }

        private class VertexEnumerator<T> : IEnumerator<T>
            where T : IVertex
        {
            public VertexEnumerator(Mesh mesh)
            {
                Mesh = mesh;
                Visitor = VertexVisitor<T>.CreateVisitor(mesh);
            }

            private Mesh Mesh;

            internal VertexVisitor<T> Visitor;

            public T Current => (T)Visitor.Value;

            object IEnumerator.Current => Visitor.Value;

            public void Dispose()
            {
                Visitor?.Dispose();
                Visitor = null;
            }

            public bool MoveNext()
            {
                if (Visitor.Index >= Mesh.VertexCount - 1)
                    return false;

                Visitor.Index++;
                return true;
            }

            public void Reset()
            {
                Visitor.Index = -1;
            }
        }
    }
}
