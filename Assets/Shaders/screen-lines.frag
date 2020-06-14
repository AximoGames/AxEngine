#version 330 core
in vec4 Color;
out vec4 FragColor;

void main()
{
	FragColor = Color; // set all 4 vector values to 1.0
}