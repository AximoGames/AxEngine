using System;

namespace Aximo
{
    public interface IDynamicArray<T> : IArray<T>
    {
        void SetLength(int length);
    }
}
