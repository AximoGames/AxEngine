SLightResult lightResult;
lightResult.Diffuse = vec4(0);
lightResult.Specular = vec4(0);
lightResult.Shadow = 0;

float originalAlpha = matDiffuse.a;

// then calculate lighting as usual
vec4 lighting  = matDiffuse * matAmbient; // hard-coded ambient component

for(int i = 0; i < LightCount; ++i)
{
    SLight light = lights[i];

    // diffuse
    vec3 lightDir;
    if(light.DirectionalLight == 1)
        lightDir = -light.Direction;
    else
        lightDir = normalize(light.Position - FragPos);

    vec4 diffuse = max(dot(normal, lightDir), 0.0) * matDiffuse * light.Color;
    
    // specular
    vec4 specular;
    if(light.DirectionalLight == 1) {
        specular = vec4(0);
    }
    else {
        vec3 halfwayDir = normalize(lightDir + viewDir);  
        float spec = pow(max(dot(normal, halfwayDir), 0.0), matShininess);
        specular = light.Color * spec * matSpecular;
    }

    // attenuation
    float attenuation;
    if(light.DirectionalLight == 1) {
        attenuation = 1.0;
    }
    else {
        float distance = length(light.Position - FragPos);
        attenuation = 1.0 / (1.0 + light.Linear * distance + light.Quadratic * (distance * distance));
    }

    //attenuation = 1.0; // debug

#ifdef USE_SHADOW
    float shadow;
    if(light.DirectionalLight == 1) {
        shadow = ShadowCalculation(vec4(FragPos, 1.0) * light.LightSpaceMatrix, light);
    }
    else {
        shadow = ShadowCalculationCubeSoft(FragPos, light);
    }
    lightResult.Shadow += shadow * attenuation;
#endif

    lightResult.Diffuse += diffuse * attenuation;
    lightResult.Specular += specular * attenuation;

    //shadow *= attenuation; // just a test
}

lighting += (lightResult.Diffuse + lightResult.Specular) * (1.0 - lightResult.Shadow);
lighting.a = originalAlpha;

FragColor = lighting;
