// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;

using Aximo.Render;
using Aximo.Render.Objects;
using Aximo.Render.OpenGL;
using OpenToolkit;
using OpenToolkit.Mathematics;

namespace Aximo.Engine.Components.Lights
{
    public abstract class LightComponent : SceneComponent
    {
        private static Serilog.ILogger Log = Aximo.Log.ForContext<LightComponent>();
        internal ILightObject LightObject;

        protected abstract LightType LightType { get; }

        private float _Linear = 0.1f;
        public float Linear
        {
            get => _Linear;
            set { if (_Linear == value) return; _Linear = value; LightAttributesChanged = true; }
        }

        private float _Quadric = 0.1f;
        public float Quadric
        {
            get => _Quadric;
            set { if (_Quadric == value) return; _Quadric = value; LightAttributesChanged = true; }
        }

        private Vector4 _Color = Vector4.One;
        public Vector4 Color
        {
            get => _Color;
            set { if (_Color == value) return; _Color = value; LightAttributesChanged = true; }
        }

        private bool LightAttributesChanged;

        private static int ShadowIdx;

        internal override void SyncChanges()
        {
            if (!HasChanges)
                return;

            if (LightObject == null)
            {
                LightObject = new LightObject();
                LightObject.LightType = LightType;
                LightObject.Name = Name;
                //LightObject.ShadowTextureIndex = ShadowIdx++;
                InternalLightManager.RequestShadowLayer(LightObject);
                RenderContext.Current.AddObject(LightObject);
                LightAttributesChanged = true;
            }

            if (LightAttributesChanged)
            {
                LightAttributesChanged = false;
                LightObject.Linear = _Linear;
                LightObject.Quadric = _Quadric;
                LightObject.Color = _Color;
            }

            if (TransformChanged)
            {
                LightObject.Position = LocalToWorld().ExtractTranslation();
                TransformChanged = false;
            }

            base.SyncChanges();
        }

        internal override void DoDeallocation()
        {
            if (!HasDeallocation)
                return;

            if (LightObject == null)
                return;

            Log.Verbose("Set LightObject.Orphaned");

            LightObject.Orphaned = true;
            LightObject = null;

            base.DoDeallocation();
        }
    }
}
