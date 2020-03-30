SLightResult lightResult;
lightResult.Diffuse = vec3(0);
lightResult.Specular = vec3(0);
lightResult.Shadow = 0;

// then calculate lighting as usual
vec3 lighting  = matDiffuse * matAmbient; // hard-coded ambient component

for(int i = 0; i < lightCount; ++i)
{
    SLight light = lights[i];

    // diffuse
    vec3 lightDir = normalize(light.position - FragPos);
    vec3 diffuse = max(dot(normal, lightDir), 0.0) * matDiffuse * light.color;
    // specular
    vec3 halfwayDir = normalize(lightDir + viewDir);  
    float spec = pow(max(dot(normal, halfwayDir), 0.0), matShininess);
    vec3 specular = light.color * spec * matSpecular;
    // attenuation
    float distance = length(light.position - FragPos);
    float attenuation = 1.0 / (1.0 + light.linear * distance + light.quadratic * distance * distance);

    //attenuation = 1.0; // debug

    float shadow;
    if(light.directionalLight == 1) {
        shadow = ShadowCalculation(vec4(FragPos, 1.0) * light.lightSpaceMatrix, light);
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
