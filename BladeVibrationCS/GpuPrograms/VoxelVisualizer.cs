using System;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace BladeVibrationCS.GpuPrograms;
public class VoxelVisualizer : AShaderProgram {
	public override (int R, int G, int B) BackgroundColor => (BACKGROUND_MIN, BACKGROUND_MID, BACKGROUND_LOW); // Teal
	public readonly VoxelObject VoxelTexture;
	public bool ShouldTransform = false;
	public float Scale = 1.0f;

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

		Matrix4 model = Matrix4.CreateScale ( 5f, 0.2f, 0.2f ) * Matrix4.CreateTranslation ( position ) * Matrix4.CreateFromQuaternion ( rotation );
		SetUniform ( "model", model );
		SetUniform ( "view", view );
		SetUniform ( "proj", projection );
		SetUniform ( "scale", Scale );
		//SetUniform ( "renderMode", ShouldTransform ? 1 : 2 );
		SetUniform ( "renderMode", 2 );

		//SetUniform ( "voxelTexture", VoxelTexture.MaterialTextureID );
		//SetUniform ( "TextureDims", VoxelTexture.VoxX, VoxelTexture.VoxY, VoxelTexture.VoxZ );
		SetUniform ( "TextureMin", VoxelTexture.BasePosition.X, VoxelTexture.BasePosition.Y, VoxelTexture.BasePosition.Z );
		SetUniform ( "TextureSize", VoxelTexture.Size.X, VoxelTexture.Size.Y, VoxelTexture.Size.Z );

		GL.ActiveTexture ( TextureUnit.Texture0 );
		GL.BindTexture ( TextureTarget.Texture3D, VoxelTexture.MaterialTextureID );
		GL.ActiveTexture ( TextureUnit.Texture1 );
		GL.BindTexture ( TextureTarget.Texture3D, VoxelTexture.ExtrasTextureID );

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

		SetUniform ( "renderMode", 3 );
		float[] X = [VoxelTexture.BasePosition.X, VoxelTexture.BasePosition.X + VoxelTexture.Size.X];
		float[] Y = [VoxelTexture.BasePosition.Y, VoxelTexture.BasePosition.Y + VoxelTexture.Size.Y];
		float[] Z = [VoxelTexture.BasePosition.Z, VoxelTexture.BasePosition.Z + VoxelTexture.Size.Z];
		for ( int x = 0; x < 2; x++ )
			for ( int y = 0; y < 2; y++ )
				for ( int z = 0; z < 2; z++ )
					PrintPoint ( X[x], Y[y], Z[z] );
	}

	private void PrintPoint (float x, float y, float z) {
		Matrix4 model = Matrix4.CreateScale ( 1f ) * Matrix4.CreateTranslation ( new Vector3 ( x, y, z ) );
		SetUniform ( "model", model );
		RenderController.StarPrimitive.Render ();
	}

	protected override void SwitchToInner ( AShaderProgram last, Vector2i screenSize ) {
		SetupNormalRendering ( screenSize.X, screenSize.Y );
	}

	protected override void DisposeInner () {
	}
}