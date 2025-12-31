#version 450 core
//in vec3 fragPos;
in vec3 fragPosNorm;
layout(binding = 0) uniform sampler3D voxelTexture;
//layout(rgba32f, binding = 1) uniform image3D voxelTex;
layout (location = 0) out vec4 FragColor;

void main() {
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
        FragColor = texture(voxelTexture, fragPosNorm.xzy);
        if (FragColor.a < 0.9) {
            FragColor = vec4(fragPosNorm, 1.0);
        }
    }
}