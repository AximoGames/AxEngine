using System;
using OpenTK;

namespace ProcEngine
{
    public class Cam
    {
        public Vector3 Position;
        public float Pitch = -0.3f;
        public Vector3 Up = Vector3.UnitZ;
        public float Facing = (float)Math.PI / 2 + 0.15f;
        public float AspectRatio;
        private float _fov = (float)Math.PI / 4;

        public Cam(Vector3 position, float aspectRatio)
        {
            Position = position;
            AspectRatio = aspectRatio;
        }

        public Cam(Vector3 position)
        {
            Position = position;
        }

        // The field of view (FOV) is the vertical angle of the camera view, this has been discussed more in depth in a
        // previous tutorial, but in this tutorial you have also learned how we can use this to simulate a zoom feature.
        // We convert from degrees to radians as soon as the property is set to improve performance
        public float Fov
        {
            get => MathHelper.RadiansToDegrees(_fov);
            set
            {
                var angle = MathHelper.Clamp(value, 1f, 45f);
                _fov = MathHelper.DegreesToRadians(angle);
            }
        }

        public Matrix4 GetViewMatrix()
        {
            var loc = new Vector3(Position.X, Position.Y, Position.Z);
            var lookatPoint = new Vector3((float)Math.Cos(Facing), (float)Math.Sin(Facing), (float)Math.Sin(Pitch));
            return Matrix4.LookAt(loc, loc + lookatPoint, Up);
        }

        public Matrix4 GetProjectionMatrix()
        {
            //return Matrix4.CreatePerspectiveFieldOfView(_fov, AspectRatio, 0.1f, 100f);
            return Matrix4.CreatePerspectiveFieldOfView(_fov, AspectRatio, 0.01f, 150f);
        }

        public void SetAspectRatio(float width, float height)
        {
            AspectRatio = width / height;
        }

    }

}