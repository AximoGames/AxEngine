// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Aximo.Render.OpenGL
{
    public class VertexLayoutBindedAttribute : VertexLayoutDefinitionAttribute
    {
        public int Index;

        internal override string GetDumpString()
        {
            return $"{base.GetDumpString()}, Index: {Index}";
        }

        public override int GetHashCode()
        {
            return Hashing.HashInteger(Index, base.GetHashCode());
        }
    }
}
