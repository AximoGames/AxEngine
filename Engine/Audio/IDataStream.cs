﻿// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using OpenToolkit.Audio;

namespace Aximo.Engine.Audio
{

    public interface IDataStream
    {
    }

    public interface IDataStream<T> : IDataStream
    {
    }
}