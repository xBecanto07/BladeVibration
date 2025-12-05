using System;
using System.IO;

namespace BladeVibrationCS; 
public static class Assets {
	private const string AssetFolderName = "Assets";
	private static readonly string AssetPath, ShaderSourcePath;

	public static string GetShaderPath ( string shaderFileName ) => Path.Combine( ShaderSourcePath, shaderFileName );

	static Assets () {
		AssetPath = GetAssetPath();
		ShaderSourcePath = Path.Combine( AssetPath, "Shaders" );

		if ( !Directory.Exists( AssetPath ) )
			throw new DirectoryNotFoundException( $"Could not find asset path: {AssetPath}" );
		if ( !Directory.Exists( ShaderSourcePath ) )
			throw new DirectoryNotFoundException( $"Could not find shader source path: {ShaderSourcePath}" );
	}

	private static string GetAssetPath () {
		DirectoryInfo exePathDir = new ( AppContext.BaseDirectory );
		DirectoryInfo[] subDirs = exePathDir.GetDirectories ( AssetFolderName );
		if ( subDirs.Length > 0 ) return subDirs[0].FullName;

		var potentialDebug = GetParent ( exePathDir );
		var potentialBin = GetParent ( potentialDebug );
		if ( potentialBin.Name == "bin" && ( potentialDebug.Name == "Debug" || potentialDebug.Name == "Release" ) ) {
			var projectDir = GetParent ( potentialBin );
			subDirs = projectDir.GetDirectories ( AssetFolderName );
			if ( subDirs.Length > 0 ) return subDirs[0].FullName;
		}

		throw new DirectoryNotFoundException ( $"Could not find asset path: {AssetFolderName}" );
	}

	private static DirectoryInfo GetParent (DirectoryInfo dir) {
		if ( dir.Parent == null )
			throw new DirectoryNotFoundException ( $"Could not find asset path: {AssetFolderName}" );
		return dir.Parent;
	}
}