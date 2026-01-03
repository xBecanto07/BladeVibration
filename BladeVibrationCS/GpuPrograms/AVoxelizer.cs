using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using SixLabors.ImageSharp;
using System;
using System.Threading.Tasks;
using PixelForm = SixLabors.ImageSharp.PixelFormats;

namespace BladeVibrationCS.GpuPrograms;
public abstract class AVoxelizer : AShaderProgram {
	public readonly int MaterialTextureID, ExtrasTextureID;
	protected readonly int WindowTmpTextureID;//, MaterialTmpTextureID, ExtrasTmpTextureID;
	public readonly Vector3 BasePosition, Size;
	public readonly float VoxelSize = 0.01f;
	protected readonly ModelHolder Model;
	public readonly int VoxX, VoxY, VoxZ;
	protected Matrix4 view;
	public Vector3 CamPos = Vector3.Zero;
	public Vector3 CamDir = Vector3.UnitZ;
	public Vector2i LastScreenSize;

	public readonly Vector3 MinCutoff, MaxCutoff;

	protected int FramebufferID = 0, depthStencilBufferID = 0;

	public AVoxelizer ( ModelHolder model, float voxelSize, bool startRepeatable, byte border, Vector3 minCutoff, Vector3 maxCutoff )
		: base ( startRepeatable //, ("Vertex.glsl", ShaderType.VertexShader)
			//, ("Fragment.glsl", ShaderType.FragmentShader) ) {
			, ("VoxelizerVertex.glsl", ShaderType.VertexShader)
			, ("VoxelizerFragment.glsl", ShaderType.FragmentShader) ) {
		Model = model;
		MinCutoff = minCutoff;
		MaxCutoff = maxCutoff;
		VoxelSize = voxelSize;
		BasePosition = new Vector3 ( MinCutoff.X, MinCutoff.Y, MinCutoff.Z );
		Size = new Vector3 ( MaxCutoff.X - MinCutoff.X, MaxCutoff.Y - MinCutoff.Y, MaxCutoff.Z - MinCutoff.Z );

		VoxX = (int)Math.Ceiling ( Size.X / VoxelSize ) + border;
		VoxY = (int)Math.Ceiling ( Size.Y / VoxelSize ) + border;
		VoxZ = (int)Math.Ceiling ( Size.Z / VoxelSize ) + border;

		MaterialTextureID = PrepareBuffer ();
		ExtrasTextureID = PrepareBuffer ();
		WindowTmpTextureID = PrepareTmpBuffer ();
		//MaterialTmpTextureID = PrepareTmpBuffer ();
		//ExtrasTmpTextureID = PrepareTmpBuffer ();
		FramebufferID = GL.GenFramebuffer ();
		GL.BindFramebuffer ( FramebufferTarget.DrawFramebuffer, FramebufferID );
		GL.FramebufferTexture2D ( FramebufferTarget.DrawFramebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D, WindowTmpTextureID, 0 );
		GL.FramebufferTextureLayer ( FramebufferTarget.DrawFramebuffer, FramebufferAttachment.ColorAttachment1, MaterialTextureID, 0, 0 );
		GL.FramebufferTextureLayer ( FramebufferTarget.DrawFramebuffer, FramebufferAttachment.ColorAttachment2, ExtrasTextureID, 0, 0 );
		GL.DrawBuffers ( 3, [
			DrawBuffersEnum.ColorAttachment0,
			DrawBuffersEnum.ColorAttachment1,
			DrawBuffersEnum.ColorAttachment2
		] );
		//GL.RenderbufferStorage ( RenderbufferTarget.Renderbuffer, RenderbufferStorage.DepthStencil, VoxX, VoxZ );

		depthStencilBufferID = GL.GenRenderbuffer ();
		GL.BindRenderbuffer ( RenderbufferTarget.Renderbuffer, depthStencilBufferID );
		GL.RenderbufferStorage ( RenderbufferTarget.Renderbuffer, RenderbufferStorage.DepthStencil, VoxX, VoxZ );
		GL.FramebufferRenderbuffer ( FramebufferTarget.DrawFramebuffer, FramebufferAttachment.DepthStencilAttachment, RenderbufferTarget.Renderbuffer, depthStencilBufferID );

		view = Matrix4.LookAt ( CamPos, CamPos + CamDir, Vector3.UnitY );
		LastProjectionOrto.Row0 = new Vector2 ( BasePosition.X, BasePosition.X + Size.X );
		LastProjectionOrto.Row1 = new Vector2 ( BasePosition.Y, BasePosition.Y + Size.Y );
		LastProjectionOrto.Row2 = new Vector2 ( MinCutoff.Z, MaxCutoff.Z );
	}

