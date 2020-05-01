using System;
using System.Collections;
using System.Collections.Generic;

namespace Aximo
{
    public interface IArray<T>
    {
        T this[int index] { get; set; }
        int Count { get; }
    }

    internal class ArrayEnumerator<T> : IEnumerator<T>
    {

        private IArray<T> Array;
        public ArrayEnumerator(IArray<T> array)
        {
            Array = array;
        }

        private int Index = -1;

        public T Current => Array[Index];

        object IEnumerator.Current => Current;

        public void Dispose()
        {
        }

        public bool MoveNext()
        {
            if (Index >= Array.Count - 1)
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
