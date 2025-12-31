#version 450 core

layout(location = 0) in vec3 aPos;
layout(location = 1) in vec3 aNormal;
layout(location = 2) in vec2 aTexCoord;
layout(location = 3) in float aMatID;

uniform vec3 colorA;
uniform vec3 colorB;
uniform vec3 offset;
uniform mat4 model;
uniform mat4 view;
uniform mat4 proj;
uniform vec3 scale;
uniform int matOffset;

out vec4 color;
out vec3 fragPos;
out vec3 fragNormal;
out vec2 uvCoords;
out vec3 fragPosOrig;

out flat int matID;

void main() {
    fragPosOrig = aPos;
    matID = int(aMatID + 0.4) + matOffset;

    vec4 tmp = model * vec4(aPos * scale + offset, 1.0);
    gl_Position = proj * view * tmp;
    fragPos = vec3(tmp);
    fragNormal = normalize(mat3(transpose(inverse(model))) * aNormal);
    uvCoords = aTexCoord;

    vec3 blended = colorA * (1.0 - aPos.y) + colorB * aPos.y;
    blended = clamp(blended, 0.0, 1.0);
    color = vec4(blended, 1.0);
}