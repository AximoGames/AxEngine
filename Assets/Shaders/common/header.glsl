
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

	// Formular: Map * Color
	sampler2D DiffuseMap; // White default map, as fallbak
	vec3 DiffuseColor; // white, if map is used

	sampler2D SpecularMap; // Defaults to white or 1.0
	float SpecularStrength; // defaults to 1.0
	float Shininess;

	float Ambient;
	int ColorBlendMode;
};

struct SLightResult {
	float Shadow;
	vec3 Diffuse;
	vec3 Specular;
};
