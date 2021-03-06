#include "lib.frag.small.glsl"

#ifdef USE_SHADOW

// array of offset direction for sampling
vec3 gridSamplingDisk[20] = vec3[]
(
   vec3(1, 1,  1), vec3( 1, -1,  1), vec3(-1, -1,  1), vec3(-1, 1,  1), 
   vec3(1, 1, -1), vec3( 1, -1, -1), vec3(-1, -1, -1), vec3(-1, 1, -1),
   vec3(1, 1,  0), vec3( 1, -1,  0), vec3(-1, -1,  0), vec3(-1, 1,  0),
   vec3(1, 0,  1), vec3(-1,  0,  1), vec3( 1,  0, -1), vec3(-1, 0, -1),
   vec3(0, 1,  1), vec3( 0, -1,  1), vec3( 0, -1, -1), vec3( 0, 1, -1)
);

vec4 ShadowCubeCoords(vec3 fragToLight, int layer)
{
#ifdef CUBE_MAP_SHADOW_ROTATED
    return vec4(fragToLight, layer);
#else
    return vec4(fragToLight.xzy, layer);
#endif
}

vec3 GetShadowCoords(vec2 projCoords, int layer) {
    return vec3(projCoords.xy, layer);
    //return vec2(projCoords.x, projCoords.y);
}

float ShadowCalculationCubeSoft(vec3 fragPos, SLight light)
{
    // get vector between fragment position and light position
    vec3 fragToLight = fragPos - light.Position;
    // use the fragment to light vector to sample from the depth map    
    // float closestDepth = texture(PointShadowMap, fragToLight).r;
    // it is currently in linear range between [0,1], let's re-transform it back to original depth value
    // closestDepth *= FarPlane;
    // now get current linear depth as the length between the fragment and light position
    float currentDepth = length(fragToLight);
    // test for shadows
    // float bias = 0.05; // we use a much larger bias since depth is now in [NearPlane, FarPlane] range
    // float shadow = currentDepth -  bias > closestDepth ? 1.0 : 0.0;
    // PCF
    // float shadow = 0.0;
    // float bias = 0.05; 
    // float samples = 4.0;
    // float offset = 0.1;
    // for(float x = -offset; x < offset; x += offset / (samples * 0.5))
    // {
        // for(float y = -offset; y < offset; y += offset / (samples * 0.5))
        // {
            // for(float z = -offset; z < offset; z += offset / (samples * 0.5))
            // {
                // float closestDepth = texture(PointShadowMap, fragToLight + vec3(x, y, z)).r; // use lightdir to lookup cubemap
                // closestDepth *= FarPlane;   // Undo mapping [0;1]
                // if(currentDepth - bias > closestDepth)
                    // shadow += 1.0;
            // }
        // }
    // }
    // shadow /= (samples * samples * samples);
    float shadow = 0.0;
    float bias = 0.15;
    int samples = 20;
    float viewDistance = length(ViewPos - fragPos);
    float diskRadius = (1.0 + (viewDistance / light.FarPlane)) / 25.0;
    for(int i = 0; i < samples; ++i)
    {
        float closestDepth = texture(PointShadowMap, ShadowCubeCoords(fragToLight + gridSamplingDisk[i] * diskRadius, light.ShadowLayer)).r;
        closestDepth *= light.FarPlane;   // undo mapping [0;1]
        if(currentDepth - bias > closestDepth)
            shadow += 1.0;
    }
    shadow /= float(samples);
        
    // display closestDepth as debug (to visualize depth cubemap)
    // FragColor = vec4(vec3(closestDepth / FarPlane), 1.0);    
        
    return shadow;
}

float ShadowCalculationCubeHard(vec3 fragPos, SLight light)
{
    // get vector between fragment position and light position
    vec3 fragToLight = fragPos - light.Position;
    // use the light to fragment vector to sample from the depth map    
    float closestDepth = texture(PointShadowMap, ShadowCubeCoords(fragToLight, light.ShadowLayer)).r;
    // it is currently in linear range between [0,1]. Re-transform back to original value
    closestDepth *= light.FarPlane;
    // now get current linear depth as the length between the fragment and light position
    float currentDepth = length(fragToLight);
    // now test for shadows
    float bias = 0.05; 
    float shadow = currentDepth -  bias > closestDepth ? 1.0 : 0.0;

    return shadow;
}  

float ShadowCalculation(vec4 fragPosLightSpace, SLight light)
{
	// perform perspective divide
	vec3 projCoords = fragPosLightSpace.xyz / fragPosLightSpace.w;
	// transform to [0,1] range
	projCoords = projCoords * 0.5 + 0.5;
	// get closest depth value from light's perspective (using [0,1] range fragPosLight as coords)
	float closestDepth = texture(DirectionalShadowMap, GetShadowCoords(projCoords.xy, light.ShadowLayer)).r;
	// get depth of current fragment from light's perspective
	float currentDepth = projCoords.z;
	// calculate bias (based on depth map resolution and slope)
	vec3 normal = normalize(Normal);
	vec3 lightDir = normalize(light.Position - FragPos);
	float bias = max(0.05 * (1.0 - dot(normal, lightDir)), 0.005);
	// check whether current frag pos is in shadow
	// float shadow = currentDepth - bias > closestDepth  ? 1.0 : 0.0;
	// PCF
	float shadow = 0.0;
	vec2 texelSize = 1.0 / textureSize(DirectionalShadowMap, 0).xy;
	for(int x =- 1; x <= 1; ++ x)
	{
		for(int y =- 1; y <= 1; ++ y)
		{
			float pcfDepth = texture(DirectionalShadowMap, GetShadowCoords(projCoords.xy + vec2(x, y) * texelSize, light.ShadowLayer)).r;
			shadow += currentDepth - bias > pcfDepth ? 1.0 : 0.0;
		}
	}
	shadow /= 9.0;
	
	// keep the shadow at 0.0 when outside the FarPlane region of the light's frustum.
	if (projCoords.z > 1.0)
    	shadow = 0.0;

	return shadow;
}

#endif