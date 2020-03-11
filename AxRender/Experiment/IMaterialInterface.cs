// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;

namespace Aximo.Render
{
    public interface IMaterialInterface
    {
        ICollection<GTexture> Textures { get; }
        void AddTexture(GTexture texture);
        void RemoveTexture(GTexture texture);
    }

}
