#version 330 core
#extension GL_GOOGLE_include_directive : enable

#include "common/header.glsl"

#ifdef FRAG_HEADER_FILE
#include FRAG_HEADER_FILE
#endif

layout (location = 0) out vec3 gPosition;
layout (location = 1) out vec3 gNormal;
layout (location = 2) out vec4 gAlbedoSpec;
layout (location = 3) out vec3 gMaterial;

#ifdef USE_VERTEX_UV
in vec2 TexCoords;
#endif
#ifdef USE_VERTEX_COLOR
in vec4 Color;
#endif
in vec3 FragPos;
in vec3 Normal;
in vec3 NormalTransposed;

uniform SMaterial material;

#include "common/lib.frag.small.glsl"

void main()
{    
    // store the fragment position vector in the first gbuffer texture
    gPosition = FragPos;
    // also store the per-fragment normals into the gbuffer
    gNormal = normalize(Normal);
    // and the diffuse per-fragment color
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

    gAlbedoSpec.rgb = matDiffuse;
#ifdef USE_VERTEX_UV
    // store specular intensity in gAlbedoSpec's alpha component
    gAlbedoSpec.a = texture(material.SpecularMap, TexCoords).r * material.SpecularStrength;
#else
    gAlbedoSpec.a = 0;
#endif

    // Extra material parameters
    gMaterial.r = material.Ambient;
    //gMaterial.r = 0;
    gMaterial.g = material.Shininess;
    gMaterial.b = 0; // unused
}