using BladeVibrationCS;
using BladeVibrationCS.GpuPrograms;
using OpenTK.Mathematics;

namespace BladeVibrationControls;
public static class Parsers {
	const string FloatFormat = "F4";
	public static Vector2i ParseLoc ( this Vector2i vec, TextBox X, TextBox Y ) => new Vector2i (
			int.Parse ( X.Text ),
			int.Parse ( Y.Text )
		);
	public static void Parse (this Action<Vector2i> setter, TextBox x, TextBox y) => setter ( new Vector2i (
			int.Parse ( x.Text ),
			int.Parse ( y.Text )
		) );
	public static void Parse ( this ref Vector2 vec, TextBox x, TextBox y ) => vec = new Vector2 (
			float.Parse ( x.Text ),
			float.Parse ( y.Text )
		);
	public static void Parse ( this ref Vector3 vec, TextBox x, TextBox y, TextBox z ) => vec = new Vector3 (
			float.Parse ( x.Text ),
			float.Parse ( y.Text ),
			float.Parse ( z.Text )
		);
	public static void Parse ( this ref bool val, CheckBox box ) => val = box.Checked;
	public static void Parse ( this ref float val, TextBox box ) => val = float.Parse ( box.Text );

	public static void Parse (this MaterialHolder materials, MaterialInfo[] materialInfos) {
		for (int i = 0; i < MaterialHolder.BUFFER_SIZE; i++) {
			var matInfo = materialInfos[i];
			float shininess = float.Parse(matInfo.Shininess.Text);
			float diffuseStrength = float.Parse(matInfo.Diffuse.Text);
			float stiffness = ModelHolder.YM_Steel; // Default stiffness
			int renderMode = matInfo.RenderMode.SelectedIndex switch {
				0 => AShaderProgram.RENDER_MODE_SIMPLE,
				1 => AShaderProgram.RENDER_MODE_HARD,
				2 => AShaderProgram.RENDER_MODE_PHONG,
				3 => AShaderProgram.RENDER_MODE_OBJECTS,
				_ => AShaderProgram.RENDER_MODE_OBJECTS
			};
			materials[i] = (shininess, diffuseStrength, stiffness, renderMode);
		}
	}

	public static void Select<T> ( this ref T val, params RadioButton[] buttons ) where T : struct {
		foreach ( var btn in buttons ) {
			if ( btn.Checked ) {
				val = Enum.Parse<T> ( btn.Text );
				return;
			}
		}
	}

	public static void Fill ( this Vector2i vec, TextBox X, TextBox Y ) {
		X.Text = vec.X.ToString ();
		Y.Text = vec.Y.ToString ();
	}
	public static void Fill ( this Vector2 vec, TextBox X, TextBox Y ) {
		X.Text = vec.X.ToString ( FloatFormat );
		Y.Text = vec.Y.ToString ( FloatFormat );
	}
	public static void Fill ( this Vector3 vec, TextBox X, TextBox Y, TextBox Z ) {
		X.Text = vec.X.ToString ( FloatFormat );
		Y.Text = vec.Y.ToString ( FloatFormat );
		Z.Text = vec.Z.ToString ( FloatFormat );
	}
	public static void Fill ( this bool val, CheckBox box ) {
		box.Checked = val;
	}
	public static void Fill ( this float val, TextBox box ) {
		box.Text = val.ToString ( FloatFormat );
	}
	public static void Fill (this MaterialHolder materials, MaterialInfo[] materialInfos) {
		for (int i = 0; i < MaterialHolder.BUFFER_SIZE; i++) {
			var (shininess, diffuseStrength, stiffness, renderMode) = materials[i];
			materialInfos[i].Shininess.Text = shininess.ToString(FloatFormat);
			materialInfos[i].Diffuse.Text = diffuseStrength.ToString(FloatFormat);
			materialInfos[i].RenderMode.SelectedIndex = renderMode switch {
				AShaderProgram.RENDER_MODE_SIMPLE => 0,
				AShaderProgram.RENDER_MODE_HARD => 1,
				AShaderProgram.RENDER_MODE_PHONG => 2,
				AShaderProgram.RENDER_MODE_OBJECTS => 3,
				_ => 3
			};
		}
	}
}