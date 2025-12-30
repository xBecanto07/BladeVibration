using System;
using System.Collections.Generic;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace BladeVibrationCS.GpuPrograms; 
public class BasicMeshRenderer : AShaderProgram {
	public override (int R, int G, int B) BackgroundColor => (BACKGROUND_MID, BACKGROUND_MIN, BACKGROUND_LOW); // Crimson

	public readonly ModelHolder Model;
	public readonly MaterialHolder MaterialHolder;

	public BasicMeshRenderer ( LoadModelRequest loadModelRequest )
		: base ( true, ("Vertex.glsl", ShaderType.VertexShader)
			, ("Fragment.glsl", ShaderType.FragmentShader) )
		{
		ArgumentNullException.ThrowIfNull ( loadModelRequest );
		MaterialHolder = new ( this );
		Model = new ( loadModelRequest.ModelPath, loadModelRequest.YoungModuli );
		Model.PushToGPU ();
		PrepareMaterialHolder ();
	}
	public BasicMeshRenderer ( ModelHolder model )
		: base ( true, ("Vertex.glsl", ShaderType.VertexShader)
			, ("Fragment.glsl", ShaderType.FragmentShader) )
	{
		ArgumentNullException.ThrowIfNull ( model );
		MaterialHolder = new ( this );
		Model = model;
		if ( !Model.IsOnGPU ) Model.PushToGPU ();
		PrepareMaterialHolder ();
	}

	private void PrepareMaterialHolder () {
		MaterialHolder.SetMaterial ( 0, 32f, 0.5f, 0.5f, RENDER_MODE_OBJECTS ); // Scary-zero error material
		MaterialHolder.SetMaterial ( 6, 16f, 0.3f, 0.8f, RENDER_MODE_HARD ); // Skybox material
		MaterialHolder.SetMaterial ( 7, 8f, 0.1f, 0.2f, RENDER_MODE_SIMPLE ); // Star material
		MaterialHolder.SetMaterial ( 1, 64f, 0.2f, 0.9f, RENDER_MODE_PHONG ); // Matte material
		MaterialHolder.SetMaterial ( 2, 4f, 0.7f, 0.6f, RENDER_MODE_PHONG ); // Shiny material
		MaterialHolder.UploadToGPU ( Handle );
	}

	public void Render ( Vector3 modelOffset, float scale, Matrix4 view, Matrix4 projection, Vector3 camPos ) {
		MaterialHolder.Use ( 0 );
		//SetUniform ( "shadingMethod", RENDER_MODE_HARD );
		SetUniform ( "view", view );
		SetUniform ( "proj", projection );
		SetUniform ( "model", Matrix4.Identity );

		SetUniform ( "lightPosMain", 200f, 5000f, 200f );
		SetUniform ( "lightPosSecond", -2500f, 900f, 150f );
		//SetUniform ( "lightPosMain", camPos.X, camPos.Y, camPos.Z );
		//SetUniform ( "lightPosSecond", 0f, 0f, 0f );
		SetUniform ( "camPos", camPos.X, camPos.Y, camPos.Z );
		SetUniform ( "specularColor", 0.8f, 1f, 0.8f );

		SetUniform ( "colorA", 1f, 1f, 0f );
		SetUniform ( "colorB", 0f, 1f, 1f );
		SetUniform ( "scale", 4.0f );
		SetUniform ( "offset", 0, 0, 0 );
		SetUniform ( "matOffset", 6 );
		RenderController.SkyBoxPrimitive.Render ();

		SetUniform ( "colorA", 1f, 0f, 0f );
		SetUniform ( "colorB", 0f, 0f, 1f );
		SetUniform ( "offset", modelOffset.X, modelOffset.Y, modelOffset.Z );
		SetUniform ( "scale", scale );
		//SetUniform ( "shadingMethod", RENDER_MODE_PHONG );
		SetUniform ( "camPos", camPos.X, camPos.Y, camPos.Z );

		GL.BindVertexArray ( Model.VAO );
		SetUniform ( "matOffset", 0 );
		GL.DrawElements ( PrimitiveType.Triangles, Model.IndexCount, DrawElementsType.UnsignedInt, 0 );

		SetUniform ( "scale", RENDER_MODE_SIMPLE );
		//SetUniform ( "shadingMethod", 1 );

		const float Bounds = 2f;
		const float Offset2D = 0.3f;
		//SetUniform ( "matOffset", 6 );
		SetUniform ( "matOffset", 7 );
		for ( float x = -Bounds; x <= Bounds; x += Offset2D )
			for ( float y = -Bounds; y <= Bounds; y += Offset2D )
				for ( float z = -Bounds; z <= Bounds; z += Offset2D ) {
					SetUniform ( "offset", x, y, z );
					RenderController.StarPrimitive.Render ();
				}
	}

	protected override void SwitchToInner ( AShaderProgram last, Vector2i screenSize ) {
		SetupNormalRendering ( screenSize.X, screenSize.Y );
	}

	protected override void DisposeInner () {}
}