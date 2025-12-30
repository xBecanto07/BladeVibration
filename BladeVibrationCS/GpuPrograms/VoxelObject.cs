using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using SixLabors.ImageSharp;
using System;
using System.Threading.Tasks;
using PixelForm = SixLabors.ImageSharp.PixelFormats;

namespace BladeVibrationCS.GpuPrograms;
public class VoxelObject : AVoxelizer {
	MaterialHolder MaterialHolder;
	public const int MAX_VOXELS_PER_AXIS = 1024;
	public override (int R, int G, int B) BackgroundColor => (BACKGROUND_HIGH, BACKGROUND_LOW, BACKGROUND_MIN); // Gold
	public int CurrentLayer = 0;

	const byte BorderSize = 2;
	public VoxelObject ( ModelHolder model, float voxelSize ) : base ( model, voxelSize, true, BorderSize ) {
		MaterialHolder = new ( this );
		MaterialHolder.SetMaterial ( 0, 32f, 0.5f, 0.5f, RENDER_MODE_OBJECTS ); // Scary-zero error material
		MaterialHolder.SetMaterial ( 6, 16f, 0.3f, 0.8f, RENDER_MODE_OBJECTS ); // Skybox material
		MaterialHolder.SetMaterial ( 7, 8f, 0.1f, 0.2f, RENDER_MODE_OBJECTS ); // Star material
		MaterialHolder.UploadToGPU ( Handle );

		//float maxDim = Math.Max ( Math.Max ( Model.MaxX - Model.MinX, Model.MaxY - Model.MinY ), Model.MaxZ - Model.MinZ );
		CamPos = new Vector3 ( 0, Model.MaxY, 0 );
		CamDir = new Vector3 ( 0, -1, 0 );
	}

	public void Voxelize () {
		//float maxDim = Math.Max ( Math.Max ( Model.MaxX - Model.MinX, Model.MaxY - Model.MinY ), Model.MaxZ - Model.MinZ );
		CamPos = new Vector3 ( 0, Model.MinY + (1 + CurrentLayer) * VoxelSize, 0 );
		CurrentLayer = (CurrentLayer + 1) % VoxY;

		Vector3 CamUp = new Vector3 ( 0, 0, -1 );
		Matrix4 camView = Matrix4.LookAt ( CamPos, CamPos + CamDir, CamUp );
		SetUniform ( "view", camView );

		//SetUniform ( "proj", ProjectionMatrix ( 150 ) );
		Matrix4 ortoProj = Matrix4.CreateOrthographicOffCenter (
			Model.MinX, Model.MaxX,
			Model.MinZ, Model.MaxZ,
			0.99f * VoxelSize, 2 * (Model.MaxY - Model.MinY)
			);
		SetUniform ( "proj", ortoProj );
		SetUniform ( "model", Matrix4.Identity );

		SetUniform ( "lightPosMain", 200f, 5000f, 200f );
		SetUniform ( "lightPosSecond", -2500f, 900f, 150f );
		Vector3 camPos = camView.Inverted().ExtractTranslation();
		SetUniform ( "camPos", camPos.X, camPos.Y, camPos.Z );
		SetUniform ( "specularColor", 0.8f, 1f, 0.8f );

		SetUniform ( "colorA", 1f, 1f, 0f );
		SetUniform ( "colorB", 0f, 1f, 1f );
		SetUniform ( "scale", 1.0f );
		SetUniform ( "offset", 0, 0, 0 );
		SetUniform ( "matOffset", 6 );
		RenderController.SkyBoxPrimitive.Render ();

		SetUniform ( "colorA", 1f, 0f, 0f );
		SetUniform ( "colorB", 0f, 0f, 1f );
		SetUniform ( "scale", 1.0f );
		//SetUniform ( "shadingMethod", RENDER_MODE_PHONG );
		SetUniform ( "camPos", camPos.X, camPos.Y, camPos.Z );

		GL.BindVertexArray ( Model.VAO );
		SetUniform ( "matOffset", 0 );
		GL.DrawElements ( PrimitiveType.Triangles, Model.IndexCount, DrawElementsType.UnsignedInt, 0 );


		//SetupNormalRendering ( 512, 512 );
		//SetProgramUniforms ( 150, VoxelizerShaderMode.MODE_BORDER );
		//RenderController.Draw2D ();

		//int fbo = GL.GenFramebuffer ();
		//GL.BindFramebuffer ( FramebufferTarget.Framebuffer, fbo );

		//SetupNormalRendering ( 512, 512 );
		//GL.CullFace ( TriangleFace.FrontAndBack );
		//for (int z = 0; z < VoxZ; z++ ) {
		//	// Draw 'border' i.e. some debugging border for each slice with the special fragment shader mode.
		//	SetProgramUniforms ( z, VoxelizerShaderMode.MODE_BORDER );
		//	RenderController.Draw2D ();
		//}
		//for ( int z = BorderSize; z < VoxZ - BorderSize; z++ ) {
		//	Pass1_StencilInput ( z, () => {
		//		// Attach the 3D texture layer as the color attachment for the framebuffer
		//		GL.FramebufferTextureLayer ( FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, MaterialTextureID, 0, z );
		//		var status = GL.CheckFramebufferStatus ( FramebufferTarget.Framebuffer );
		//		if ( status != FramebufferErrorCode.FramebufferComplete )
		//			throw new Exception ( $"Framebuffer is not complete: {status}" );
		//	} );
		//	Pass2_StencilTest ( z );
		//}

		//GL.Disable ( EnableCap.DepthTest );
		//GL.DepthMask ( false );
		//GL.Enable ( EnableCap.StencilTest );

		//for ( int z = 0; z < VoxZ; z++ ) {
		//	// Attach the 3D texture layer as the color attachment for the framebuffer
		//	GL.FramebufferTextureLayer ( FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, MaterialTextureID, 0, z );
		//	var status = GL.CheckFramebufferStatus ( FramebufferTarget.Framebuffer );
		//	if ( status != FramebufferErrorCode.FramebufferComplete )
		//		throw new Exception ( $"Framebuffer is not complete: {status}" );

		//	// Clear out the buffers
		//	GL.ClearColor ( 0f, 0f, 0f, 0f );
		//	GL.Clear ( ClearBufferMask.ColorBufferBit | ClearBufferMask.StencilBufferBit );
		//	GL.ColorMask ( false, false, false, false );

		//	// 1st pass: setup stencil buffer
		//	GL.StencilFunc ( StencilFunction.Always, 1, 0xff ); // Ignore the stencil buffer
		//	GL.StencilOpSeparate ( StencilFace.Back, StencilOp.Keep, StencilOp.Keep, StencilOp.Incr ); // Entering object, increment (allow overflow)
		//	GL.StencilOpSeparate ( StencilFace.Front, StencilOp.Keep, StencilOp.Keep, StencilOp.Decr ); // Exiting object, decrement (allow underflow)

		//	DrawStencilSlice ( z );

		//	GL.StencilFunc ( StencilFunction.Equal, 1, 0xff ); // Pass where stencil value is 1, i.e., inside the object
		//	GL.StencilOp ( StencilOp.Keep, StencilOp.Keep, StencilOp.Keep );
		//	GL.ColorMask ( true, true, true, true ); // Enable color writes


		//	// UNFINISHED
		//}

		//GL.DeleteFramebuffer ( fbo );
		//Program.StdOut ( "Voxelization complete." );
	}

