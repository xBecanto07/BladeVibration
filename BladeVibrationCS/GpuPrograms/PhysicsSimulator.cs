using System;
using System.Linq;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace BladeVibrationCS.GpuPrograms;
public class PhysicsSimulator : AShaderProgram {
	public override (int R, int G, int B) BackgroundColor => (BACKGROUND_LOW, BACKGROUND_LOW, BACKGROUND_MID); // Teal
	public readonly PhysicsCompute ComputeShader;
	public readonly BasicMeshRenderer BMR;
	public float Scale = 1.0f;

	public enum SwizzleMode { XYZ = 0, XZY = 1, YXZ = 2, YZX = 3, ZXY = 4, ZYX = 5 }
	public enum AxisSelect { None = 0, X = 1, Y = 2, Z = 3, XY = 4, XZ = 5, YZ = 6, XYZ = 7 }
	public enum VisualizeMode { Diff = 0, World = 1, Coords = 2, Value = 3 }
	public SwizzleMode SwizzleIndex = SwizzleMode.XYZ;
	public SwizzleMode SwizzleData = SwizzleMode.XYZ;
	public AxisSelect InvertIndex = AxisSelect.None;
	public AxisSelect InvertData = AxisSelect.None;
	public AxisSelect DisplayAxis = AxisSelect.XYZ;
	//public float Visualize = 1.0f;
	public VisualizeMode Visualize = VisualizeMode.Diff;

	public PhysicsSimulator ( VoxelObject voxelTexture, BasicMeshRenderer bmr )
		: base ( true, ("PhysicsVertex.glsl", ShaderType.VertexShader)
			, ("PhysicsFragment.glsl", ShaderType.FragmentShader) ) {
		ArgumentNullException.ThrowIfNull ( voxelTexture );
		ComputeShader = new ( voxelTexture );
		BMR = bmr;
		//CheckVertices ();
	}

	const int MV = 1;
	class VoxelInfo {
		public struct Variant {
			public readonly Vector3 Vox, Diff;
			public readonly Vector3i Index;
			public readonly float Dist;
			public bool Valid => Dist >= 0;
			public Variant ( Vector3 me, Vector3i index, Vector3? vox ) {
				Index = index;
				if ( vox == null ) {
					Vox = Diff = Vector3.Zero; Dist = -1;
				} else {
					Vox = vox.Value;
					Diff = Vox - me;
					Dist = MathF.Round ( Diff.LengthFast * ROUND_MULT ) / ROUND_MULT;
				}
			}

			public override string ToString () => Valid ? $"{Vox.X:F2}|{Vox.Y:F2}|{Vox.Z:F2} e{Dist:F2} [{Index.X};{Index.Y};{Index.Z}]" : "Invalid";
		}
		public readonly Vector3 Me;
		public readonly Vector3?[][] Probes;
		public readonly Variant[] Variants;

		public override string ToString () => $"{Me.X:F2}|{Me.Y:F2}|{Me.Z:F2} / {Variants[MV].Vox.X:F2}|{Variants[MV].Vox.Y:F2}|{Variants[MV].Vox.Z:F2} e{Variants[MV].Dist:F2} [{Variants[MV].Index.X};{Variants[MV].Index.Y};{Variants[MV].Index.Z}]";

		public VoxelInfo ( Vector3 me, Vector3?[][] probes, params (Vector3i index, Vector3? vox)[] variants ) {
			Me = me;
			Probes = probes;
			Variants = variants.Select ( v => new Variant ( me, v.index, v.vox ) ).ToArray ();
		}
	}

	const int ROUND_MULT = 50;

