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

vec3 BlendColor(vec3 textureColor, vec3 color, int blendMode) {
    switch(blendMode)
    {
        case 0:
            return textureColor;
        case 1:
            return color;
        case 2:
            return textureColor * color;
        case 3:
            return textureColor + color;
        case 4:
            return textureColor - color;
    }
}

void main()
{
	vec3 viewDir = normalize(viewPos - FragPos);
    vec3 color;
    float txtSpec;

#ifndef OVERRIDE_GET_MATERIAL_DIFFUSE_FILE
    color = BlendColor(texture(material.diffuse, TexCoords).rgb, material.color, material.colorBlendMode);
#else
#include OVERRIDE_GET_MATERIAL_DIFFUSE_FILE
#endif

    txtSpec = texture(material.specular, TexCoords).r;

    //vec3 color = material.color; // solid color for debugging
    vec3 normal = normalize(Normal);
    // ambient
    vec3 ambient = material.ambient * color;

    vec3 finalColor = ambient;

    //int lightCount = 1;
	for(int x = 0; x < lightCount; x++) {
        Light light = lights[x];

        // diffuse
        vec3 lightDir = normalize(light.position - FragPos);
        float diff = max(dot(lightDir, normal), 0.0);
        vec3 diffuse = diff * light.color;
        // specular
        vec3 reflectDir = reflect(-lightDir, normal);
        float spec = 0.0;
        vec3 halfwayDir = normalize(lightDir + viewDir);
        spec = pow(max(dot(normal, halfwayDir), 0.0), material.shininess);
        // float specularStrength;
        // //specularStrength = material.specularStrength;
        // specularStrength = ;
        vec3 specular = light.color * spec * txtSpec;

        // calculate shadow
   
        float shadow;
        if(light.directionalLight == 1) {
            shadow = ShadowCalculation(vec4(FragPos, 1.0) * light.lightSpaceMatrix, light);
        }
        else {
            shadow = ShadowCalculationCubeSoft(FragPos, light);
        }

        //float shadowCube = ShadowCalculationCubeHard(FragPos, light);

        //shadow = 0;
        vec3 lighting = (ambient + (1.0 - shadow) * (diffuse + specular)) * color;
        //vec3 lighting = (ambient + (1.0 - shadowCube) * (diffuse + specular)) * color;

        finalColor = finalColor + lighting;
    }


	
    // Combine both shadows, for debugging
    //lighting = (lighting + lighting2) / 2;

	FragColor = vec4(finalColor, 1.0);
}
