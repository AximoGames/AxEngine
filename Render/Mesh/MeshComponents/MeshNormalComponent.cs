using OpenToolkit.Mathematics;

namespace Aximo
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
