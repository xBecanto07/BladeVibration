#version 450 core
in vec3 fragPos;
uniform sampler3D voxelTexture;
layout (location = 0) out vec4 FragColor;

void main() {
    if (fragPos.x < 0.0 || fragPos.x > 1.0 ||
        fragPos.y < 0.0 || fragPos.y > 1.0 ||
        fragPos.z < 0.0 || fragPos.z > 1.0) {
        //FragColor = vec4(0.1, 0.4, 0.0, 1.0); // Draw out-of-bounds areas in dark green
        float dXn = -fragPos.x;
        float dXp = fragPos.x - 1.0;
        float dYn = -fragPos.y;
        float dYp = fragPos.y - 1.0;
        float dZn = -fragPos.z;
        float dZp = fragPos.z - 1.0;
        float d = max(max(dXn, dXp), max(max(dYn, dYp), max(dZn, dZp)));
        d = 1/(1.0 + d);
        FragColor = d * vec4(0.1, 0.6, 0.0, 1.0); // Dark green fading out
    } else {
        float v = texture(voxelTexture, fragPos).r;
        if (v < 0.001)
            FragColor = vec4(1.0, 1.0, 0.0, 1.0); // Highlight empty voxels in yellow
        else
            FragColor = vec4(1 - v, 0.0, v, 1.0); // Color based on voxel value
    }
}