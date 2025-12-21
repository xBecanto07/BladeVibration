using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace BladeVibrationCS;
public static class Extensions {
	public static void Move ( this ref Vector3 pos, KeyboardState input, Matrix4 space, float speed, Keys W, Keys S, Keys A, Keys D, Keys Q, Keys E ) {
		Vector3 forward = Vector3.TransformVector ( Vector3.UnitZ, space );
		Vector3 right = Vector3.TransformVector ( Vector3.UnitX, space );
		Vector3 up = Vector3.TransformVector ( Vector3.UnitY, space );
		if ( W != Keys.Unknown && input.IsKeyDown ( W ) ) pos += forward * speed;
		if ( S != Keys.Unknown && input.IsKeyDown ( S ) ) pos -= forward * speed;
		if ( A != Keys.Unknown && input.IsKeyDown ( A ) ) pos -= right * speed;
		if ( D != Keys.Unknown && input.IsKeyDown ( D ) ) pos += right * speed;
		if ( Q != Keys.Unknown && input.IsKeyDown ( Q ) ) pos -= up * speed;
		if ( E != Keys.Unknown && input.IsKeyDown ( E ) ) pos += up * speed;
	}

	//public static void Move ( this Vector2 pos, KeyboardState input, float speed, Keys W, Keys S, Keys A, Keys D ) {
	//	if ( input.IsKeyDown ( W ) ) pos += Vector2.UnitY * speed;
	//	if ( input.IsKeyDown ( S ) ) pos -= Vector2.UnitY * speed;
	//	if ( input.IsKeyDown ( A ) ) pos -= Vector2.UnitX * speed;
	//	if ( input.IsKeyDown ( D ) ) pos += Vector2.UnitX * speed;
	//}
}