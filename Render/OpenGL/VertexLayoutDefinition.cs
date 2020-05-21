// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using OpenToolkit;
using OpenToolkit.Graphics.OpenGL4;

namespace Aximo.Render.OpenGL
{
    public class VertexLayoutDefinition
    {
        public VertexLayoutBinded BindToShader(Shader shader)
        {
            var layout = new VertexLayoutBinded();
            layout._Stride = _Stride;
            foreach (var srcAttr in Attributes)
            {
                var attr = layout.CreateAttributeInstance() as VertexLayoutBindedAttribute;
                srcAttr.CopyTo(attr);
                attr.Index = shader.GetAttribLocation(shader.AttributeNameForField(attr.Name));
                layout.AddAttribute(attr);
            }
            return layout;
        }

        private List<VertexLayoutDefinitionAttribute> _Attributes;
        public ReadOnlyCollection<VertexLayoutDefinitionAttribute> Attributes { get; private set; }

        public VertexLayoutDefinition()
        {
            _Attributes = new List<VertexLayoutDefinitionAttribute>();
            Attributes = new ReadOnlyCollection<VertexLayoutDefinitionAttribute>(_Attributes);
        }

        public static VertexLayoutDefinition CreateDefinitionFromVertexStruct<T>()
        {
            return CreateDefinitionFromVertexStruct(typeof(T));
        }

        public static VertexLayoutDefinition CreateDefinitionFromVertexStruct(Type type)
        {
            var layout = new VertexLayoutDefinition();
            foreach (var field in type.GetFields())
            {
                layout.AddAttribute(field.FieldType, field.Name);
            }
            return layout;
        }

        protected virtual void AddAttribute(VertexLayoutDefinitionAttribute attr)
        {
            _Attributes.Add(attr);
        }

        private int _Stride;
        public int Stride => _Stride;

        protected virtual VertexLayoutDefinitionAttribute CreateAttributeInstance()
        {
            return new VertexLayoutDefinitionAttribute();
        }

        public virtual VertexLayoutDefinitionAttribute AddAttribute<T>(string name, bool normalized = false)
        {
            return AddAttribute(typeof(T), name, normalized);
        }

        public virtual VertexLayoutDefinitionAttribute AddAttribute(Type type, string name, bool normalized = false)
        {
            return AddAttribute(type, name, StructHelper.GetFieldsOf(type), normalized);
        }

        public virtual VertexLayoutDefinitionAttribute AddAttribute<T>(string name, int size, bool normalized = false)
        {
            return AddAttribute(typeof(T), name, size, normalized);
        }

        public virtual VertexLayoutDefinitionAttribute AddAttribute(Type type, string name, int size, bool normalized = false)
        {
            var offset = _Stride;
            _Stride += size * StructHelper.GetFieldSizeOf(type);

            var attr = CreateAttributeInstance();
            attr.Name = name;
            attr.Size = size;
            attr.Type = StructHelper.GetVertexAttribPointerType(type);
            attr.Normalized = normalized;
            attr.Stride = 0; // will be set in UpdateStride()
            attr.Offset = offset;

            AddAttribute(attr);
            UpdateStride();
            return attr;
        }

        private void UpdateStride()
        {
            foreach (var attr in Attributes)
                attr.Stride = _Stride;
        }

        public void DumpDebug()
        {
            Console.WriteLine($"Dump of {GetType().Name}. Stride: {Stride}");
            foreach (var attr in Attributes)
                Console.WriteLine(attr.GetDumpString());
        }
    }
}
