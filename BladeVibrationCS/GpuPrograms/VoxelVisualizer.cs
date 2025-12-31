using System;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace BladeVibrationCS.GpuPrograms;
internal class VoxelVisualizer : AShaderProgram {
	public override (int R, int G, int B) BackgroundColor => (BACKGROUND_MIN, BACKGROUND_MID, BACKGROUND_LOW); // Teal
	public readonly VoxelObject VoxelTexture;

	public VoxelVisualizer ( VoxelObject voxelTexture )
		: base ( true, ("VoxelViewVertex.glsl", ShaderType.VertexShader)
			, ("VoxelViewFragment.glsl", ShaderType.FragmentShader) ) {
		ArgumentNullException.ThrowIfNull ( voxelTexture );
		VoxelTexture = voxelTexture;
	}

	public void Render (Vector3 position, Quaternion rotation, Matrix4 view, Matrix4 projection) {
		Use ();
		GL.ClearColor ( BackgroundColor.R / 255f, BackgroundColor.G / 255f, BackgroundColor.B / 255f, 1.0f );
		GL.Clear ( ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit );

		Matrix4 model = Matrix4.CreateTranslation ( position ) * Matrix4.CreateFromQuaternion ( rotation );
		SetUniform ( "model", model );
		SetUniform ( "view", view );
		SetUniform ( "proj", projection );
		//SetUniform ( "voxelTexture", VoxelTexture.MaterialTextureID );
		//SetUniform ( "TextureDims", VoxelTexture.VoxX, VoxelTexture.VoxY, VoxelTexture.VoxZ );
		SetUniform ( "TextureMin", VoxelTexture.BasePosition.X, VoxelTexture.BasePosition.Y, VoxelTexture.BasePosition.Z );
		SetUniform ( "TextureSize", VoxelTexture.Size.X, VoxelTexture.Size.Y, VoxelTexture.Size.Z );

		GL.ActiveTexture ( TextureUnit.Texture0 );
		//SetUniform ( "voxelTexture", 0 );
		GL.BindTexture ( TextureTarget.Texture3D, VoxelTexture.MaterialTextureID );
		//GL.BindImageTexture ( 1, VoxelTexture.MaterialTextureID, 0, true, 0, TextureAccess.ReadOnly, SizedInternalFormat.Rgba32f );

		//Vector4[] TextureData = new Vector4[VoxelTexture.VoxX * VoxelTexture.VoxY * VoxelTexture.VoxZ];
		//GL.BindTexture ( TextureTarget.Texture3D, VoxelTexture.MaterialTextureID );
		//GL.GetTexImage ( TextureTarget.Texture3D, 0, PixelFormat.Rgba, PixelType.Float, TextureData );
		//int totVoxels = VoxelTexture.VoxX * VoxelTexture.VoxY * VoxelTexture.VoxZ;
		//int filledVoxels = 0, firstNonEmpty = -1, lastNonEmpty = -1;
		//for ( int i = 0; i < totVoxels; i++ ) {
		//	bool isEmpty = TextureData[i].X == 0 && TextureData[i].Y == 0 && TextureData[i].Z == 0 && TextureData[i].W == 0;
		//	if ( !isEmpty ) {
		//		filledVoxels++;
		//		if ( firstNonEmpty == -1 ) firstNonEmpty = i;
		//		lastNonEmpty = i;
		//	}
		//}
		//EntryProgram.StdOut ( $"Visualization starting. Filled voxels: {filledVoxels} / {totVoxels} ({(100f * filledVoxels) / totVoxels:0.00}%). First non-empty voxel index: {firstNonEmpty}, last non-empty voxel index: {lastNonEmpty}. This[0,0,0] = {TextureData[0]}" );

		RenderController.PlanePrimitive.Render ();

		GL.BindTexture ( TextureTarget.Texture3D, 0 );
	}

	protected override void SwitchToInner ( AShaderProgram last, Vector2i screenSize ) {
		SetupNormalRendering ( screenSize.X, screenSize.Y );
	}

	protected override void DisposeInner () {
	}
}