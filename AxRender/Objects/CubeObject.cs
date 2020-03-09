using OpenTK;
using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;

namespace Aximo.Render
{

    public class CubeObject : SimpleVertexObject
    {

        public override void Init() {
            SetVertices(DataHelper.Cube);

            base.Init();
        }

    }

}