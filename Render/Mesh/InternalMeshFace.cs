using System;

namespace Aximo
{
    // public unsafe struct Triangle
    // {
    //     private fixed int blah[3];
    //     public Span<int> Points => new Span<byte>(Unsafe.AsPointer(ref blah[0], 3));
    // }

    internal struct InternalMeshFace
    {
        public int StartIndex;
        public int Count;

        public int this[int faceVertexIndex]
        {
            get
            {
                if (faceVertexIndex >= Count)
                    throw new IndexOutOfRangeException();

                return StartIndex + faceVertexIndex;
            }
        }

        public bool IsPoint => Count == 1;
        public bool IsLine => Count == 2;
        public bool IsTriangle => Count == 3;
        public bool IsQuad => Count == 4;
        public bool IsNgon => Count > 4;

        public MeshFaceType Type
        {
            get
            {
                var cnt = Count;
                if (cnt > 4)
                    return MeshFaceType.Ngon;
                else
                    return (MeshFaceType)cnt;
            }
        }

    }

}
