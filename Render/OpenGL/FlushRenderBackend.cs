// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Aximo.Render
{
    public enum FlushRenderBackend
    {
        /// <summary>
        /// No flush.
        /// </summary>
        None,
        /// <summary>
        /// Flush after render pipeline
        /// </summary>
        End,
        /// <summary>
        /// Flush after every draw call
        /// </summary>
        Draw,
    }

}
