// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using OpenTK;
using OpenTK.Graphics.OpenGL4;

namespace Aximo.Render
{

    public class VertexLayoutDefinition
    {

        protected List<VertexLayoutDefinitionAttribute> Attributes = new List<VertexLayoutDefinitionAttribute>();

        private int _Stride;
        public int Stride => _Stride;

        protected virtual VertexLayoutDefinitionAttribute CreateAttributeInstance()
        {
            return new VertexLayoutDefinitionAttribute();
        }

        public virtual VertexLayoutDefinitionAttribute AddAttribute<T>(bool normalized = false)
        {
            return AddAttribute<T>(StructHelper.GetFieldsOf<T>(), normalized);
        }

        public virtual VertexLayoutDefinitionAttribute AddAttribute<T>(int size, bool normalized = false)
        {
            var offset = _Stride;
            _Stride += size * StructHelper.GetFieldSizeOf<T>();

            var attr = CreateAttributeInstance();
            attr.Size = size;
            attr.Type = StructHelper.GetVertexAttribPointerType<T>();
            attr.Normalized = normalized;
            attr.Stride = 0; // will be set in UpdateStride()
            attr.Offset = offset;

            Attributes.Add(attr);
            UpdateStride();
            return attr;
        }

        private void UpdateStride()
        {
            foreach (var attr in Attributes)
                attr.Stride = _Stride;
        }

    }

}
