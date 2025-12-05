using System;
namespace BladeVibrationCS; 
public class Program {
   static void Main ( string[] args ) {
      using ( var window = new WindowsHolder ( 1600, 900 ) ) {
         window.Run ();
		}


		//Console.WriteLine ( "Press any key to exit..." );
      //Console.ReadKey ( true );
	}
}