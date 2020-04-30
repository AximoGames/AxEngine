using OpenToolkit.Mathematics;

namespace Aximo.Engine.Mesh2
{
    public class MeshUVComponent : MeshComponent<Vector2>
    {
        public MeshUVComponent()
            : base(MeshComponentType.UV)
        {
        }

        public override MeshComponent CloneEmpty() => new MeshUVComponent();
    }

}
