#version 450 core

layout(location = 0) in vec3 aPos;
layout(location = 1) in vec3 aNormal;
layout(location = 2) in vec2 aTexCoord;
layout(location = 3) in float _aMatID;

layout(binding = 0) uniform sampler3D voxelTexture;
layout(binding = 1) uniform sampler3D extrasTexture;

uniform vec3 colorA;
uniform vec3 colorB;
uniform vec3 offset;
uniform mat4 model;
uniform mat4 view;
uniform mat4 proj;
uniform vec3 scale;
uniform vec3 TextureMin, TextureSize;
uniform int SwizzleIndex, SwizzleData, InvertAxis;
uniform float Visualize;

out vec4 color;
out vec3 fragPos;
out vec3 fragNormal;
out vec2 uvCoords;
out vec3 fragPosOrig;

out flat int matID;

#define SWZ_XYZ 0
#define SWZ_XZY 1
#define SWZ_YXZ 2
#define SWZ_YZX 3
#define SWZ_ZXY 4
#define SWZ_ZYX 5
#define INV_None 0
#define INV_X 1
#define INV_Y 2
#define INV_Z 3
#define INV_XY 4
#define INV_XZ 5
#define INV_YZ 6
#define INV_XYZ 7

vec3 SwizzleVec3(vec3 v, int swizzle, int invert) {
    switch (swizzle) {
        case SWZ_XYZ: break;
        case SWZ_XZY: v = v.xzy; break;
        case SWZ_YXZ: v = v.yxz; break;
        case SWZ_YZX: v = v.yzx; break;
        case SWZ_ZXY: v = v.zxy; break;
        case SWZ_ZYX: v = v.zyx; break;
    }
    switch (invert) {
        case INV_None: break;
        case INV_X: v.x = -v.x; break;
        case INV_Y: v.y = -v.y; break;
        case INV_Z: v.z = -v.z; break;
        case INV_XY: v.xy = -v.xy; break;
        case INV_XZ: v.xz = -v.xz; break;
        case INV_YZ: v.yz = -v.yz; break;
        case INV_XYZ: v = -v; break;
    }
    return v;
}

void main() {
    vec3 vPos = aPos;
    float aMatID = _aMatID;

    vec3 blended = colorA * (1.0 - vPos.y) + colorB * vPos.y;
    blended = clamp(blended, 0.0, 1.0);
    color = vec4(blended, 1.0);

    vec3 posNorm = SwizzleVec3((vPos - TextureMin) / TextureSize, SwizzleIndex, INV_None);
    if (posNorm.x > 0.0 && posNorm.x < 1.0 &&
        posNorm.y > 0.0 && posNorm.y < 1.0 &&
        posNorm.z > 0.0 && posNorm.z < 1.0)
    {
        vec4 voxelData = texture(voxelTexture, posNorm);
        vec4 extraData = texture(extrasTexture, posNorm);
        //vPos = voxelData.xyz;
        aMatID = extraData.x;
        color = vec4(SwizzleVec3(voxelData.xyz, SwizzleData, InvertAxis), 1.0);
    } else {
        color = vec4(aPos, 1.0);
    }
    if (Visualize < 0)
        color = vec4(color.xyz, 1.0);
    else
        color = vec4(vPos.xyz, 1.0);
    
    color = vec4(fract(abs(color.xyz)), 1.0);
    
    fragPosOrig = vPos;
    //matID = int(aMatID + 0.4);
    matID = 7;

    vec4 tmp = model * vec4(vPos * scale + offset, 1.0);
    gl_Position = proj * view * tmp;
    fragPos = vec3(tmp);
    fragNormal = normalize(mat3(transpose(inverse(model))) * aNormal);
    uvCoords = aTexCoord;

    //vec3 blended = colorA * (1.0 - vPos.y) + colorB * vPos.y;
    //blended = clamp(blended, 0.0, 1.0);
    //color = vec4(blended, 1.0);
}