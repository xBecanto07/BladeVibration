using System;
using System.Collections.Generic;
using OpenTK.Graphics.OpenGL;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Mathematics;

namespace BladeVibrationCS; 
public class WindowsHolder : GameWindow {
	const float BACKGROUND_MULTIPLIER = 0.5f;
	const float BACKGROUND_R = (123 / 255f) * BACKGROUND_MULTIPLIER;
	const float BACKGROUND_G = (27 / 255f) * BACKGROUND_MULTIPLIER;
	const float BACKGROUND_B = (56 / 255f) * BACKGROUND_MULTIPLIER;
	ModelHolder Model;
	ShaderProgram ProgramGPU;
	Vector3 ModelOffset;
	float scale = 1f;

	public const string TITLE = "Blade Vibration C#";
	public WindowsHolder ( int width, int height ) : base ( GameWindowSettings.Default, new NativeWindowSettings () { ClientSize = (width, height), Title = TITLE } ) {
		Model = new ModelHolder ( "C:\\Maldus\\School\\Ing3Z\\PGR\\Testing\\2HSword0.FBX", new Dictionary<string, float> {
			{ "Blade_Wood", 9.5e9f },
			{ "Blade_Bronze", 112e9f },
			{ "Blade_Steel", 200e9f },
		} );
		ProgramGPU = new ShaderProgram ( ( "Vertex.glsl", ShaderType.VertexShader ), ( "Fragment.glsl", ShaderType.FragmentShader ) );
	}

	protected override void OnLoad () {
		base.OnLoad ();

		GL.ClearColor ( BACKGROUND_R, BACKGROUND_G, BACKGROUND_B, 1.0f );
		//GL.Enable ( EnableCap.DepthTest );

		Model.PushToGPU ();
		ProgramGPU.Use ();
	}

	public override void Close () {
		base.Close ();
		ProgramGPU.Dispose ();
	}

	protected override void OnRenderFrame ( FrameEventArgs args ) {
		base.OnRenderFrame ( args );
		// Clear the screen
		GL.Clear ( ClearBufferMask.ColorBufferBit );

		// Render the model
		ProgramGPU.Use ();
		ProgramGPU.SetUniform ( "colorA", 1f, 0f, 0f );
		ProgramGPU.SetUniform ( "colorB", 0f, 0f, 1f );
		ProgramGPU.SetUniform ( "offset", ModelOffset.X, ModelOffset.Y, ModelOffset.Z );
		ProgramGPU.SetUniform ( "scale", scale );
		ProgramGPU.SetUniform ( "time", (float)(DateTime.Now.TimeOfDay.TotalMilliseconds / 1000) );

		GL.BindVertexArray ( Model.VAO );
		GL.DrawArrays ( PrimitiveType.Triangles, 0, Model.VertexCount );

		SwapBuffers ();

		System.Threading.Thread.Sleep ( 1000 / 60 );
	}


	protected override void OnUpdateFrame ( FrameEventArgs args ) {
		base.OnUpdateFrame ( args );
		var input = KeyboardState;
		if ( input.IsKeyDown ( Keys.Escape ) )
			Close ();

		float speed = 0.1f;
		if ( input.IsKeyDown ( Keys.LeftShift ) ) speed = 0.5f;
		if ( input.IsKeyDown ( Keys.RightShift ) ) speed = 0.9f;
		if ( input.IsKeyDown ( Keys.LeftControl ) ) speed = 0.02f;
		if ( input.IsKeyDown ( Keys.RightControl ) ) speed = 0.01f;

		if ( input.IsKeyDown ( Keys.A ) ) ModelOffset.X -= speed * (float)args.Time;
		if ( input.IsKeyDown ( Keys.D ) ) ModelOffset.X += speed * (float)args.Time;
		if ( input.IsKeyDown ( Keys.W ) ) ModelOffset.Y += speed * (float)args.Time;
		if ( input.IsKeyDown ( Keys.S ) ) ModelOffset.Y -= speed * (float)args.Time;
		//if ( input.IsKeyDown ( Keys.Q ) ) ModelOffset.Z += speed * (float)args.Time;
		//if ( input.IsKeyDown ( Keys.E ) ) ModelOffset.Z -= speed * (float)args.Time;
		if ( input.IsKeyDown ( Keys.Space ) ) ModelOffset = Vector3.Zero;

		if ( input.IsKeyDown ( Keys.Z ) ) scale *= 1.0f + speed * (float)args.Time;
		if ( input.IsKeyDown ( Keys.X ) ) scale *= 1.0f - speed * (float)args.Time;
		if ( input.IsKeyDown ( Keys.C ) ) scale = 1.0f;
	}

	protected override void OnFramebufferResize ( FramebufferResizeEventArgs e ) {
		base.OnFramebufferResize ( e );
		GL.Viewport ( 0, 0, e.Width, e.Height );
	}
}
