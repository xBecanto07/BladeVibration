using BladeVibrationCS.GpuPrograms;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;

namespace BladeVibrationCS; 
public class RenderController {
	const float QuadSide = 10;
	const float QuadBase = -QuadSide / 2;
	const float QuadTop = QuadBase + QuadSide;

	public readonly static float[] quadVertices = {
			QuadBase,  QuadTop, 0,
			QuadTop,  QuadTop, 0,
			QuadTop,  QuadBase, 0,
			QuadBase,  QuadBase, 0,
			0, 0,  QuadBase,
			};
	public readonly static int[] quadIndices = {
			0, 1, 2,
			0, 1, 3,
			0, 1, 4,
			1, 2, 4,
			2, 3, 4,
			3, 0, 4,

			1, 2, 0, 
			1, 3, 0, 
			1, 4, 0, 
			2, 4, 1, 
			3, 4, 2, 
			0, 4, 3,
			};
	public static readonly int quadVBO;
	public static readonly int quadVAO;
	public static readonly int quadEBO;

	static RenderController () {
		(quadVBO, quadVAO, quadEBO) = ModelHolder.PushModelToGPU ( quadVertices, quadIndices );
	}

	readonly Controler Controler;
	private readonly Queue<AShaderProgram> DrawRequests = new ();
	private readonly List<(ModelHolder model, VoxelObject voxel)> Models = [];

	public readonly Vector2i ScreenSize;

	private AShaderProgram lastProgram = null;
	private readonly Queue<AShaderProgram> nextDrawQueue = new ();
	private readonly HashSet<AShaderProgram> programScrapYard = new ();
	public AShaderProgram NextDrawRequest {
		get {
			if ( DrawRequests.Count == 0 ) return null;
			AShaderProgram program = DrawRequests.Dequeue ();
			if ( program.IsRepeatable )
				nextDrawQueue.Enqueue ( program );
			else
				programScrapYard.Add ( program );
			program.SwitchTo ( lastProgram, ScreenSize );
			lastProgram = program;
			return program;
		}
	}

	public void FinishDrawCycle () {
		while ( nextDrawQueue.Count > 0 )
			DrawRequests.Enqueue ( nextDrawQueue.Dequeue () );
		foreach ( var program in programScrapYard )
			program.Dispose ();
	}

	public RenderController ( Controler controler, Vector2i screenSize ) {
		Controler = controler;
		ScreenSize = screenSize;
	}

	public void Process () {
		while ( Controler.LoadModelRequests.Reader.TryRead ( out var loadRequest ) ) {
			int modelId = Models.Count;
			Models.Add ( ( new ( loadRequest.ModelPath, loadRequest.YoungModuli ), null) );
			Program.StdOut ( "Model loaded successfully." );
		}

		while ( Controler.VoxelizeRequests.Reader.TryRead ( out var voxelizeRequest ) ) {
			int requests = 0;
			for (int i = 0; i < Models.Count; i++ ) {
				if ( Models[i].voxel != null ) continue;
				VoxelObject vo = new ( Models[i].model, (Models[i].model.MaxX - Models[i].model.MinX) / 256 );
				Models[i] = (Models[i].model, vo);
				if ( vo.IsRepeatable ) {
					foreach ( var shader in DrawRequests ) shader.IsRepeatable = false;
				}
				DrawRequests.Enqueue ( vo );
				requests++;
			}
			Program.StdOut ( $"Voxelization request ({requests}) pushed to render queue" );
		}

		while ( Controler.SaveVoxelRequests.Reader.TryRead ( out var saveVoxelRequest ) ) {
			int saved = 0;
			for (int i = 0; i < Models.Count; i++ ) {
				if ( Models[i].voxel == null ) continue;
				Models[i].voxel.Save ( $"{saveVoxelRequest.Filename}_model{i}" );
				saved++;
			}
			Program.StdOut ( $"Saved {saved} voxel objects." );
		}

		while ( Controler.RenderModeRequests.Reader.TryRead ( out var renderMode ) ) {
			if ( Models.Count == 0 ) {
				Program.StdOut ( "Load a model before changing render mode." );
				return;
			}

			foreach ( var shader in DrawRequests ) shader.IsRepeatable = false;

			switch ( renderMode.Mode ) {
			case RenderModeRequest.RenderMode.Solid:
				foreach ( (var model, var _) in Models ) {
					DrawRequests.Enqueue ( new BasicMeshRenderer ( model ) );
				}
				Program.StdOut ( $"Switched to Solid render mode for {Models.Count} models." );
				break;
			case RenderModeRequest.RenderMode.Voxel:
				int voxelCount = 0;
				foreach ( (var _, var voxel) in Models ) {
					if ( voxel != null ) {
						DrawRequests.Enqueue ( new VoxelVisualizer ( voxel ) );
						voxelCount++;
					}
				}
				Program.StdOut ( $"Switched to Voxel render mode for {voxelCount} models. Control position using the IJKLUO keys and rotation with TFGHRY." );
				break;
			}
		}
	}



	/// <summary> Draw simple plane on near-plane to overlay 2D content </summary>
	public static void Draw2D () {
		GL.BindVertexArray ( quadVAO );
		//GL.BindBuffer ( BufferTarget.ArrayBuffer, quadVBO );
		//GL.BindBuffer ( BufferTarget.ElementArrayBuffer, quadEBO );
		GL.DrawElements ( PrimitiveType.Triangles, quadIndices.Length, DrawElementsType.UnsignedInt, 0 );
	}
}