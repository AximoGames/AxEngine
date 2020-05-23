Aximo Game Engine Api Reference
===============================

Here's a small sample:

```c#
using Aximo;
using Aximo.Engine;
using Aximo.Engine.Components.Geometry;
using Aximo.Engine.Components.Lights;
using OpenToolkit.Mathematics;

public class MyApplication : RenderApplication
{
    public MyApplication(RenderApplicationConfig startup) : base(startup)
    {
    }

    protected override void SetupScene()
    {
        GameContext.AddActor(new Actor(new DirectionalLightComponent()
        {
            RelativeTranslation = new Vector3(2f, -1.5f, 3.25f),
            Name = "StaticLight",
        }));

        GameContext.AddActor(new Actor(new CubeComponent()
        {
            Name = "Box1",
            RelativeRotation = new Vector3(0, 0, 0.5f).ToQuaternion(),
            RelativeScale = new Vector3(1),
            RelativeTranslation = new Vector3(0, 0, 0.5f),
            Material = new GameMaterial
            {
                Color = new Vector4(1, 0, 1, 1),
            },
        }));
    }
}
```

Now initialize the application:

```c#
using Aximo.Engine;
using OpenToolkit.Mathematics;
using OpenToolkit.Windowing.Common;

internal class Program
{
    public static void Main(string[] args)
    {
        var config = new RenderApplicationConfig
        {
            WindowTitle = "Sample",
            WindowSize = new Vector2i(800, 600),
            VSync = VSyncMode.Off,
            UseConsole = true,
            IsMultiThreaded = true,
        };

        new GameStartup<MyApplication>(config).Start();
    }
}
```