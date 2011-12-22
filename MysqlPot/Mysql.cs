using System;

namespace MysqlPot
{
	public class Mysql
	{
		public Mysql ()
		{
			
		}
		
		
		/*
		 * 
		 * return an error packet based on the data, the packetnumber, the length and an error number
		 * 
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
	
	
	}
}

