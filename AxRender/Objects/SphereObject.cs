namespace Aximo.Render
{
    public class SphereObject : SimpleVertexObject
    {

        public override void Init()
        {
            var ico = new Util.IcoSphere.Mesh_SphereICO(2);
            SetVertices(ico.Vertices);
            SetIndicies(ico.Indicies);

            base.Init();
        }

    }

}