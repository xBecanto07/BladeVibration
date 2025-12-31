using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using SixLabors.ImageSharp;
using System;
using System.Threading.Tasks;
using PixelForm = SixLabors.ImageSharp.PixelFormats;

namespace BladeVibrationCS.GpuPrograms;
public class VoxelObject : AVoxelizer {
	MaterialHolder MaterialHolder;
	public const int MAX_VOXELS_PER_AXIS = 4096;
	public override (int R, int G, int B) BackgroundColor => (BACKGROUND_HIGH, BACKGROUND_LOW, BACKGROUND_MIN); // Gold
	public float CurrentLayer = 0;

	const byte BorderSize = 2;
	public VoxelObject ( ModelHolder model, float voxelSize )
		: base ( model, voxelSize, true, BorderSize
		, new ( 2.5f, -1.3f, -4f )
		, new ( model.MaxX, 1.3f, 4f )
		) {

		MaterialHolder = new ( this );
		MaterialHolder.SetMaterial ( 0, 32f, 0.5f, ModelHolder.YM_Air, RENDER_MODE_OBJECTS ); // Scary-zero error material
		MaterialHolder.SetMaterial ( 1, 64f, 0.2f, ModelHolder.YM_Wood, RENDER_MODE_PHONG ); // Matte material
		MaterialHolder.SetMaterial ( 2, 4f, 0.7f, ModelHolder.YM_Steel, RENDER_MODE_PHONG ); // Shiny material
		MaterialHolder.UploadToGPU ( Handle );

		//float maxDim = Math.Max ( Math.Max ( MaxCutoff.X - MinCutoff.X, MaxCutoff.Y - MinCutoff.Y ), MaxCutoff.Z - MinCutoff.Z );
		CamPos = new Vector3 ( 0, MaxCutoff.Y, 0 );
		CamDir = new Vector3 ( 0, -1, 0 );
	}

	public void Voxelize () {
		MaterialHolder.Use ( 0 );
		//float maxDim = Math.Max ( Math.Max ( MaxCutoff.X - MinCutoff.X, MaxCutoff.Y - MinCutoff.Y ), MaxCutoff.Z - MinCutoff.Z );
		CamPos = new Vector3 ( 0, MinCutoff.Y + (1 + CurrentLayer) * VoxelSize, 0 );
		CurrentLayer += 1.0f;
		if ( CurrentLayer > VoxY ) CurrentLayer = 0;

		Vector3 CamUp = new Vector3 ( 0, 0, -1 );
		Matrix4 camView = Matrix4.LookAt ( CamPos, CamPos + CamDir, CamUp );
		SetUniform ( "view", camView );
		SetUniform ( "DefaultStiffness", 1 );

		//SetUniform ( "proj", ProjectionMatrix ( 150 ) );
		Matrix4 ortoProj = Matrix4.CreateOrthographicOffCenter (
			MinCutoff.X, MaxCutoff.X,
			MinCutoff.Z, MaxCutoff.Z,
			//0.99f * VoxelSize, 2 * (MaxCutoff.Y - MinCutoff.Y)
			0.99f * VoxelSize, 150f
			);
		//float aspectRatio = (MaxCutoff.X - MinCutoff.X) / (MaxCutoff.Z - MinCutoff.Z);
		SetUniform ( "screenSize", LastScreenSize.X, LastScreenSize.Y );
		SetUniform ( "proj", ortoProj );
		SetUniform ( "model", Matrix4.Identity );
		float maxX = MaxCutoff.X - MinCutoff.X;
		float maxZ = MaxCutoff.Z - MinCutoff.Z;
		SetUniform ( "boundRadius", Math.Max ( maxX, maxZ ) * 0.5f );
		GL.BindVertexArray ( Model.VAO );
		SetUniform ( "matOffset", 0 );
		SetUniform ( "Y", CurrentLayer / VoxY );
		SetUniform ( "Yi", (int)CurrentLayer );
		SetUniform ( "minBound", MinCutoff.X, MinCutoff.Y, MinCutoff.Z );
		SetUniform ( "boundSize", MaxCutoff.X - MinCutoff.X, MaxCutoff.Y - MinCutoff.Y, MaxCutoff.Z - MinCutoff.Z );
		SetUniform ( "voxelCounts", VoxX, VoxY, VoxZ );
		//SetUniform ( "voxelSize", VoxelSize );

		GL.BindImageTexture ( 0, MaterialTextureID, 0, true, 0, TextureAccess.WriteOnly, SizedInternalFormat.Rgba32f );

		Pass0_BackgroundClear ();
		Pass1_StencilInput ();
		Pass2_StencilTest ();

		GL.MemoryBarrier ( MemoryBarrierFlags.ShaderImageAccessBarrierBit );

		//Vector4[] TextureData = new Vector4[VoxX * VoxY * VoxZ];
		//GL.BindTexture ( TextureTarget.Texture3D, MaterialTextureID );
		//GL.GetTexImage ( TextureTarget.Texture3D, 0, PixelFormat.Rgba, PixelType.Float, TextureData );
		//int totVoxels = VoxX * VoxY * VoxZ;
		//int filledVoxels = 0, firstNonEmpty = -1, lastNonEmpty = -1;
		//for ( int i = 0; i < totVoxels; i++ ) {
		//	bool isEmpty = TextureData[i].X == 0 && TextureData[i].Y == 0 && TextureData[i].Z == 0 && TextureData[i].W == 0;
		//	if ( !isEmpty ) {
		//		filledVoxels++;
		//		if ( firstNonEmpty == -1 ) firstNonEmpty = i;
		//		lastNonEmpty = i;
		//	}
		//}
		//EntryProgram.StdOut ( $"Voxelization complete. Filled voxels: {filledVoxels} / {totVoxels} ({(100f * filledVoxels) / totVoxels:0.00}%). First non-empty voxel index: {firstNonEmpty}, last non-empty voxel index: {lastNonEmpty}. This[0,0,0] = {TextureData[0]}" );

		GL.BindTexture ( TextureTarget.Texture3D, 0 );
	}

