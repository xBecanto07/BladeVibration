#version 450 core

layout(location = 0) in vec3 aPos;
layout(location = 1) in vec3 aNormal;
layout(location = 2) in vec2 aTexCoord;
layout(location = 3) in float aMatID;

out vec3 fragPos;
out flat int matID;
out float matIDf;

uniform mat4 model;
uniform mat4 view;
uniform mat4 proj;
uniform int matOffset;
uniform vec3 offset;
uniform vec3 scale;
uniform vec3 minBound, boundSize;

void main() {
    matID = int(aMatID + 0.4) + matOffset;
    matIDf = aMatID + float(matOffset);
    vec4 tmp = model * vec4(aPos * scale + offset, 1.0);
    gl_Position = proj * view * 0.25 * tmp;
    fragPos = vec3((tmp.xyz - minBound) / boundSize);
}