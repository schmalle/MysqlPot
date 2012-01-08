using System;

namespace MysqlPot
{
	public class MysqlDefs
	{

		
		private String serverVersion = "5.5.15";			// + 1 for termination
		private String threadID      = "1234";
		private String pluginData    = "mysql_native_password";
		private byte[] scrambleBuf 	 = {11,22,33,44,55,66,77,88};
        private byte[] filler        = {0,0,0,0,0,0,0,0,0,0 };
		private byte[] filler12      = {0x42, 0x42, 0x42, 0x42, 0x42, 0x42, 0x42, 0x42, 0x42, 0x42, 0x42, 0x42};
        private byte protocolVersion = 0xa;
		
		public String getServerVersion() { return serverVersion; }
		public String getThreadID() { return threadID; }
		public String getPluginData() { return pluginData; }
		public byte[] getScrambleBuf() { return scrambleBuf; }
		public byte[] getFiller() { return filler; }
		public byte[] getFiller12() { return filler12; }
		public byte   getProtocolVersion() { return protocolVersion; }
		

		
		public MysqlDefs ()
		{
		}
		
		
		
	}
}

