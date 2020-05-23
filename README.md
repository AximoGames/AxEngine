![License](https://img.shields.io/badge/license-MIT-blue.svg) [![Actions Status](https://github.com/abanu-org/abanu/workflows/Tests/badge.svg)](https://github.com/abanu-org/abanu/actions) [![CodeFactor](https://www.codefactor.io/repository/github/aximogames/axengine/badge/master)](https://www.codefactor.io/repository/github/aximogames/axengine/overview/master)

Documentation: http://www.aximo.games

Aximo Game Engine is written purly in C#.

### Features

* Platform independent (runs on Windows & Linux)
* Multi-Threaded
* Dynamic Mesh Creation and Manipulation
* Several Geometry Components.
* Phong-Lighning
* Multiple Lights with Shadows (Directional- and Pointlights)
* Deferred and forward shading (can be mixed)
* UI Components (Buttons, Panels, Labels, Performance Statistics)
* Custom Shaders / Materials
* Asset Management (incl. dynamic generation on-the-fly)
* Scene Management via Actors, Components and relative transformations.
* Audio (only global).
* Flexible Vertex data layout.

### Small sample

```shell
mkdir mkdir TestGame
cd TestGame
dotnet new console # create a new console application

dotnet add package Aximo
```

Now place to code files, for example MyApplication.cs:

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

Initialize the application, for example in Program.cs:

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

Run your application via `dotnet run`.

### Build the engine from source

To clone this repository, (currently) you need git LFS, otherwise you need to download the Assets folder manually.

Don't forget the `--recursive` option:
```
git clone --recursive https://github.com/AximoGames/AxEngine.git
```

Build from command line:

```
cd AxEngine
dotnet build
```

Run the shipped sample:
```
dotnet run -p Demo
```

```
Keymapping:
C -> Control Camera (default)
B -> Control Box1
L -> Control static light
J -> Move Camera to current controlling object

W, S, A, D -> Moving object in XY.
PageUp, PageDown -> Moving object in Z.
Arrows -> Rotate

ESC -> Quit
```
