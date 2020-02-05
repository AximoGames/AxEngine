
#define CUBE_MAP_SHADOW_ROTATED
//#undef CUBE_MAP_SHADOW_ROTATED

struct Material {
	sampler2D diffuse;
	sampler2D specular;
	float shininess;
	float specularStrength;
	vec3 color;
	float ambient; // 0.3
};

