# Changelog

### 1.0.9

* Check Cache version
* Added UISlider
* Added OpenAL
* Added some Audio Modules
* Implemented Scaled UI Pixels for resolution independent positions
* Fixed Default Font Selection Bug

### 1.0.8

* Fixed Shadows if they are out of shadow map
* Corrected Directional light
* Removed SetConfig() from public API.
* Preparation for OpenAL

### 1.0.7

* Renamed Animataion to Tween. Make Tween generic.
* Skip unnecessary uniform set
* Cache Shaders (deduplication)
* Cache Vertex Data (deduplication)
* new tweening system

### 1.0.6
Fixed:

* Detection of AppRootDir. Dtected wrong .cache directory.

### 1.0.5

Refactor:

* GameMaterial --> Material
* RenderApplication --> Application
* GameShader --> Shader
* GameObject --> SceneObject
* GameContext --> SceneContext

* Moved Cameras to Aximo namespace.
* Moved Mesh classes to Aximo namespace.
