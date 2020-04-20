#version 330 core

layout(location = 0)in vec3 aPos;
layout(location = 1)in vec3 aNormal;
#ifdef USE_VERTEX_UV
layout(location = 2)in vec2 aTexCoords;
#ifdef USE_VERTEX_COLOR
layout(location = 3)in vec4 aColor;
#endif
#else
#ifdef USE_VERTEX_COLOR
layout(location = 2)in vec4 aColor;
#endif
#endif

uniform mat4 Model;
uniform mat4 View;
uniform mat4 Projection;
uniform mat4 LightSpaceMatrix;

out vec3 Normal;
out vec3 NormalTransposed;
out vec3 FragPos;
#ifdef USE_VERTEX_UV
out vec2 TexCoords;
#endif
#ifdef USE_VERTEX_COLOR
out vec4 Color;
#endif

out vec4 FragPosLightSpace;

void main()
{
	gl_Position = vec4(aPos, 1.0) * Model * View * Projection;
	FragPos = vec3(vec4(aPos, 1.0) * Model);

    mat3 normalMatrix = transpose(inverse(mat3(Model)));
    Normal = aNormal * normalMatrix;
    NormalTransposed = normalMatrix * aNormal;

#ifdef USE_VERTEX_UV
	TexCoords = aTexCoords;
#endif
#ifdef USE_VERTEX_COLOR
	Color = aColor;
#endif

	// shadow
	FragPosLightSpace = vec4(FragPos, 1.0) * LightSpaceMatrix; // BUGFIX: aPos --> FragPos
}