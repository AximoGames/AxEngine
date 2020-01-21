using System;
using OpenTK;

namespace ProcEngine
{
    public enum CameraType
    {
        PerspectiveFieldOfView,
        Orthographic,
    }

    public class PerspectiveFieldOfViewCamera : Camera
    {
        public override CameraType Type => CameraType.PerspectiveFieldOfView;

        public PerspectiveFieldOfViewCamera(Vector3 position, float aspectRatio) : base(position)
        {
            AspectRatio = aspectRatio;
        }

        public PerspectiveFieldOfViewCamera(Vector3 position) : base(position)
        {
        }

        public override Matrix4 GetProjectionMatrix()
        {
            return Matrix4.CreatePerspectiveFieldOfView(_fov, AspectRatio, NearPlane, FarPlane);
        }

    }

    public class OrthographicCamera : Camera
    {
        public override CameraType Type => CameraType.Orthographic;

        public OrthographicCamera(Vector3 position) : base(position)
        {
        }

        public override Matrix4 GetProjectionMatrix()
        {
            float near_plane = 0.01f;
            float far_plane = 7.5f;

            return Matrix4.CreateOrthographicOffCenter(-10, 10, -10, 10, near_plane, far_plane);
        }

    }

    public abstract class Camera
    {
        public Vector3 Position;
        public abstract CameraType Type { get; }
        public float Pitch = -0.3f;
        protected float _fov = (float)Math.PI / 4;

        public float FarPlane;
        public float NearPlane;

        public Camera(Vector3 position)
        {
            Position = position;
        }

        public Vector3 Up = Vector3.UnitZ;
        public float Facing = (float)Math.PI / 2 + 0.15f;
        public float AspectRatio;

        public Vector3? LookAt;

        public virtual Matrix4 GetViewMatrix()
        {
            var loc = new Vector3(Position.X, Position.Y, Position.Z);

            Vector3 lookatPoint;
            if (LookAt != null)
            {
                lookatPoint = (Vector3)LookAt;
                return Matrix4.LookAt(loc, lookatPoint, Up);
            }
            else
            {
                lookatPoint = new Vector3((float)Math.Cos(Facing) * (float)Math.Cos(Pitch), (float)Math.Sin(Facing) * (float)Math.Cos(Pitch), (float)Math.Sin(Pitch));
                return Matrix4.LookAt(loc, loc + lookatPoint, Up);
            }
        }

        public abstract Matrix4 GetProjectionMatrix();

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

        public void SetAspectRatio(float width, float height)
        {
            AspectRatio = width / height;
        }
    }

}