// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Aximo.VertexData;

namespace Aximo
{
    public class MeshComponent<T> : MeshComponent, IDynamicArray<T>
        where T : unmanaged
    {
        public List<T> _Values = new List<T>();
        public IList<T> Values => _Values;

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
            for (var i = start; i < start + count; i++)
                _Values.Add(src.Values[i]);
        }

        public void AddRange(IEnumerable<T> values)
        {
            _Values.AddRange(values);
        }

        public virtual void AddRange(IEnumerable<IVertex> values)
        {
            throw new NotSupportedException(values.GetType().Name);
        }

        public void AddRange<TVertex>(IEnumerable<TVertex> values)
            where TVertex : IVertex
        {
            AddRange(values.Cast<IVertex>());
        }

        public void Clear() => Values.Clear();

        public override MeshComponent CloneEmpty() => new MeshComponent<T>(Type);

        public void SetLength(int length)
        {
            var sizeDiff = length - Values.Count;
            for (var i = 0; i < sizeDiff; i++)
                Values.Add(default(T));
        }
    }
}
