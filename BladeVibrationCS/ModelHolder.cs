using System;
using System.Collections.Generic;
using System.Linq;
using OpenTK.Graphics.OpenGL4;

namespace BladeVibrationCS; 
public class ModelHolder : IDisposable {
	private static uint MatID = 1;
	public const float YM_Air = 1e3f;
	public const float YM_Wood = 9.5e9f;
	public const float YM_Bronze = 112e9f;
	public const float YM_Steel = 200e9f;

	List<string> Errors = [];

	public List<Vertex> Vertices = [];
	List<int> Indices = [];
	float[] Data = null;
	int[] DataIndices = null;

	public int VBO { get; private set; }
	public int VAO { get; private set; }
	public int EBO { get; private set; }
	public int VertexCount => Vertices.Count;
	public int FaceCount => Indices.Count / 3;
	public int IndexCount => Indices.Count;

	public bool IsOnGPU => VBO != 0 && VAO != 0;

	public ModelHolder (string modelPath, Dictionary<string, float> YoungModuli) {
		if ( string.IsNullOrEmpty ( modelPath ) )
			throw new ArgumentException ( "modelPath is null or empty", nameof ( modelPath ) );
		if ( !System.IO.File.Exists ( modelPath ) )
			throw new ArgumentException ( $"modelPath '{modelPath}' does not exist", nameof ( modelPath ) );

		var importer = new Assimp.AssimpContext ();
		var scene = importer.ImportFile ( modelPath, Assimp.PostProcessSteps.Triangulate );

		List<float> matIDs = [];

		foreach ( var mesh in scene.Meshes ) {
			float matId = MatID++;
			matIDs.Add ( matId );
			int vN = mesh.VertexCount;
			Vertices.Capacity += vN;
			float YM = YoungModuli.GetValueOrDefault ( mesh.Name, YM_Air );
			int vertexOffset = Vertices.Count;

			for ( int i = 0; i < vN; i++ ) {
				Vertices.Add ( new Vertex {
					X = mesh.Vertices[i].X,
					Y = mesh.Vertices[i].Y,
					Z = mesh.Vertices[i].Z,
					R = mesh.Normals[i].X,
					S = mesh.Normals[i].Y,
					T = mesh.Normals[i].Z,
					U = mesh.TextureCoordinateChannelCount > 0 ? mesh.TextureCoordinateChannels[0][i].X : 0f,
					V = mesh.TextureCoordinateChannelCount > 0 ? mesh.TextureCoordinateChannels[0][i].Y : 0f,
					matID = matId,
				} );
			}

			int tN = mesh.FaceCount;
			Indices.Capacity += tN * 3;
			for ( int i = 0; i < tN; i++ ) {
				var face = mesh.Faces[i];
				if ( face.IndexCount != 3 ) {
					Errors.Add ( $"Non-triangular face detected in mesh '{mesh.Name}'." );
					continue;
				}
				Indices.Add ( face.Indices[0] + vertexOffset );
				Indices.Add ( face.Indices[1] + vertexOffset );
				Indices.Add ( face.Indices[2] + vertexOffset );
			}
		}

		Data = [.. Vertices.SelectMany ( v => new[] { v.X, v.Y, v.Z, v.R, v.S, v.T, v.U, v.V, v.matID } )];
		DataIndices = Indices.ToArray ();

		EntryProgram.StdOut ( $"Model '{modelPath}' loaded: {VertexCount} vertices, {FaceCount} faces. Materials used:\n\t{string.Join ( ", ", matIDs )}\nLimits: X[{MinX}, {MaxX}], Y[{MinY}, {MaxY}], Z[{MinZ}, {MaxZ}]" );
	}

	public void PushToGPU () {
		if ( IsOnGPU ) throw new InvalidOperationException ( "Model is already pushed to GPU." );
		(VBO, VAO, EBO) = PushModelToGPU ( Data, DataIndices );
	}

	public static (int vbo, int vao, int ebo) PushModelToGPU ( float[] vertices, int[] indices ) {
		int vao = GL.GenVertexArray ();
		int vbo = GL.GenBuffer ();
		int ebo = GL.GenBuffer ();

		GL.BindVertexArray ( vao ); // Binding VAO must come first because it stores the state of VBO and EBO bindings
		//int stride = 9 * sizeof ( float );
		//GL.VertexAttribPointer ( 0, 3, VertexAttribPointerType.Float, false, stride, 0 );
		//GL.EnableVertexAttribArray ( 0 );

		GL.BindBuffer ( BufferTarget.ArrayBuffer, vbo );
		GL.BufferData ( BufferTarget.ArrayBuffer, vertices.Length * sizeof ( float ), vertices, BufferUsageHint.StaticDraw );

		GL.BindBuffer ( BufferTarget.ElementArrayBuffer, ebo );
		GL.BufferData ( BufferTarget.ElementArrayBuffer, indices.Length * sizeof ( int ), indices, BufferUsageHint.StaticDraw );

		GL.VertexAttribPointer ( 0, 3, VertexAttribPointerType.Float, false, 9 * sizeof ( float ), 0 ); // Position
		GL.VertexAttribPointer ( 1, 3, VertexAttribPointerType.Float, false, 9 * sizeof ( float ), 3 * sizeof ( float ) ); // Normal
		GL.VertexAttribPointer ( 2, 2, VertexAttribPointerType.Float, false, 9 * sizeof ( float ), 6 * sizeof ( float ) ); // UV
		GL.VertexAttribPointer ( 3, 1, VertexAttribPointerType.Float, false, 9 * sizeof ( float ), 8 * sizeof ( float ) ); // Mat ID
		GL.EnableVertexAttribArray ( 0 );
		GL.EnableVertexAttribArray ( 1 );
		GL.EnableVertexAttribArray ( 2 );
		GL.EnableVertexAttribArray ( 3 );

		GL.BindVertexArray ( 0 );
		GL.BindBuffer ( BufferTarget.ArrayBuffer, 0 );

		return ( vbo, vao, ebo );
	}

	public float MinX => Vertices.Min ( v => v.X );
	public float MaxX => Vertices.Max ( v => v.X );
	public float MinY => Vertices.Min ( v => v.Y );
	public float MaxY => Vertices.Max ( v => v.Y );
	public float MinZ => Vertices.Min ( v => v.Z );
	public float MaxZ => Vertices.Max ( v => v.Z );

	private bool isDisposed = false;
	protected virtual void Dispose ( bool disposing ) {
		if ( !isDisposed ) {
			GL.DeleteBuffer ( VBO );
			GL.DeleteVertexArray ( VAO );
			isDisposed = true;
		}
	}
	public void Dispose () {
		Dispose ( true );
		GC.SuppressFinalize ( this );
	}
	~ModelHolder () {
		if ( !isDisposed ) throw new Exception ( "ModelHolder was not disposed properly before being finalized." );
	}
}

public struct Vertex {
	public float X, Y, Z; // Position
	public float R, S, T; // Normal
	public float U, V;    // Texture Coordinates
	public float matID;       // Material ID / mesh ID
}