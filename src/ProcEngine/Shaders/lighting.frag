#version 330 core
#extension GL_GOOGLE_include_directive : enable

#include "common/header.glsl"

out vec4 FragColor;

//In order to calculate some basic lighting we need a few things per model basis, and a few things per fragment basis:
uniform Light light;
uniform vec3 viewPos; //The position of the view and/or of the player.
uniform Material material;

uniform sampler2D shadowMap;

in vec3 Normal; //The normal of the fragment is calculated in the vertex shader.
in vec3 FragPos; //The fragment position.
in vec2 TexCoords;
in vec4 FragPosLightSpace;

uniform float far_plane;
uniform samplerCube depthMap;

#include "common/lib.frag.glsl"

void main()
{
	vec3 color = texture(material.diffuse, TexCoords).rgb;
	//vec3 color = material.color; // solid color for debugging
	vec3 normal = normalize(Normal);
	// ambient
	vec3 ambient = material.ambient * color;
	// diffuse
	vec3 lightDir = normalize(light.lightPos - FragPos);
	float diff = max(dot(lightDir, normal), 0.0);
	vec3 diffuse = diff * light.lightColor;
	// specular
	vec3 viewDir = normalize(viewPos - FragPos);
	vec3 reflectDir = reflect(-lightDir, normal);
	float spec = 0.0;
	vec3 halfwayDir = normalize(lightDir + viewDir);
	spec = pow(max(dot(normal, halfwayDir), 0.0), material.shininess);
	vec3 specular = material.specularStrength * spec * light.lightColor;
	// calculate shadow

	float shadow = ShadowCalculation(FragPosLightSpace, light);
	float shadowCube = ShadowCalculationCubeSoft(FragPos, light);
	//float shadowCube = ShadowCalculationCubeHard(FragPos, light);

	//shadow = 0;
	vec3 lighting = (ambient + (1.0 - shadow) * (diffuse + specular)) * color;
	//vec3 lighting = (ambient + (1.0 - shadowCube) * (diffuse + specular)) * color;
	
    // Combine both shadows, for debugging
    //lighting = (lighting + lighting2) / 2;

	FragColor = vec4(lighting, 1.0);
}
