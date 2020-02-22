#version 330 core
layout (location = 0) in vec3 aPos;

out vec3 TexCoords;

uniform mat4 projection;
uniform mat4 view;

void main()
{
    TexCoords = aPos;
    //TexCoords = aPos.xzy;
    gl_Position = vec4(aPos, 1.0) * view * projection;
    gl_Position.z = gl_Position.w; // Force Depth Test at specific position
}