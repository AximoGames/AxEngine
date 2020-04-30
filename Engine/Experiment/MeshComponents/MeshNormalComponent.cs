using OpenToolkit.Mathematics;

namespace Aximo.Engine.Mesh2
{
    public class MeshNormalComponent : MeshComponent<Vector3>
    {
        public MeshNormalComponent()
            : base(MeshComponentType.Normal)
        {
        }

        public override MeshComponent CloneEmpty() => new MeshNormalComponent();
    }

}
