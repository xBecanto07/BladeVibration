#version 450 core

layout(location = 0) in vec3 aPos;
layout(location = 1) in vec3 aNormal;
layout(location = 2) in vec2 aTexCoord;
layout(location = 3) in float aMatID;

out vec3 fragPos, fragPosNorm;
out flat int matID;
out float matIDf;

uniform mat4 view;
uniform mat4 proj;
uniform int matOffset;
uniform vec3 offset;
uniform vec3 scale;
uniform float Y;
uniform vec3 minBound, boundSize;
uniform float voxelSize;

void main() {
    matID = int(aMatID + 0.4) + matOffset;
    matIDf = aMatID + float(matOffset);
    vec4 tmp = vec4(aPos * scale + offset, 1.0);
    gl_Position = proj * view * tmp;
    fragPosNorm = vec3((tmp.xyz - minBound) / boundSize);
    fragPosNorm.y = Y;
    fragPos = vec3(tmp.x, Y * voxelSize + minBound.y, tmp.z);
}