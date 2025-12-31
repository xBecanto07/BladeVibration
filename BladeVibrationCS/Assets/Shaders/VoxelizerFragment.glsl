#version 450 core
in vec3 fragPos;
in flat int matID;
in float matIDf;

layout(location = 0) out vec4 outProperties;
#define MAX_MATERIALS 8

uniform float MatProps_stiffness[MAX_MATERIALS];
uniform float boundRadius;
uniform float Y;
uniform int RENDER_MODE;
uniform vec2 screenSize;
uniform float DefaultStiffness;

void main() {
	// We can't be rendering simply one plane as we want to get the stiffness per-material
	// Thus, we're rendering the full model, but we want to render the insides, not the outside as is usual
	// So, assuming depth test is enabled, we render backfaces as setters of the stiffness
	//   and frontfaces as resetters to default values

	//float stiffnessMultiplier = fragPos.x * fragPos.x + Y * Y;
	//float stiffness = MatProps_stiffness[matID];
	//stiffness *= mix(1.0, 1.1, stiffnessMultiplier/boundRadius);
	//float stiffness = MatProps_stiffness[matID];
	float stiffness = gl_FrontFacing ? DefaultStiffness : MatProps_stiffness[matID];

	float modelID = matIDf / 7.0;
	//float modelID = float(matID % 2);
	float depth = (boundRadius + fragPos.z) / (2 * boundRadius);

	if (RENDER_MODE == 1) { // Debug: stiffness visualization
		float stiffVis = log2(stiffness / 1e9 + 1.0) / 10.0;
		stiffVis = clamp(stiffVis, 0.0, 1.0);

		outProperties = vec4(stiffVis, modelID, depth, 1.0);

	} else if (RENDER_MODE == 2) { // Actual stiffness values
		outProperties = vec4(stiffness, modelID, depth, 1.0);

	} else if (RENDER_MODE == 3) { // Height visualization
		float depthVis = gl_FragCoord.x < (0.5 * screenSize.x) ? Y : depth;
		outProperties = vec4(0.0, modelID, depthVis, 1.0);

	} else {
		float R = 0.5 + 0.5 * sin(fragPos.x);
		float G = 0.5 + 0.5 * sin(fragPos.y);
		float B = 0.5 + 0.5 * sin(fragPos.z);
		outProperties = vec4(R, G, B, 1.0);
	}
}