// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;
using Aximo.Render;
using OpenToolkit;
using OpenToolkit.Mathematics;
using SixLabors.Primitives;

#pragma warning disable CA1044 // Properties should not be write only

namespace Aximo.Engine
{

    public class ScreenTextureComponent : StaticMeshComponent
    {
        public ScreenTextureComponent() : base(MeshDataBuilder.Quad(), MaterialManager.CreateScreenMaterial())
        {
        }

        public ScreenTextureComponent(string texturePath) : base(MeshDataBuilder.Quad(), MaterialManager.CreateScreenMaterial(texturePath))
        {
        }

        internal override void SyncChanges()
        {
            base.SyncChanges();
        }

        public RectangleF RectangleUV
        {
            set
            {
                var pos = new Vector3(
                    ((value.X + (value.Width / 2f)) * 2) - 1.0f,
                    ((1 - (value.Y + (value.Height / 2f))) * 2) - 1.0f,
                    0);

                var scale = new Vector3(value.Width, -value.Height, 1.0f);
                RelativeTranslation = pos;
                RelativeScale = scale;
            }
        }

        public RectangleF RectanglePixels
        {
            set
            {
                var pos1 = new Vector2(value.X, value.Y) * RenderContext.Current.PixelToUVFactor;
                var pos2 = new Vector2(value.Right, value.Bottom) * RenderContext.Current.PixelToUVFactor;

                RectangleUV = new RectangleF(pos1.X, pos1.Y, pos2.X - pos1.X, pos2.Y - pos1.Y);
            }
        }

    }

}
