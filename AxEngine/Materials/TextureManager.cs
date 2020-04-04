// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;

namespace Aximo.Engine
{
    public static class TextureManager
    {

        private static Dictionary<string, GameTexture> FileTextures = new Dictionary<string, GameTexture>();

        public static GameTexture GetFromFile(string path)
        {
            lock (FileTextures)
            {
                GameTexture txt;
                if (FileTextures.TryGetValue(path, out txt))
                    return txt;
                else
                    FileTextures.Add(path, txt = GameTexture.CreateFromFile(path));
                return txt;
            }
        }

    }

}
