SLightResult lightResult;
lightResult.Diffuse = vec3(0);
lightResult.Specular = vec3(0);
lightResult.Shadow = 0;

// then calculate lighting as usual
vec3 lighting  = matDiffuse * matAmbient; // hard-coded ambient component

for(int i = 0; i < LightCount; ++i)
{
    SLight light = lights[i];

    // diffuse
    vec3 lightDir = normalize(light.Position - FragPos);
    vec3 diffuse = max(dot(normal, lightDir), 0.0) * matDiffuse * light.Color;
    // specular
    vec3 halfwayDir = normalize(lightDir + viewDir);  
    float spec = pow(max(dot(normal, halfwayDir), 0.0), matShininess);
    vec3 specular = light.Color * spec * matSpecular;
    // attenuation
    float distance = length(light.Position - FragPos);
    float attenuation = 1.0 / (1.0 + light.Linear * distance + light.Quadratic * distance * distance);

    //attenuation = 1.0; // debug

    float shadow;
    if(light.DirectionalLight == 1) {
        shadow = ShadowCalculation(vec4(FragPos, 1.0) * light.LightSpaceMatrix, light);
        //shadow = 0;
    }
    else {
        shadow = ShadowCalculationCubeSoft(FragPos, light);
        //shadow = 0;
    }

    lightResult.Diffuse += diffuse * attenuation;
    lightResult.Specular += specular * attenuation;
    lightResult.Shadow += shadow * attenuation;

    //shadow *= attenuation; // just a test
}

lighting += (lightResult.Diffuse + lightResult.Specular) * (1.0 - lightResult.Shadow);

FragColor = vec4(lighting, 1.0);
