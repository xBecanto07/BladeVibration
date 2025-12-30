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
	protected Matrix4 view;
	public Vector3 CamPos = Vector3.Zero;
	public Vector3 CamDir = Vector3.UnitZ;

	public AVoxelizer ( ModelHolder model, float voxelSize, bool startRepeatable, byte border )
		: base ( startRepeatable, ("Vertex.glsl", ShaderType.VertexShader)
			, ("Fragment.glsl", ShaderType.FragmentShader) ) {
		//, ("VoxelizerVertex.glsl", ShaderType.VertexShader)
		//, ("VoxelizerFragment.glsl", ShaderType.FragmentShader) ) {
		Model = model;
		VoxelSize = voxelSize;
		BasePosition = new Vector3 ( model.MinX, model.MinY, model.MinZ );
		Size = new Vector3 ( model.MaxX - model.MinX, model.MaxY - model.MinY, model.MaxZ - model.MinZ );

		VoxX = (int)Math.Ceiling ( Size.X / VoxelSize ) + border;
		VoxY = (int)Math.Ceiling ( Size.Y / VoxelSize ) + border;
		VoxZ = (int)Math.Ceiling ( Size.Z / VoxelSize ) + border;

		//PrepareBuffer ();

		view = Matrix4.LookAt ( CamPos, CamPos + CamDir, Vector3.UnitY );
		LastProjectionOrto.Row0 = new Vector2 ( BasePosition.X, BasePosition.X + Size.X );
		LastProjectionOrto.Row1 = new Vector2 ( BasePosition.Y, BasePosition.Y + Size.Y );
		LastProjectionOrto.Row2 = new Vector2 ( Model.MinZ, Model.MaxZ );
	}

	private void PrepareBuffer () {
		// Create and bind the buffer for 3D texture with material description
		MaterialTextureID = GL.GenTexture ();
		GL.BindTexture ( TextureTarget.Texture3D, MaterialTextureID );
		GL.TexImage3D ( TextureTarget.Texture3D, 0, PixelInternalFormat.Rgba32f,
			VoxX, VoxY, VoxZ, 0,
			PixelFormat.Rgba, PixelType.Float, nint.Zero );
		GL.TexParameter ( TextureTarget.Texture3D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear );
		GL.TexParameter ( TextureTarget.Texture3D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear );
		GL.TexParameter ( TextureTarget.Texture3D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge );
		GL.BindTexture ( TextureTarget.Texture3D, 0 ); // Unbind
	}



	protected override void DisposeInner () {
		GL.DeleteTexture ( MaterialTextureID );
	}




	public enum ProjectionType { Orthographic, Perspective }
	public ProjectionType CurrentProjMode = ProjectionType.Orthographic;
	public Matrix3x2 LastProjectionOrto, LastProjectionPersp;
	// Set up orthographic projection and view matrix for slicing
	//   Use the near plane to slice for current Z index
	protected Matrix4 ProjectionMatrix ( int z ) {
		view = Matrix4.LookAt ( CamPos, CamPos + CamDir, Vector3.UnitY );
		switch ( CurrentProjMode ) {
		case ProjectionType.Orthographic:
			return Matrix4.CreateOrthographicOffCenter (
				BasePosition.X, BasePosition.X + Size.X,
				BasePosition.Y, BasePosition.Y + Size.Y,
				Model.MinZ, Model.MinZ + z * VoxelSize );

		case ProjectionType.Perspective:
			return Matrix4.CreatePerspectiveFieldOfView (
				MathHelper.DegreesToRadians ( 60f ),
				(float)VoxX / VoxY,
				0.01f,
				1000f );
		default: throw new NotImplementedException ( $"Projection type '{CurrentProjMode}' is not implemented in Voxelizer." );
		}
	}

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