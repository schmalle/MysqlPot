using System;
using System.Net;
using System.Net.Sockets;


namespace MysqlPot
{
	public class Server
	{

        /*
         * 
         * variable area
         * 
         */
		
		private	TcpListener	m_socket = null;
		private bool		m_debug = false;
		
		/*
		 * constructor for the server class
		 */
		public Server (int port, bool debug)
		{

			m_debug = debug;
            IPAddress adr = IPAddress.Parse("127.0.0.1");
			m_socket = new TcpListener(adr, port);
			//m_ipEnd = new IPEndPoint(IPAddress.Any, port);
		}
		
		
		/*
		 * starts the listerner
		 */
		public int start()
		{
			int recv;
      		byte[] data = new byte[1024];
			Mysql x = new Mysql();
			

            m_socket.Start();
      		Console.WriteLine("Waiting for a client...");

            TcpClient client = m_socket.AcceptTcpClient();
      		NetworkStream ns = client.GetStream();

      		while(true)
      		{
	      		Console.WriteLine("Stream caught...");
				
         		data = new byte[1024];
				
				byte[] dataOut = x.getGreetingPacket(0);
               
                ns.Write(dataOut, 0, dataOut.Length);
				break;
      		}
      
			ns.Close();
      		client.Close();
      		m_socket.Stop();
			
			return 0;
			
		}	// start()
		
	}	// server class

}	// mysql namespace

