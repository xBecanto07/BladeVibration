using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.IO;

namespace BladeVibrationCS.GpuPrograms; 
public abstract class AShaderProgram : IDisposable {
	const float BACKGROUND_MULTIPLIER = 0.5f;
	protected const int BACKGROUND_HIGH = 20;
	protected const int BACKGROUND_MID = 12;
	protected const int BACKGROUND_LOW = 56;
	protected const int BACKGROUND_MIN = 27;

	public const int RENDER_MODE_SIMPLE = 1;
	public const int RENDER_MODE_HARD = 2;
	public const int RENDER_MODE_PHONG = 3;
	public const int RENDER_MODE_OBJECTS = 4;

	public readonly int Handle;
	private readonly List<Shader> Shaders;

	public bool IsRepeatable { get; set; } = false;
	public abstract (int R, int G, int B) BackgroundColor { get; }

	public AShaderProgram ( bool startRepeatable, params (string Name, ShaderType Type)[] shaderInfos ) {
		IsRepeatable = startRepeatable;
		if (shaderInfos == null || shaderInfos.Length == 0 )
			throw new ArgumentException ( "At least one shader must be provided to create a shader program", nameof ( shaderInfos ) );

		Shaders = new ();
		foreach ( var (Name, Type) in shaderInfos )
			Shaders.Add ( new ( Name, Type ) );

		Handle = GL.CreateProgram ();
		foreach ( var shader in Shaders ) GL.AttachShader ( Handle, shader.Handle );

		GL.LinkProgram ( Handle );
		GL.GetProgram ( Handle, GetProgramParameterName.LinkStatus, out int status );
		if ( status == 0 ) {
			string infoLog = GL.GetProgramInfoLog ( Handle );
			throw new Exception ( $"Error linking shader program: {infoLog}" );
		}

		foreach ( var shader in Shaders ) {
			GL.DetachShader ( Handle, shader.Handle );
			GL.DeleteShader ( shader.Handle );
		}
	}

	public void Use () {
		GL.ClearColor (
			(BACKGROUND_MULTIPLIER * BackgroundColor.R) / 255f,
			(BACKGROUND_MULTIPLIER * BackgroundColor.G) / 255f,
			(BACKGROUND_MULTIPLIER * BackgroundColor.B) / 255f,
			1f );
		GL.UseProgram ( Handle );
	}

	public void SwitchTo ( AShaderProgram last, Vector2i screenSize ) {
		if ( last?.GetType () == GetType () ) return;
		Use ();
		SwitchToInner ( last, screenSize );
		EntryProgram.StdOut ( $"Switching shader from {last?.GetType().Name ?? "null"} to {GetType().Name}" );
	}

	protected abstract void SwitchToInner ( AShaderProgram last, Vector2i screenSize );


	private int GetUniformLocation ( string name ) {
		int location = GL.GetUniformLocation ( Handle, name );
		if ( location == -1 )
			throw new Exception ( $"Uniform '{name}' not found in shader program." );
		return location;
	}
	public void SetUniform ( string name, int X, int Y, int Z, int W )
		=> GL.Uniform4 ( GetUniformLocation ( name ), X, Y, Z, W );
	public void SetUniform ( string name, float X, float Y, float Z, float W )
		=> GL.Uniform4 ( GetUniformLocation ( name ), X, Y, Z, W );
	public void SetUniform ( string name, float X, float Y, float Z )	
		=> GL.Uniform3 ( GetUniformLocation ( name ), X, Y, Z );
	public void SetUniform ( string name, float X, float Y )				
		=> GL.Uniform2 ( GetUniformLocation ( name ), X, Y );
	public void SetUniform ( string name, float X )							
		=> GL.Uniform1 ( GetUniformLocation ( name ), X );
	public void SetUniform ( string name, Matrix4 matrix )
		=> GL.UniformMatrix4 ( GetUniformLocation ( name ), false, ref matrix );
	public void SetUniform ( string name, int textID)
		=> GL.Uniform1 ( GetUniformLocation ( name ), textID );
	public void SetUniform ( string name, int[] vals )
		=> GL.Uniform1 ( GetUniformLocation ( name ), vals.Length, vals );
	public void SetUniform ( string name, float[] vals )
		=> GL.Uniform1 ( GetUniformLocation ( name ), vals.Length, vals );



	private bool isDisposed = false;
	protected abstract void DisposeInner ();
	private void Dispose ( bool disposing ) {
		if ( !isDisposed ) {
			DisposeInner ();
			GL.DeleteProgram ( Handle );
			isDisposed = true;
		}
	}
	public void Dispose () {
		Dispose ( true );
		GC.SuppressFinalize ( this );
	}
	~AShaderProgram () {
		if ( !isDisposed) 			throw new Exception ( $"ShaderProgram not disposed before finalization. Did you forget to call Dispose()?" );
	}




	protected static void SetupNormalRendering (int width, int height) {
		GL.Enable ( EnableCap.DepthTest );
		GL.DepthFunc ( DepthFunction.Less );
		GL.DepthMask ( true );
		GL.Disable ( EnableCap.Blend );
		GL.Disable ( EnableCap.StencilTest );
		GL.BindFramebuffer ( FramebufferTarget.Framebuffer, 0 );
		GL.Viewport ( 0, 0, width, height );
		GL.ColorMask ( true, true, true, true );
		GL.CullFace ( TriangleFace.FrontAndBack );
	}




	private class Shader {
		public readonly int Handle;
		public readonly string Source;
		public readonly ShaderType Type;

		public Shader ( string shaderName, ShaderType shaderType ) {
			Type = shaderType;
			Source = shaderName;

			string shaderPath = Assets.GetShaderPath ( shaderName );
			if ( !File.Exists ( shaderPath ) )
				throw new FileNotFoundException ( $"Shader file '{shaderName}'({shaderType}) not found: {shaderPath}" );
			string shaderSource = File.ReadAllText ( shaderPath );
			Handle = GL.CreateShader ( shaderType );
			GL.ShaderSource ( Handle, shaderSource );
			GL.CompileShader ( Handle );
			GL.GetShader ( Handle, ShaderParameter.CompileStatus, out int status );
			if ( status == 0 ) {
				string infoLog = GL.GetShaderInfoLog ( Handle );
				throw new Exception ( $"Error compiling shader '{shaderName}'({shaderType}): {infoLog}" );
			}
		}
	}
}