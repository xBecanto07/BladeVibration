#version 450 core
//in vec3 fragPos;
in vec3 fragPosNorm;
layout(binding = 0) uniform sampler3D voxelTexture;
layout(binding = 1) uniform sampler3D extrasTexture;
layout (location = 0) out vec4 FragColor;
uniform int renderMode;

void main() {
    if (renderMode == 0) {
        FragColor = vec4(
            0.5 + 0.5 * sin(fragPosNorm.x),
            0.5 + 0.5 * sin(fragPosNorm.y),
            0.5 + 0.5 * sin(fragPosNorm.z),
            1.0);
        return;
    }
    if (renderMode == 3) {
        FragColor = vec4(
            1.0 - fragPosNorm.x,
            1.0 - fragPosNorm.y,
            1.0 - fragPosNorm.z,
            1.0);
        return;
    }

    FragColor = vec4(fragPosNorm, 1.0);
    if (fragPosNorm.x <= -0.1 || fragPosNorm.x >= 1.1 ||
        fragPosNorm.y <= -0.1 || fragPosNorm.y >= 1.1 ||
        fragPosNorm.z <= -0.1 || fragPosNorm.z >= 1.1 ) {
        //discard; // Discard fragments that are far out of bounds
        FragColor *= 0.6;
    } else if ( fragPosNorm.x <= 0 || fragPosNorm.x >= 1 ||
                fragPosNorm.y <= 0 || fragPosNorm.y >= 1 ||
                fragPosNorm.z <= 0 || fragPosNorm.z >= 1 ) {
        //FragColor = vec4(0.1, 0.4, 0.0, 1.0); // Draw out-of-bounds areas in dark green
        float dX = max( -fragPosNorm.x, fragPosNorm.x - 1.0);
        float dY = max( -fragPosNorm.y, fragPosNorm.y - 1.0);
        float dZ = max( -fragPosNorm.z, fragPosNorm.z - 1.0);
        float d = max(dX, max(dY, dZ));
        d = 1/(1.0 + d);
        //FragColor = d * vec4(0.1, 0.6, 0.0, 1.0); // Dark green fading out
        FragColor = mix(FragColor, vec4(0.1, 0.6, 0.0, 1.0) * d, 0.2);

    } else {
        vec4 data = texture(voxelTexture, fragPosNorm.xzy);
        vec4 extra = texture(extrasTexture, fragPosNorm.xzy);
        if (extra.w < 0.9) {
            FragColor = vec4(fragPosNorm, 1.0);
        } else {
            if (renderMode == 2) {
                float weight = log2(data.w / 1e9 + 1.0) / 10.0;
                FragColor = vec4(
                    fract(data.x * 0.01 * weight),
                    fract(data.y * 0.01 * weight),
                    fract(data.z * 0.01 * weight),
                    1.0);

            } if (renderMode == 1) {
                FragColor = vec4(
                    //log2(data.w / 1e9 + 1.0) / 10.0,
                    extra.w,
                    (extra.x / 255.0) * 10.0, // Model ID * 10 for visibility
                    data.y, // Y-depth
                    1.0);
            } else {
                FragColor = data;
            }
        }
    }
}