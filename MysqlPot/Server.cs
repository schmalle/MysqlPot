using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Reflection;


/**
 * 
 * 
 * 
 * */
namespace MysqlPot
{
	public class MySqlServer
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
			LOGIN_OK = 3,
			FINISH = 666
		}
		
		private	TcpListener		m_socket = null;
		private State			m_state = State.NOT_CONNECTED;
		private StreamWriter 	m_writer = null;
		private int				readBytes = 0;
		private byte[]			data = new byte[1024];
		private int				m_port = 3306;
		private String			m_fileName = "";
		private String			m_ip = null;
		
		
		/*
		 * calculate own IP address
		 * 
		 */
		public String getMyIP()
		{
			if (m_ip != null)
				return m_ip;

			return "192.168.1.36";
/*
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

*/

		}

		
		/*
		 * constructor for the server class
		 */
		public MySqlServer (int port, String fileName, String ip)
		{
			m_ip = ip;
			m_port = port;
			m_fileName = fileName;
			m_writer = File.CreateText(fileName);

			doSetup(port, fileName);
		}

		/*
		 * setup socket, listeners etc
		 */
		private void doSetup (int port, String fileName)
		{
            IPAddress adr = IPAddress.Parse(getMyIP());
			m_socket = new TcpListener(adr, port);
		}

		/*
		 * socketCheck wait 50 * 100 milli seconds before quiting,
		 * in the mean time it reads / tries to read from a socket
		 * 
		 */
		private Boolean socketCheck (NetworkStream ns)
		{

			int runner = 1000;
			while (runner-- != 0) 
			{

				try
				{
					readBytes = ns.Read (data, 0, 1024);
					if (readBytes != 0)
						return true;
				}
				catch (Exception e)
				{
                    Console.WriteLine("Info: Caught exception while waiting for network data....");
                    Console.WriteLine(e.ToString());
					return false;
				}

				System.Threading.Thread.Sleep(100);

                try
                {

				    if (m_socket.Server.Poll(1000, SelectMode.SelectRead) && m_socket.Server.Available == 0 )
				    {
					    Console.WriteLine("Info: Server disconnected...");
					    doSetup(m_port, m_fileName);
				    }
				    else
				    {
					    //Console.WriteLine("Info: Server connected...");
				    }
                }
                catch (System.Net.Sockets.SocketException e)
				{
                    Console.WriteLine("Info: Caught SocketException  exception while polling / data....");
					return false;
				}

			}	// while loop

			Console.WriteLine("Info: Closing down as no client is connecting");
			return false;
		}	// socketCheck



		/**
		 *  start the listener, if one listener dies, create another one
		 */
		public void start (String username, String token, String host)
		{
			while (true)
			{
				startInternal(username, token, host);
			}
		}

		/*
		 * starts the listerner
		 */
		public void startInternal(String username, String token, String host)
		{
			Mysql x = new Mysql(0, m_writer);

			// set initial state
			m_state = State.NOT_CONNECTED;

			// start socket and wait for incoming client connection // add here multi client stuff
           	m_socket.Start();
            TcpClient client = m_socket.AcceptTcpClient();  
			NetworkStream ns = client.GetStream();
			StreamWriter sw = new StreamWriter(ns);
			sw.AutoFlush = true;

				if (!ns.CanRead)
				{
					Console.WriteLine("Info: Stream no readable....");
					return;
				}

				var pi = ns.GetType().GetProperty("Socket", BindingFlags.NonPublic | BindingFlags.Instance);
				var socketIP = ((Socket)pi.GetValue(ns, null)).RemoteEndPoint.ToString();
				Console.WriteLine("Connecting client: " + socketIP);				

				// send the server greeting (static for now)
				handleNotConnected(x, ns, client);
				ns = client.GetStream();

      			while(socketCheck(ns))
      			{

					Console.WriteLine("Info: Read " + readBytes + " bytes....");

					 // if not connected, do a straight forward "send a greeting packet"
					if (m_state == State.NOT_CONNECTED)
					{
						Console.WriteLine("Info: Mysqlpot in state NOT CONNECTED, not possible state, error");				
					}
					else if (m_state == State.GREETING_PACKET_SEND)
					{
						Console.WriteLine("Info: Mysqlpot in state GREETING_PACKET_SEND");
						x.handleLoginPacket(data, socketIP, token, username, host);
						m_state = State.LOGIN_OK;

						Console.WriteLine("Info: Login packet retrieved, now trying to generate answer packet...");
						byte[] okPacket = x.generateOKPacket(x.getPacketNumber(data) + 1, 1); // packetnumber, affeceted lines
						
						Console.WriteLine("Info: Now writing OK packet back on the line..");

						sw.Write (System.Text.Encoding.UTF8.GetString(okPacket).ToCharArray());
						ns = client.GetStream();


					}
					else if (m_state == State.LOGIN_OK)
					{
						Console.WriteLine("Info: Mysqlpot in state LOGIN_OK");
						
						byte[] queryString = x.handleQueryPacket(data, socketIP, token, username, host);
						byte[] answer = x.getAnswerPacket(queryString, x.getPacketNumber(data) + 1);
						if (answer != null)
						{
							ns.Write(answer, 0, answer.Length);

							// sw.Write (System.Text.Encoding.UTF8.GetString(answer).ToCharArray());
							ns = client.GetStream();

						}
						else
						{
							Console.WriteLine("Info: Detected no suitable answer for the SQL query");
						}

					}
					else
					{
						Console.WriteLine("Info: Mysqlpot in unknown state");
						break;
					}
				
      			}

			Console.WriteLine("Error: Ausbruch aus dem while loop");

			ns.Close();
      		client.Close();
				
      		m_socket.Stop();
			m_state = State.NOT_CONNECTED;
						
		}	// start()
		
		
		/**
		 * handles the send of the greeting packet if not send already
		 * 
		 */
		private void handleNotConnected (Mysql x, NetworkStream ns, TcpClient client)
		{				
			byte[] dataOut = x.getGreetingPacket (0);
			if (dataOut != null) 
			{	
				m_state = State.GREETING_PACKET_SEND;
				ns.Write (dataOut, 0, dataOut.Length);
				ns.Flush ();
				ns = client.GetStream(); 

				Console.WriteLine("Info: Send greeting packet....");

			} 
			else 
			{
				Console.WriteLine("Info: Error retrieving greeting packet....");
			}

		}	// handleNotConnected
		
		
	}	// server class

}	// mysql namespace

