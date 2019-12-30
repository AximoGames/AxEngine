#version 330 core

layout (location = 0) in vec3 aPos;
layout (location = 1) in vec3 aNormal;
layout (location = 2) in vec2 aTexCoords;

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;
uniform mat4 lightSpaceMatrix;

out vec3 Normal;
out vec3 FragPos;
out vec2 TexCoords;
out vec4 FragPosLightSpace;

out VS_FS_INTERFACE { 
    vec4 shadow_coord; 
    vec3 world_coord; 
    vec3 eye_coord; 
    vec3 normal; 
} vertex;

void main()
{/*
    gl_Position = vec4(aPos, 1.0) * model * view * projection;
*/    FragPos = vec3(vec4(aPos, 1.0) * model);
    Normal = transpose(inverse(mat3(model))) * aNormal;
    TexCoords = aTexCoords;
    // shadow
    FragPosLightSpace = lightSpaceMatrix * vec4(FragPos, 1.0); // BUGFIX: aPos --> FragPos

    vec4 position = vec4(aPos,1.0);
    vec4 world_pos =  position * model ;
    vec4 eye_coord = world_pos * view ;
    vec4 clip_pos = eye_coord * projection ;

    vertex.world_coord = world_pos.xyz;
    vertex.eye_coord = eye_coord.xyz;
    vertex.shadow_coord =  world_pos * lightSpaceMatrix ;
    vertex.normal =   aNormal * mat3( model * view) ;
    gl_Position = clip_pos;
}