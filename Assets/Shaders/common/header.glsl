
#define CUBE_MAP_SHADOW_ROTATED
//#undef CUBE_MAP_SHADOW_ROTATED

#define MAX_NUM_TOTAL_LIGHTS 100

struct SLight {
	vec3 Position; //The position of the light.
	vec3 Color; //The color of the light.
	mat4 LightSpaceMatrix;
	int ShadowLayer;

	int DirectionalLight;
    float Linear;
    float Quadratic;
};

struct SMaterial {
	sampler2D DiffuseMap;
	vec3 DiffuseColor;

	sampler2D SpecularMap;
	float SpecularStrength;
	float Shininess;

	float Ambient;
	int ColorBlendMode;
};

struct SLightResult {
	float Shadow;
	vec3 Diffuse;
	vec3 Specular;
};
