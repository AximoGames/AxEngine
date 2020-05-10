#version 330 core
#extension GL_ARB_shading_language_include : enable

#include "common/header.glsl"

in vec4 FragPos;

uniform SLight Light;

void main()
{
	float lightDistance = length(FragPos.xyz - Light.Position);
	
	// map to [0;1] range by dividing by FarPlane
	lightDistance = lightDistance / Light.FarPlane;
	
	// write this as modified depth
	gl_FragDepth = lightDistance;
}