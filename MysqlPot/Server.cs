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
		
		enum State
		{
			NOT_CONNECTED = 1,
			GREETING_PACKET_SEND = 2
		}
		
		private	TcpListener	m_socket = null;
		private int			m_state = (int)State.NOT_CONNECTED;
		
		/*
		 * constructor for the server class
		 */
		public Server (int port)
		{

            IPAddress adr = IPAddress.Parse("127.0.0.1");
			m_socket = new TcpListener(adr, port);
			//m_ipEnd = new IPEndPoint(IPAddress.Any, port);
		}
		
		
		/*
		 * starts the listerner
		 */
		public int start()
		{
      		byte[] data = new byte[1024];
			Mysql x = new Mysql(0);
			

            m_socket.Start();
      		Console.WriteLine("Waiting for a client...");

            TcpClient client = m_socket.AcceptTcpClient();
      		NetworkStream ns = client.GetStream();

      		while(true)
      		{
	      		
				Console.WriteLine("Stream caught...");
				
				if (m_state == (int)State.NOT_CONNECTED)
				{
					Console.WriteLine("Info: Mysqlpot in state NOT CONNECTED");
					handleNotConnected(x, ns);
				
				}
				else if (m_state == (int)State.GREETING_PACKET_SEND)
				{
					Console.WriteLine("Info: Mysqlpot in state GREETING_PACKET_SEND");
					ns.Read(data, 0, 1024);
					x.handleLoginPacket(data);
					
				}
				else
				{
					Console.WriteLine("Info: Mysqlpot in unknown state");
					break;
				}
				
				
				
				
 				
      		}
      
			ns.Close();
      		client.Close();
      		m_socket.Stop();
			
			return 0;
			
		}	// start()
		
		
		/**
		 * handles the send of the greeting packet if not send already
		 * 
		 */
		private void handleNotConnected(Mysql x, NetworkStream ns)
		{				
			byte[] dataOut = x.getGreetingPacket(0);
            if (dataOut != null)
			{	
				m_state = (int)State.GREETING_PACKET_SEND;
				ns.Write(dataOut, 0, dataOut.Length);
			}
		}	// handleNotConnected
		
		
	}	// server class

}	// mysql namespace

