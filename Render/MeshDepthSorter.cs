// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using OpenToolkit.Mathematics;

namespace Aximo.Render
{
    public class MeshDepthSorter : IComparer<IRenderableObject>
    {
        public Camera Camera;
        private Vector3 CameraPosition;
        private bool DistanceCheck;

        public MeshDepthSorter(Camera camera, bool distanceCheck = true)
        {
            DistanceCheck = distanceCheck;
            Camera = camera;
            CameraPosition = camera.Position;
        }

        // returns: Draw-Order.
        public int Compare(IRenderableObject x, IRenderableObject y)
        {
            // Draw Early: -1.
            // Draw Late: 1

            if (x.UseTransparency != y.UseTransparency)
                return x.UseTransparency.CompareTo(y.UseTransparency);

            if (x.DrawPriority != y.DrawPriority)
                return x.DrawPriority.CompareTo(y.DrawPriority);

            var reverse = 0;
            if (x.UseTransparency) // no check for Y needed
                reverse = 1;

            if (!DistanceCheck)
                return 0; // TODO: Use index in array

            var boundsX = x as IBounds;
            var boundsY = y as IBounds;
            var hasBoundsX = boundsX != null ? 1 : 0;
            var hasBoundsY = boundsY != null ? 1 : 0;
            if (hasBoundsX != hasBoundsY)
                return hasBoundsX.CompareTo(hasBoundsY);

            if (hasBoundsX == 0) // no check for Y needed
                return 0;

            var distanceX = Vector3.Distance(CameraPosition, boundsX.WorldBounds.Center);
            var distanceY = Vector3.Distance(CameraPosition, boundsY.WorldBounds.Center);
            return distanceX.CompareTo(distanceY) * reverse;
        }
    }
}
