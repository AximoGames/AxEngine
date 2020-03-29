#version 430 core
#extension GL_GOOGLE_include_directive : enable

#include "common/header.glsl"

#ifdef FRAG_HEADER_FILE
#include FRAG_HEADER_FILE
#endif

layout (location = 0) out vec3 gPosition;
layout (location = 1) out vec3 gNormal;
layout (location = 2) out vec4 gAlbedoSpec;

in vec2 TexCoords;
in vec3 FragPos;
in vec3 Normal;

uniform Material material;

#include "common/lib.frag.small.glsl"

void main()
{    
    // store the fragment position vector in the first gbuffer texture
    gPosition = FragPos;
    // also store the per-fragment normals into the gbuffer
    gNormal = normalize(Normal);
    // and the diffuse per-fragment color
    gAlbedoSpec.rgb = BlendColor(texture(material.diffuse, TexCoords).rgb, material.color, material.colorBlendMode);
    // store specular intensity in gAlbedoSpec's alpha component
    gAlbedoSpec.a = texture(material.specular, TexCoords).r;
}