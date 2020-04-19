[![CodeFactor](https://www.codefactor.io/repository/github/aximogames/axengine/badge/master)](https://www.codefactor.io/repository/github/aximogames/axengine/overview/master)

To clone this repository, you need git LFS, otherwise you need to download the Assets folder manually.

Don't forget the `--recursive` option:
```
git clone --recursive https://github.com/AximoGames/AxEngine.git
```

Build from command line:

```
cd Experiments
dotnet build
```

Run:
```
dotnet run -p Demo
```

```Keymapping:
C -> Control Camera (default)
B -> Control Box1
L -> Control static light
J -> Move Camera to current controlling object

W, S, A, D -> Moving object in XY.
PageUp, PageDown -> Moving object in Z.
Arrows -> Rotate

ESC -> Quit
```
