#version 450 core
in vec3 fragPosNorm, fragPos;
in flat int matID;
in float matIDf;

layout(location = 0) out vec4 outProperties;
layout(rgba32f, binding = 0) uniform image3D voxelTex;
layout(rgba32f, binding = 1) uniform image3D extrasTex;
#define MAX_MATERIALS 8

uniform float MatProps_stiffness[MAX_MATERIALS];
uniform float boundRadius;
uniform float Y;
uniform int Yi;
uniform int RENDER_MODE;
uniform vec2 screenSize;
uniform float DefaultStiffness;
uniform vec3 minBound, boundSize;
//uniform float voxelSize;
uniform vec3 voxelCounts;
uniform vec3 fragOffset;

void main() {
	if (RENDER_MODE == 4) {
		outProperties = vec4(fragPosNorm, 1.0);
		return;
	}

	// We can't be rendering simply one plane as we want to get the stiffness per-material
	// Thus, we're rendering the full model, but we want to render the insides, not the outside as is usual
	// So, assuming depth test is enabled, we render backfaces as setters of the stiffness
	//   and frontfaces as resetters to default values

	//float stiffnessMultiplier = fragPosNorm.x * fragPosNorm.x + Y * Y;
	//float stiffness = MatProps_stiffness[matID];
	//stiffness *= mix(1.0, 1.1, stiffnessMultiplier/boundRadius);
	//float stiffness = MatProps_stiffness[matID];
	float stiffness = gl_FrontFacing ? DefaultStiffness : MatProps_stiffness[matID];

	float modelID = matIDf / 7.0;
	//float modelID = float(matID % 2);
	float depth = (boundRadius + fragPosNorm.z) / (2 * boundRadius);

	if (RENDER_MODE == 1) { // Debug: stiffness visualization
		float stiffVis = log2(stiffness / 1e9 + 1.0) / 10.0;
		stiffVis = clamp(stiffVis, 0.0, 1.0);

		outProperties = vec4(stiffVis, modelID, depth, 1.0);
		outProperties = vec4(1-fragPosNorm, 1.0);

	} else if (RENDER_MODE == 2) { // Actual stiffness values
		outProperties = vec4(fragPos, stiffness);

	} else if (RENDER_MODE == 3) { // Height visualization
		vec3 posVis = vec3 (
			gl_FragCoord.x < (0.5 * screenSize.x) ? fragPosNorm.x : 0.0,
			gl_FragCoord.x < (0.5 * screenSize.x) ? Y : depth,
			gl_FragCoord.x < (0.5 * screenSize.x) ? fragPosNorm.z : modelID );
		outProperties = vec4(posVis, 1.0);

	} else {
		float R = 0.5 + 0.5 * sin(fragPosNorm.x);
		float G = 0.5 + 0.5 * sin(fragPosNorm.y);
		float B = 0.5 + 0.5 * sin(fragPosNorm.z);
		outProperties = vec4(R, G, B, 1.0);
	}

	ivec3 coord = ivec3(int(gl_FragCoord.x), Yi, int(gl_FragCoord.y));
	coord += ivec3(fragOffset);

	imageStore(voxelTex, coord, outProperties);
	imageStore(extrasTex, coord, vec4(float(matID), depth, 0.0, 1.0));

	if (RENDER_MODE == 2) {
		// Convert real data to something easier to visualize (aka use RenderMode 1)
		float stiffVis = log2(stiffness / 1e9 + 1.0) / 10.0;
		stiffVis = clamp(stiffVis, 0.0, 1.0);
		outProperties = vec4(stiffVis, modelID, depth, 1.0);
	}
}