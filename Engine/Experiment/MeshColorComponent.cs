using OpenToolkit.Mathematics;

namespace Aximo.Engine.Mesh2
{
    public class MeshColorComponent : MeshComponent<Vector4>
    {
        public MeshColorComponent()
            : base(MeshComponentType.Color)
        {
        }

        public override MeshComponent CloneEmpty() => new MeshColorComponent();
    }

}
