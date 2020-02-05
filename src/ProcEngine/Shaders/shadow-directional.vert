#version 330 core

layout(location = 0)in vec3 aPos;
layout(location = 1)in vec3 aNormal;
layout(location = 2)in vec2 aTexCoords;

uniform mat4 model;
uniform mat4 lightSpaceMatrix;

void main()
{
	gl_Position = vec4(aPos, 1.0) * model * lightSpaceMatrix;
}