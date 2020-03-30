#version 330 core
layout(triangles)in;
layout (triangle_strip, max_vertices = 3) out;

uniform int ShadowLayer;

out vec4 FragPos; // FragPos from GS (output per emitvertex)

void main()
{
	gl_Layer = ShadowLayer; // built-in variable that specifies to which face we render.
	for(int i = 0; i < 3; ++ i)// for each triangle's vertices
	{
		gl_Position = gl_in[i].gl_Position;
		EmitVertex();
	}
	EndPrimitive();
}