	private void CheckVertices () {
		var VoxelTexture = ComputeShader.VoxelTexture;
		Vector4[] TextureData = new Vector4[VoxelTexture.VoxX * VoxelTexture.VoxY * VoxelTexture.VoxZ];
		GL.BindTexture ( TextureTarget.Texture3D, VoxelTexture.MaterialTextureID );
		GL.GetTexImage ( TextureTarget.Texture3D, 0, PixelFormat.Rgba, PixelType.Float, TextureData );

		int XN = VoxelTexture.VoxX;
		int YN = VoxelTexture.VoxY;
		int ZN = VoxelTexture.VoxZ;

		Vector3? GetVal ( Vector3i coord ) {
			if ( coord.X == XN ) coord.X = XN - 1;
			if ( coord.Y == YN ) coord.Y = YN - 1;
			if ( coord.Z == ZN ) coord.Z = ZN - 1;

			int index = coord.Z * ZN * XN + coord.Y * XN + coord.X;
			bool inBounds1 = index >= 0 && index < TextureData.Length;
			bool inBounds2 = coord.X >= 0 && coord.X < XN && coord.Y >= 0 && coord.Y < YN && coord.Z >= 0 && coord.Z < ZN;
			if ( !inBounds1 || !inBounds2 ) {
				//if ( inBounds1 != inBounds2 ) {
				//	EntryProgram.StdOut ( $"VoxelVisualizer: Inconsistent bounds check for coord {coord} (Index //{index}): inBounds1={inBounds1}, inBounds2={inBounds2}" );
				//}
				return null;
			}
			Vector4 voxelData = TextureData[index];
			voxelData.Z *= -1;
			return (voxelData.Xyz * ROUND_MULT).Round () / ROUND_MULT;
		}
		Vector3?[][] Probes ( Vector3i coord ) {
			Vector3?[] X = new Vector3?[XN], Y = new Vector3?[YN], Z = new Vector3?[ZN];
			for ( int x = 0; x < XN; x++ )
				X[x] = GetVal ( new Vector3i ( x, coord.Y, coord.Z ) );
			for ( int y = 0; y < YN; y++ )
				Y[y] = GetVal ( new Vector3i ( coord.X, y, coord.Z ) );
			for ( int z = 0; z < ZN; z++ )
				Z[z] = GetVal ( new Vector3i ( coord.X, coord.Y, z ) );
			return [X, Y, Z];
		}

		VoxelInfo GetInfo ( float x, float y, float z ) {
			Vector3 myPos = new Vector3 ( x, y, z );
			Vector3 normPos = (myPos - VoxelTexture.BasePosition) / VoxelTexture.Size;
			Vector3i voxelCoord = new Vector3i (
				(int)MathF.Floor ( normPos.X * XN ),
				(int)MathF.Floor ( normPos.Y * YN ),
				(int)MathF.Floor ( normPos.Z * ZN )
			);
			var probes = Probes ( voxelCoord );
			Vector3? voxelPos = GetVal ( voxelCoord );

			Vector3i voxelCoordAlt1 = voxelCoord.Xzy;
			Vector3? voxelPosAlt1 = GetVal ( voxelCoordAlt1 );

			Vector3i voxelCoordAlt2 = new Vector3i (
				(int)MathF.Floor ( normPos.X * XN ),
				(int)MathF.Floor ( normPos.Z * YN ),
				(int)MathF.Floor ( normPos.Y * ZN )
			);
			Vector3? voxelPosAlt2 = GetVal ( voxelCoordAlt2 );

			Vector3i voxelCoordAlt3 = voxelCoord.Xzy;
			Vector3? voxelPosAlt3 = GetVal ( voxelCoordAlt3 );

			myPos = (myPos * ROUND_MULT).Round () / ROUND_MULT;

			return new ( myPos, probes, (voxelCoord, voxelPos), (voxelCoordAlt1, voxelPosAlt1), (voxelCoordAlt2, voxelPosAlt2), (voxelCoordAlt3, voxelPosAlt3) );
		}


		var infoAtMin = GetInfo ( VoxelTexture.BasePosition.X, VoxelTexture.BasePosition.Y, VoxelTexture.BasePosition.Z );
		var infoAtXMax = GetInfo ( VoxelTexture.BasePosition.X + VoxelTexture.Size.X, VoxelTexture.BasePosition.Y, VoxelTexture.BasePosition.Z );
		var infoAtYMax = GetInfo ( VoxelTexture.BasePosition.X, VoxelTexture.BasePosition.Y + VoxelTexture.Size.Y, VoxelTexture.BasePosition.Z );
		var infoAtZMax = GetInfo ( VoxelTexture.BasePosition.X, VoxelTexture.BasePosition.Y, VoxelTexture.BasePosition.Z + VoxelTexture.Size.Z );
		var infoAtAMax = GetInfo ( VoxelTexture.BasePosition.X + VoxelTexture.Size.X, VoxelTexture.BasePosition.Y + VoxelTexture.Size.Y, VoxelTexture.BasePosition.Z + VoxelTexture.Size.Z );
		var infoAtMMax = GetInfo ( VoxelTexture.BasePosition.X + VoxelTexture.Size.X / 2, VoxelTexture.BasePosition.Y + VoxelTexture.Size.Y / 2, VoxelTexture.BasePosition.Z + VoxelTexture.Size.Z / 2 );

		var miniCube = new VoxelInfo[3, 3, 3];
		for ( int dx = 0; dx <= 2; dx++ ) {
			float X = VoxelTexture.BasePosition.X + dx * VoxelTexture.VoxelSize * 4.5f;
			for ( int dy = 0; dy <= 2; dy++ ) {
				float Y = VoxelTexture.BasePosition.Y + dy * VoxelTexture.VoxelSize * 4.5f;
				for ( int dz = 0; dz <= 2; dz++ ) {
					float Z = VoxelTexture.BasePosition.Z + dz * VoxelTexture.VoxelSize * 4.5f;
					miniCube[dx, dy, dz] = GetInfo ( X, Y, Z );
				}
			}
		}

		var info = new VoxelInfo[VoxelTexture.Model.VertexCount];
		float minDistance = float.PositiveInfinity, maxDistance = float.NegativeInfinity, totDistance = 0;
		int numDistances = 0;

		for ( int v = 0; v < VoxelTexture.Model.VertexCount; v++ ) {
			var vertex = VoxelTexture.Model.Vertices[v];
			info[v] = GetInfo ( vertex.X, vertex.Y, vertex.Z );
			if ( info[v].Variants[MV].Valid )
				continue;

			float distance = info[v].Variants[MV].Dist;
			minDistance = MathF.Min ( minDistance, distance );
			maxDistance = MathF.Max ( maxDistance, distance );
			totDistance += distance;
			numDistances++;
		}

		if ( numDistances > 0 ) {
			float avgDistance = totDistance / numDistances;
			EntryProgram.StdOut ( $"VoxelVisualizer: Vertex to voxel distances -- Min: {minDistance:F6}, Max: {maxDistance:F6}, Avg: {avgDistance:F6} over {numDistances} vertices." );
		} else {
			EntryProgram.StdOut ( $"VoxelVisualizer: No vertices found inside voxel texture bounds." );
		}
	}

