// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using OpenToolkit.Mathematics;

namespace Aximo
{
    public enum CameraType
    {
        PerspectiveFieldOfView,
        Orthographic,
    }

    public abstract class Camera : IPosition, IData
    {
        private Dictionary<string, object> Data = new Dictionary<string, object>();

        public T GetExtraData<T>(string name, T defaultValue = default)
        {
            return IDataHelper.GetData(Data, name, defaultValue);
        }

        public bool HasExtraData(string name)
        {
            return IDataHelper.HasData(Data, name);
        }

        public bool SetExraData<T>(string name, T value, T defaultValue = default)
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
            ViewProjectionMatrix = ViewMatrix * ProjectionMatrix;
            InvertedViewProjectionMatrix = Matrix4.Invert(ViewProjectionMatrix);
            UpdateAABB();

            CameraChangedInternal?.Invoke();
        }

        public event CameraChangedDelegate CameraChangedInternal;

        public delegate void CameraChangedDelegate();

        public Camera(Vector3 position)
        {
            Position = position;
        }

        private Vector3 _Up = Vector3.UnitZ;
        public Vector3 Up
        {
            get { return _Up; }
            set { if (_Up == value) return; _Up = value; OnCameraChanged(); }
        }

        private float _Facing = ((float)Math.PI / 2) + 0.15f;
        public float Facing
        {
            get { return _Facing; }
            set { if (_Facing == value) return; _Facing = value; OnCameraChanged(); }
        }

        private float _AspectRatio = 1.0f;
        public float AspectRatio
        {
            get { return _AspectRatio; }
            set { if (_AspectRatio == value) return; _AspectRatio = value; OnCameraChanged(); }
        }

        private Vector3? _LookAt;
        public Vector3? LookAt
        {
            get { return _LookAt; }
            set { if (_LookAt == value) return; _LookAt = value; OnCameraChanged(); }
        }

        public Matrix4 ViewMatrix { get; private set; }
        public Matrix4 ProjectionMatrix { get; private set; }
        public Matrix4 ViewProjectionMatrix { get; private set; }
        public Matrix4 InvertedViewProjectionMatrix { get; private set; }

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

        public Vector3 UnProject(Vector2 normalizedScreenCoordinates, float z)
        {
            return UnProject(new Vector3(normalizedScreenCoordinates.X, normalizedScreenCoordinates.Y, z));
        }

        public Vector3 UnProject(Vector3 normalizedScreenCoordinates)
        {
            Vector4 vec;

            vec.X = normalizedScreenCoordinates.X;
            vec.Y = normalizedScreenCoordinates.Y;
            vec.Z = normalizedScreenCoordinates.Z;
            vec.W = 1.0f;

            vec = Vector4.Transform(vec, InvertedViewProjectionMatrix);
            //vec = vec * InvertedViewProjectionMatrix;

            return vec.Xyz / vec.W;
        }

        public Box3 AABB { get; private set; }
        private void UpdateAABB()
        {
            var screenPoints = new Vector3[]
            {
                new Vector3(-1, -1, -1),
                new Vector3(-1, 1, -1),
                new Vector3(1, -1, -1),
                new Vector3(1, 1, -1),

                new Vector3(-1, -1, 1),
                new Vector3(-1, 1, 1),
                new Vector3(1, -1, 1),
                new Vector3(1, 1, 1),
            };

            var points = new List<Vector3>();
            foreach (var sp in screenPoints)
            {
                var p = UnProject(sp);
                points.Add(p);
            }

            AABB = points.GetBoundingBox();
        }

        public bool InFrustum(Box3 box)
        {
            //return true;
            if (!AABB.Contains(box))
                return false;

            var frustum = ExtractFrustum(ViewProjectionMatrix);
            if (frustum.Near.IsOutside(box)
                || frustum.Left.IsOutside(box)
                || frustum.Right.IsOutside(box)
                || frustum.Top.IsOutside(box)
                || frustum.Bottom.IsOutside(box)
                || frustum.Far.IsOutside(box))
                return false;

            return true;
        }

        public static Frustum ExtractFrustum(Matrix4 viewProjMatrix)
        {
            Frustum frustum = new Frustum();

            Plane left = new Plane();
            left.A = viewProjMatrix.M14 + viewProjMatrix.M11;
            left.B = viewProjMatrix.M24 + viewProjMatrix.M21;
            left.C = viewProjMatrix.M34 + viewProjMatrix.M31;
            left.D = viewProjMatrix.M44 + viewProjMatrix.M41;
            left.Normalize();
            frustum.Left = left;

            Plane right = new Plane();
            right.A = viewProjMatrix.M14 - viewProjMatrix.M11;
            right.B = viewProjMatrix.M24 - viewProjMatrix.M21;
            right.C = viewProjMatrix.M34 - viewProjMatrix.M31;
            right.D = viewProjMatrix.M44 - viewProjMatrix.M41;
            right.Normalize();
            frustum.Right = right;

            Plane top = new Plane();
            top.A = viewProjMatrix.M14 - viewProjMatrix.M12;
            top.B = viewProjMatrix.M24 - viewProjMatrix.M22;
            top.C = viewProjMatrix.M34 - viewProjMatrix.M32;
            top.D = viewProjMatrix.M44 - viewProjMatrix.M42;
            top.Normalize();
            frustum.Top = top;

            Plane bottom = new Plane();
            bottom.A = viewProjMatrix.M14 + viewProjMatrix.M12;
            bottom.B = viewProjMatrix.M24 + viewProjMatrix.M22;
            bottom.C = viewProjMatrix.M34 + viewProjMatrix.M32;
            bottom.D = viewProjMatrix.M44 + viewProjMatrix.M42;
            bottom.Normalize();
            frustum.Bottom = bottom;

            Plane near = new Plane();
            near.A = viewProjMatrix.M13;
            near.B = viewProjMatrix.M23;
            near.C = viewProjMatrix.M33;
            near.D = viewProjMatrix.M43;
            near.Normalize();
            frustum.Near = near;

            Plane far = new Plane();
            far.A = viewProjMatrix.M14 - viewProjMatrix.M13;
            far.B = viewProjMatrix.M24 - viewProjMatrix.M23;
            far.C = viewProjMatrix.M34 - viewProjMatrix.M33;
            far.D = viewProjMatrix.M44 - viewProjMatrix.M43;
            far.Normalize();
            frustum.Far = far;

            return frustum;
        }

    }
}
