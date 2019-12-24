
using OpenTK;
using LearnOpenTK.Common;
using System;
using System.Collections.Generic;
using OpenTK.Graphics.OpenGL4;

namespace ProcEngine
{

    public class VertexLayout
    {

        private int _Stride;

        public void AddAttribute(int index, int size, Type type, bool normalized)
        {
            var offset = _Stride;
            _Stride += size * GetSizeOf(type);
            var attr = new VertexLayoutAttribute
            {
                Index = index,
                Size = size,
                Type = GetVertexAttribPointerType(type),
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
            foreach (var attr in Attributes)
            {
                GL.EnableVertexAttribArray(attr.Index);
                GL.VertexAttribPointer(attr.Index, attr.Size, attr.Type, attr.Normalized, attr.Stride, attr.Offset);
            }
        }

        private static VertexAttribPointerType GetVertexAttribPointerType(Type type)
        {
            if (type == typeof(float))
                return VertexAttribPointerType.Float;
            throw new NotImplementedException();
        }

        private static int GetSizeOf(Type type)
        {
            if (type == typeof(float))
                return 4;
            throw new NotImplementedException();
        }

        private List<VertexLayoutAttribute> Attributes = new List<VertexLayoutAttribute>();

    }

}
