using System;
using System.Collections.Generic;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace BladeVibrationCS.GpuPrograms;
internal class VoxelVisualizer : AShaderProgram {
	public override (int R, int G, int B) BackgroundColor => (BACKGROUND_MIN, BACKGROUND_MID, BACKGROUND_LOW); // Teal
	public readonly VoxelObject VoxelTexture;

	float[] PlaneVertices = {
		-100f, -100f, 0f,
		 100f, -100f, 0f,
		 100f,  100f, 0f,
		-100f,  100f, 0f
	};
	int[] PlaneIndices = {
		0, 1, 2,
		2, 3, 0
	};

	int VBO, VAO, EBO, posHandle;

	public VoxelVisualizer ( VoxelObject voxelTexture )
		: base ( true, ("VoxelViewVertex.glsl", ShaderType.VertexShader)
			, ("VoxelViewFragment.glsl", ShaderType.FragmentShader) ) {
		ArgumentNullException.ThrowIfNull ( voxelTexture );
		VoxelTexture = voxelTexture;

		VAO = GL.GenVertexArray ();
		VBO = GL.GenBuffer ();
		EBO = GL.GenBuffer ();

		GL.BindVertexArray ( VAO );

		GL.BindBuffer ( BufferTarget.ArrayBuffer, VBO );
		GL.BufferData ( BufferTarget.ArrayBuffer, PlaneVertices.Length * sizeof ( float ), PlaneVertices, BufferUsageHint.StaticDraw );

		GL.BindBuffer ( BufferTarget.ElementArrayBuffer, EBO );
		GL.BufferData ( BufferTarget.ElementArrayBuffer, PlaneIndices.Length * sizeof ( int ), PlaneIndices, BufferUsageHint.StaticDraw );

		posHandle = GL.GetAttribLocation ( Handle, "aPos" );
		GL.EnableVertexAttribArray ( posHandle );
		GL.VertexAttribPointer ( posHandle, 3, VertexAttribPointerType.Float, false, 3 * sizeof ( float ), 0 );

		GL.BindBuffer ( BufferTarget.ArrayBuffer, 0 );
		GL.BindVertexArray ( 0 );
	}

	public void Render (Vector3 position, Quaternion rotation, Matrix4 view, Matrix4 projection) {
		Use ();
		Matrix4 model = Matrix4.CreateTranslation ( position ) * Matrix4.CreateFromQuaternion ( rotation );
		SetUniform ( "model", model );
		SetUniform ( "view", view );
		SetUniform ( "proj", projection );
		SetUniform ( "voxelTexture", VoxelTexture.MaterialTextureID );

		GL.ActiveTexture ( TextureUnit.Texture0 );
		GL.BindTexture ( TextureTarget.Texture3D, VoxelTexture.MaterialTextureID );

		GL.BindVertexArray ( VAO );
		GL.DrawElements ( PrimitiveType.Triangles, PlaneIndices.Length, DrawElementsType.UnsignedInt, 0 );
		GL.BindVertexArray ( 0 );

		GL.BindTexture ( TextureTarget.Texture3D, 0 );
	}

	protected override void SwitchToInner ( AShaderProgram last, Vector2i screenSize ) {
		SetupNormalRendering ( screenSize.X, screenSize.Y );
	}

	protected override void DisposeInner () {
		GL.DeleteBuffer ( VBO );
		GL.DeleteBuffer ( EBO );
		GL.DeleteVertexArray ( VAO );
	}
}