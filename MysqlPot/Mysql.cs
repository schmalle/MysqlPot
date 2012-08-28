using System;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace MysqlPot
{
	public class Mysql
	{
		
		// variable area 
		private int	m_packetNumber = 0;
		private MysqlDefs	m_mysqlDefs = null;
		private StreamWriter m_writer = null;
		
		
		/**
		 * returns the byte array for a given string
		 */
		private byte[] getBytes(String inStr)
		{
			System.Text.ASCIIEncoding enc = new System.Text.ASCIIEncoding();
    		return enc.GetBytes(inStr);
		}
		
		
		/**
		 * constructor for the Mysql class
		 */
		public Mysql (int packetNumber, StreamWriter w)
		{	
			m_mysqlDefs = new MysqlDefs();
			m_packetNumber = packetNumber;
			m_writer = w;
		}	// Mysql
		

		/**
		 * 
		 * returns a Server greeting packet
		 * @out: packet for the client
		 * 
		 * */
		public byte[] getGreetingPacket()
		{
			return getGreetingPacket(m_packetNumber);
		}
		
		
		
		/**
		 * 
		 * returns a Server greeting packet
		 * @in: number of the packet
		 * @out: packet for the client
		 * 
		 * */
		public byte[] getGreetingPacket(int packetNumber)
		{
			
			// create packet and fill it with dummy bytes to attack programming mistake
			Int32 length = m_mysqlDefs.getServerVersion().Length + 1 + 1 + 4 + 8 + 1+ 2+ 1+ 2 +2 +  1 + 10+ 1 + 12 + 1 + m_mysqlDefs.getPluginData().Length;
			byte[] packet = new byte[length + 4];
            for (int i = 0; i <= packet.Length - 1; i++)
            {
                packet[i] = 0x0;
            }						
			
			// 3 byte laenge
			// 1 byte packet nummer
			byte[] lengthInPacket = convert32To3Byte(length);
			packet = copyBytes(lengthInPacket, packet, 0, false);
			packet[3] = (byte)packetNumber;
					
			// copy protocol version
			packet[4] = m_mysqlDefs.getProtocolVersion(); 
			packet = copyBytes(getBytes(m_mysqlDefs.getServerVersion()), packet, 5, true);
			
            // calculate offset for thread id including null terminated length of serverversion
			int offset = m_mysqlDefs.getServerVersion().Length + 6;
			packet = copyBytes(getBytes(m_mysqlDefs.getThreadID()), packet, offset, false);
			offset += 4;
			
			// copy scramble buf
			packet = copyBytes(m_mysqlDefs.getScrambleBuf(), packet, offset, true);
			offset += 9;
			
            // hardcoded server capabilities
            packet[offset++] = 0xff;
            packet[offset++] = 0xf7;

            // hardcoded server language
            packet[offset++] = 0x8;

            // hardcoded server status
            packet[offset++] = 0x02;
            packet[offset++] = 0x00;

            // hardcoded server capabilities (upper two bytes)
            packet[offset++] = 0xf;
            packet[offset++] = 0x80;

            // length of the scramble
            packet[offset++] = (byte)m_mysqlDefs.getPluginData().Length;
			
            // copy filler buf
            packet = copyBytes(m_mysqlDefs.getFiller(), packet, offset, false);
            offset += 10;

			// copy filler buf
            packet = copyBytes(m_mysqlDefs.getFiller12(), packet, offset, true);
            offset += 13;
			
			// copy pluginData buf
            packet = copyBytes(getBytes(m_mysqlDefs.getPluginData()), packet, offset, true);
            offset += 10;           
 
			// fix internal packetnumber
			m_packetNumber++;
			
            return packet;		
		}
		
		/**
		 * copies an 
		 */
		public byte[] copyBytes(byte[] source, byte[] dest, int offsetDest, bool nullTerminated)
		{
            int runner = 0;
			
			for (; runner <= source.Length - 1; runner++)
			{
				dest[runner + offsetDest] = source[runner];
			}

            // if null termination is requiredm fullfull the need
            if (nullTerminated)
                dest[runner + offsetDest] = 0x0;
			
			return dest;
		}	// copyBytes
		

		/**
		 * extracts the number of a packet
		 * 
		 */
		public byte getPacketNumber (byte[] data)
		{
			return data[3];
		}

		/**
		 * converts an 32 bit value to a 3 byte array
		 * @in: Int32
		 * @out: ByteArray
		 */
		public byte[] convert32To3Byte(Int32 input)
		{

            byte[] destPacket = new byte[3];
			byte[] packet = new byte[4];

            packet = BitConverter.GetBytes(input);

            for (int runner = 0; runner <= 2; runner++)
            {
                destPacket[runner] = packet[runner];
            }


			return destPacket;
			
		} // convert32To3Byte
		
		
		/**
		 * 
		 * return an error packet based on the data, the packetnumber, the length and an error number
		 */
		public byte[] getPacketError(String data, int packetNumber, Int64 length, Int64 errorNumber)
		{
			
			     // 3 byte laenge
        // 1 byte number
        // 0xff fuellbyte
        // 2 byte errocode
        // n String
			
			byte[] dataBack = null;
			
			dataBack = new byte[data.Length + 3 + 1 + 1 + 2];
					
			dataBack[0] = (byte)data.Length;;
			dataBack[1] = 0x00;
			dataBack[2] = 0x00;
			
			dataBack[3] = 0x00;
			
			dataBack[4] = 0xff;
			
			dataBack[5] = 0x6a;
			dataBack[6] = 0x04;
			
			System.Text.ASCIIEncoding enc = new System.Text.ASCIIEncoding();

			byte[] dataBytes = enc.GetBytes(data);
			
			for (int runner = 0; runner <= data.Length- 1 ; runner++)
			{
				dataBack[7+ runner] = dataBytes[runner];
			}
			
			return dataBack;
		}	// getPacket
		
		
		/**
		 * 
		 * 
		 * 
		 */
		public void handleLoginPacket(byte[] dataIn, String clientIP, String token, String username, String host)
		{
			// 3 byte packet laenge
			// for the moment we ignore the upper two bytes
			int length = dataIn[0];
						
			int packetNumber = dataIn[3];
			
			// allocate dummy buffer for the username
			byte[] uNameBytes =  new byte[1024];
			
			int runner = 0x24;
			while (runner != dataIn.Length -1 && dataIn[runner] != 0x0)
			{
				uNameBytes[runner-0x24] = dataIn[runner++];
			}
			
			dataIn[runner-0x24] = 0x0;
			
			String outStr = "Login from " + clientIP +  " try with username("+ DateTime.Now.ToString("HH:mm:ss tt") + "): " + System.Text.Encoding.Default.GetString(uNameBytes); 

			EWSSender sender = new EWSSender();

			// yyyy-MM-dd HH:mm:ss

			sender.send(token, username, clientIP, "100", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "MySQL.Login", username, host); 

			Console.WriteLine(outStr);
			m_writer.WriteLine(outStr);
			
		} // handleLoginPacket


		/**

			generates an OK packet

		 */
		public byte[] generateOKPacket (int packetNumber, int affectedRows)
		{

			Console.WriteLine("Info: Starting generation of OK packet with packetnumber " + packetNumber + " and affected rows: " + affectedRows);

			byte[] packet = new byte[11];
			byte[] lengthInPacket = convert32To3Byte(7);

			packet = copyBytes(lengthInPacket, packet, 0, false);

			packet[3] = (byte)packetNumber;	

			packet[4] = 0x0;					// dummy byte	
			packet[5] = (byte)affectedRows;		// affected rows

			packet[6] = 0x2;					// server status	
			packet[7] = 0x0;					// server status

			packet[8] = 0x0;					// warnings
			packet[9] = 0x0;					// warnings
			packet[10] = 0x0;					// warnings


			Console.WriteLine("Info: Generated OK packet with packetnumber " + packetNumber + " and affected rows: " + affectedRows);

			return packet;

		}	// sendOKPacket


		/**
		 * 
		 * retrieve query command
		 * 
		 */
		public byte[] handleQueryPacket(byte[] dataIn, String clientIP, String token, String username, String host)
		{
			// 3 byte packet laenge
			// for the moment we ignore the upper two bytes
			int length = dataIn[0];
						
			int packetNumber = dataIn[3];
			
			// allocate dummy buffer for the username
			byte[] uNameBytes =  new byte[1024];
			
			int runner = 0x5;
			while (runner != dataIn.Length -1 && dataIn[runner] != 0x0)
			{
				uNameBytes[runner-0x5] = dataIn[runner++];
			}
			
			dataIn[runner-0x5] = 0x0;
			
			String outStr = "Query from " + clientIP +  " with command ("+ DateTime.Now.ToString("HH:mm:ss tt") + "): " + System.Text.Encoding.Default.GetString(uNameBytes); 

			Console.WriteLine(outStr);
			m_writer.WriteLine(outStr);

			return uNameBytes;
			
		} // handleQueryPacket


		public byte[] generateAnswerPacket (String queryString)
		{
			return null;
		}

		/*

		   VERSION 4.1
 Bytes                        Name
 -----                        ----
 4                            client_flags
 4                            max_packet_size
 1                            charset_number
 23                           (filler) always 0x00...
 n (Null-Terminated String)   user
 n (Length Coded Binary)      scramble_buff (1 + x bytes)
 n (Null-Terminated String)   databasename (optional)
 
 client_flags:            CLIENT_xxx options. The list of possible flag
                          values is in the description of the Handshake
                          Initialisation Packet, for server_capabilities.
                          For some of the bits, the server passed "what
                          it's capable of". The client leaves some of the
                          bits on, adds others, and passes back to the server.
                          One important flag is: whether compression is desired.
                          Another interesting one is: CLIENT_CONNECT_WITH_DB,
                          which shows the presence of the optional databasename.
 
 max_packet_size:         the maximum number of bytes in a packet for the client
 
 charset_number:          in the same domain as the server_language field that
                          the server passes in the Handshake Initialization packet.
 
 user:                    identification
 
 scramble_buff:           the password, after encrypting using the scramble_buff
                          contents passed by the server (see "Password functions"
                          section elsewhere in this document)
                          if length is zero, no password was given
 
 databasename:            name of schema to use initially
 		  
		  
		*/


		/*
		 * gives an answer packet back based on the seen query !!!
		 * 
		 */
		public byte[] getAnswerPacket (byte[] queryBytes, int packetNumber)
		{
			byte[] packetVer = {0x01, 0x00, 0x00, (byte)packetNumber , 0x01 , 0x27 , 0x00 , 0x00 , 0x02 , 0x03 , 0x64 , 0x65 , 0x66 , 0x00   
			, 0x00 , 0x00 , 0x11 , 0x40 , 0x40 , 0x76 , 0x65 , 0x72 , 0x73 , 0x69 , 0x6f , 0x6e , 0x5f , 0x63 , 0x6f , 0x6d   
			, 0x6d , 0x65 , 0x6e , 0x74 , 0x00 , 0x0c , 0x21 , 0x00 , 0x18 , 0x00 , 0x00 , 0x00 , 0xfd , 0x00 , 0x00 , 0x1f   
			, 0x00 , 0x00 , 0x05 , 0x00 , 0x00 , 0x03 , 0xfe , 0x00 , 0x00 , 0x02 , 0x00 , 0x09 , 0x00 , 0x00 , 0x04 , 0x08   
			, 0x28 , 0x55 , 0x62 , 0x75 , 0x6e , 0x74 , 0x75 , 0x29 , 0x05 , 0x00 , 0x00 , 0x05 , 0xfe , 0x00 , 0x00 , 0x02   
			, 0x00};

			// answer packet for "show databases";
			byte[] packetDBs = {	
			
									01 , 0x00 , 0x00 , (byte)packetNumber , 0x01 , 0x4b , 0x00 , 0x00 , 0x02 , 0x03 , 0x64 , 0x65 , 0x66 , 0x12  // .......K....def.
						, 0x69 , 0x6e , 0x66 , 0x6f , 0x72 , 0x6d , 0x61 , 0x74 , 0x69 , 0x6f , 0x6e , 0x5f , 0x73 , 0x63 , 0x68 , 0x65  // information_sche
						, 0x6d , 0x61 , 0x08 , 0x53 , 0x43 , 0x48 , 0x45 , 0x4d , 0x41 , 0x54 , 0x41 , 0x08 , 0x53 , 0x43 , 0x48 , 0x45  // ma.SCHEMATA.SCHE
						, 0x4d , 0x41 , 0x54 , 0x41 , 0x08 , 0x44 , 0x61 , 0x74 , 0x61 , 0x62 , 0x61 , 0x73 , 0x65 , 0x0b , 0x53 , 0x43  // MATA.Database.SC
						, 0x48 , 0x45 , 0x4d , 0x41 , 0x5f , 0x4e , 0x41 , 0x4d , 0x45 , 0x0c , 0x21 , 0x00 , 0xc0 , 0x00 , 0x00 , 0x00  // HEMA_NAME.!.....
						, 0xfd , 0x01 , 0x00 , 0x00 , 0x00 , 0x00 , 0x05 , 0x00 , 0x00 , 0x03 , 0xfe , 0x00 , 0x00 , 0x22 , 0x00 , 0x13  // ............."..
						, 0x00 , 0x00 , 0x04 , 0x12 , 0x69 , 0x6e , 0x66 , 0x6f , 0x72 , 0x6d , 0x61 , 0x74 , 0x69 , 0x6f , 0x6e , 0x5f  // ....information_
						, 0x73 , 0x63 , 0x68 , 0x65 , 0x6d , 0x61 , 0x05 , 0x00 , 0x00 , 0x05 , 0x04 , 0x44 , 0x65 , 0x6d , 0x6f , 0x07  // schema.....Demo.
						, 0x00 , 0x00 , 0x06 , 0x06 , 0x53 , 0x63 , 0x61 , 0x6e , 0x69 , 0x74 , 0x06 , 0x00 , 0x00 , 0x07 , 0x05 , 0x6d  // ....Scanit.....m
						, 0x79 , 0x73 , 0x71 , 0x6c , 0x13 , 0x00 , 0x00 , 0x08 , 0x12 , 0x70 , 0x65 , 0x72 , 0x66 , 0x6f , 0x72 , 0x6d  // ysql.....perform
						, 0x61 , 0x6e , 0x63 , 0x65 , 0x5f , 0x73 , 0x63 , 0x68 , 0x65 , 0x6d , 0x61 , 0x05 , 0x00 , 0x00 , 0x09 , 0x04  // ance_schema.....
						, 0x74 , 0x65 , 0x73 , 0x74 , 0x05 , 0x00 , 0x00 , 0x0a , 0xfe , 0x00 , 0x00 , 0x22 , 0x00           // test.......".


			};

			String query = queryBytes.ToString ();
			Console.WriteLine("Checking query " + query);

			if (byteArrayCompare(getBytes("select @@version_comment"), queryBytes)) 
			{
				Console.WriteLine ("Info: Select Version packet detected...");
				return packetVer;
			} 
			else if (byteArrayCompare(getBytes("show databases"), queryBytes)) 
			{
				Console.WriteLine ("Info: Select Version packet detected...");
				return packetDBs;
			} 
			else 
			{
				Console.WriteLine("Info: Unknown SQL query found....");
				return null;
			}

		}



		/*
		 * compare two arrays and return true, of the shorter array is contained in the bigger one
		 * 
		 */
		private bool byteArrayCompare (byte[] shortArray, byte[] longArray)
		{
			if (shortArray.Length >= longArray.Length)
				return false;

			for (int runner = 0; runner <= shortArray.Length - 1; runner++) 
			{
				if (0x00 != (shortArray[runner] - longArray[runner]))
				{
					Console.WriteLine("Info(byteArrayCompare): Data not equal");
					return false;
				}
			}

			Console.WriteLine("Info(byteArrayCompare): Data equal");
			return true;


		}

	}	// MySQL class
}

 