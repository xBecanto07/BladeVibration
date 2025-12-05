using System;
using System.Collections.Generic;
using System.Linq;
using OpenTK.Graphics.OpenGL4;

namespace BladeVibrationCS; 
public class ModelHolder {
	const float YM_Air = 1e3f;
	const float YM_Wood = 9.5e9f;
	const float YM_Bronze = 112e9f;
	const float YM_Steel = 200e9f;

	List<Vertex> Vertices = [];
	float[] Data = null;

	public int VBO { get; private set; }
	public int VAO { get; private set; }
	public int VertexCount => Vertices.Count;

	public ModelHolder (string modelPath, Dictionary<string, float> YoungModuli) {
		if ( string.IsNullOrEmpty ( modelPath ) )
			throw new ArgumentException ( "modelPath is null or empty", nameof ( modelPath ) );
		if ( !System.IO.File.Exists ( modelPath ) )
			throw new ArgumentException ( $"modelPath '{modelPath}' does not exist", nameof ( modelPath ) );

		var importer = new Assimp.AssimpContext ();
		var scene = importer.ImportFile ( modelPath, Assimp.PostProcessSteps.Triangulate );

		foreach ( var mesh in scene.Meshes ) {
			int vN = mesh.VertexCount;
			Vertices.Capacity += vN;
			float YM = YoungModuli.GetValueOrDefault ( mesh.Name, YM_Air );

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
					E = YM
				} );
			}
		}

		Data = [.. Vertices.SelectMany ( v => new[] { v.X, v.Y, v.Z, v.R, v.S, v.T, v.U, v.V, v.E } )];
	}

	public void PushToGPU () {
		VBO = GL.GenBuffer ();
		GL.BindBuffer ( BufferTarget.ArrayBuffer, VBO );
		GL.BufferData ( BufferTarget.ArrayBuffer, Data.Length * sizeof ( float ), Data, BufferUsageHint.StaticDraw );

		VAO = GL.GenVertexArray ();
		GL.BindVertexArray ( VAO );
		int stride = 9 * sizeof ( float );
		GL.VertexAttribPointer ( 0, 3, VertexAttribPointerType.Float, false, stride, 0 );
		GL.EnableVertexAttribArray ( 0 );
	}
}

public struct Vertex {
	public float X, Y, Z; // Position
	public float R, S, T; // Normal
	public float U, V;    // Texture Coordinates
	public float E;       // Young's Modulus
}