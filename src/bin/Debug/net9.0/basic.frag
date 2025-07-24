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
    // Get normalized directions
    vec3 norm = normalize(normalWorld);
    vec3 lightDir = light.position.w == 0.0
        ? normalize(-light.position.xyz)
        : normalize(light.position.xyz - fragmentWorld);
    
    // Calculate spotlight effect
    float theta = dot(lightDir, normalize(-light.direction));
    float intensity = smoothstep(light.outerCutOff, light.cutOff, theta);
    
    // Ambient lighting (apply intensity to ambient too)
    vec3 ambient = light.color * material.diffuse * intensity;
    
    // Diffuse lighting
    float diff = max(dot(norm, lightDir), 0.0);
    vec3 diffuse = light.color * diff * material.diffuse * intensity;
    
    // Specular lighting
    vec3 viewDir = normalize(cameraPosWorld - fragmentWorld);
    vec3 reflectDir = reflect(-lightDir, norm);
    float spec = pow(max(dot(viewDir, reflectDir), 0.0), material.shininess);
    vec3 specular = light.color * spec * material.specular * intensity;
    
    // Combine results
    vec3 finalColor = ambient + diffuse + specular;
    outColor = vec4(finalColor, 1.0);
}

