using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using SixLabors.ImageSharp;
using System;
using System.Threading.Tasks;
using PixelForm = SixLabors.ImageSharp.PixelFormats;

namespace BladeVibrationCS.GpuPrograms;
public abstract class AVoxelizer : AShaderProgram {
	public int MaterialTextureID { get; protected set; }
	public readonly Vector3 BasePosition, Size;
	public readonly float VoxelSize = 0.01f;
	protected readonly ModelHolder Model;
	public readonly int VoxX, VoxY, VoxZ;
	protected readonly Matrix4 view;

	public AVoxelizer ( ModelHolder model, float voxelSize, bool startRepeatable, byte border )
		: base ( startRepeatable
			, ("VoxelizerVertex.glsl", ShaderType.VertexShader)
			, ("VoxelizerFragment.glsl", ShaderType.FragmentShader) ) {
		Model = model;
		VoxelSize = voxelSize;
		BasePosition = new Vector3 ( model.MinX, model.MinY, model.MinZ );
		Size = new Vector3 ( model.MaxX - model.MinX, model.MaxY - model.MinY, model.MaxZ - model.MinZ );

		VoxX = (int)Math.Ceiling ( Size.X / VoxelSize ) + border;
		VoxY = (int)Math.Ceiling ( Size.Y / VoxelSize ) + border;
		VoxZ = (int)Math.Ceiling ( Size.Z / VoxelSize ) + border;

		PrepareBuffer ();

		// Setup the view matrix to look down the Z-axis
		view = Matrix4.LookAt ( new Vector3 ( 0f, 0f, 1f ), new Vector3 ( 0f, 0f, 0f ), Vector3.UnitY );
	}

	private void PrepareBuffer () {
		// Create and bind the buffer for 3D texture with material description
		MaterialTextureID = GL.GenTexture ();
		GL.BindTexture ( TextureTarget.Texture3D, MaterialTextureID );
		GL.TexImage3D ( TextureTarget.Texture3D, 0, PixelInternalFormat.R32f,
			VoxX, VoxY, VoxZ, 0,
			PixelFormat.Red, PixelType.Float, nint.Zero );
		GL.TexParameter ( TextureTarget.Texture3D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear );
		GL.TexParameter ( TextureTarget.Texture3D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear );
		GL.TexParameter ( TextureTarget.Texture3D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge );
		GL.BindTexture ( TextureTarget.Texture3D, 0 ); // Unbind
	}



	protected override void DisposeInner () {
		GL.DeleteTexture ( MaterialTextureID );
	}



	// Set up orthographic projection and view matrix for slicing
	//   Use the near plane to slice for current Z index
	protected Matrix4 ProjectionMatrix (int z) => Matrix4.CreateOrthographicOffCenter (
		BasePosition.X, BasePosition.X + Size.X,
		BasePosition.Y, BasePosition.Y + Size.Y,
		Model.MinZ + z * VoxelSize, Model.MinZ );

	protected void Pass1_StencilInput ( int z, Action prepFrameBuffer ) {
		ArgumentNullException.ThrowIfNull ( prepFrameBuffer );
		prepFrameBuffer ();

		GL.Disable ( EnableCap.DepthTest );
		GL.DepthMask ( false );
		GL.Enable ( EnableCap.StencilTest );

		GL.ClearColor ( 0f, 0f, 0f, 0f );
		GL.Clear ( ClearBufferMask.ColorBufferBit | ClearBufferMask.StencilBufferBit );
		GL.ColorMask ( false, false, false, false );

		// 1st pass: setup stencil buffer
		GL.StencilFunc ( StencilFunction.Always, 1, 0xff ); // Ignore the stencil buffer
		GL.StencilOpSeparate ( StencilFace.Back, StencilOp.Keep, StencilOp.Keep, StencilOp.Incr ); // Entering object, increment (allow overflow)
		GL.StencilOpSeparate ( StencilFace.Front, StencilOp.Keep, StencilOp.Keep, StencilOp.Decr ); // Exiting object, decrement (allow underflow)

		SetProgramUniforms ( z, VoxelizerShaderMode.MODE_CENTER );
		GL.BindVertexArray ( Model.VAO );
		GL.DrawArrays ( PrimitiveType.Triangles, 0, Model.VertexCount );
	}

	protected void Pass2_StencilTest ( int z ) {
		// This probably doesn't actually need to draw the actual model as the stencil buffer already has the info.
		//   Drawing single rectangle covering the slice just to call a fragment shader for each pixel in the slice should be enough.
		GL.StencilFunc ( StencilFunction.Equal, 1, 0xff ); // Pass where stencil value is 1, i.e., inside the object
		GL.StencilOp ( StencilOp.Keep, StencilOp.Keep, StencilOp.Keep );
		GL.ColorMask ( true, true, true, true ); // Enable color writes
															  // Set up orthographic projection and view matrix for slicing
															  //   Use the near plane to slice for current Z index

		SetProgramUniforms ( z, VoxelizerShaderMode.MODE_CENTER );
		GL.BindVertexArray ( Model.VAO );
		GL.DrawArrays ( PrimitiveType.Triangles, 0, Model.VertexCount );
	}

	protected enum VoxelizerShaderMode { MODE_BORDER = 1, MODE_CENTER = 2 }
	protected void SetProgramUniforms ( int z, VoxelizerShaderMode mode ) {
		SetUniform ( "model", Matrix4.Identity );
		SetUniform ( "view", view );
		SetUniform ( "proj", ProjectionMatrix ( z ) );

		SetUniform ( "ActMode", (int)mode );
		SetUniform ( "zPlaneStart", Model.MinZ + z * VoxelSize );
		SetUniform ( "zPlaneEnd", Model.MinZ + (z + 1) * VoxelSize );
		SetUniform ( "modelMin", Model.MinX, Model.MinY, Model.MinZ );
		SetUniform ( "modelMax", Model.MaxX, Model.MaxY, Model.MaxZ );
		SetUniform ( "youngsModulus", ModelHolder.YM_Steel );
	}
}