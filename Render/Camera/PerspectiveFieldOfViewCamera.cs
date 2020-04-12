// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using OpenToolkit;
using OpenToolkit.Mathematics;

namespace Aximo.Render
{
    public class PerspectiveFieldOfViewCamera : Camera
    {
        public override CameraType Type => CameraType.PerspectiveFieldOfView;

        public PerspectiveFieldOfViewCamera(Vector3 position, float aspectRatio) : base(position)
        {
            AspectRatio = aspectRatio;
        }

        public PerspectiveFieldOfViewCamera(Vector3 position) : base(position)
        {
        }

        protected override Matrix4 GetProjectionMatrix()
        {
            return Matrix4.CreatePerspectiveFieldOfView(FovInternal, AspectRatio, NearPlane, FarPlane);
        }
    }
}
