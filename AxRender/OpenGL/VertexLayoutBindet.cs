// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using OpenTK;
using OpenTK.Graphics.OpenGL4;

namespace Aximo.Render
{

    public class VertexLayoutBinded : VertexLayoutDefinition
    {

        protected override VertexLayoutDefinitionAttribute CreateAttributeInstance()
        {
            return new VertexLayoutBindedAttribute();
        }

        public virtual VertexLayoutBindedAttribute AddAttribute<T>(int index, bool normalized = false)
        {
            return AddAttribute<T>(index, StructHelper.GetFieldsOf<T>(), normalized);
        }

        protected override void AddAttribute(VertexLayoutDefinitionAttribute attr)
        {
            if (!(attr is VertexLayoutBindedAttribute))
                throw new InvalidOperationException();
            base.AddAttribute(attr);
        }

        public virtual VertexLayoutBindedAttribute AddAttribute<T>(int index, int size, bool normalized = false)
        {
            var attr = base.AddAttribute<T>("", size, normalized) as VertexLayoutBindedAttribute;
            attr.Index = index;
            return attr;
        }

        internal void InitAttributes()
        {
            ObjectManager.PushDebugGroup("Init", "VertexLayout");
            foreach (VertexLayoutBindedAttribute attr in Attributes)
            {
                if (attr.Index < 0)
                    continue;

                GL.EnableVertexAttribArray(attr.Index);
                GL.VertexAttribPointer(attr.Index, attr.Size, attr.Type, attr.Normalized, attr.Stride, attr.Offset);
            }
            ObjectManager.PopDebugGroup();
        }

    }

}
