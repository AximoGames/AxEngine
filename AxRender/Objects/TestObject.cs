using OpenTK;
using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;

namespace Aximo.Render
{

    public class TestObject : SimpleVertexObject
    {

        public override void Init()
        {
            if (Debug)
                SetVertices(DataHelper.CubeDebug);
            else
                SetVertices(DataHelper.Cube);

            base.Init();
        }

    }

}