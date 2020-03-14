// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using OpenTK;
using OpenTK.Graphics.OpenGL4;

namespace Aximo.Render
{

    public class VertexLayout
    {

        private int _Stride;
        public int Stride => _Stride;
        public void AddAttribute<T>(int index, bool normalized = false)
        {
            AddAttribute<T>(index, GetElementsOf<T>(), normalized);
        }

        public void AddAttribute<T>(int index, int size, bool normalized = false)
        {
            var offset = _Stride;
            _Stride += size * GetSizeOf<T>();
            var attr = new VertexLayoutAttribute
            {
                Index = index,
                Size = size,
                Type = GetVertexAttribPointerType<T>(),
                Normalized = normalized,
                Stride = 0, // will be set in UpdateStride()
                Offset = offset,
            };
            Attributes.Add(attr);
            UpdateStride();
        }

        private void UpdateStride()
        {
            foreach (var attr in Attributes)
                attr.Stride = _Stride;
        }

        internal void InitAttributes()
        {
            ObjectManager.PushDebugGroup("Init", "VertexLayout");
            foreach (var attr in Attributes)
            {
                if (attr.Index < 0)
                    continue;
                GL.EnableVertexAttribArray(attr.Index);
                GL.VertexAttribPointer(attr.Index, attr.Size, attr.Type, attr.Normalized, attr.Stride, attr.Offset);
            }
            ObjectManager.PopDebugGroup();
        }

        private static VertexAttribPointerType GetVertexAttribPointerType<T>()
        {
            var type = typeof(T);
            if (type == typeof(float))
                return VertexAttribPointerType.Float;
            throw new NotImplementedException();
        }

        private static int GetSizeOf<T>()
        {
            var type = typeof(T);
            if (type == typeof(float))
                return 4;
            throw new NotImplementedException();
        }

        private static int GetElementsOf<T>()
        {
            var type = typeof(T);
            if (type == typeof(float))
                return 1;
            if (type == typeof(Vector4))
                return 4;
            if (type == typeof(Vector3))
                return 3;
            if (type == typeof(Vector2))
                return 1;
            throw new NotImplementedException();
        }

        private List<VertexLayoutAttribute> Attributes = new List<VertexLayoutAttribute>();

    }

}