	protected override void SwitchToInner ( AShaderProgram last, Vector2i screenSize ) {
		float ratio = (Model.MaxZ - Model.MinZ) / (Model.MaxX - Model.MinX);
		SetupNormalRendering ( screenSize.X, (int)(screenSize.X * ratio) );

		// Currently all settings is probably already done in Voxelize()
		EntryProgram.StdOut ( $"Switched to Voxelizer. Using projection metrix of ({BasePosition.X} - {BasePosition.X + Size.X} | {BasePosition.Y} - {BasePosition.Y + Size.Y}). Z∈({Model.MinZ}, {Model.MinZ + VoxelSize}, {Model.MinZ + 2 * VoxelSize} .. {Model.MinZ + 10 * VoxelSize} .. .. {Model.MinZ + (VoxZ - 1) * VoxelSize})" );
	}

	public void Save ( string baseFilePath ) {
		// Save the 3D texture under 'materialTextureID' to a png file, layer by layer
		GL.BindTexture ( TextureTarget.Texture3D, MaterialTextureID );
		using ( var gif = new Image<PixelForm.Rgba32> ( VoxX, VoxY ) ) {
			for ( int z = 0; z < VoxZ; z++ ) {
				float[] layerData = new float[VoxX * VoxY];
				GL.GetTexImage ( TextureTarget.Texture3D, 0, PixelFormat.Red, PixelType.Float, layerData );
				using ( var image = new Image<PixelForm.Rgba32> ( VoxX, VoxY ) ) {
					Parallel.For ( 0, VoxY, y => {
						for ( int x = 0; x < VoxX; x++ ) {
							float value = layerData[y * VoxX + x];
							image[x, y] = new PixelForm.Rgba32 ( 1 - value, 0, value, value == 0 ? 0 : 1 );
						}
					});
					gif.Frames.AddFrame ( image.Frames[0] );
				}
			}
			gif.Frames.RemoveFrame ( 0 ); // Remove the initial empty frame
			gif.SaveAsGif ( baseFilePath + ".gif" );
		}
	}



	protected override void DisposeInner () {
		GL.DeleteTexture ( MaterialTextureID );
	}
}