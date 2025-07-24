#version 330 core

struct Material {
    vec3 specular;
    float shininess;
};
uniform Material material;

struct Light {
    vec4 position;
    vec3 direction;
    float cutOff;
    float outerCutOff;
    vec3 color;
};
uniform Light light;

uniform vec3 cameraPosWorld;
uniform sampler2D diffuseTex;

uniform float ambientStrength; // Ambient light strength
uniform float whiteFade; // White fade factor

in vec3 normalWorld;
in vec3 fragmentWorld;
in vec2 texCoordVS;

out vec4 outColor;

void main() {
    vec3 norm = normalize(normalWorld);
    vec3 lightDir;
    float intensity = 1.0;

    if (light.position.w == 0.0) {
        lightDir = normalize(-light.position.xyz); 
    } else {
        lightDir = normalize(light.position.xyz - fragmentWorld);
        float theta = dot(lightDir, normalize(-light.direction));
        intensity = smoothstep(light.outerCutOff, light.cutOff, theta);
    }

    vec3 texColor = texture(diffuseTex, texCoordVS).rgb;

    // ✅ Minimal ambient light always applied (regardless of spotlight)
    vec3 ambient = light.color * texColor * ambientStrength;

    // Diffuse lighting (affected by spotlight intensity)
    float diff = max(dot(norm, lightDir), 0.0);
    vec3 diffuse = light.color * diff * texColor * intensity;

    // Specular lighting (also affected by spotlight intensity)
    vec3 viewDir = normalize(cameraPosWorld - fragmentWorld);
    vec3 reflectDir = reflect(-lightDir, norm);
    float spec = pow(max(dot(viewDir, reflectDir), 0.0), material.shininess);
    vec3 specular = light.color * spec * material.specular * intensity;

    vec3 finalColor = ambient + diffuse + specular;
    vec3 fadedColor = mix(finalColor, vec3(1.0, 1.0, 1.0), whiteFade);
    outColor = vec4(fadedColor, 1.0);
}
