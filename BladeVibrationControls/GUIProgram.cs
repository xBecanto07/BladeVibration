using BladeVibrationCS;

namespace BladeVibrationControls;
internal static class GUIProgram {
	static void Main () {
		EntryProgram.Run ( RunGUI );
	}

	[STAThread]
	static void RunGUI ( WindowsHolder window, Controler controler, int X, int Y ) {
		ApplicationConfiguration.Initialize ();
		ViewerForm form = new ( window, controler );
		form.StartPosition = FormStartPosition.Manual;
		form.Location = new Point ( X, Y );
		Application.Run ( form );
	}
}