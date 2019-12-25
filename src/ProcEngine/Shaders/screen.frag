#version 330 core
out vec4 FragColor;
  
in vec2 TexCoords;

uniform sampler2D screenTexture;

void main()
{ 
    vec4 color = texture(screenTexture, TexCoords);

    // Negative color (effect)
    //color = vec4(1-color.r, 1-color.g, 1-color.b, color.a);

    FragColor = color;
}