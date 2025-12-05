using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.IO;

namespace BladeVibrationCS; 
public class ShaderProgram {
	public readonly int Handle;

	public ShaderProgram ( params (string Name, ShaderType Type)[] shaderInfos ) {
		if (shaderInfos == null || shaderInfos.Length == 0 )
			throw new ArgumentException ( "At least one shader must be provided to create a shader program", nameof ( shaderInfos ) );

		List<Shader> shaders = new ();
		foreach ( var (Name, Type) in shaderInfos ) {
			shaders.Add ( new ( Name, Type ) );
		}

		Handle = GL.CreateProgram ();
		foreach ( var shader in shaders ) GL.AttachShader ( Handle, shader.Handle );

		GL.LinkProgram ( Handle );
		GL.GetProgram ( Handle, GetProgramParameterName.LinkStatus, out int status );
		if ( status == 0 ) {
			string infoLog = GL.GetProgramInfoLog ( Handle );
			throw new Exception ( $"Error linking shader program: {infoLog}" );
		}

		foreach ( var shader in shaders ) {
			GL.DetachShader ( Handle, shader.Handle );
			GL.DeleteShader ( shader.Handle );
		}
	}

	public void Use () => GL.UseProgram ( Handle );


	private int GetUniformLocation ( string name ) {
		int location = GL.GetUniformLocation ( Handle, name );
		if ( location == -1 )
			throw new Exception ( $"Uniform '{name}' not found in shader program." );
		return location;
	}
	public void SetUniform ( string name, int X, int Y, int Z, int W ) => GL.Uniform4 ( GetUniformLocation ( name ), X, Y, Z, W );
	public void SetUniform ( string name, float X, float Y, float Z )	 => GL.Uniform3 ( GetUniformLocation ( name ), X, Y, Z );
	public void SetUniform ( string name, float X, float Y )				 => GL.Uniform2 ( GetUniformLocation ( name ), X, Y );
	public void SetUniform ( string name, float X )							 => GL.Uniform1 ( GetUniformLocation ( name ), X );



	private bool isDisposed = false;
	protected virtual void Dispose ( bool disposing ) {
		if ( !isDisposed ) {
			GL.DeleteProgram ( Handle );
			isDisposed = true;
		}
	}
	public void Dispose () {
		Dispose ( true );
		GC.SuppressFinalize ( this );
	}
	~ShaderProgram () {
		if ( !isDisposed) {
			throw new Exception ( $"ShaderProgram not disposed before finalization. Did you forget to call Dispose()?" );
		}
	}




	private class Shader {
		public readonly int Handle;

		public Shader ( string shaderName, ShaderType shaderType ) {
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