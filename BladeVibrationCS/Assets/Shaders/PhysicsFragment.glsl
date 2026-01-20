#version 450 core

in vec3 wPos;

layout(binding = 0) uniform sampler3D voxelTexture;
layout(binding = 1) uniform sampler3D extrasTexture;

layout (location = 0) out vec4 FragColor;

uniform int SwizzleIndex, SwizzleData, InvertIndex, InvertData, DisplayAxis, Visualize;
uniform vec3 TextureMin, TextureSize;

vec3 SwizzleVec3(vec3 v, int swizzle, int invert) {
	switch (swizzle) {
		case 0: break;
		case 1: v = v.xzy; break;
		case 2: v = v.yxz; break;
		case 3: v = v.yzx; break;
		case 4: v = v.zxy; break;
		case 5: v = v.zyx; break;
		default: v = vec3(0.5, 0.0, 0.0); break; // Error case
	}
	switch (invert) {
		case 0: break;
		case 1: v.x = -v.x; break;
		case 2: v.y = -v.y; break;
		case 3: v.z = -v.z; break;
		case 4: v.xy = -v.xy; break;
		case 5: v.xz = -v.xz; break;
		case 6: v.yz = -v.yz; break;
		case 7: v = -v; break;
		default: v = vec3(0.0, 0.5, 0.0); break; // Error case
	}
	return v;
}

float Vis (float val) {
	// (0.3+1/(1+abs(x))) * (0.5 + 0.5*sin(pi*(2x-0.5)))
	//float mult = 1.5 / (1.0 + abs(val));
	//float mult = val < 0 ? 0.5 : 1.0;
	float mult1 = 1.5 / (1.0 + abs(val));
	float mult2 = 1.0 + 0.8 / (-1.0 - abs(val));
	float mult = clamp(val < 0.0 ? mult2 : mult1, 0.0, 2.0);
	float wave = 0.5 + 0.5 * sin(3.14159265 * (2.0 * val - 0.5));
	return clamp(mult * wave, 0.0, 1.0);

	//return fract(val);
}

void main() {
	// 1) Calculate normalized texture coordinates to sample the voxel texture
	vec3 texCoords = (wPos - TextureMin) / TextureSize;
	texCoords = SwizzleVec3(texCoords, SwizzleIndex, InvertIndex); // Enables debugging of axis orientation

	if (any(lessThan(texCoords, vec3(0.0))) || any(greaterThan(texCoords, vec3(1.0)))) {
		// Outside the texture bounds
		FragColor = vec4(0.0, 0.0, 0.0, 1.0);

	} else {
		// 2) Load the simulated vertex position from the texture
		vec3 sPos = texture(voxelTexture, texCoords).xyz;
		sPos = SwizzleVec3(sPos, SwizzleData, InvertData);

		// 3) Calculate displacement color
		//float dispMag = length(sPos - wPos);
		//vec4 color = vec4(
		//	clamp(dispMag * 100.0, 0.0, 1.0),
		//	clamp(dispMag * 1.0, 0.0, 1.0),
		//	clamp(dispMag * 0.01, 0.0, 1.0),
		//	1.0);

		//	public enum VisualizeMode { Diff = 0, World = 1, Coords = 2, Value = 3 }
		switch (Visualize) {
			case 0: sPos = sPos - wPos; break;	// Diff
			case 1: sPos = wPos; break;			// World
			case 2: sPos = texCoords; break;	// Coords
			case 3: sPos = sPos; break;			// Value
			default: sPos = vec3(0.3); break;	// Error case
		}

		//FragColor = vec4(fract(sPos.xyz), 1.0);
		float X = Vis(sPos.x);
		float Y = Vis(sPos.y);
		float Z = Vis(sPos.z);

		// X - OK
		// Z - Index.XZY, -Z

		//	public enum AxisSelect { None = 0, X = 1, Y = 2, Z = 3, XY = 4, XZ = 5, YZ = 6, XYZ = 7 }
		switch (DisplayAxis) {
			case 0: FragColor = vec4(0.4, 0.4, 0.4, 1.0); break;
			case 1: FragColor = vec4(X, 0.0, 0.0, 1.0); break;
			case 2: FragColor = vec4(0.0, Y, 0.0, 1.0); break;
			case 3: FragColor = vec4(0.0, 0.0, Z, 1.0); break;
			case 4: FragColor = vec4(X, Y, 0.0, 1.0); break;
			case 5: FragColor = vec4(X, 0.0, Z, 1.0); break;
			case 6: FragColor = vec4(0.0, Y, Z, 1.0); break;
			case 7: FragColor = vec4(X, Y, Z, 1.0); break;
			default: FragColor = vec4(0.0, 0.5, 0.0, 1.0); break; // Error case
		}
	}
}