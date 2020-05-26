// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections;
using System.Collections.Generic;
using Aximo.VertexData;

namespace Aximo
{
    public class MeshVertexList<T> : IList<T>
        where T : IVertex
    {
        // public void AddRange(params T[] items)
        // {

        // }

        public MeshVertexList(IDynamicArray<T> innerList)
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
}
