#version 430 core
#extension GL_GOOGLE_include_directive : enable

#include "common/header.glsl"

out vec4 FragColor;

in vec2 TexCoords;

uniform sampler2D gPosition;
uniform sampler2D gNormal;
uniform sampler2D gAlbedoSpec;

// struct Light {
//     vec3 Position;
//     vec3 Color;
    
//     float Linear;
//     float Quadratic;
// };

uniform samplerCubeArray depthMap;
uniform float far_plane;
uniform sampler2DArray shadowMap;

uniform vec3 viewPos;
uniform int lightCount;
layout(std140) uniform lightsArray { Light lights[MAX_NUM_TOTAL_LIGHTS]; };

vec3 Normal;
vec3 FragPos;

#include "common/lib.frag.glsl"

void main()
{             
    // retrieve data from gbuffer
    FragPos = texture(gPosition, TexCoords).rgb;
    Normal = texture(gNormal, TexCoords).rgb;
    vec3 Diffuse = texture(gAlbedoSpec, TexCoords).rgb;
    float Specular = texture(gAlbedoSpec, TexCoords).a;
    
    // then calculate lighting as usual
    vec3 lighting  = Diffuse * 0.5; // hard-coded ambient component
    vec3 viewDir  = normalize(viewPos - FragPos);
    int lightCount = 2;
    for(int i = 0; i < lightCount; ++i)
    {
        Light light = lights[i];

        // diffuse
        vec3 lightDir = normalize(lights[i].position - FragPos);
        vec3 diffuse = max(dot(Normal, lightDir), 0.0) * Diffuse * lights[i].color;
        // specular
        vec3 halfwayDir = normalize(lightDir + viewDir);  
        float spec = pow(max(dot(Normal, halfwayDir), 0.0), 16.0);
        vec3 specular = lights[i].color * spec * Specular;
        // attenuation
        float distance = length(lights[i].position - FragPos);
        float attenuation = 1.0 / (1.0 + lights[i].linear * distance + lights[i].quadratic * distance * distance);

        attenuation=1.0; // debug

        float shadow;
        if(light.directionalLight == 1) {
            shadow = ShadowCalculation(vec4(FragPos, 1.0) * light.lightSpaceMatrix, light);
            //shadow = 0;
        }
        else {
            shadow = ShadowCalculationCubeSoft(FragPos, light);
        }

        diffuse *= attenuation;
        specular *= attenuation;
        lighting += (diffuse + specular) * (1.0 - shadow);        
    }
    FragColor = vec4(lighting, 1.0);
}
