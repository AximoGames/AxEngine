// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Runtime.CompilerServices;
using Aximo.Render.Pipelines;
using OpenToolkit.Graphics.OpenGL4;

namespace Aximo.Render.OpenGL
{
    public static class ObjectManager
    {
        public static void SetLabel(IObjectLabel obj)
        {
            if (!Enabled)
                return;

            // if (MaxLabelLength == -1)
            //     MaxLabelLength = GL.GetInteger((GetIndexedPName)(int)All.MaxLabelLength);

            var name = obj.ObjectLabelIdentifier.ToString() + " " + obj.Handle.ToString() + " [" + obj.ObjectLabel + "]";
            //RenderContext.Current.LogInfoMessage("Label:" + name);
            //name = "xxx\0";
            GL.ObjectLabel(obj.ObjectLabelIdentifier, obj.Handle, -1, name);
        }

        public static void PushDebugGroup(string verb, string nome)
        {
            if (!Enabled)
                return;

            var name = $"{verb} {nome}]";
            GL.PushDebugGroup(DebugSourceExternal.DebugSourceApplication, -1, name.Length, name);
        }

        public static bool Enabled
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
            get
            {
                return Renderer.Current.UseFrameDebug;
            }
        }

        public static void PushDebugGroup(string verb, IRenderObject obj)
        {
            if (!Enabled)
                return;

            var objName = obj.Name;
            if (string.IsNullOrEmpty(objName))
                objName = obj.GetType().Name;
            var name = $"{verb} GameObject {obj.Id} [{objName}]";
            GL.PushDebugGroup(DebugSourceExternal.DebugSourceApplication, obj.Id, name.Length, name);
        }

        public static void PushDebugGroup(string verb, IRenderPipeline obj)
        {
            if (!Enabled)
                return;

            var name = $"{verb} RenderPipeline {obj.GetType().Name}]";
            GL.PushDebugGroup(DebugSourceExternal.DebugSourceApplication, -1, name.Length, name);
        }

        public static void PopDebugGroup()
        {
            if (!Enabled)
                return;

            GL.PopDebugGroup();
        }
    }

    public interface IObjectHandle
    {
        int Handle { get; }
    }

    public interface IObjectIdentifier : IObjectHandle
    {
        ObjectLabelIdentifier ObjectLabelIdentifier { get; }
    }

    public interface IObjectLabel : IObjectIdentifier
    {
        string ObjectLabel { get; set; }
    }
}
