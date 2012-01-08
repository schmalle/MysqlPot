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
			
//			Mysql x = new Mysql();
//			x.getGreetingPacket(100);
			
		
			
			Console.WriteLine ("Starting mysql pot....");
			Server mysqlServer = new Server(3306);
			mysqlServer.start();
			
			
		}	// main function
		
	}	// MainClass
	

// SALT fehlt


}	// MysqlPot
