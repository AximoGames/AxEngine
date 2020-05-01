using System;
using System.Collections.Generic;
using System.Text;
using OpenToolkit.Mathematics;
using System.Linq;
using Aximo.Render;
using System.Collections;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

namespace Aximo
{

    public class Mesh3
    {
        public IList<MeshComponent> Components { get; private set; } = new List<MeshComponent>();

        public MeshComponent AddComponent(MeshComponentType componentType)
        {
            var comp = MeshComponent.Create(componentType);
            Components.Add(comp);
            return comp;
        }

        public void AddComponent(MeshComponent component)
        {
            Components.Add(component);
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

        internal IList<InternalMeshFace> InternalMeshFaces = new List<InternalMeshFace>();

        /// <remarks>
        /// Fixed Face Types (Poly, quad, ...)
        /// </remarks>
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

        public Mesh3 CloneEmpty()
        {
            var mesh = new Mesh3();
            foreach (var c in Components)
                AddComponent(c.CloneEmpty());
            return mesh;
        }

        public int VertexCount => Components == null ? 0 : Components[0].Count;

        public int FaceCount => InternalMeshFaces.Count;

        public Mesh3 ToPolygons()
        {
            var newMesh = CloneEmpty();
            foreach (var face in InternalMeshFaces)
            {
                var newFace = new List<int>();
                if (face.IsTriangle)
                {
                    newFace.Add(newMesh.AddVertex(this, Indicies[face[0]]));
                    newFace.Add(newMesh.AddVertex(this, Indicies[face[1]]));
                    newFace.Add(newMesh.AddVertex(this, Indicies[face[2]]));
                }
                else if (face.IsQuad)
                {
                    newFace.Add(newMesh.AddVertex(this, Indicies[face[0]]));
                    newFace.Add(newMesh.AddVertex(this, Indicies[face[1]]));
                    newFace.Add(newMesh.AddVertex(this, Indicies[face[2]]));

                    newFace.Add(newMesh.AddVertex(this, Indicies[face[2]]));
                    newFace.Add(newMesh.AddVertex(this, Indicies[face[3]]));
                    newFace.Add(newMesh.AddVertex(this, Indicies[face[1]]));
                }
                else
                {
                    throw new NotSupportedException();
                }
                newMesh.AddFace(newFace);
            }
            return newMesh;
        }

        public IList<MeshFace<T>> FaceView<T>()
            where T : IVertex
        {
            return new FaceList<T>(this);
        }

        public int AddVertex(Mesh3 src, int index)
        {
            AddVertices(src, index, 1);
            return VertexCount - 1;
        }

        public void AddVertices(Mesh3 src, int start, int count)
        {
            foreach (var c in Components)
            {
                var srcComp = src.GetComponent(c.Type);
                if (srcComp == null)
                    continue;

                c.AddRange(srcComp, start, count);
            }
        }

        public MeshData<T> ToMeshData<T>()
            where T : struct, IVertex
        {
            var polyMesh = ToPolygons();
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
                if (c is MeshPositionComponent c1)
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
        }

        private class FaceList<T> : IList<MeshFace<T>>
            where T : IVertex
        {

            private Mesh3 Mesh;
            public FaceList(Mesh3 mesh)
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

            public VertexEnumerator(Mesh3 mesh)
            {
                Mesh = mesh;
                Visitor = VertexVisitor<T>.CreateVisitor(mesh);
            }

            private Mesh3 Mesh;

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

        private abstract class VertexVisitor<T> : IDisposable, IVertex, IDynamicArray<T>
            where T : IVertex
        {
            public static VertexVisitor<T> CreateVisitor(Mesh3 mesh)
            {
                var type = typeof(T);

                if (type == typeof(IVertexPosition3))
                    return (VertexVisitor<T>)(object)new VertexPosition3Visitor(mesh);

                if (type == typeof(IVertexPosNormalUV))
                    return (VertexVisitor<T>)(object)new VertexPosNormalUVVisitor(mesh);

                throw new NotSupportedException(type.Name);
            }

            internal VertexVisitor(Mesh3 mesh)
            {
                Mesh = mesh;
            }

            protected Mesh3 Mesh;
            public int Index;
            public IVertex Value
            {
                get => this;
                set => Set(value);
            }

            public int Count => Mesh.Components[0].Count;

            T IArray<T>.this[int index] { get => (T)this[index]; set => this[index] = value; }

            public IVertex this[int index]
            {
                get
                {
                    Index = index;
                    return this;
                }
                set
                {
                    Index = index;
                    Set(value);
                }
            }

            protected virtual void Set(IVertex vertex)
            {
                throw new NotImplementedException();
            }

            public void Dispose()
            {
            }

            public abstract void SetLength(int length);

        }

        private class VertexPosition3Visitor : VertexVisitor<IVertexPosition3>, IVertexPosition3
        {
            private IDynamicArray<Vector3> PositionComponent;

            public VertexPosition3Visitor(Mesh3 mesh) : base(mesh)
            {
                PositionComponent = mesh.GetComponent<MeshPositionComponent>();
            }

            public override void SetLength(int length)
            {
                PositionComponent.SetLength(length);
            }

            private void EnsureSize()
            {
                SetLength(Index + 1);
            }

            public Vector3 Position
            {
                get => PositionComponent.GetValueWithExpand(Index);
                set => PositionComponent.SetValueWithExpand(Index, value);
            }
            protected override void Set(IVertex vertex)
            {
                if (vertex is IVertexPosition3 position)
                    Position = position.Position;
            }

        }

        private class VertexPosNormalUVVisitor : VertexVisitor<IVertexPosNormalUV>, IVertexPosNormalUV
        {
            private IDynamicArray<Vector3> PositionComponent;
            private IDynamicArray<Vector3> NormalComponent;
            private IDynamicArray<Vector2> UVComponent;

            public VertexPosNormalUVVisitor(Mesh3 mesh) : base(mesh)
            {
                PositionComponent = mesh.GetComponent<MeshPositionComponent>();
                NormalComponent = mesh.GetComponent<MeshNormalComponent>();
                UVComponent = mesh.GetComponent<MeshUVComponent>();
            }

            public Vector3 Position
            {
                get
                {
                    EnsureSize();
                    return PositionComponent[Index];
                }

                set
                {
                    EnsureSize();
                    PositionComponent[Index] = value;
                }
            }

            public Vector3 Normal
            {
                get
                {
                    EnsureSize();
                    return NormalComponent[Index];
                }

                set
                {
                    EnsureSize();
                    NormalComponent[Index] = value;
                }
            }

            public Vector2 UV
            {
                get
                {
                    EnsureSize();
                    return UVComponent[Index];
                }

                set
                {
                    EnsureSize();
                    UVComponent[Index] = value;
                }
            }

            public override void SetLength(int length)
            {
                PositionComponent.SetLength(length);
                NormalComponent.SetLength(length);
                UVComponent.SetLength(length);
            }

            private void EnsureSize()
            {
                SetLength(Index + 1);
            }

            protected override void Set(IVertex vertex)
            {
                EnsureSize();
                if (vertex is IVertexPosition3 position)
                    Position = position.Position;
                if (vertex is IVertexNormal normal)
                    Normal = normal.Normal;
                if (vertex is IVertexUV uv)
                    UV = uv.UV;
            }

        }

    }

}
