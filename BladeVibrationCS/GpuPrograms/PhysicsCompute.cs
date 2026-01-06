using System;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace BladeVibrationCS.GpuPrograms;
public class PhysicsCompute : AShaderProgram {
	public override (int R, int G, int B) BackgroundColor => (BACKGROUND_LOW, BACKGROUND_LOW, BACKGROUND_HIGH); // Teal
	public readonly VoxelObject VoxelTexture;

	private int[] MaterialIDs, ExtrasIDs;
	private int ActID = 0;

	public PhysicsCompute ( VoxelObject voxelTexture )
		: base ( true, ("PhysicsCompute.glsl", ShaderType.ComputeShader) ) {
		ArgumentNullException.ThrowIfNull ( voxelTexture );
		VoxelTexture = voxelTexture;
		MaterialIDs = [VoxelTexture.MaterialTextureID, VoxelTexture.PrepareBuffer ()];
		ExtrasIDs = [VoxelTexture.ExtrasTextureID, VoxelTexture.PrepareBuffer ()];
	}

	public void Calc () {
		Use ();

		int nextID = (ActID + 1) % 2;
		GL.BindImageTexture ( 0, MaterialIDs[ActID], 0, true, 0, TextureAccess.ReadOnly, SizedInternalFormat.Rgba32f );
		GL.BindImageTexture ( 1, ExtrasIDs[ActID], 0, true, 0, TextureAccess.ReadOnly, SizedInternalFormat.Rgba32f );
		GL.BindImageTexture ( 3, MaterialIDs[nextID], 0, true, 0, TextureAccess.WriteOnly, SizedInternalFormat.Rgba32f );
		GL.BindImageTexture ( 4, ExtrasIDs[nextID], 0, true, 0, TextureAccess.WriteOnly, SizedInternalFormat.Rgba32f );
		ActID = nextID;

		GL.DispatchCompute ( VoxelTexture.VoxX / 8 + 1, VoxelTexture.VoxY / 8 + 1, VoxelTexture.VoxZ / 8 + 1 );
		GL.MemoryBarrier ( MemoryBarrierFlags.ShaderImageAccessBarrierBit );
	}

	public void ActivateTextures () {
		GL.ActiveTexture ( TextureUnit.Texture0 );
		GL.BindTexture ( TextureTarget.Texture3D, MaterialIDs[ActID] );
		GL.ActiveTexture ( TextureUnit.Texture1 );
		GL.BindTexture ( TextureTarget.Texture3D, ExtrasIDs[ActID] );
	}

	protected override void SwitchToInner ( AShaderProgram last, Vector2i screenSize ) {
		SetupNormalRendering ( screenSize.X, screenSize.Y );
	}

	protected override void DisposeInner () {
	}
}