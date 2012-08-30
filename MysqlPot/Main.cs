using System;
using System.Configuration;

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
			// defaults, either bei config file or hard coded defaults... (see Config class below)
			int port = Config.Port;
			String logFile = Config.LogFilename;
			String ip = Config.IP;
			String host = Config.Host;

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
			Console.WriteLine ("Starting mysql pot with port " + port + " and logfile " + logFile + " and IP " + ip);
			Console.WriteLine ("Command line parameters: port(3306 default), logfile(/Users/flake/mysqlpot.log default) and IP to bind to...");
			MySqlServer mysqlServer = new MySqlServer(port, logFile, ip);
			mysqlServer.start(username, token, host);

		}	// main function
		
	}	// MainClass

	/**
 	* 
 	*	Helper Class to read configuration entries from <<appname>>.exe.config 
 	* 
 	**/
	public static class Config {

		/**
 		* 
 		*	Port number to "bind" mysql daemon on 
 		* 
 		**/
   		public static int Port {
       		get { 
				int port;
				if (!int.TryParse(ConfigurationManager.AppSettings["mysql.port"], out port)){
					// no entry found or conversion not succeeded, use default
					port = 3306;
				}
				return port; 
			}
   		}

		/**
 		* 
 		*	IP to bind on
 		* 
 		**/
   		public static String IP {
       		get { 
				String value = ConfigurationManager.AppSettings["mysql.ip"];
				if (value == null){
					// no entry found, use default
					value = "127.0.0.1";
				}
				return value; 
			}
   		}


		/**
 		* 
 		*	full qualified filename for log entries
 		* 
 		**/
   		public static String LogFilename {
       		get { 
				String value = ConfigurationManager.AppSettings["mysql.log"];
				if (value == null){
					// no entry found, use default
					value = "/Users/flake/mysqlpot.log";
				}
				return value; 
			}
   		}


		/**
 		* 
 		*	Host name, will be used as target for Alerts
 		* 
 		**/
   		public static String Host {
       		get { 
				String value = ConfigurationManager.AppSettings["mysql.host"];
				if (value == null){
					// no entry found, use default
					value = "www.emailplaceng.de";
				}
				return value; 
			}
   		}

		/**
 		* 
 		*	URL for posting Alerts
 		* 
 		**/
   		public static String ServerPostAlertsURL {
       		get { 
				String url = ConfigurationManager.AppSettings["server.postAlertsURL"];
				if (url == null){
					// no entry found, use default
					url = "https://www.t-sec-radar.de:9443/ews-0.1/alert/postSimpleMessage";
				}
				return url; 
			}
   		}
}	// Config class




}	// MysqlPot
