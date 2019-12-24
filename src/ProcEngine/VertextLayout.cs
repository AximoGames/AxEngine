
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

        public void AddAttribute(int index, int size, Type type, bool normalized, int offset)
        {
            _Stride += size * GetSizeOf(type);
            InitAttribsList.Add(() =>
            {
                GL.EnableVertexAttribArray(index);
                GL.VertexAttribPointer(index, size, GetVertexAttribPointerType(type), normalized, _Stride, offset);
            });
        }

        internal void InitAttributes()
        {
            foreach (var act in InitAttribsList)
            {
                act();
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

        private List<Action> InitAttribsList = new List<Action>();

    }

}
