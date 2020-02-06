
#define CUBE_MAP_SHADOW_ROTATED
//#undef CUBE_MAP_SHADOW_ROTATED

#define MAX_NUM_TOTAL_LIGHTS 100

struct Light {
	vec3 lightColor; //The color of the light.
	vec3 lightPos; //The position of the light.
};

struct Material {
	sampler2D diffuse;
	sampler2D specular;
	float shininess;
	float specularStrength;
	vec3 color;
	float ambient; // 0.3
};