	protected override void SwitchToInner ( AShaderProgram last, Vector2i screenSize ) {
		LastScreenSize = screenSize;

		// Currently all settings is probably already done in Voxelize()
		EntryProgram.StdOut ( $"Switched to Voxelizer. Using projection metrix of ({BasePosition.X} - {BasePosition.X + Size.X} | {BasePosition.Y} - {BasePosition.Y + Size.Y}). Z∈({MinCutoff.Z}, {MinCutoff.Z + VoxelSize}, {MinCutoff.Z + 2 * VoxelSize} .. {MinCutoff.Z + 10 * VoxelSize} .. .. {MinCutoff.Z + (VoxZ - 1) * VoxelSize})\n" +
			$"Voxel texture size: {VoxX} x {VoxY} x {VoxZ} = {(VoxX * VoxY * VoxZ) / 1000000}M voxels." );
	}

	public void Save ( string baseFilePath ) {
		// Save the 3D texture under 'materialTextureID' to a png file, layer by layer
		GL.BindTexture ( TextureTarget.Texture3D, MaterialTextureID );
		using ( var gif = new Image<PixelForm.Rgba32> ( VoxX, VoxZ ) ) {
			Vector4[] layerData = new Vector4[VoxX * VoxY * VoxZ];
			GL.GetTexImage ( TextureTarget.Texture3D, 0, PixelFormat.Rgba, PixelType.Float, layerData );

			for ( int y = 0; y < VoxY; y++ ) {
				using ( var image = new Image<PixelForm.Rgba32> ( VoxX, VoxZ ) ) {
					Parallel.For ( 0, VoxZ, z => {
						for ( int x = 0; x < VoxX; x++ ) {
							Vector4 value = layerData[z * VoxX * VoxY + y * VoxX + x];
							image[x, z] = new PixelForm.Rgba32 ( value.X, value.Y, value.Z, value.W );
						}
					});
					gif.Frames.AddFrame ( image.Frames[0] );
				}
			}
			gif.Frames.RemoveFrame ( 0 ); // Remove the initial empty frame
			gif.SaveAsGif ( baseFilePath + ".gif" );
		}
	}
}