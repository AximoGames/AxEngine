// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Aximo.Render;
using Aximo.Render.Objects;
using Aximo.Render.OpenGL;
using OpenToolkit.Mathematics;

namespace Aximo.Engine.Components.Lights
{
    public abstract class LightComponent : SceneComponent
    {
        private static Serilog.ILogger Log = Aximo.Log.ForContext<LightComponent>();
        internal ILightObject LightObject;

        protected abstract LightType LightType { get; }

        private Vector4 _Color = Vector4.One;
        public Vector4 Color
        {
            get => _Color;
            set { if (_Color == value) return; _Color = value; LightAttributesChanged(); }
        }

        private bool _CastShadow = true;
        public bool CastShadow
        {
            get => _CastShadow;
            set { if (_CastShadow == value) return; _CastShadow = value; LightAttributesChanged(); }
        }

        private bool _LightAttributesChanged;
        private protected void LightAttributesChanged()
        {
            _LightAttributesChanged = true;
            HasChanges = true;
        }

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
                _LightAttributesChanged = true;
            }

            if (_LightAttributesChanged)
            {
                _LightAttributesChanged = false;
                if (this is PointLightComponent pl)
                {
                    LightObject.Linear = pl.Linear;
                    LightObject.Quadric = pl.Quadric;
                }
                LightObject.Color = _Color;
                LightObject.Shadows = CastShadow;
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
