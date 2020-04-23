using System;
using System.Collections.Generic;
using System.Text;
using OpenToolkit.Mathematics;
using System.Linq;
using Aximo.Render;
using System.Collections;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

namespace Aximo.Engine.Mesh2
{

    public enum MeshComponentType
    {
        Position,
        Normal,
        UV,
        Color,
    }

    public abstract class MeshComponent
    {
        public virtual MeshComponentType Type { get; protected set; }

        public static MeshComponent Create(MeshComponentType componentType)
        {
            switch (componentType)
            {
                case MeshComponentType.Position:
                    return new MeshComponent<Vector3>(componentType);
                case MeshComponentType.Normal:
                    return new MeshComponent<Vector3>(componentType);
                case MeshComponentType.Color:
                    return new MeshComponent<Vector4>(componentType);
                case MeshComponentType.UV:
                    return new MeshComponent<Vector2>(componentType);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public abstract int Count { get; }

        public abstract void AddRange(MeshComponent src, int start, int count);

        public abstract MeshComponent CloneEmpty();
    }

    public class MeshPositionComponent : MeshComponent<Vector3>
    {
        public MeshPositionComponent()
            : base(MeshComponentType.Position)
        {
        }

        public override MeshComponent CloneEmpty() => new MeshPositionComponent();

    }

    public class MeshNormalComponent : MeshComponent<Vector3>
    {
        public MeshNormalComponent()
            : base(MeshComponentType.Normal)
        {
        }

        public override MeshComponent CloneEmpty() => new MeshNormalComponent();
    }

    public class MeshColorComponent : MeshComponent<Vector4>
    {
        public MeshColorComponent()
            : base(MeshComponentType.Color)
        {
        }

        public override MeshComponent CloneEmpty() => new MeshColorComponent();
    }

    public class MeshUVComponent : MeshComponent<Vector2>
    {
        public MeshUVComponent()
            : base(MeshComponentType.UV)
        {
        }

        public override MeshComponent CloneEmpty() => new MeshUVComponent();
    }

    public class MeshComponent<T> : MeshComponent, IDynamicArray<T>
    {
        public IList<T> Values { get; private set; } = new List<T>();

        public T this[int index]
        {
            get => Values[index];
            set => Values[index] = value;
        }

        public override int Count => Values.Count;

        public MeshComponent(MeshComponentType componentType)
        {
            Type = componentType;
        }

        public int Add(T value)
        {
            Values.Add(value);
            return Values.Count - 1;
        }

        public override void AddRange(MeshComponent src, int start, int count)
        {
            AddRange((MeshComponent<T>)src, start, count);
        }

        public void AddRange(MeshComponent<T> src, int start, int count)
        {
            for (var i = start; i < count; i++)
                Values.Add(src.Values[i]);
        }

        public override MeshComponent CloneEmpty() => new MeshComponent<T>(Type);

        public void SetLength(int length)
        {
            var sizeDiff = length - Values.Count;
            for (var i = 0; i < sizeDiff; i++)
                Values.Add(default(T));
        }
    }

    public enum MeshFaceType
    {
        None = 0,
        Point = 1,
        Line = 2,
        Triangle = 3,
        Quad = 4,
        Ngon = 5,
    }

    // public unsafe struct Triangle
    // {
    //     private fixed int blah[3];
    //     public Span<int> Points => new Span<byte>(Unsafe.AsPointer(ref blah[0], 3));
    // }

    public struct MeshFace
    {


        public MeshFace(params int[] indicies)
        {
            //var triangeles = MemoryMarshal.Cast<int, Triangle>(indicies.AsSpan());

            int n = 14;
            int i = 3;
            var ngon = indicies.AsSpan().Slice(i * n, n);

            if (indicies.Length > 0)
                Indicies = indicies.ToArray();
            else
                Indicies = new int[] { };
        }

        public void Add(int vertexIndex)
        {
            var newArray = new int[Indicies.Length + 1];
            Indicies.CopyTo(newArray, 0);
            newArray[newArray.Length - 1] = vertexIndex;
            Indicies = newArray;
        }

        public int this[int index] => Indicies[index];

        public int[] Indicies { get; private set; }
        public int Count => Indicies.Length;

        public bool IsPoint => Indicies.Length == 1;
        public bool IsLine => Indicies.Length == 2;
        public bool IsTriangle => Indicies.Length == 3;
        public bool IsQuad => Indicies.Length == 4;
        public bool IsNgon => Indicies.Length > 4;

        public MeshFaceType Type
        {
            get
            {
                var cnt = Indicies.Length;
                if (cnt > 4)
                    return MeshFaceType.Ngon;
                else
                    return (MeshFaceType)cnt;
            }
        }
    }

    public class Mesh
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

        public IList<MeshFace> Faces { get; private set; } = new List<MeshFace>();

        public MeshFace AddFace(params int[] indicies)
        {
            var face = new MeshFace(indicies);
            Faces.Add(face);
            return face;
        }

        public void AddFace(MeshFace face)
        {
            Faces.Add(face);
        }

        public Mesh CloneEmpty()
        {
            var mesh = new Mesh();
            foreach (var c in Components)
                AddComponent(c.CloneEmpty());
            return mesh;
        }

        public int VertexCount => Components == null ? 0 : Components[0].Count;

        public Mesh ToPolygons()
        {
            var newMesh = CloneEmpty();
            foreach (var face in Faces)
            {
                var newFace = new MeshFace();
                if (face.IsTriangle)
                {
                    newFace.Add(newMesh.AddVertex(this, face[0]));
                    newFace.Add(newMesh.AddVertex(this, face[1]));
                    newFace.Add(newMesh.AddVertex(this, face[2]));
                }
                else if (face.IsQuad)
                {
                    newFace.Add(newMesh.AddVertex(this, face[0]));
                    newFace.Add(newMesh.AddVertex(this, face[1]));
                    newFace.Add(newMesh.AddVertex(this, face[2]));

                    newFace.Add(newMesh.AddVertex(this, face[2]));
                    newFace.Add(newMesh.AddVertex(this, face[3]));
                    newFace.Add(newMesh.AddVertex(this, face[1]));
                }
                else
                {
                    throw new NotSupportedException();
                }
                newMesh.AddFace(newFace);
            }
            return newMesh;
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

        public MeshData<T> ToMeshData<T>()
            where T : struct, IVertex
        {
            var polyMesh = ToPolygons();
            var data = new T[polyMesh.VertexCount];
            for (var i = 0; i < polyMesh.VertexCount; i++)
                data[i] = polyMesh.ToMeshData<T>(i);
            var indicies = polyMesh.Faces.ToIndiciesList().Select(index => (ushort)index).ToArray();
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

        public IList<T> View<T>()
            where T : IVertex
        {
            return new VertexList<T>(VertexVisitor<T>.CreateVisitor(this));
        }

        private class VertexList<T> : IList<T>
            where T : IVertex
        {

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
                throw new NotImplementedException();
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

        private abstract class VertexVisitor<T> : IDisposable, IVertex, IDynamicArray<T>
            where T : IVertex
        {
            public static VertexVisitor<T> CreateVisitor(Mesh mesh)
            {
                var type = typeof(T);

                if (type == typeof(IVertexPosition3))
                    return (VertexVisitor<T>)(object)new VertexPosition3Visitor(mesh);

                throw new NotSupportedException(type.Name);
            }

            internal VertexVisitor(Mesh mesh)
            {
                Mesh = mesh;
            }

            protected Mesh Mesh;
            public int Index;
            public IVertex Value
            {
                get => this;
                set => Set(value);
            }

            public int Count => throw new NotImplementedException();

            T IArray<T>.this[int index] { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

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

            public void SetLength(int length)
            {
                throw new NotImplementedException();
            }
        }

        private class VertexPosition3Visitor : VertexVisitor<IVertexPosition3>, IVertexPosition3, IVertexColor
        {
            private IDynamicArray<Vector3> PositionComponent;

            public VertexPosition3Visitor(Mesh mesh) : base(mesh)
            {
            }

            public Vector3 Position
            {
                get => PositionComponent.GetValueWithExpand(Index);
                set => PositionComponent.SetValueWithExpand(Index, value);
            }
            public Vector4 Color { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

            protected override void Set(IVertex vertex)
            {
                if (vertex is IVertexPosition3 position)
                    Position = position.Position;
            }

        }

    }

    public static class FaceExtensions
    {
        public static IEnumerable<int> ToIndiciesList(this IList<MeshFace> faces)
        {
            return faces.SelectMany(face => face.Indicies);
        }
    }

    public interface IArray<T>
    {
        T this[int index] { get; set; }
        int Count { get; }
    }

    public interface IDynamicArray<T> : IArray<T>
    {
        void SetLength(int length);
    }

    internal static class DynamicArrayExtensions
    {
        static internal void SetValueWithExpand<T>(this IDynamicArray<T> array, int index, T value)
        {
            array.SetLength(index + 1);
            array[index] = value;
        }

        static internal T GetValueWithExpand<T>(this IDynamicArray<T> array, int index)
        {
            array.SetLength(index + 1);
            return array[index];
        }

        internal static void Clear<T>(this IDynamicArray<T> array)
        {
            array.SetLength(0);
        }

        internal static bool Contains<T>(this IDynamicArray<T> array, T value)
        {
            var length = array.Count;
            for (var i = 0; i < length; i++)
                if (array[i].Equals(value))
                    return true;
            return false;
        }

        internal static bool Remove<T>(this IDynamicArray<T> array, T value)
        {
            throw new NotImplementedException();
        }

        internal static void RemoveAt<T>(this IDynamicArray<T> array, int index)
        {
            throw new NotImplementedException();
        }

    }

}
