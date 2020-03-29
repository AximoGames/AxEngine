vec3 BlendColor(vec3 textureColor, vec3 color, int blendMode) {
    switch(blendMode)
    {
        case 0:
            return textureColor;
        case 1:
            return color;
        case 2:
            return textureColor * color;
        case 3:
            return textureColor + color;
        case 4:
            return textureColor - color;
    }
}
