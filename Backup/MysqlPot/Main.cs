using System;

/*
 * 
 *  MySQL Honeypot by Markus "Flake" Schmall
 * 
 */
namespace MysqlPot
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			Console.WriteLine ("Starting mysql pot....");
			Server mysql = new Server(63306);
			mysql.start();
			
		}	// main function
		
	}	// MainClass
	
}	// MysqlPot
