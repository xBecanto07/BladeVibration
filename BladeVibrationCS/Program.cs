using System;
using System.Collections.Generic;
using System.Threading.Channels;
using System.Threading;
using System.Linq;

namespace BladeVibrationCS; 
public static class Program {
	static Controler controler = new ();
	static Dictionary<string, Action<string[]>> Commands = new () {
		{ "exit", _ => controler.RequestStop () },
		{ "stop", _ => controler.RequestStop () },
		{ "quit", _ => controler.RequestStop () },
		{ "voxelize", args => {
			var request = VoxelizeRequest.Parse ( args );
			controler.VoxelizeRequests.Writer.WriteAsync ( request );
		} },
		{ "loadmodel", args => {
			var request = LoadModelRequest.Parse ( args, controler );
			controler.LoadModelRequests.Writer.WriteAsync ( request );
		} },
		{ "gpu", args => {
			bool request = true;
			if (args.Length > 0 && bool.TryParse ( args[0], out var parsed ) )
				request = parsed;
			if (!request) controler.RunGPURequests.Reader.TryRead ( out _ ); // Clear any pending requests
			else controler.RunGPURequests.Writer.WriteAsync ( request );
		} },
		{ "young", args => {
			if ( args.Length < 2 ) {
				StdOut ( "Usage: young <material_name> <value>" );
				return;
			}
			string materialName = args[0];
			if ( !float.TryParse ( args[1], out var value ) ) {
				StdOut ( $"Invalid value for Young's modulus: {args[1]}" );
				return;
			}
			controler.YoungModuli[materialName] = value;
		} },
		{ "list", args => {
			if (args.Length < 1 ) {
				StdOut ( "Usage: list young" );
				return;
			}
			string dir = args[0] switch {
				"models" => Assets.ModelPath,
				"textures" => Assets.TexturePath,
				_ => throw new ArgumentException ( $"Unknown list category: {args[0]}, supported are: models, textures" )
			};
			StdOut ( $"Listing files in '{dir}':\n{string.Join("\n - ", Assets.ListDirectory(dir))}" );
		} },
		{
			"savevoxel", args => {
				string fileName = args.Length > 0 ? args[0] : "voxels";
				controler.SaveVoxelRequests.Writer.WriteAsync ( new SaveVoxelRequest ( Assets.GetOutputPath ( fileName ) ) );
		} },
		{
			"mode", args => {
				if ( args.Length < 1 ) {
					StdOut ( "Usage: mode <wireframe|solid|tension|voxel>" );
					return;
				}
				string modeStr = args[0].ToLower ();
				if (!Enum.TryParse<RenderModeRequest.RenderMode> ( modeStr, true, out var mode ) ) {
					StdOut ( $"Unknown render mode: {modeStr}, supported are: {string.Join ( ", ", Enum.GetNames ( typeof ( RenderModeRequest.RenderMode ) ) ) }" );
					return;
				}
				controler.RenderModeRequests.Writer.WriteAsync ( new RenderModeRequest ( mode ) );
			}
		},
	};

	static void Main ( string[] args ) {
		Thread consoleThread = new ( RunConsole );
		consoleThread.Start ();
		while ( consoleThread.IsAlive ) {
			controler.RunGPURequests.Reader.TryRead ( out bool startGPU );
			if ( startGPU ) RunWindow (); // Blocking call
			else Thread.Sleep ( 250 );
		}
	}

