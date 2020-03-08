using System;
using OpenToolkit.Mathematics;
using OpenToolkit.Graphics.OpenGL4;
using AxEngine;
using System.IO;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using OpenToolkit.Windowing.Desktop;
using OpenToolkit.Input;
using OpenToolkit.Windowing.Common;

namespace AxEngine
{

    public class Window : GameWindow
    {

        //public static Matrix4 CameraMatrix;
        //private float[] MouseSpeed = new float[3];
        //private Vector2 MouseDelta;
        //private float UpDownDelta;
        //private Vector3 CameraLocation;
        //private Vector3 Up = Vector3.UnitZ;
        //private float Pitch = -0.3f;
        //private float Facing = (float)Math.PI / 2 + 0.15f;

        public Window(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings) : base(gameWindowSettings, nativeWindowSettings) { }


        protected override void OnLoad()
        {
            base.OnLoad();
        }

        protected override void OnFocusedChanged(FocusedChangedEventArgs e)
        {
            //if (IsFocused)
            //{
            //    TargetRenderFrequency = 1 / 60.0;
            //    TargetUpdatePeriod = 1 / 60.0;
            //}
            //else
            //{
            //    TargetRenderFrequency = 1 / 30.0;
            //    TargetUpdatePeriod = 1 / 30.0;
            //}
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);
        }


        protected override void OnKeyDown(KeyboardKeyEventArgs e)
        {
            base.OnKeyDown(e);
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);
        }

        protected override void OnMouseMove(MouseMoveEventArgs e)
        {
            base.OnMouseMove(e);
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);
        }

        protected override void OnUnload()
        {
            base.OnUnload();
        }
    }
}