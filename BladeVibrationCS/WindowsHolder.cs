using System;
using System.Collections.Generic;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Mathematics;
using BladeVibrationCS.GpuPrograms;
using BladeVibrationCS.Primitives;

namespace BladeVibrationCS; 
public class WindowsHolder : GameWindow {
	AShaderProgram Program = null;
	ModelHolder Model = null;
	VoxelObject VoxelObject = null;
	public Vector3 ModelOffset = Vector3.Zero;
	//public Vector3 CameraPosition = new ( 0.14f, 1.5f, 1.3f );
	/// <summary>X - Left/Right | Y - Up/Down</summary>
	//public Vector2 CameraRotation = new ( -0.45f, -0.8f );
	Vector3 VoxelSlicePosition = Vector3.Zero;
	Quaternion VoxelSliceRotation = Quaternion.Identity;
	//Matrix4 viewMatrix = Matrix4.Identity;
	Matrix4 projectionMatrix = Matrix4.Identity;
	int VoxelDisplayLayer = 0;
	//RenderModeRequest.RenderMode CurrentRenderMode = RenderModeRequest.RenderMode.Solid;
	public float scale = 0.03f;
	public readonly Controler controler;
	public readonly RenderController renderController;
	public readonly Camera Camera = new ( new ( -0.5f, 0.6f, -0.6f ), new ( 0.9f, 0.5f ) );
	double lastTime = 0.0;

	public const string TITLE = "Blade Vibration C#";
	public WindowsHolder ( Controler control, int width, int height ) : base ( GameWindowSettings.Default, new NativeWindowSettings () { ClientSize = (width, height), Title = TITLE } ) {
		controler = control;
		renderController = new ( controler, new Vector2i ( width, height ) );
	}

	protected override void OnLoad () {
		base.OnLoad ();
		projectionMatrix = Matrix4.CreatePerspectiveFieldOfView ( MathHelper.DegreesToRadians ( 60f ), (float)Size.X / Size.Y, 0.01f, 1000f );

		//GL.Enable ( EnableCap.DepthTest );

		//Model.PushToGPU ();
		Program?.Use ();
		GL.ClearColor ( 0.5f, 0.5f, 0.5f, 1.0f ); // Default, no program used, gray background
	}

	public override void Close () {
		base.Close ();
		Program?.Dispose ();
		Program = null;
		Model?.Dispose ();
		Model = null;
	}

	protected override void OnRenderFrame ( FrameEventArgs args ) {
		base.OnRenderFrame ( args );
		// Clear the screen
		GL.Clear ( ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit );

		// Render the model
		AShaderProgram program;
		while ( (program = renderController.NextDrawRequest) != null ) {
			switch ( program ) {
			case BasicMeshRenderer basicMeshRenderer:
				basicMeshRenderer.Render ( ModelOffset, scale, Camera.View, projectionMatrix, Camera.Position );
				break;
			case VoxelVisualizer voxelVisualizer:
				voxelVisualizer.Render ( VoxelSlicePosition, VoxelSliceRotation, Camera.View, projectionMatrix );
				break;
			case VoxelObject voxelObject:
				voxelObject.Voxelize ();
				// As voxelObject draws into texture, let's clear with yellow to indicate voxelization step
				//GL.ClearColor ( 0.5f, 0.5f, 0f, 1.0f );
				//GL.Clear ( ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit );
				break;
			case PhysicsSimulator physicsSimulator:
				physicsSimulator.Render ( ModelOffset, scale, Camera.View, projectionMatrix, Camera.Position );
				break;
			default:
				EntryProgram.StdOut ( $"Unknown program type: {program.GetType ().Name}" ); break;
			}
		}

		SwapBuffers ();
		renderController.FinishDrawCycle ();

		double nowTime = GLFW.GetTime ();
		double deltaTime = nowTime - lastTime;
		lastTime = nowTime;
		double waitTime = Math.Max ( 0.0, (1.0 / 60.0) - deltaTime );
		System.Threading.Thread.Sleep ( TimeSpan.FromSeconds ( waitTime ) );
	}


	protected override void OnUpdateFrame ( FrameEventArgs args ) {
		base.OnUpdateFrame ( args );

		renderController.Process ();

		var input = KeyboardState;
		var mouse = MouseState;
		if ( input.IsKeyDown ( Keys.Escape ) || controler.StopRequested )
			Close ();

		float speed = 0.1f;
		if ( input.IsKeyDown ( Keys.LeftShift ) ) speed = 0.5f;
		if ( input.IsKeyDown ( Keys.RightShift ) ) speed = 0.9f;
		if ( input.IsKeyDown ( Keys.LeftControl ) ) speed = 0.02f;
		if ( input.IsKeyDown ( Keys.RightControl ) ) speed = 0.01f;

		ModelOffset.Move (input, Matrix4.Identity, (float)(speed * args.Time), Keys.W, Keys.S, Keys.A, Keys.D, Keys.Q, Keys.E);
		if ( input.IsKeyDown ( Keys.Space ) ) ModelOffset = Vector3.Zero;

		if ( mouse.IsButtonDown ( MouseButton.Left ) )
			Camera.ApplyRotationDelta ( mouse.Delta );

		if (mouse.IsButtonDown(MouseButton.Right)) {
			Camera.Position -= Vector3.TransformVector ( Vector3.UnitX, Camera.CamSpace ) * mouse.Delta.X * 0.01f;
			Camera.Position -= Vector3.TransformVector ( Vector3.UnitY, Camera.CamSpace ) * mouse.Delta.Y * 0.01f;
		}
		Vector3 forward = Vector3.TransformVector ( Vector3.UnitZ, Camera.CamSpace );
		Camera.Position += forward * mouse.ScrollDelta.Y * 0.1f;
		if ( input.IsKeyDown ( Keys.Tab ) ) {
			Camera.Position = Vector3.UnitX;
			Camera.Rotation = Vector2.Zero;
		}
		//viewMatrix = Matrix4.LookAt (Camera.Position, Camera.Position - forward, Vector3.UnitY);

		VoxelSlicePosition.Move (input, Matrix4.Identity, (float)(20 * speed * args.Time), Keys.I, Keys.K, Keys.J, Keys.L, Keys.U, Keys.O);

		Vector3 sliceRot = Vector3.Zero;
		sliceRot.Move (input, Matrix4.Identity, (float)(speed * args.Time), Keys.T, Keys.G, Keys.F, Keys.H, Keys.R, Keys.Y);
		Quaternion deltaRot = Quaternion.FromEulerAngles ( sliceRot.X, sliceRot.Y, sliceRot.Z );
		VoxelSliceRotation = deltaRot * VoxelSliceRotation;

		if ( input.IsKeyDown ( Keys.Z ) ) scale *= 1.0f + speed * (float)args.Time;
		if ( input.IsKeyDown ( Keys.X ) ) scale *= 1.0f - speed * (float)args.Time;
		if ( input.IsKeyDown ( Keys.C ) ) scale = 1.0f;
	}

	protected override void OnFramebufferResize ( FramebufferResizeEventArgs e ) {
		base.OnFramebufferResize ( e );
		GL.Viewport ( 0, 0, e.Width, e.Height );
	}
}