#version 430 core
#extension GL_GOOGLE_include_directive : enable

#include "common/header.glsl"

out vec4 FragColor;

in vec2 TexCoords;

uniform SMaterial material;

uniform sampler2D gPosition;
uniform sampler2D gNormal;
uniform sampler2D gAlbedoSpec;
uniform sampler2D gMaterial;

// struct Light {
//     vec3 Position;
//     vec3 Color;
    
//     float Linear;
//     float Quadratic;
// };

uniform samplerCubeArray depthMap;
uniform float FarPlane;
uniform sampler2DArray shadowMap;

uniform vec3 viewPos;
uniform int lightCount;
layout(std140) uniform lightsArray { SLight lights[MAX_NUM_TOTAL_LIGHTS]; };

vec3 Normal;
vec3 FragPos;

#include "common/lib.frag.glsl"

void main()
{             
    // retrieve data from gbuffer
    FragPos = texture(gPosition, TexCoords).rgb;
    if(FragPos == vec3(0))
        discard;

    vec3 viewDir  = normalize(viewPos - FragPos);

    vec3 normal = texture(gNormal, TexCoords).rgb;
    Normal = normal;

    vec3 matDiffuse = texture(gAlbedoSpec, TexCoords).rgb;
    float matSpecular = texture(gAlbedoSpec, TexCoords).a;
    float matAmbient = texture(gMaterial, TexCoords).r;
    float matShininess = texture(gMaterial, TexCoords).g;

#include "common/light.glsl"
}
