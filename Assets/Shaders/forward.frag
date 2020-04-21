#version 330 core
#extension GL_GOOGLE_include_directive : enable
#extension GL_ARB_texture_cube_map_array : enable

#include "common/header.glsl"

#ifdef FRAG_HEADER_FILE
#include FRAG_HEADER_FILE
#endif

out vec4 FragColor;

//In order to calculate some basic lighting we need a few things per model basis, and a few things per fragment basis:
uniform vec3 ViewPos; //The position of the view and/or of the player.
uniform SMaterial material;

in vec3 Normal; //The normal of the fragment is calculated in the vertex shader.
in vec3 NormalTransposed;
in vec3 FragPos; //The fragment position.
#ifdef USE_VERTEX_UV
in vec2 TexCoords;
#endif
#ifdef USE_VERTEX_COLOR
in vec4 Color;
#endif

uniform sampler2DArray DirectionalShadowMap;
uniform samplerCubeArray PointShadowMap;
uniform float FarPlane;

uniform int LightCount;
layout(std140) uniform LightsArray { SLight lights[MAX_NUM_TOTAL_LIGHTS]; };

#include "common/lib.frag.glsl"

void main()
{
	vec3 viewDir = normalize(ViewPos - FragPos);
    vec3 matDiffuse = material.DiffuseColor;
#ifndef OVERRIDE_GET_MATERIAL_DIFFUSE_FILE

#ifdef USE_VERTEX_UV
    matDiffuse = texture(material.DiffuseMap, TexCoords).rgb * material.DiffuseColor;
#endif
#ifdef USE_VERTEX_COLOR
    matDiffuse = Color.rgb * material.DiffuseColor;
#endif

#else
#include OVERRIDE_GET_MATERIAL_DIFFUSE_FILE
#endif

    float matSpecular = 0.0;
#ifdef USE_VERTEX_UV
    matSpecular = texture(material.SpecularMap, TexCoords).r * material.SpecularStrength;
#endif
    float matAmbient = material.Ambient;
    float matShininess = material.Shininess;

    //vec3 color = material.DiffuseColor; // solid color for debugging
    vec3 normal = normalize(Normal);

#include "common/lighting.glsl"

}
