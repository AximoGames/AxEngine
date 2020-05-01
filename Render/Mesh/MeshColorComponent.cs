using OpenToolkit.Mathematics;

namespace Aximo
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