   static void RunWindow () {
		using ( var window = new WindowsHolder ( controler, 1600, 900 ) ) {
			window.Run ();
		}
	}
	static void RunConsole () {
		Commands["gpu"] ( [] );
		Commands["loadmodel"] ( ["test"] );
		Commands["mode"] ( ["solid"] );
		//Commands["voxelize"] ( [] );

		StdOut ( "Available commands:" );
		// Split the list into two columns
		var cmdList = Commands.Keys.ToList ();
		int longest = cmdList.Max ( k => k.Length ) + 2;
		for ( int i = 0; i < cmdList.Count; i += 2 ) {
			string first = cmdList.ElementAt ( i );
			string second = ( i + 1 ) < cmdList.Count ? cmdList.ElementAt ( i + 1 ) : "";
			StdOut ( $" - {first.PadRight ( longest )}| {second}" );
		}
		//foreach ( var cmd in Commands.Keys )
		//	StdOut ( $" - {cmd}" );

		while ( !controler.StopRequested ) {
			/*
			Normally, 'StopRequested' would need to be marked as volatile as someone might set it from another thread.
				Without volatile (locks, barriers, etc.), it could read wrong old value and freeze on the ReadLine.
			But here, RequestStop should only be called from inside of this loop so the 'StopRequested' read is always the latest.
			*/
			Console.Write ( "> " );
			string input = Console.ReadLine ();
			if ( string.IsNullOrEmpty ( input ) ) continue;

			int separatorIndex = input.IndexOf ( ' ' );
			string cmd = separatorIndex == -1 ? input : input.Substring ( 0, separatorIndex );
			string[] cmdArgs = separatorIndex == -1 ? Array.Empty<string> () : input.Substring ( separatorIndex + 1 ).Split ( ' ' );

			if ( !Commands.TryGetValue ( cmd, out var action ) ) {
				StdOut ( $"Unknown command: {cmd}" );
				continue;
			}
			try {
				action ( cmdArgs );
			} catch ( Exception ex ) {
				StdOut ( $"Error executing command '{cmd}': {ex.Message}" );
			}
		}
	}


	private static string lastMessage = null;
	public static void StdOut ( string message ) {
		if ( message == lastMessage ) return;
		lastMessage = message;
		Console.WriteLine ( message );
	}
}

public class Controler {
	public bool StopRequested { get; private set; } = false;

	public void RequestStop () => StopRequested = true;
	public readonly Dictionary<string, float> YoungModuli = new () {
		{ "Air", ModelHolder.YM_Air },
		{ "Wood", ModelHolder.YM_Wood},
		{ "Bronze", ModelHolder.YM_Bronze},
		{ "Steel", ModelHolder.YM_Steel},
	};

	public Channel<bool> RunGPURequests;
	public Channel<VoxelizeRequest> VoxelizeRequests;
	public Channel<LoadModelRequest> LoadModelRequests;
	public Channel<SaveVoxelRequest> SaveVoxelRequests;
	public Channel<RenderModeRequest> RenderModeRequests;

	public Controler () {
		RunGPURequests = Channel.CreateBounded<bool> ( 1 );
		VoxelizeRequests = Channel.CreateBounded<VoxelizeRequest> ( 1 );
		LoadModelRequests = Channel.CreateBounded<LoadModelRequest> ( 1 );
		SaveVoxelRequests = Channel.CreateBounded<SaveVoxelRequest> ( 1 );
		RenderModeRequests = Channel.CreateBounded<RenderModeRequest> ( 1 );
	}

	public void Dispose () {
		RunGPURequests.Writer.Complete ();
		VoxelizeRequests.Writer.Complete ();
		LoadModelRequests.Writer.Complete ();
		SaveVoxelRequests.Writer.Complete ();
		RenderModeRequests.Writer.Complete ();
	}
}
public class VoxelizeRequest {
	public static VoxelizeRequest Parse ( string[] args) {
		return new VoxelizeRequest ();
	}
}
public class LoadModelRequest {
	public string ModelPath;
	public readonly Dictionary<string, float> YoungModuli;

	public LoadModelRequest (string modelPath, Controler owner ) {
		ModelPath = modelPath;
		YoungModuli = owner.YoungModuli;
	}

	public static LoadModelRequest Parse (string[] args, Controler owner) {
		string modelPath = args.Length > 0 ? args[0] : throw new ArgumentException ( "Model path is required" );
		if ( modelPath == "test" ) modelPath = "2HSword0.FBX";
		modelPath = Assets.GetModelPath ( modelPath );
		if ( !System.IO.File.Exists ( modelPath ) )
			throw new ArgumentException ( $"Model path '{modelPath}' does not exist" );
		return new LoadModelRequest ( modelPath, owner );
	}
}
public class SaveVoxelRequest {
	public readonly string Filename;

	public SaveVoxelRequest ( string filename ) { Filename = filename; }
}
public class RenderModeRequest {
	public enum RenderMode { Wireframe, Solid, Voxel, Tension, Voxelizer, Slicer }
	public readonly RenderMode Mode;

	public RenderModeRequest ( RenderMode mode ) { Mode = mode; }
}