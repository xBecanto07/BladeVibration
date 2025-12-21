#version 450 core
layout(location = 0) in vec3 aPos;
uniform vec3 colorA;
uniform vec3 colorB;
uniform vec3 offset;
uniform mat4 model;
uniform mat4 view;
uniform mat4 proj;
uniform float scale;
out vec4 color;
out vec2 texCoords;

void main() {
    // Rotate around X axis based on time:
    gl_Position = proj * view * model * vec4(aPos * 0.03 * scale + offset, 1.0);

    vec3 blended = colorA * (1.0 - aPos.y) + colorB * aPos.y;
    blended = clamp(blended, 0.0, 1.0);
    color = vec4(blended, 1.0);
    texCoords = vec2(aPos.x, aPos.y);
}