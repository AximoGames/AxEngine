// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using OpenToolkit.Mathematics;

namespace Aximo.Render
{
    public interface IVertex
    {
    }

    public interface IVertexPosition : IVertex
    {
    }

    public interface IVertexPosition3 : IVertexPosition
    {
        Vector3 Position { get; set; }
    }

    public interface IVertexPosition2 : IVertexPosition
    {
        Vector2 Position { get; set; }
    }

    public interface IVertexNormal : IVertex
    {
        Vector3 Normal { get; set; }
    }

    public interface IVertexColor : IVertex
    {
        Vector4 Color { get; set; }
    }

    public interface IVertexUV : IVertex
    {
        Vector2 UV { get; set; }
    }

}
