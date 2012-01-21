using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

/*
 * 
 *  MySQL Honeypot by Markus "Flake" Schmall
 * 
 */
namespace MysqlPot
{
	class MainClass
	{
		
		/* define states for the server context */
		enum State
		{
			NONE = 1,
			PORT,
			FILENAME
		};
		
		
		/**
		 * 
		 * 
		 */
		public static void Main (string[] args)
		{
			
			
			
			//
			// parse commandline
			//
			// -p port -f file
			//
			foreach( string s in args )
			{
				String parameter = s;
				Console.WriteLine(s);
			}
			
			
			
			
			Console.WriteLine ("Starting mysql pot....");
			Server mysqlServer = new Server(3306, "/Users/flake/mysqlpot.log");
			mysqlServer.start();
			
			
		}	// main function
		
	}	// MainClass
	

// SALT fehlt


}	// MysqlPot
