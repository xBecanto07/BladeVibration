using System;
using System.Collections.Generic;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace BladeVibrationCS.GpuPrograms; 
public class BasicMeshRenderer : AShaderProgram {
	public override (int R, int G, int B) BackgroundColor => (BACKGROUND_MID, BACKGROUND_MIN, BACKGROUND_LOW); // Crimson

	public readonly ModelHolder Model;

	public BasicMeshRenderer ( LoadModelRequest loadModelRequest )
		: base ( true, ("Vertex.glsl", ShaderType.VertexShader)
			, ("Fragment.glsl", ShaderType.FragmentShader) )
		{
		ArgumentNullException.ThrowIfNull ( loadModelRequest );
		Model = new ( loadModelRequest.ModelPath, loadModelRequest.YoungModuli );
		Model.PushToGPU ();
	}
	public BasicMeshRenderer ( ModelHolder model )
		: base ( true, ("Vertex.glsl", ShaderType.VertexShader)
			, ("Fragment.glsl", ShaderType.FragmentShader) )
	{
		ArgumentNullException.ThrowIfNull ( model );
		Model = model;
		if ( !Model.IsOnGPU ) Model.PushToGPU ();
	}

	public void Render (Vector3 modelOffset, float scale, Matrix4 view, Matrix4 projection ) {
		SetUniform ( "colorA", 1f, 0f, 0f );
		SetUniform ( "colorB", 0f, 0f, 1f );
		SetUniform ( "offset", modelOffset.X, modelOffset.Y, modelOffset.Z );
		SetUniform ( "scale", scale );
		SetUniform ( "view", view );
		SetUniform ( "proj", projection );
		SetUniform ( "model", Matrix4.Identity );

		//RenderController.Draw2D ();
		GL.BindVertexArray ( Model.VAO );
		//GL.BindBuffer ( BufferTarget.ArrayBuffer, Model.VBO );
		//GL.BindBuffer ( BufferTarget.ElementArrayBuffer, Model.EBO );
		GL.DrawElements ( PrimitiveType.Triangles, Model.IndexCount, DrawElementsType.UnsignedInt, 0 );

		SetUniform ( "offset", modelOffset.X + 0.3f, modelOffset.Y - 0.3f, modelOffset.Z + 0.5f );
		//GL.DrawElements ( PrimitiveType.Triangles, Model.IndexCount, DrawElementsType.UnsignedInt, 0 );
		RenderController.Draw2D ();
	}

	protected override void SwitchToInner ( AShaderProgram last, Vector2i screenSize ) {
		SetupNormalRendering ( screenSize.X, screenSize.Y );
	}

	protected override void DisposeInner () {}
}