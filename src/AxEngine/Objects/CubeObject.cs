using OpenTK;
using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;

namespace AxEngine
{

    public class CubeObject : SimpleVertexObject
    {

        public override void Init()
        {
            SetVertices(DataHelper.Cube);

            base.Init();
        }

    }

}