	private VoxelObject VO => ComputeShader.VoxelTexture;

	public void Render ( Vector3 modelOffset, float scale, Matrix4 view, Matrix4 projection, Vector3 camPos ) {
		//ComputeShader.Calc ();
		ComputeShader.ActivateTextures ();

		Use ();
		GL.ClearColor ( BackgroundColor.R / 255f, BackgroundColor.G / 255f, BackgroundColor.B / 255f, 1.0f );
		GL.Clear ( ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit );

		SetUniform ( "model", Matrix4.Identity );
		BMR.MaterialHolder.Use ( 0 );
		SetUniform ( "view", view );
		SetUniform ( "proj", projection );
		SetUniform ( "scale", Scale );

		SetUniform ( "TextureMin", VO.BasePosition.X, VO.BasePosition.Y, VO.BasePosition.Z );
		SetUniform ( "TextureSize", VO.Size.X, VO.Size.Y, VO.Size.Z );

		SetUniform ( "offset", modelOffset.X, modelOffset.Y, modelOffset.Z );
		SetUniform ( "scale", scale, scale, scale );
		SetUniform ( "SwizzleIndex", (int)SwizzleIndex );
		SetUniform ( "SwizzleData", (int)SwizzleData );
		SetUniform ( "InvertIndex", (int)InvertIndex );
		SetUniform ( "InvertData", (int)InvertData );
		SetUniform ( "DisplayAxis", (int)DisplayAxis );
		SetUniform ( "Visualize", (int)Visualize );

		GL.BindVertexArray ( VO.Model.VAO );
		GL.DrawElements ( PrimitiveType.Triangles, VO.Model.IndexCount, DrawElementsType.UnsignedInt, 0 );
	}

	protected override void SwitchToInner ( AShaderProgram last, Vector2i screenSize ) {
		SetupNormalRendering ( screenSize.X, screenSize.Y );
	}

	protected override void DisposeInner () {
	}
}