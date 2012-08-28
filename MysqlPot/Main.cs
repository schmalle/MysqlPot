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

			int port = 3306;
			String logFile = "/Users/flake/mysqlpot.log";
			String ip = null;


			// lame parsing of command line parameters
			if (args.Length >= 1)
			{
				port = Convert.ToInt32(args[0]);	
			
				if (args.Length >= 2)
				{
					logFile = args[1];	

					if (args.Length >= 3)
					{
						ip = args[2];	
					}				
				
				}						
			
			}


			//
			// parse commandline
			//
			// programm name_of_configfile
			//
			foreach( string s in args )
			{
				String parameter = s;
				Console.WriteLine(s);
			}

			String token = "blah";
			String username = "MYSQL-EMAILPLACE";
			String host = "www.emailplaceng.de";
			Console.WriteLine ("Starting mysql pot with port " + port + " and logfile " + logFile + " and IP " + ip);
			Console.WriteLine ("Command line parameters: port(3306 default), logfile(/Users/flake/mysqlpot.log default) and IP to bind to...");
			MySqlServer mysqlServer = new MySqlServer(port, logFile, ip);
			mysqlServer.start(username, token, host);

		}	// main function
		
	}	// MainClass


}	// MysqlPot
