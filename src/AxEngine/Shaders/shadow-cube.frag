#version 330 core
#extension GL_GOOGLE_include_directive : enable

#include "common/header.glsl"

in vec4 FragPos;

uniform Light light;
uniform float far_plane;

void main()
{
	float lightDistance = length(FragPos.xyz - light.position);
	
	// map to [0;1] range by dividing by far_plane
	lightDistance = lightDistance / far_plane;
	
	// write this as modified depth
	gl_FragDepth = lightDistance;
}