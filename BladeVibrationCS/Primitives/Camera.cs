using System;
using OpenTK.Mathematics;

namespace BladeVibrationCS.Primitives; 
public class Camera {
	const float PitchLimit = MathHelper.PiOver2 - 0.01f;
	public Vector3 Position = Vector3.Zero;
	public Vector3 Direction = -Vector3.UnitZ;
	/// <summary>X - Left/Right | Y - Up/Down</summary>
	public Vector2 Rotation = Vector2.Zero;

	public Matrix4 View => Matrix4.LookAt ( Position, Position + Direction, Vector3.UnitY );
	public Matrix4 CamSpace { get; private set; } = Matrix4.Identity;

	public Camera (Vector3 initPos, Vector2 initRot) {
		Position = initPos;
		Rotation = initRot;
		ApplyAngle ();
	}

	public void ApplyAngle () {
		Matrix4 yawLeftRightRotation = Matrix4.CreateRotationY ( Rotation.X );
		Matrix4 pitchUpDownRotation = Matrix4.CreateRotationX ( Rotation.Y );
		CamSpace = pitchUpDownRotation * yawLeftRightRotation;
		Direction = Vector3.TransformVector ( Vector3.UnitZ, CamSpace );
	}
	public void ApplyDirection () {
		Rotation.Y = MathF.Asin ( -Direction.Y );
		Rotation.X = Rotation.Y > PitchLimit || Rotation.Y < -PitchLimit ? 0 : MathF.Atan2 ( Direction.X, -Direction.Z );
	}

	public void ApplyRotationDelta (Vector2 delta ) {
		Rotation.Y += delta.Y * 0.01f;
		Rotation.X -= delta.X * 0.01f;
		Rotation.Y = MathHelper.Clamp ( Rotation.Y, -PitchLimit, PitchLimit );
		ApplyAngle ();
	}
}