	private int PrepareBuffer () {
		// Create and bind the buffer for 3D texture with material description
		int ret = GL.GenTexture ();
		GL.BindTexture ( TextureTarget.Texture3D, ret );

		GL.TexImage3D ( TextureTarget.Texture3D, 0, PixelInternalFormat.Rgba32f,
			VoxX, VoxZ, VoxY, 0,
			PixelFormat.Rgba, PixelType.Float, nint.Zero );
		GL.TexParameter ( TextureTarget.Texture3D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest );
		GL.TexParameter ( TextureTarget.Texture3D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest );
		GL.TexParameter ( TextureTarget.Texture3D, TextureParameterName.TextureBaseLevel, 0 );
		GL.TexParameter ( TextureTarget.Texture3D, TextureParameterName.TextureMaxLevel, 0 );
		GL.TexParameter ( TextureTarget.Texture3D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge );
		GL.TexParameter ( TextureTarget.Texture3D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge );
		GL.TexParameter ( TextureTarget.Texture3D, TextureParameterName.TextureWrapR, (int)TextureWrapMode.ClampToEdge );

		GL.BindTexture ( TextureTarget.Texture3D, 0 ); // Unbind
		return ret;
	}

	private int PrepareTmpBuffer () {
		// Create and bind the buffer for 3D texture with material description
		int ret = GL.GenTexture ();
		GL.BindTexture ( TextureTarget.Texture2D, ret );
		GL.TexImage2D ( TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba32f,
			VoxX, VoxZ, 0,
			PixelFormat.Rgba, PixelType.Float, nint.Zero );
		GL.TexParameter ( TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest );
		GL.TexParameter ( TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest );
		GL.TexParameter ( TextureTarget.Texture2D, TextureParameterName.TextureBaseLevel, 0 );
		GL.TexParameter ( TextureTarget.Texture2D, TextureParameterName.TextureMaxLevel, 0 );
		GL.TexParameter ( TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge );
		GL.TexParameter ( TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge );

		GL.BindTexture ( TextureTarget.Texture2D, 0 );
		return ret;
	}



	protected override sealed void DisposeInner () {
		GL.DeleteTexture ( MaterialTextureID );
		EntryProgram.StdOut ( "Voxelizer disposed." );
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
				MinCutoff.X, MinCutoff.Z + z * VoxelSize );

