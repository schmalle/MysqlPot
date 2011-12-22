using System;
using System.Net;
using System.Net.Sockets;


namespace MysqlPot
{
	public class Server
	{
		
		private	TcpListener	m_socket = null;
		private IPEndPoint m_ipEnd = null;
		
		/*
		 * constructor for the server class
		 * 
		 * 
		 */
		public Server (int port)
		{
			m_socket = new TcpListener(port);
			m_ipEnd = new IPEndPoint(IPAddress.Any, port);
		}
		
		
		/*
		 * starts the listerner
		 * 
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

      		string welcome = "Welcome to my test server";
			System.Text.ASCIIEncoding enc = new System.Text.ASCIIEncoding();
      		data = enc.GetBytes(welcome);
   //   		ns.Write(data, 0, data.Length);

      		while(true)
      		{
         		data = new byte[1024];
      /*   		recv = ns.Read(data, 0, data.Length);
         		if (recv == 0)
           			break;
       */
				data = x.getPacketError("hi flake", 0, 100, 100);
         		ns.Write(data, 0, 100);
				break;
      		}
      
			ns.Close();
      		client.Close();
      		m_socket.Stop();
			
			return 0;
			
		}	// start()
		
	}	// server class

}	// mysql namespace

