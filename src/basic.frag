# version 330 core

struct Material {
    vec3 diffuse;
    vec3 specular;
    float shininess;
};
uniform Material material;

struct Light {
    vec4  position;
    vec3  direction;
    float cutOff;
    float outerCutOff;

    vec3 color;
    
    /*
    vec3 ambient;
    vec3 diffuse;
    vec3 specular;*/
};
uniform Light light;

uniform vec3 cameraPosWorld;

in vec3 normalWorld;
in vec3 fragmentWorld;

out vec4 outColor;

void main() {
    // Ambient lighting (no change here)
    vec3 ambient = light.color * material.diffuse;

    vec3 norm = normalize(normalWorld);

    vec3 lightDir = light.position.w == 0.0
        ? normalize(- light.position.xyz)
        : normalize(light.position.xyz - fragmentWorld);
    float theta = dot(lightDir, normalize(-light.direction));
    float intensity = smoothstep(light.outerCutOff, light.cutOff, theta);    

    if(theta > light.cutOff) {
        float diff = max(dot(norm, lightDir), 0.0);
        vec3 diffuse = light.color * diff * material.diffuse * intensity;

        vec3 viewDir = normalize(cameraPosWorld - fragmentWorld);
        vec3 reflectDir = reflect(-lightDir.xyz, norm);
        
        float spec = max(0, pow(dot(reflectDir, viewDir) , material.shininess));
        vec3 specular = light.color * spec * material.specular * intensity;

        vec3 finalColor = ambient + diffuse + specular;
        outColor = vec4(finalColor, 1.0);
   } else {
        outColor = vec4(light.color * 0.5 * (material.diffuse * 0.1), 1.0);
  }
}

