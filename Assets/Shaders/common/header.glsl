
#define CUBE_MAP_SHADOW_ROTATED
//#undef CUBE_MAP_SHADOW_ROTATED

#define MAX_NUM_TOTAL_LIGHTS 100

struct SLight {
	vec3 position; //The position of the light.
	vec3 color; //The color of the light.
	mat4 LightSpaceMatrix;
	int ShadowLayer;

	int directionalLight;
    float linear;
    float quadratic;
};

struct SMaterial {
	sampler2D diffuse;
	sampler2D specular;
	float shininess;
	float specularStrength;
	vec3 color;
	float ambient; // 0.3
	int colorBlendMode;
};

struct SLightResult {
	float Shadow;
	vec3 Diffuse;
	vec3 Specular;
};
