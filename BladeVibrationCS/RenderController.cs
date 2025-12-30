using BladeVibrationCS.GpuPrograms;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;

namespace BladeVibrationCS; 
public class RenderController {
	const float QuadSide = 0.3f;
	const float QuadBase = -QuadSide / 2;
	const float QuadTop = QuadBase + QuadSide;

	public readonly static Primitives.Star StarPrimitive = new ();
	public readonly static Primitives.Plane PlanePrimitive = new ();
	public readonly static Primitives.Cross3 Cross3DPrimitive = new ();
	public readonly static Primitives.SkyBox SkyBoxPrimitive = new ();

	readonly Controler Controler;
	private readonly Queue<AShaderProgram> DrawRequests = new ();
	private readonly List<(ModelHolder model, VoxelObject voxel)> Models = [];

	public readonly Vector2i ScreenSize;

	private AShaderProgram lastProgram = null;
	private readonly Queue<AShaderProgram> nextDrawQueue = new ();
	private readonly HashSet<AShaderProgram> programScrapYard = new ();

	public AShaderProgram LastRepeatableProgram { get; private set; } = null;
	public AShaderProgram NextDrawRequest {
		get {
			if ( DrawRequests.Count == 0 ) return null;
			AShaderProgram program = DrawRequests.Dequeue ();
			if ( program.IsRepeatable ) {
				nextDrawQueue.Enqueue ( program );
				LastRepeatableProgram = program;
			} else
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
			EntryProgram.StdOut ( "Model loaded successfully." );
		}

		while ( Controler.VoxelizeRequests.Reader.TryRead ( out var voxelizeRequest ) ) {
			int requests = 0;
			for (int i = 0; i < Models.Count; i++ ) {
				if ( Models[i].voxel != null ) continue;
				VoxelObject vo = new ( Models[i].model, (Models[i].model.MaxX - Models[i].model.MinX) / VoxelObject.MAX_VOXELS_PER_AXIS );
				Models[i] = (Models[i].model, vo);
				if ( vo.IsRepeatable ) {
					foreach ( var shader in DrawRequests )
						shader.IsRepeatable = false;
				}
				DrawRequests.Enqueue ( vo );
				requests++;
			}
			EntryProgram.StdOut ( $"Voxelization request ({requests}) pushed to render queue" );
		}

		while ( Controler.SaveVoxelRequests.Reader.TryRead ( out var saveVoxelRequest ) ) {
			int saved = 0;
			for (int i = 0; i < Models.Count; i++ ) {
				if ( Models[i].voxel == null ) continue;
				Models[i].voxel.Save ( $"{saveVoxelRequest.Filename}_model{i}" );
				saved++;
			}
			EntryProgram.StdOut ( $"Saved {saved} voxel objects." );
		}

		while ( Controler.RenderModeRequests.Reader.TryRead ( out var renderMode ) ) {
			if ( Models.Count == 0 ) {
				EntryProgram.StdOut ( "Load a model before changing render mode." );
				return;
			}

			foreach ( var shader in DrawRequests ) shader.IsRepeatable = false;

			switch ( renderMode.Mode ) {
			case RenderModeRequest.RenderMode.Solid:
				foreach ( (var model, var _) in Models ) {
					DrawRequests.Enqueue ( new BasicMeshRenderer ( model ) );
				}
				EntryProgram.StdOut ( $"Switched to Solid render mode for {Models.Count} models." );
				break;
			case RenderModeRequest.RenderMode.Voxel:
				int voxelCount = 0;
				foreach ( (var _, var voxel) in Models ) {
					if ( voxel != null ) {
						DrawRequests.Enqueue ( new VoxelVisualizer ( voxel ) );
						voxelCount++;
					}
				}
				EntryProgram.StdOut ( $"Switched to Voxel render mode for {voxelCount} models. Control position using the IJKLUO keys and rotation with TFGHRY." );
				break;
			}
		}
	}
}