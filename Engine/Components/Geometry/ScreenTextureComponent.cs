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
            if (!HasChanges)
                return;

            base.SyncChanges();

            if (OrderChanged)
            {
                RenderableObject.Order = Order;
                OrderChanged = false;
            }
        }

        private bool OrderChanged = false;
        private int Order = 5000;

        internal void SetOrders()
        {
            SetOrders(Order);
        }

        internal void SetOrders(int order)
        {
            Visit<ScreenTextureComponent>(c =>
            {
                c.Order = order++;
                c.OrderChanged = true;
                c.Update();

            });
        }

        protected override void OnAttached()
        {
            if (Parent is ScreenTextureComponent s)
                s.SetOrders();
            else
                SetOrders();
        }

    }

}
