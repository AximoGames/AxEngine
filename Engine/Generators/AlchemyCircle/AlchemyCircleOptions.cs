// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

// Reference: https://github.com/CiaccoDavide/Alchemy-Circles-Generator
// Credits: CiaccoDavide

using SixLabors.ImageSharp;

namespace Aximo.Generators.AlchemyCircle
{
    public class AlchemyCircleOptions
    {
        public int Seed;
        public Color BackgroundColor = Color.Transparent;
        public Color Color = Color.White;
        public int Size;
        public int Thickness = 4;

        public override string ToString()
        {
            return $"Seed{Seed}Size{Size}Thickness{Thickness}Color{Color}BackgroundColor{BackgroundColor}";
        }
    }
}
