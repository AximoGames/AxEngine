// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using OpenToolkit.Mathematics;

namespace Aximo.Render
{
    public class OrthographicCamera : Camera
    {
        public override CameraType Type => CameraType.Orthographic;

        private Vector2 _Size = new Vector2(20, 20);
        public Vector2 Size
        {
            get { return _Size; }
            set { if (_Size == value) return; _Size = value; OnCameraChanged(); }
        }

        public OrthographicCamera(Vector3 position) : base(position)
        {
        }

        protected override Matrix4 GetProjectionMatrix()
        {
            // float near_plane = 0.01f;
            // float FarPlane = 7.5f;

            //return Matrix4.CreateOrthographicOffCenter(-Size.X / 2, Size.X / 2, -Size.Y / 2, Size.Y / 2, NearPlane, FarPlane);
            return Matrix4.CreateOrthographic(Size.X, Size.Y, NearPlane, FarPlane);
        }
    }
}
