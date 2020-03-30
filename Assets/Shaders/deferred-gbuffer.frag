#version 430 core
#extension GL_GOOGLE_include_directive : enable

#include "common/header.glsl"

#ifdef FRAG_HEADER_FILE
#include FRAG_HEADER_FILE
#endif

layout (location = 0) out vec3 gPosition;
layout (location = 1) out vec3 gNormal;
layout (location = 2) out vec4 gAlbedoSpec;
layout (location = 3) out vec3 gMaterial;

in vec2 TexCoords;
in vec3 FragPos;
in vec3 Normal;

uniform SMaterial material;

#include "common/lib.frag.small.glsl"

void main()
{    
    // store the fragment position vector in the first gbuffer texture
    gPosition = FragPos;
    // also store the per-fragment normals into the gbuffer
    gNormal = normalize(Normal);
    // and the diffuse per-fragment color
    vec3 matDiffuse;
#ifndef OVERRIDE_GET_MATERIAL_DIFFUSE_FILE
    matDiffuse = BlendColor(texture(material.diffuse, TexCoords).rgb, material.color, material.colorBlendMode);
#else
#include OVERRIDE_GET_MATERIAL_DIFFUSE_FILE
#endif
    gAlbedoSpec.rgb = matDiffuse;
    // store specular intensity in gAlbedoSpec's alpha component
    gAlbedoSpec.a = texture(material.specular, TexCoords).r;

    // Extra material parameters
    gMaterial.r = material.ambient;
    //gMaterial.r = 0;
    gMaterial.g = material.shininess;
    gMaterial.b = material.specularStrength; // Used?
}