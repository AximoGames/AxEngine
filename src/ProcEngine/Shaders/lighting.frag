#version 330 core
out vec4 FragColor;

struct Material{
	sampler2D diffuse;
	sampler2D specular;
	float shininess;
};

//In order to calculate some basic lighting we need a few things per model basis, and a few things per fragment basis:
uniform vec3 objectColor;//The color of the object.
uniform vec3 lightColor;//The color of the light.
uniform vec3 lightPos;//The position of the light.
uniform vec3 viewPos;//The position of the view and/or of the player.
uniform Material material;

uniform sampler2D shadowMap;
uniform mat4 debugMatrix;

in vec3 Normal;//The normal of the fragment is calculated in the vertex shader.
in vec3 FragPos;//The fragment position.
in vec2 TexCoords;
in vec4 FragPosLightSpace;

float ShadowCalculation(vec4 fragPosLightSpace);

void main()
{
	vec3 color=texture(material.diffuse,TexCoords).rgb;
	vec3 normal=normalize(Normal);
	vec3 lightColor=vec3(.3);
	// ambient
	vec3 ambient=.2*color;
	// diffuse
	vec3 lightDir=normalize(lightPos-FragPos);
	float diff=max(dot(lightDir,normal),0.);
	vec3 diffuse=diff*lightColor;
	// specular
	vec3 viewDir=normalize(viewPos-FragPos);
	vec3 reflectDir=reflect(-lightDir,normal);
	float spec=0.;
	vec3 halfwayDir=normalize(lightDir+viewDir);
	spec=pow(max(dot(normal,halfwayDir),0.),64.);
	vec3 specular=spec*lightColor;
	// calculate shadow
	float shadow=ShadowCalculation(FragPosLightSpace);
	//shadow=0;
	vec3 lighting=(ambient+(1.-shadow)*(diffuse+specular))*color;
	
	FragColor=vec4(lighting,1.);
}

float ShadowCalculation(vec4 fragPosLightSpace)
{
	// perform perspective divide
	vec3 projCoords=fragPosLightSpace.xyz/fragPosLightSpace.w;
	// transform to [0,1] range
	projCoords=projCoords*.5+.5;
	// get closest depth value from light's perspective (using [0,1] range fragPosLight as coords)
	float closestDepth=texture(shadowMap,projCoords.xy).r;
	// get depth of current fragment from light's perspective
	float currentDepth=projCoords.z;
	// check whether current frag pos is in shadow
	float shadow=currentDepth>closestDepth?1.:0.;
	
	return shadow;
}