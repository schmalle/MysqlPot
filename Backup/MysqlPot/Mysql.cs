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
			byte[] dataBack = null;
			
			dataBack = new byte[data.Length + 2 + 3 + 1 + 1 + 2];
			
			dataBack[0] = 0x2;
			dataBack[1] = 0x2a;
			
			dataBack[2] = (byte)data.Length;
			dataBack[3] = 0x0;
			dataBack[4] = 0x0;
			
			dataBack[5] = (byte)packetNumber;
			dataBack[6] = 0xff;
			
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

