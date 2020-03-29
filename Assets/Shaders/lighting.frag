#version 430 core
#extension GL_GOOGLE_include_directive : enable

#include "common/header.glsl"

#ifdef FRAG_HEADER_FILE
#include FRAG_HEADER_FILE
#endif

out vec4 FragColor;

//In order to calculate some basic lighting we need a few things per model basis, and a few things per fragment basis:
uniform vec3 viewPos; //The position of the view and/or of the player.
uniform Material material;

uniform sampler2DArray shadowMap;

in vec3 Normal; //The normal of the fragment is calculated in the vertex shader.
in vec3 FragPos; //The fragment position.
in vec2 TexCoords;

uniform float far_plane;
uniform samplerCubeArray depthMap;
uniform int lightCount;
layout(std140) uniform lightsArray { Light lights[MAX_NUM_TOTAL_LIGHTS]; };

#include "common/lib.frag.glsl"

void main()
{
	vec3 viewDir = normalize(viewPos - FragPos);
    vec3 matDiffuse;

#ifndef OVERRIDE_GET_MATERIAL_DIFFUSE_FILE
    matDiffuse = BlendColor(texture(material.diffuse, TexCoords).rgb, material.color, material.colorBlendMode);
#else
#include OVERRIDE_GET_MATERIAL_DIFFUSE_FILE
#endif

    float matSpecular = texture(material.specular, TexCoords).r;
    float matAmbient = material.ambient;
    float matShininess = material.shininess;

    //vec3 color = material.color; // solid color for debugging
    vec3 normal = normalize(Normal);

    LightResult lightResult;
    lightResult.Diffuse = vec3(0);
    lightResult.Specular = vec3(0);
    lightResult.Shadow = 0;

#include "common/light.glsl"

}
