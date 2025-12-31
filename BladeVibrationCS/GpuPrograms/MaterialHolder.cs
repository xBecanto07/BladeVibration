using System;
using OpenTK.Graphics.OpenGL4;

namespace BladeVibrationCS.GpuPrograms;
public class MaterialHolder {
	public const int BUFFER_SIZE = 8;
	public const int INT_SIZE = BUFFER_SIZE * sizeof ( int );
	public const int FLOAT_SIZE = BUFFER_SIZE * sizeof ( float );
	//private int uboHandle;
	private readonly AShaderProgram OwnerProgram;

	private int[] renderMode = new int[BUFFER_SIZE];
	private float[] shininess = new float[BUFFER_SIZE];
	private float[] diffuseStrength = new float[BUFFER_SIZE];
	private float[] stiffness = new float[BUFFER_SIZE];

	public MaterialHolder ( AShaderProgram owner ) {
		OwnerProgram = owner;
		for ( int i = 0; i < BUFFER_SIZE; i++ ) {
			shininess[i] = 32f;
			diffuseStrength[i] = 0.5f;
			stiffness[i] = ModelHolder.YM_Air;
			renderMode[i] = AShaderProgram.RENDER_MODE_PHONG;
		}
	}

	public void SetMaterial ( int index, float shininessVal, float reflectivityVal, float stiffnessVal, int renderModeVal )
		=> this[index] = (shininessVal, reflectivityVal, stiffnessVal, renderModeVal);

	public (float shininess, float diffuseStrength, float stiffness, int renderMode) this[int index] {
		get {
			if ( index < 0 || index >= BUFFER_SIZE )
				throw new ArgumentOutOfRangeException ( nameof ( index ), $"Index must be between 0 and {BUFFER_SIZE - 1}." );
			return (shininess[index], diffuseStrength[index], stiffness[index], renderMode[index]);
		}
		set {
			if ( index < 0 || index >= BUFFER_SIZE )
				throw new ArgumentOutOfRangeException ( nameof ( index ), $"Index must be between 0 and {BUFFER_SIZE - 1}." );
			(shininess[index], diffuseStrength[index], stiffness[index], renderMode[index]) = value;
		}
	}

	public void UploadToGPU ( int shaderProgramHandle ) {
		//uboHandle = GL.GenBuffer ();
		//GL.BindBuffer ( BufferTarget.UniformBuffer, uboHandle );
		//int totalSize = (FLOAT_SIZE * 3 + INT_SIZE);
		//GL.BufferData ( BufferTarget.UniformBuffer, totalSize, IntPtr.Zero, BufferUsageHint.StaticDraw );
		//UpdateGPU ();
	}

	public void UpdateGPU () {
		//GL.BindBuffer ( BufferTarget.UniformBuffer, uboHandle );
		//GL.BufferSubData ( BufferTarget.UniformBuffer, IntPtr.Zero, INT_SIZE, renderMode );
		//GL.BufferSubData ( BufferTarget.UniformBuffer, (IntPtr)(INT_SIZE), FLOAT_SIZE, diffuseStrength );
		//GL.BufferSubData ( BufferTarget.UniformBuffer, (IntPtr)(INT_SIZE + FLOAT_SIZE), INT_SIZE, renderMode );
		//GL.BufferSubData ( BufferTarget.UniformBuffer, (IntPtr)(INT_SIZE + FLOAT_SIZE * 2), FLOAT_SIZE, stiffness );
		//GL.BindBuffer ( BufferTarget.UniformBuffer, 0 );
	}

	public void Use ( int index ) {
		//GL.BindBufferBase ( BufferRangeTarget.UniformBuffer, index, uboHandle );
		try { OwnerProgram.SetUniform ( "MatProps_renderMode", renderMode ); } catch ( Exception _ ) {}
		try { OwnerProgram.SetUniform ( "MatProps_shininess", shininess ); } catch ( Exception _ ) {}
		try { OwnerProgram.SetUniform ( "MatProps_diffuseStrength", diffuseStrength ); } catch ( Exception _ ) {}
		try { OwnerProgram.SetUniform ( "MatProps_stiffness", stiffness ); } catch ( Exception _ ) { }
	}
}