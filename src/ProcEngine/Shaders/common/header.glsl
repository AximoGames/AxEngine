
#define CUBE_MAP_SHADOW_ROTATED
//#undef CUBE_MAP_SHADOW_ROTATED

#define MAX_NUM_TOTAL_LIGHTS 100

layout(std140) struct Light {
	vec3 position; //The position of the light.
	vec3 color; //The color of the light.
	int shadowLayer;

	int DirectionalLight;
    float linear;
    float quadratic;
};

struct Material {
	sampler2D diffuse;
	sampler2D specular;
	float shininess;
	float specularStrength;
	vec3 color;
	float ambient; // 0.3
};

