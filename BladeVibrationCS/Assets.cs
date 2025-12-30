using System;
using System.IO;
using System.Linq;

namespace BladeVibrationCS; 
public static class Assets {
	private const string AssetFolderName = "Assets";
	public static readonly string AssetPath, ShaderSourcePath, ModelPath, TexturePath, OutputPath;

	public static string GetShaderPath ( string shaderFileName ) => Path.Combine( ShaderSourcePath, shaderFileName );
	public static string GetModelPath ( string modelFileName ) => Path.Combine( ModelPath, modelFileName );
	public static string GetTexturePath ( string textureFileName ) => Path.Combine( TexturePath, textureFileName );
	public static string GetOutputPath ( string outputFileName ) => Path.Combine( OutputPath, outputFileName );

	public static string GetRelativePath ( string relativePath ) {
		if ( File.Exists ( relativePath ) ) return relativePath; // Already an absolute path
		if ( Directory.Exists ( relativePath ) ) return relativePath;
		return Path.Combine( AssetPath, relativePath );
	}

	static Assets () {
		AssetPath = GetAssetPath();
		ShaderSourcePath = Path.Combine( AssetPath, "Shaders" );
		ModelPath = Path.Combine( AssetPath, "Models" );
		TexturePath = Path.Combine( AssetPath, "Textures" );
		OutputPath = Path.Combine( AssetPath, "Output" );

		if ( !Directory.Exists( AssetPath ) )
			throw new DirectoryNotFoundException( $"Could not find asset path: {AssetPath}" );
		if ( !Directory.Exists( ShaderSourcePath ) )
			throw new DirectoryNotFoundException( $"Could not find shader source path: {ShaderSourcePath}" );
		if ( !Directory.Exists( ModelPath ) )
			throw new DirectoryNotFoundException( $"Could not find model path: {ModelPath}" );
		if ( !Directory.Exists( TexturePath ) )
			throw new DirectoryNotFoundException( $"Could not find texture path: {TexturePath}" );
		if ( !Directory.Exists( OutputPath ) ) 
			Directory.CreateDirectory( OutputPath );
	}

	private static string GetAssetPath () {
		DirectoryInfo exePathDir = new ( AppContext.BaseDirectory );
		DirectoryInfo[] subDirs = exePathDir.GetDirectories ( AssetFolderName );
		if ( subDirs.Length > 0 ) return subDirs[0].FullName;


		var potentialDebug = GetParent ( exePathDir );
		var potentialBin = GetParent ( potentialDebug );
		var potentialMainProj = GetParent ( potentialBin );
		if ( potentialBin?.Name == "bin"
			&& ( potentialDebug?.Name == "Debug" || potentialDebug?.Name == "Release" ) ) {
			if ( potentialMainProj?.Name != nameof ( BladeVibrationCS ) ) {
				// Started from another project
				var solutionDir = GetParent ( potentialMainProj );
				potentialMainProj = solutionDir.GetDirectories ( nameof ( BladeVibrationCS ) ).FirstOrDefault ();
			}

			//var projectDir = GetParent ( potentialBin );
			subDirs = potentialMainProj?.GetDirectories ( AssetFolderName );
			if ( subDirs?.Length > 0 )
				return subDirs[0].FullName;
		}

		throw new DirectoryNotFoundException ( $"Could not find asset path: {AssetFolderName}" );
	}

	private static DirectoryInfo GetParent (DirectoryInfo dir) {
		if ( dir.Parent == null )
			throw new DirectoryNotFoundException ( $"Could not find asset path: {AssetFolderName}" );
		return dir.Parent;
	}

	public static string[] ListDirectory (string path) {
		if ( !Directory.Exists ( path ) )
			throw new DirectoryNotFoundException ( $"Directory not found: {path}" );
		return [.. Directory.GetFileSystemEntries ( path ).Select ( s => s.Remove ( 0, path.Length + 1 ) )];
	}
}