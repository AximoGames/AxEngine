// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;

using Aximo.Render;

namespace Aximo.Engine
{

    public abstract class LightComponent : ActorComponent
    {
        internal ILightObject LightObject;

        internal override void ApplyChanges()
        {
            if (!HasChanges)
                return;

            if (LightObject == null)
                LightObject = new LightObject();

            base.ApplyChanges();
        }
    }

}
