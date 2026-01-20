#version 450 core

layout(location = 0) in vec3 aPos;
layout(location = 1) in vec3 aNormal;
layout(location = 2) in vec2 aTexCoord;
layout(location = 3) in float _aMatID;

uniform vec3 offset;
uniform mat4 model;
uniform mat4 view;
uniform mat4 proj;
uniform vec3 scale;
//uniform int SwizzleIndex, SwizzleData, InvertAxis;
//uniform vec3 TextureMin, TextureSize;

out vec3 wPos;

void main() {
    gl_Position = proj * view * model * vec4(aPos * scale + offset, 1.0);
    wPos = aPos;
}