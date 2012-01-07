using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace MysqlPot
{
	public class Mysql
	{
		
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
		public Mysql ()
		{	
			
		}	// Mysql
		
		
		/**
		 * 
		 * returns a Server greeting packet
		 * @in: number of the packet
		 * @out: packet for the client
		 * 
		 * */
		public byte[] getGreetingPacket(int packetNumber)
		{
			String serverVersion = "5.5.15";			// + 1 for termination
			String threadID 		 = "1234";
			String pluginData    = "mysql_native_password";
			byte[] scrambleBuf 	 = {11,22,33,44,55,66,77,88};
            byte[] filler        = {0,0,0,0,0,0,0,0,0,0 };
			byte[] filler12      = {0x42, 0x42, 0x42, 0x42, 0x42, 0x42, 0x42, 0x42, 0x42, 0x42, 0x42, 0x42};
            byte protocolVersion = 0xa;

			
			Int32 length = serverVersion.Length + 1 + 1 + 4 + 8 + 1+ 2+ 1+ 2 +2 +  1 + 10+ 1 + 12 + 1 + pluginData.Length;
			
            // create packet and fill it with dummy bytes to attack programming mistakes
			byte[] packet = new byte[length + 4];
            for (int i = 0; i <= packet.Length - 1; i++)
            {
                packet[i] = 0x42;
            }
						
			
			// 3 byte laenge
			// 1 byte packet nummer
			byte[] lengthInPacket = convert32To3Byte(length);
			packet = copyBytes(lengthInPacket, packet, 0, false);
			packet[3] = (byte)packetNumber;
					
			// copy protocol version
			packet[4] = protocolVersion;
			packet = copyBytes(getBytes(serverVersion), packet, 5, true);
			
			// copy thread id

            // calculate offset for thread id including null terminated length of serverversion
			int offset = serverVersion.Length + 6;
			packet = copyBytes(getBytes(threadID), packet, offset, false);
			offset += 4;
			
			// copy scramble buf
			packet = copyBytes(scrambleBuf, packet, offset, true);
			offset += 9;
			
			Console.WriteLine("Writing server capabilities to offset: " + offset);
			
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
            packet[offset++] = (byte)pluginData.Length;
			
			
            // copy filler buf
            packet = copyBytes(filler, packet, offset, false);
            offset += 10;

			// copy filler buf
            packet = copyBytes(filler12, packet, offset, true);
            offset += 13;
			
			// copy pluginData buf
            packet = copyBytes(getBytes(pluginData), packet, offset, true);
            offset += 10;
            
 

            /*
			 
                1                            protocol_version
                n (Null-Terminated String)   server_version
                 4                            thread_id
                 8                            scramble_buff
                 1                            (filler) always 0x00
                 2                            server_capabilities
                 1                            server_language
                 2                            server_status
                 2                            server capabilities (two upper bytes)
                 1                            length of the scramble
                10                            (filler)  always 0
                 n                            rest of the plugin provided data (at least 12 bytes) 
                 1                            \0 byte, terminating the second part of a scramble			 
             */

			


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

	}	// MySQL class
}

 