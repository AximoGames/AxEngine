#version 330 core
layout(triangles)in;
layout(triangle_strip, max_vertices = 18)out;

uniform mat4 shadowMatrices[6];
uniform int ShadowLayer;

out vec4 FragPos; // FragPos from GS (output per emitvertex)

void main()
{
	int offset = ShadowLayer * 6;
	for(int face = 0; face < 6; ++ face)
	{
		gl_Layer = offset + face; // built-in variable that specifies to which face we render.
		for(int i = 0; i < 3; ++ i)// for each triangle's vertices
		{
			FragPos = gl_in[i].gl_Position;
			gl_Position = FragPos * shadowMatrices[face];
			EmitVertex();
		}
		EndPrimitive();
	}
}