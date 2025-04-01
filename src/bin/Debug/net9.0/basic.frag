# version 330 core

struct Material {
    vec3 ambient;
    vec3 diffuse;
    vec3 specular;
    float shininess;
};
uniform Material material;

struct Light {
    vec3  position;
    vec3  direction;
    float cutOff;

    vec3 ambient;
    vec3 diffuse;
    vec3 specular;
};
uniform Light light;

uniform vec3 cameraPosWorld;

in vec3 normalWorld;
in vec3 fragmentWorld;

out vec4 outColor;

void main() {
    // Ambient lighting (no change here)
    vec3 ambient = light.ambient * material.diffuse * 0.1f;

    vec3 norm = normalize(normalWorld);

    vec3 lightDir = normalize(light.position - fragmentWorld);
    float theta = dot(lightDir, normalize(-light.direction));

    if(theta > light.cutOff) {
        float diff = max(dot(norm, lightDir), 0.0);
        vec3 diffuse = light.diffuse * (diff * material.diffuse);

        vec3 viewDir = normalize(cameraPosWorld - fragmentWorld);
        vec3 reflectDir = reflect(-lightDir, norm);
        float spec = pow(max(dot(viewDir, reflectDir), 0.0), material.shininess);
        vec3 specular = light.specular * (spec * material.specular);

        vec3 finalColor = ambient + diffuse + specular;

        outColor = vec4(finalColor, 1.0);
   } else {
        outColor = vec4(ambient, 1.0);
  }
}

