using System;

namespace Aximo.Engine.Mesh2
{
    public interface IDynamicArray<T> : IArray<T>
    {
        void SetLength(int length);
    }
}
