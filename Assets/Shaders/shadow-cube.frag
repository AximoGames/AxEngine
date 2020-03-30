#version 330 core
#extension GL_GOOGLE_include_directive : enable

#include "common/header.glsl"

in vec4 FragPos;

uniform SLight Light;
uniform float FarPlane;

void main()
{
	float lightDistance = length(FragPos.xyz - Light.Position);
	
	// map to [0;1] range by dividing by FarPlane
	lightDistance = lightDistance / FarPlane;
	
	// write this as modified depth
	gl_FragDepth = lightDistance;
}