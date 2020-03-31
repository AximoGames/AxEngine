#version 330 core
layout (location = 0) in vec3 aPos;
layout (location = 1) in vec3 aNormal;
layout (location = 2) in vec2 aTexCoords;

out vec3 FragPos;
out vec2 TexCoords;
out vec3 Normal;

uniform mat4 Model;
uniform mat4 View;
uniform mat4 Projection;

void main()
{
    vec4 worldPos = vec4(aPos, 1.0) * Model;
    //FragPos = worldPos.xyz; 
    FragPos = worldPos.xyz; 
    TexCoords = aTexCoords;
    
    mat3 normalMatrix = transpose(inverse(mat3(Model)));
    Normal = aNormal * normalMatrix;

    gl_Position =  worldPos * View * Projection;
}