#version 450 core
layout(location = 0) in vec3 aPos;
flat out vec3 vWorldPos;
uniform mat4 model;
uniform mat4 view;
uniform mat4 proj;

void main() {
    gl_Position = proj * view * model * vec4(aPos, 1.0);
    vWorldPos = vec3(model * vec4(aPos, 1.0));
}