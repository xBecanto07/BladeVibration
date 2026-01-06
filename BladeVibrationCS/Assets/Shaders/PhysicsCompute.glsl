#version 450 core

layout (local_size_x = 8, local_size_y = 8, local_size_z = 8) in;
layout(rgba32f, binding = 0) uniform image3D voxels_In;
layout(rgba32f, binding = 2) uniform image3D voxels_Out;

void main () {
	vec4 vox = imageLoad(voxels_In, ivec3(gl_GlobalInvocationID));
	vox.x += 0.001; // Dummy operation to simulate computation, slowly move along x-axis
	imageStore(voxels_Out, ivec3(gl_GlobalInvocationID), vox);
}