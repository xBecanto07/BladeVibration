using System;
using System.Collections.Generic;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Mathematics;
using BladeVibrationCS.GpuPrograms;

namespace BladeVibrationCS; 
public class WindowsHolder : GameWindow {
	AShaderProgram Program = null;
	ModelHolder Model = null;
	VoxelObject VoxelObject = null;
	Vector3 ModelOffset = Vector3.Zero;
	Vector3 CameraPosition = new ( 0.14f, 1.5f, 1.3f );
	/// <summary>X - Left/Right | Y - Up/Down</summary>
	Vector2 CameraRotation = new ( -0.45f, -0.8f );
	Vector3 VoxelSlicePosition = Vector3.Zero;
	Quaternion VoxelSliceRotation = Quaternion.Identity;
	Matrix4 viewMatrix = Matrix4.Identity;
	Matrix4 projectionMatrix = Matrix4.Identity;
	int VoxelDisplayLayer = 0;
	//RenderModeRequest.RenderMode CurrentRenderMode = RenderModeRequest.RenderMode.Solid;
	float scale = 1f;
	readonly Controler controler;
	readonly RenderController renderController;

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
				basicMeshRenderer.Render ( ModelOffset, scale, viewMatrix, projectionMatrix );
				break;
			case VoxelVisualizer voxelVisualizer:
				voxelVisualizer.Render ( VoxelSlicePosition, VoxelSliceRotation, viewMatrix, projectionMatrix );
				break;
			case VoxelObject voxelObject:
				voxelObject.Voxelize ();
				// As voxelObject draws into texture, let's clear with yellow to indicate voxelization step
				GL.ClearColor ( 0.5f, 0.5f, 0f, 1.0f );
				GL.Clear ( ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit );
				break;
			}
		}

		SwapBuffers ();
		renderController.FinishDrawCycle ();

		System.Threading.Thread.Sleep ( 1000 / 60 );
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

		ModelOffset.Move (input, Matrix4.Identity, (float)(speed * args.Time), Keys.Left, Keys.Right, Keys.Up, Keys.Down, Keys.Unknown, Keys.Unknown);
		if ( input.IsKeyDown ( Keys.Space ) ) ModelOffset = Vector3.Zero;

		if ( mouse.IsButtonDown ( MouseButton.Left ) ) {
			CameraRotation.Y -= mouse.Delta.Y * 0.01f;
			CameraRotation.X -= mouse.Delta.X * 0.01f;
			CameraRotation.Y = MathHelper.Clamp ( CameraRotation.Y, -MathHelper.PiOver2 + 0.01f, MathHelper.PiOver2 - 0.01f );
		}
		Matrix4 yawLeftRightRotation = Matrix4.CreateRotationY ( CameraRotation.X );
		Matrix4 pitchUpDownRotation = Matrix4.CreateRotationX ( CameraRotation.Y );
		Matrix4 cameraSpace = pitchUpDownRotation * yawLeftRightRotation;
		//CameraPosition.Move (input, cameraSpace, (float)(speed * args.Time), Keys.W, Keys.S, Keys.A, Keys.D, Keys.Q, Keys.E);
		if (mouse.IsButtonDown(MouseButton.Right)) {
			CameraPosition += Vector3.TransformVector ( Vector3.UnitX, cameraSpace ) * mouse.Delta.X * 0.01f;
			CameraPosition -= Vector3.TransformVector ( Vector3.UnitY, cameraSpace ) * mouse.Delta.Y * 0.01f;
		}
		Vector3 forward = Vector3.TransformVector ( Vector3.UnitZ, cameraSpace );
		CameraPosition -= forward * mouse.ScrollDelta.Y * 0.1f;
		if ( input.IsKeyDown ( Keys.Tab ) ) {
			CameraPosition = Vector3.UnitX;
			CameraRotation = Vector2.Zero;
		}
		viewMatrix = Matrix4.LookAt (CameraPosition, CameraPosition - forward, Vector3.UnitY);

		VoxelSlicePosition.Move (input, Matrix4.Identity, (float)(speed * args.Time), Keys.I, Keys.K, Keys.J, Keys.L, Keys.U, Keys.O);

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