#version 330 core

layout(location = 0)in vec3 aPos;
layout(location = 1)in vec4 aColor;

uniform mat4 Model;
uniform mat4 View;
uniform mat4 Projection;

out vec3 Normal;
out vec4 Color;

void main()
{
	gl_Position = vec4(aPos, 1.0) * Model * View * Projection;
	Color = aColor;
}