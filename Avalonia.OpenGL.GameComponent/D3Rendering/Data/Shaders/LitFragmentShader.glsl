#version 330 core
out vec4 FragColor;

in vec3 Normal;
uniform vec4 uColor;

uniform vec3 lightDir;
uniform vec3 lightColor;
uniform float ambientStrength;

void main()
{
    vec3 ambient = ambientStrength * lightColor;
  
    vec3 norm = normalize(Normal);
    vec3 normalizedLightDir = normalize(lightDir); 
    float diff = max(dot(norm, -normalizedLightDir), 0.0); 
    vec3 diffuse = diff * lightColor;
    
    vec3 result = (ambient + diffuse) * uColor.rgb;
    FragColor = vec4(result, uColor.a);
}