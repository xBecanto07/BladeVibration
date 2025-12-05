#version 450 core
layout(location = 0) in vec3 aPos;
uniform vec3 colorA;
uniform vec3 colorB;
uniform vec3 offset;
uniform float scale;
uniform float time;
out vec4 color;
out vec2 texCoords;

void main() {
    gl_Position = vec4(aPos * 0.03 * scale + offset, 1.0);
    // Rotate around X axis based on time:
    float angle = sin(time + aPos.y * 10.0) * 0.5;
    float cosAngle = cos(angle);
    mat3 rotationX = mat3(
        1.0, 0.0, 0.0,
        0.0, cosAngle, -sin(angle),
        0.0, sin(angle), cosAngle
    );
    gl_Position.xyz = rotationX * gl_Position.xyz;

    vec3 blended = colorA * (1.0 - aPos.y) + colorB * aPos.y;
    blended = clamp(blended, 0.0, 1.0);
    color = vec4(blended, 1.0);
    texCoords = vec2(aPos.x, aPos.y);
}