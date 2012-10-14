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
			String token = Config.Usertoken;
			String username = Config.Username;

			// lame parsing of command line parameters
			if (args.Length >= 1)
			{
				try
				{
					port = Convert.ToInt32(args[0]);
				}
				catch (System.FormatException e)
				{
					port = 3306;
					Console.WriteLine("Info: Supplied port number was not valid. Defaulting to 3306");
				}
			
				if (args.Length >= 2)
				{
					logFile = args[1];	

					if (args.Length >= 3)
					{
						ip = args[2];	
					}				

					if (args.Length >= 4)
					{
						username = args[3];	
					}

					if (args.Length >= 5)
					{
						token = args[4];	
					}

					if (args.Length >= 6)
					{
						host = args[5];	
					}

				}						
			
			}

			Console.WriteLine ("Starting mysql pot with port " + port + " and logfile " + logFile + " and IP " + ip + " and username " + username + " and password " + token);
			Console.WriteLine ("Command line parameters: port logfile ip username password host");
			MySqlServer mysqlServer = new MySqlServer(port, logFile, ip);
			mysqlServer.start(username, token, host);

		}	// main function
		
	}	// MainClass

	/**
 	* 
 	*	Helper Class to read configuration entries from <<appname>>.exe.config 
 	* 
 	**/
	public static class Config 
	{

		/**
 		* 
 		*	Port number to "bind" mysql daemon on 
 		* 
 		**/
   		public static int Port 
		{
       		get 
			{ 
				int port = 3306;
				if (!int.TryParse(ConfigurationManager.AppSettings["mysql.port"], out port))
                {
					// no entry found or conversion not succeeded, use default
				}
				return port; 
			}
   		}

		/**
 		* 
 		*	IP to bind on
 		* 
 		**/
   		public static String IP 
		{
       		get 
			{ 
				String value = ConfigurationManager.AppSettings["mysql.ip"];
				if (value == null)
				{
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
   		public static String LogFilename 
        {
       		get 
			{ 
				String value = ConfigurationManager.AppSettings["mysql.log"];
				if (value == null)
				{
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
   		public static String Host 
        {
       		get 
			{ 
				String value = ConfigurationManager.AppSettings["mysql.host"];
				if (value == null)
				{
					// no entry found, use default
					value = "unknown_host";
				}
				return value; 
			}
   		}

		/**
 		* 
 		*	Host name, will be used as target for Alerts
 		* 
 		**/
   		public static String Username 
        {
       		get { 
				String value = ConfigurationManager.AppSettings["server.username"];
				if (value == null)
				{
					// no entry found, use default
					value = "<user-name>";
				}
				return value; 
			}
   		}

				/**
 		* 
 		*	Host name, will be used as target for Alerts
 		* 
 		**/
   		public static String Usertoken {
       		get { 
				String value = ConfigurationManager.AppSettings["server.token"];
				if (value == null){
					// no entry found, use default
					value = "<password>";
				}
				return value; 
			}
   		}

		/**
 		* 
 		*	URL for posting Alerts
 		* 
 		**/
   		public static String ServerPostAlertsURL 
        {
       		get { 
				String url = ConfigurationManager.AppSettings["server.postAlertsURL"];
				if (url == null)
				{
					// no entry found, use default
					url = "https://www.t-sec-radar.de:9443/ews-0.1/alert/postSimpleMessage";
				}
				return url; 
			}
   		}
}	// Config class




}	// MysqlPot
