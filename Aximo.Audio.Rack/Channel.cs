// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Aximo.Engine.Audio
{

    public class Channel
    {
        public int Number;
        public bool Active;
        public float Voltage;
        public Port Port;

        public Channel(int number, Port port)
        {
            Number = number;
            Port = port;
        }
    }
}
