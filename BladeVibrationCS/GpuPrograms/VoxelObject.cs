using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using SixLabors.ImageSharp;
using System;
using System.Threading.Tasks;
using PixelForm = SixLabors.ImageSharp.PixelFormats;

namespace BladeVibrationCS.GpuPrograms;
public class VoxelObject : AVoxelizer {
	public override (int R, int G, int B) BackgroundColor => (BACKGROUND_HIGH, BACKGROUND_LOW, BACKGROUND_MIN); // Gold

	const byte BorderSize = 2;
	public VoxelObject ( ModelHolder model, float voxelSize ) : base ( model, voxelSize, true, BorderSize ) {
	}

	public void Voxelize () {
		//int fbo = GL.GenFramebuffer ();
		//GL.BindFramebuffer ( FramebufferTarget.Framebuffer, fbo );

		SetupNormalRendering ( 256, 256 );
		GL.CullFace ( TriangleFace.FrontAndBack );
		for (int z = 0; z < VoxZ; z++ ) {
			// Draw 'border' i.e. some debugging border for each slice with the special fragment shader mode.
			SetProgramUniforms ( z, VoxelizerShaderMode.MODE_BORDER );
			RenderController.Draw2D ();
		}
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
		Program.StdOut ( "Voxelization complete." );
	}

	protected override void SwitchToInner ( AShaderProgram last, Vector2i screenSize ) {
		// Currently all settings is probably already done in Voxelize()
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