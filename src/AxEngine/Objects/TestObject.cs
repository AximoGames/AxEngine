using OpenTK;
using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;

namespace AxEngine
{

    public class TestObject : SimpleVertexObject
    {

        public override void Init()
        {
            if (Debug)
                SetVertices(DataHelper.CubeDebug);
            else
                SetVertices(DataHelper.Cube);

            var ico = new Util.IcoSphere.Mesh_SphereICO(1);
            SetVertices(ico.Vertices);
            SetIndicies(ico.Indicies);

            base.Init();
        }

    }

}