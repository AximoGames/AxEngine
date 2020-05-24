Aximo Game Engine Api Reference
===============================

Here's a small sample:

```c#
using Aximo;
using Aximo.Engine;
using Aximo.Engine.Components.Geometry;
using Aximo.Engine.Components.Lights;
using OpenToolkit.Mathematics;

internal class Program
{
    public static void Main(string[] args)
    {
        new MyApplication().Start();
    }
}

public class MyApplication : Application
{
    protected override void SetupScene()
    {
        // it's not required, but we should have a least one light.
        SceneContext.AddActor(new Actor(new PointLightComponent()
        {
            Name = "StaticLight",
            RelativeTranslation = new Vector3(2f, -1.5f, 3.25f),
        }));

        // add a cube
        SceneContext.AddActor(new Actor(new CubeComponent()
        {
            Name = "Box1",
            RelativeRotation = new Vector3(0, 0, 0.5f).ToQuaternion(),
            RelativeScale = new Vector3(1),
            RelativeTranslation = new Vector3(0, 0, 0.5f),
            Material = new Material
            {
                Color = new Vector4(1, 0, 1, 1),
            },
        }));
    }
}
```