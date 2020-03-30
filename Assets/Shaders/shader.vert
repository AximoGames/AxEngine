#version 330 core

layout(location = 0)in vec3 aPos;
layout(location = 1)in vec3 aNormal;
layout(location = 2)in vec2 aTexCoords;

uniform mat4 Model;
uniform mat4 View;
uniform mat4 Projection;
uniform mat4 lightSpaceMatrix;

out vec3 Normal;
out vec3 FragPos;
out vec2 TexCoords;
out vec4 FragPosLightSpace;

void main()
{
	gl_Position = vec4(aPos, 1.0) * Model * View * Projection;
	FragPos = vec3(vec4(aPos, 1.0) * Model);
	Normal = transpose(inverse(mat3(Model))) * aNormal;
	TexCoords = aTexCoords;
	// shadow
	FragPosLightSpace = vec4(FragPos, 1.0) * lightSpaceMatrix; // BUGFIX: aPos --> FragPos
}