#version 330 core
out vec4 FragColor;

in vec3 TexCoords;

uniform samplerCube skybox;

void main()
{
    gl_FragDepth = 1.0; // TODO, Pereformance: writing to gl_FragDepth disables all depth check optimizations
    FragColor = texture(skybox, TexCoords.xzy);
}