#version 330 core
#extension GL_GOOGLE_include_directive : enable

#include "common/header.glsl"

out vec4 FragColor;

//In order to calculate some basic lighting we need a few things per model basis, and a few things per fragment basis:
uniform vec3 objectColor; //The color of the object.
uniform vec3 lightColor; //The color of the light.
uniform vec3 lightPos; //The position of the light.
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
	//vec3 color = texture(material.diffuse, TexCoords).rgb;
	vec3 color = vec3(1.0, 1.0, 0.0); // solid color for debugging
	vec3 normal = normalize(Normal);
	vec3 lightColor = vec3(0.4);
	// ambient
	vec3 ambient = 0.3 * color;
	// diffuse
	vec3 lightDir = normalize(lightPos - FragPos);
	float diff = max(dot(lightDir, normal), 0.0);
	vec3 diffuse = diff * lightColor;
	// specular
	vec3 viewDir = normalize(viewPos - FragPos);
	vec3 reflectDir = reflect(-lightDir, normal);
	float spec = 0.0;
	vec3 halfwayDir = normalize(lightDir + viewDir);
	spec = pow(max(dot(normal, halfwayDir), 0.0), 64.0);
	vec3 specular = spec * lightColor;
	// calculate shadow

	float shadow = ShadowCalculation(FragPosLightSpace);
	//float shadowCube = ShadowCalculationCubeSoft(FragPos);
	float shadowCube = ShadowCalculationCubeHard(FragPos);

	//shadow = 0;
	//vec3 lighting = (ambient + (1.0 - shadow) * (diffuse + specular)) * color;
	vec3 lighting = (ambient + (1.0 - shadowCube) * (diffuse + specular)) * color;
	
    // Combine both shadows, for debugging
    //lighting = (lighting + lighting2) / 2;

	FragColor = vec4(lighting, 1.0);
}
