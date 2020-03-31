#version 430 core
#extension GL_GOOGLE_include_directive : enable

#include "common/header.glsl"

#ifdef FRAG_HEADER_FILE
#include FRAG_HEADER_FILE
#endif

out vec4 FragColor;

//In order to calculate some basic lighting we need a few things per model basis, and a few things per fragment basis:
uniform vec3 ViewPos; //The position of the view and/or of the player.
uniform SMaterial material;

in vec3 Normal; //The normal of the fragment is calculated in the vertex shader.
in vec3 NormalRotated;
in vec3 FragPos; //The fragment position.
in vec2 TexCoords;

uniform sampler2DArray DirectionalShadowMap;
uniform samplerCubeArray PointShadowMap;
uniform float FarPlane;

uniform int LightCount;
layout(std140) uniform LightsArray { SLight lights[MAX_NUM_TOTAL_LIGHTS]; };

#include "common/lib.frag.glsl"

void main()
{
	vec3 viewDir = normalize(ViewPos - FragPos);
    vec3 matDiffuse;

#ifndef OVERRIDE_GET_MATERIAL_DIFFUSE_FILE
    matDiffuse = texture(material.DiffuseMap, TexCoords).rgb * material.DiffuseColor;
#else
#include OVERRIDE_GET_MATERIAL_DIFFUSE_FILE
#endif

    float matSpecular = texture(material.SpecularMap, TexCoords).r * material.SpecularStrength;
    float matAmbient = material.Ambient;
    float matShininess = material.Shininess;

    //vec3 color = material.DiffuseColor; // solid color for debugging
    vec3 normal = normalize(Normal);

#include "common/lighting.glsl"

}
