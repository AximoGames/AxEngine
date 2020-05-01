using OpenToolkit.Mathematics;

namespace Aximo
{
    public class MeshPositionComponent : MeshComponent<Vector3>
    {
        public MeshPositionComponent()
            : base(MeshComponentType.Position)
        {
        }

        public override MeshComponent CloneEmpty() => new MeshPositionComponent();

    }

}
