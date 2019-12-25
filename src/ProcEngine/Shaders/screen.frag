#version 330 core
out vec4 FragColor;
  
in vec2 TexCoords;

uniform sampler2D screenTexture;

void main()
{ 
    vec4 color = texture(screenTexture, TexCoords);

    // Negative color (effect)
    //color = vec4(1-color.r, 1-color.g, 1-color.b, color.a);

    //color = vec4(0, (1-gl_FragCoord.z)*100000, 0, 1);


    FragColor = color;
}