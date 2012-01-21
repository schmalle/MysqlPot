using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Reflection;


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
			GREETING_PACKET_SEND = 2,
			FINISH = 666
		}
		
		private	TcpListener		m_socket = null;
		private State			m_state = State.NOT_CONNECTED;
		private StreamWriter 	m_writer = null;
		
		
		public String getMyIP()
		{
			IPHostEntry host;
			string localIP = "?";
			host = Dns.GetHostEntry(Dns.GetHostName());
			foreach (IPAddress ip in host.AddressList)
			{
			    if (ip.AddressFamily.ToString() == "InterNetwork")
			    {
			        localIP = ip.ToString();
			    }
			}
			return localIP;			
		}
		
		/*
		 * constructor for the server class
		 */
		public Server (int port, String fileName)
		{
			
            IPAddress adr = IPAddress.Parse(getMyIP());
			m_socket = new TcpListener(adr, port);
			m_writer = File.CreateText(fileName);
		}
		
		
		/*
		 * starts the listerner
		 */
		public int start()
		{
      		byte[] data = new byte[1024];
			Mysql x = new Mysql(0, m_writer);
			

   
			while (true)
			{
            m_socket.Start();

			Console.WriteLine("Waiting for a client...");

            TcpClient client = m_socket.AcceptTcpClient();
      		NetworkStream ns = client.GetStream();
			
			var pi = ns.GetType().GetProperty("Socket", BindingFlags.NonPublic | BindingFlags.Instance);
			var socketIP = ((Socket)pi.GetValue(ns, null)).RemoteEndPoint.ToString();
			Console.WriteLine(socketIP);				
				
      		while(m_state != State.FINISH)
      		{
	      		
				Console.WriteLine("Stream caught...");
				
				if (m_state == State.NOT_CONNECTED)
				{
					Console.WriteLine("Info: Mysqlpot in state NOT CONNECTED");
					handleNotConnected(x, ns);
				
				}
				else if (m_state == State.GREETING_PACKET_SEND)
				{
					Console.WriteLine("Info: Mysqlpot in state GREETING_PACKET_SEND");
					ns.Read(data, 0, 1024);
					x.handleLoginPacket(data, socketIP);
						m_state = State.FINISH;
					
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
				m_state = State.NOT_CONNECTED;

			}
						
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
				m_state = State.GREETING_PACKET_SEND;
				ns.Write(dataOut, 0, dataOut.Length);
			}
		}	// handleNotConnected
		
		
	}	// server class

}	// mysql namespace

