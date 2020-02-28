using System;
using System.Collections.Generic;
using OpenTK;

namespace AxEngine
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

        protected override Matrix4 GetProjectionMatrix()
        {
            return Matrix4.CreatePerspectiveFieldOfView(FovInternal, AspectRatio, NearPlane, FarPlane);
        }

    }

    public class OrthographicCamera : Camera
    {
        public override CameraType Type => CameraType.Orthographic;

        public OrthographicCamera(Vector3 position) : base(position)
        {
        }

        protected override Matrix4 GetProjectionMatrix()
        {
            // float near_plane = 0.01f;
            // float far_plane = 7.5f;

            return Matrix4.CreateOrthographicOffCenter(-10, 10, -10, 10, NearPlane, FarPlane);
        }

    }

    public abstract class Camera : IPosition, IData
    {
        private Dictionary<string, object> Data = new Dictionary<string, object>();

        public T GetData<T>(string name, T defaultValue = default)
        {
            return IDataHelper.GetData(Data, name, defaultValue);
        }

        public bool HasData(string name)
        {
            return IDataHelper.HasData(Data, name);
        }

        public bool SetData<T>(string name, T value, T defaultValue = default)
        {
            return IDataHelper.SetData(Data, name, value, defaultValue);
        }

        protected Vector3 _Position;
        public Vector3 Position
        {
            get { return _Position; }
            set { if (_Position == value) return; _Position = value; OnCameraChanged(); }
        }

        public abstract CameraType Type { get; }

        public float _Pitch = -0.3f;
        public float Pitch
        {
            get { return _Pitch; }
            set { if (_Pitch == value) return; _Pitch = value; OnCameraChanged(); }
        }

        protected float _FovInternal = (float)Math.PI / 4;
        internal float FovInternal
        {
            get { return _FovInternal; }
            set { if (_FovInternal == value) return; _FovInternal = value; OnCameraChanged(); }
        }

        private float _NearPlane = 1;
        public float NearPlane
        {
            get { return _NearPlane; }
            set { if (_NearPlane == value) return; _NearPlane = value; OnCameraChanged(); }
        }

        private float _FarPlane = 100;
        public float FarPlane
        {
            get { return _FarPlane; }
            set { if (_FarPlane == value) return; _FarPlane = value; OnCameraChanged(); }
        }

        protected void OnCameraChanged()
        {
            ViewMatrix = GetViewMatrix(Position);
            ProjectionMatrix = GetProjectionMatrix();
            CameraChangedInternal?.Invoke();
        }

        internal event CameraChangedDelegate CameraChangedInternal;

        internal delegate void CameraChangedDelegate();

        public Camera(Vector3 position)
        {
            Position = position;
        }

        public Vector3 _Up = Vector3.UnitZ;
        public Vector3 Up
        {
            get { return _Up; }
            set { if (_Up == value) return; _Up = value; OnCameraChanged(); }
        }

        public float _Facing = (float)Math.PI / 2 + 0.15f;
        public float Facing
        {
            get { return _Facing; }
            set { if (_Facing == value) return; _Facing = value; OnCameraChanged(); }
        }

        public float _AspectRatio = 1.0f;
        public float AspectRatio
        {
            get { return _AspectRatio; }
            set { if (_AspectRatio == value) return; _AspectRatio = value; OnCameraChanged(); }
        }

        public Vector3? _LookAt;
        public Vector3? LookAt
        {
            get { return _LookAt; }
            set { if (_LookAt == value) return; _LookAt = value; OnCameraChanged(); }
        }

        public Matrix4 ViewMatrix { get; private set; }

        public Matrix4 ProjectionMatrix { get; private set; }

        public virtual Matrix4 GetViewMatrix(Vector3 eye)
        {
            Vector3 lookatPoint;
            if (LookAt != null)
            {
                lookatPoint = (Vector3)LookAt;
                return Matrix4.LookAt(eye, lookatPoint, Up);
            }
            else
            {
                lookatPoint = new Vector3((float)Math.Cos(Facing) * (float)Math.Cos(Pitch), (float)Math.Sin(Facing) * (float)Math.Cos(Pitch), (float)Math.Sin(Pitch));
                return Matrix4.LookAt(eye, eye + lookatPoint, Up);
            }
        }

        protected abstract Matrix4 GetProjectionMatrix();

        public Matrix4 GetViewProjectionMatrix()
        {
            return ViewMatrix * GetProjectionMatrix();
        }

        public Matrix4 GetInvertedViewProjectionMatrix()
        {
            return Matrix4.Invert(GetViewProjectionMatrix());
        }

        // The field of view (FOV) is the vertical angle of the camera view, this has been discussed more in depth in a
        // previous tutorial, but in this tutorial you have also learned how we can use this to simulate a zoom feature.
        // We convert from degrees to radians as soon as the property is set to improve performance
        public float Fov
        {
            get => MathHelper.RadiansToDegrees(FovInternal);
            set
            {
                var angle = MathHelper.Clamp(value, 1f, 90f);
                FovInternal = MathHelper.DegreesToRadians(angle);
            }
        }

        public void SetAspectRatio(float width, float height)
        {
            AspectRatio = width / height;
        }
    }

}