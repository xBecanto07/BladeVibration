#version 450 core
in vec3 vWorldPos;
layout(location = 0) out vec4 outProperties;
#define MODE_BORDER 1

#define MODE_CENTER 2
uniform int ActMode;
uniform float zPlaneStart, zPlaneEnd;
uniform vec3 modelMin, modelMax;
uniform float youngsModulus;

void main() {
	if (ActMode == MODE_BORDER) {
		outProperties = vec4(1.0, 0.0, 0.0, 1.0); // Default to red for border mode

		//vec3 normalizedPos = (vWorldPos - modelMin) / (modelMax - modelMin);
		//outStiffness = normalizedPos.z;
		//outStiffness = sin(vWorldPos.x * 3.14159265 * 2.0)
		//	+ cos(vWorldPos.y * 3.14159265 * 2.0)
		//	+ sin(vWorldPos.z * 3.14159265 * 2.0);

		//outProperties = vec4(
		//	sin(vWorldPos.x * 3.14159265 * 2.0) * 0.5 + 0.5,
		//	cos(vWorldPos.y * 3.14159265 * 2.0) * 0.5 + 0.5,
		//	sin(vWorldPos.z * 3.14159265 * 2.0) * 0.5 + 0.5,
		//	1.0);

	} else if (ActMode == MODE_CENTER) {
		if (vWorldPos.z < zPlaneStart || vWorldPos.z > zPlaneEnd) {
			discard;
		}

		// No valid logic to set stiffness of the material at [X,Y,Z] at this time.
		//   So interpolate between 1.0 at the center to 0.5 at the edges.
		float maxDist = (modelMax.x - modelMin.x) * (modelMax.x - modelMin.x)
					  + (modelMax.y - modelMin.y) * (modelMax.y - modelMin.y)
					  + (modelMax.z - modelMin.z) * (modelMax.z - modelMin.z);
		float distToMin = (vWorldPos.x - modelMin.x) * (vWorldPos.x - modelMin.x)
						+ (vWorldPos.y - modelMin.y) * (vWorldPos.y - modelMin.y)
						+ (vWorldPos.z - modelMin.z) * (vWorldPos.z - modelMin.z);
		float distToMax = (vWorldPos.x - modelMax.x) * (vWorldPos.x - modelMax.x)
						+ (vWorldPos.y - modelMax.y) * (vWorldPos.y - modelMax.y)
						+ (vWorldPos.z - modelMax.z) * (vWorldPos.z - modelMax.z);
		float mult = distToMin < distToMax ? distToMin : distToMax;
		mult = mult / maxDist;
		outProperties = vec4(mix(1.0, 0.5, mult) * youngsModulus, 1.0, 1.0, 1.0);
	} else {
		//discard;
		outProperties = vec4(0.0, 0.0, 1.0, 1.0); // Default to blue for other modes
	}
}