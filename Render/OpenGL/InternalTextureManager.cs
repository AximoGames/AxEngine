// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using OpenToolkit.Mathematics;

namespace Aximo.Render.OpenGL
{
    public static class InternalTextureManager
    {
        private static Serilog.ILogger Log = Aximo.Log.ForContext(nameof(InternalTextureManager));

        public static RendererTexture White { get; private set; }
        public static RendererTexture Black { get; private set; }
        public static RendererTexture Gray { get; private set; }
        public static RendererTexture Red { get; private set; }
        public static RendererTexture Green { get; private set; }
        public static RendererTexture Blue { get; private set; }

        public static void Init()
        {
            White = new RendererTexture(new Vector3(1, 1, 1), nameof(White));
            Black = new RendererTexture(new Vector3(0, 0, 0), nameof(Black));
            Gray = new RendererTexture(new Vector3(0.5f, 0.5f, 0.5f), nameof(Gray));
            Red = new RendererTexture(new Vector3(1, 0, 0), nameof(Red));
            Green = new RendererTexture(new Vector3(0, 1, 0), nameof(Green));
            Blue = new RendererTexture(new Vector3(0, 0, 1), nameof(Blue));
        }

        private static Dictionary<int, WeakReference<RendererTexture>> References = new Dictionary<int, WeakReference<RendererTexture>>();

        internal static void DeleteOrphaned()
        {
            lock (References)
            {
                foreach (var reference in References.Values.ToArray())
                {
                    RendererTexture texture;
                    if (reference.TryGetTarget(out texture))
                    {
                        if (texture.Orphaned)
                            texture.Dispose();
                    }
                }
            }
        }

        public static void DumpInfo(bool listItems)
        {
            Log.Info("Allocated Textures: {TextureCount}", ReferencedCount());
            if (listItems)
            {
                lock (References)
                {
                    foreach (var reference in References.Values)
                    {
                        RendererTexture texture;
                        if (reference.TryGetTarget(out texture))
                        {
                            Log.Info("Texture #{Handle} {Name}", texture.Handle, texture.ObjectLabel);
                        }
                    }
                }
            }
        }

        public static int ReferencedCount()
        {
            return References.Count;
        }

        internal static void AddRef(RendererTexture texture)
        {
            lock (References)
                References.Set(texture.Handle, new WeakReference<RendererTexture>(texture));
        }

        internal static void RemoveRef(RendererTexture texture)
        {
            lock (References)
                References.Remove(texture.Handle);
        }
    }
}
