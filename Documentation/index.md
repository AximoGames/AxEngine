Aximo Game Engine
=================

**Aximo Engine** is a simple Game Engine written in C#.

<div class="row">
    <div class="col-md-4">
        <div class="panel panel-default" style="min-height: 140px">
            <div class="panel-body">
                <p><strong><a href="api/index.html">API-Reference</a></strong></p>
                <p>Checkut the API-Reference.</p>
            </div>
        </div>
    </div>
</div>

Here's a small sample:

```c#
using Aximo;
using Aximo.Engine;
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

Initialize the application:

```c#
using Aximo.Engine;
using OpenToolkit.Mathematics;
using OpenToolkit.Windowing.Common;

namespace Aximo.PlayGround1
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            var config = new RenderApplicationConfig
            {
                WindowTitle = "PlayGround1",
                WindowSize = new Vector2i(800, 600),
                VSync = VSyncMode.Off,
                UseConsole = true,
                IsMultiThreaded = true,
            };

            new GameStartup<MyApplication, GtkUI>(config).Start();
        }
    }
}
```