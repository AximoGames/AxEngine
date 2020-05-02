using System.Collections.Generic;
using OpenToolkit.Mathematics;
using Aximo.Render;
using System.Linq;

namespace Aximo
{
    public class MeshPositionComponent : MeshComponent<Vector3>
    {
        public MeshPositionComponent()
            : base(MeshComponentType.Position)
        {
        }

        public override MeshComponent CloneEmpty() => new MeshPositionComponent();

        public MeshPositionComponent(ICollection<IVertexPosition3> values)
            : this()
        {
            AddRange(values.Select(v => v.Position));
        }

        public void AddRange(IEnumerable<IVertexPosition3> values)
        {
            _Values.AddRange(values.Select(v => v.Position));
        }

        public override void AddRange(IEnumerable<IVertex> values)
        {
            foreach (var v in values)
                if (v is IVertexPosition3 p)
                    Add(p.Position);
        }

    }

}
