using OpenTK;
using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;

namespace ProcEngine
{

    public class CubeObject : SimpleVertexObject
    {

        public override void Init()
        {
            SetTexture("Ressources/woodenbox.png", "Ressources/woodenbox_specular.png");
            SetVertices(DataHelper.Cube);

            base.Init();
        }

    }

}