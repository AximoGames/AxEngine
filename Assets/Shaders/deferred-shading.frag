#version 330 core
#extension GL_GOOGLE_include_directive : enable
#extension GL_ARB_texture_cube_map_array : enable

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

uniform sampler2DArray DirectionalShadowMap;
uniform samplerCubeArray PointShadowMap;
uniform float FarPlane;

uniform vec3 ViewPos;
uniform int LightCount;
layout(std140) uniform LightsArray { SLight lights[MAX_NUM_TOTAL_LIGHTS]; };

vec3 Normal;
vec3 FragPos;

#include "common/lib.frag.glsl"

void main()
{             
    // retrieve data from gbuffer
    FragPos = texture(gPosition, TexCoords).rgb;
    if(FragPos == vec3(0))
        discard;

    vec3 viewDir  = normalize(ViewPos - FragPos);

    vec3 normal = texture(gNormal, TexCoords).rgb;
    Normal = normal;

    vec4 matDiffuse = vec4(texture(gAlbedoSpec, TexCoords).rgb, 1.0);
    float matSpecular = texture(gAlbedoSpec, TexCoords).a;
    float matAmbient = texture(gMaterial, TexCoords).r;
    float matShininess = texture(gMaterial, TexCoords).g;

#include "common/lighting.glsl"
}
