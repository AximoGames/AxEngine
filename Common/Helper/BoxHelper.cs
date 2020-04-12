// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using OpenToolkit.Mathematics;
using SixLabors.ImageSharp;

namespace Aximo
{

    public static class BoxHelper
    {
        public static Box2 FromSize(Vector2 location, Vector2 size)
        {
            return new Box2(location, location + size);
        }

    }

}
