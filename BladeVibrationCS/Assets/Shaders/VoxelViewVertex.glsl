#version 450 core
layout(location = 0) in vec3 aPos;
uniform mat4 model;
uniform mat4 view;
uniform mat4 proj;
uniform vec3 TextureMin, TextureSize;
//out vec3 fragPos;
out vec3 fragPosNorm;

void main() {
    vec4 tmp = model * vec4(aPos, 1.0);
    gl_Position = proj * view * tmp;
    //fragPos = vec3(tmp);
    fragPosNorm = (vec3(tmp) - TextureMin) / TextureSize;
    //fragPosNorm *= 8.0;
    //fragPosNorm = vec3(tmp);
}