		case ProjectionType.Perspective:
			return Matrix4.CreatePerspectiveFieldOfView (
				MathHelper.DegreesToRadians ( 60f ),
				(float)VoxX / VoxY,
				0.01f,
				1000f );
		default: throw new NotImplementedException ( $"Projection type '{CurrentProjMode}' is not implemented in Voxelizer." );
		}
	}

	//protected void Pass1_StencilInput ( int z, Action prepFrameBuffer ) {
	//	ArgumentNullException.ThrowIfNull ( prepFrameBuffer );
	//	prepFrameBuffer ();

	protected const int PARTS = 3;

	protected void Pass0_BackgroundClear () {
		float ratio = (MaxCutoff.Z - MinCutoff.Z) / (MaxCutoff.X - MinCutoff.X);
		GL.DepthFunc ( DepthFunction.Less );
		GL.Viewport ( 0, 0, VoxX, VoxZ );
		GL.Disable ( EnableCap.CullFace );
		SetUniform ( "RENDER_MODE", 3 );
		GL.Enable ( EnableCap.DepthTest );
		GL.Disable ( EnableCap.StencilTest );
		GL.StencilFunc ( StencilFunction.Always, 0, 0xff );
		GL.StencilOp ( StencilOp.Keep, StencilOp.Keep, StencilOp.Keep );
		GL.DepthMask ( true );
		GL.ColorMask ( true, true, true, true );
		GL.ClearColor ( 0.4f, 0.4f, 0f, 1f );
		GL.Clear ( ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit );
		RenderPlane ();
	}

	protected void Pass1_StencilInput () {
		GL.Disable ( EnableCap.DepthTest );
		GL.DepthMask ( false );
		GL.Enable ( EnableCap.StencilTest );

		//GL.ColorMask ( false, false, false, false );
		GL.ColorMask ( true, true, true, true );
		GL.ClearStencil ( 0 );
		GL.Clear ( ClearBufferMask.StencilBufferBit );
		//GL.Enable ( EnableCap.CullFace );

		// 1st pass: setup stencil buffer
		GL.StencilFunc ( StencilFunction.Always, 1, 0xff ); // Ignore the stencil buffer
		GL.StencilOpSeparate ( StencilFace.Back, StencilOp.Keep, StencilOp.Keep, StencilOp.IncrWrap ); // Entering object, increment (allow overflow)
		GL.StencilOpSeparate ( StencilFace.Front, StencilOp.Keep, StencilOp.Keep, StencilOp.DecrWrap ); // Exiting object, decrement (allow underflow)

		//SetProgramUniforms ( z, VoxelizerShaderMode.MODE_CENTER );
		SetUniform ( "RENDER_MODE", 4 );
		SetUniform ( "scale", 1.0f, 1.0f, 1.0f );
		SetUniform ( "offset", 0.0f, 0.0f, 0.0f );
		GL.BindVertexArray ( Model.VAO );
		GL.DrawElements ( PrimitiveType.Triangles, Model.IndexCount, DrawElementsType.UnsignedInt, 0 );
	}

	protected void Pass2_StencilTest () {
		// This probably doesn't actually need to draw the actual model as the stencil buffer already has the info.
		//   Drawing single rectangle covering the slice just to call a fragment shader for each pixel in the slice should be enough.
		GL.StencilFunc ( StencilFunction.Equal, 1, 0xff ); // Pass where stencil value is 1, i.e., inside the object
		GL.StencilOp ( StencilOp.Keep, StencilOp.Keep, StencilOp.Keep );
		GL.ColorMask ( true, true, true, true ); // Enable color writes
															  // Set up orthographic projection and view matrix for slicing
															  //   Use the near plane to slice for current Z index

		SetUniform ( "scale", 1.0f, 1.0f, 1.0f );
		SetUniform ( "offset", 0.0f, 0.0f, 0.0f );
		SetUniform ( "matOffset", 2 );
		SetUniform ( "RENDER_MODE", 1 );
		GL.Disable ( EnableCap.CullFace );
		//GL.BindVertexArray ( Model.VAO );
		//GL.DrawElements ( PrimitiveType.Triangles, Model.IndexCount, DrawElementsType.UnsignedInt, 0 );
		RenderPlane ();

		// Draw skybox to mark empty space
		GL.StencilFunc ( StencilFunction.Notequal, 1, 0xff );
		SetUniform ( "RENDER_MODE", 4 );
		SetUniform ( "matOffset", 3 );
		RenderPlane ();
	}

	private void RenderPlane () {
		Vector3 center = new Vector3 ( MinCutoff.X + MaxCutoff.X, MinCutoff.Y + MaxCutoff.Y, MinCutoff.Z + MaxCutoff.Z ) * 0.5f;
		SetUniform ( "scale", 8.0f, 0.2f, 8.0f );
		SetUniform ( "offset", center.X, center.Y, center.Z );
		RenderController.SkyBoxPrimitive.Render ();
	}

	protected enum VoxelizerShaderMode { MODE_BORDER = 1, MODE_CENTER = 2 }
	protected void SetProgramUniforms ( int z, VoxelizerShaderMode mode ) {
		SetUniform ( "model", Matrix4.Identity );
		SetUniform ( "view", view );
		SetUniform ( "proj", ProjectionMatrix ( z ) );

		SetUniform ( "ActMode", (int)mode );
		SetUniform ( "zPlaneStart", MinCutoff.Z + z * VoxelSize );
		SetUniform ( "zPlaneEnd", MinCutoff.Z + (z + 1) * VoxelSize );
		SetUniform ( "modelMin", MinCutoff.X, MinCutoff.Y, MinCutoff.Z );
		SetUniform ( "modelMax", MaxCutoff.X, MaxCutoff.Y, MaxCutoff.Z );
		SetUniform ( "youngsModulus", ModelHolder.YM_Steel );
	}
}