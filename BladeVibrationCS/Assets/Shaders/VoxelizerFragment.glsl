#version 450 core
in vec3 fragPosNorm, fragPos;
in flat int matID;
in float matIDf;

layout(location = 0) out vec4 outProperties;
layout(location = 1) out vec4 voxelTex;
layout(location = 2) out vec4 extrasTex;
#define MAX_MATERIALS 8

uniform float MatProps_stiffness[MAX_MATERIALS];
uniform float boundRadius;
uniform float Y;
//uniform int Yi;
uniform int RENDER_MODE;
uniform vec2 screenSize;
uniform float DefaultStiffness;

//#define SIMPLIFIED

#if defined(SIMPLIFIED)
void main() {
	extrasTex = vec4(0.0);

	vec3 color = vec3(0.0);
	switch (RENDER_MODE) {
	case 0: color = vec3(0.7); break;
	case 1: color = vec3(1.0, 0.0, 0.0); break; // Red
	case 2: color = vec3(0.0, 1.0, 0.0); break; // Green
	case 3: color = vec3(0.0, 0.0, 1.0); break; // Blue
	case 4: color = vec3(0.9, 0.9, 0.0); break; // Yellow
	case 5: color = vec3(0.9, 0.0, 0.9); break; // Magenta
	case 6: color = vec3(
		MatProps_stiffness[matID] / boundRadius,
		Y * screenSize.x / DefaultStiffness, // Use all the uniforms to avoid warnings
		float(matID) / float(MAX_MATERIALS)
	); break;
	}
	outProperties = vec4(color, 1.0);
	voxelTex = outProperties.zyxw; // Swizzle to test because why not :)
}

#else

void main() {
	if (gl_FragCoord.x > (0.4 * screenSize.x)) {
		outProperties = vec4(1.0, float(matID) / 8, sin(4 * fragPosNorm.x), 1.0);
		voxelTex = outProperties;
		extrasTex = outProperties;
		return;
	}
	if (RENDER_MODE == 4) { // Simple returns during stencil-draw pass or simple test output
		outProperties = vec4(float(matID) / 8, 1.0, sin(4 * fragPosNorm.x), 1.0);
		voxelTex = outProperties;
		extrasTex = outProperties;
		return;
	}

	// We can't be rendering simply one plane as we want to get the stiffness per-material
	// Thus, we're rendering the full model, but we want to render the insides, not the outside as is usual
	// So, assuming depth test is enabled, we render backfaces as setters of the stiffness
	//   and frontfaces as resetters to default values

	float stiffness = gl_FrontFacing ? DefaultStiffness : MatProps_stiffness[matID];

	float modelID = matIDf / 7.0;
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
		outProperties = vec4(posVis * 0.5, 1.0);

	} else {
		float R = 0.5 + 0.5 * sin(fragPosNorm.x);
		float G = 0.5 + 0.5 * sin(fragPosNorm.y);
		float B = 0.5 + 0.5 * sin(fragPosNorm.z);
		outProperties = vec4(R, G, B, 1.0);
	}

	voxelTex = outProperties;
	extrasTex = vec4(float(matID), depth, 0.0, 1.0);

	if (RENDER_MODE == 2) {
		// Convert real data to something easier to visualize (aka use RenderMode 1)
		float stiffVis = log2(stiffness / 1e9 + 1.0) / 10.0;
		stiffVis = clamp(stiffVis, 0.0, 1.0);
		outProperties = vec4(stiffVis, modelID, depth, 1.0);
	}
}
#endif