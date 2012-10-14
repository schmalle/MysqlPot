using System;
using System.IO;
using System.Net;
using System.Text;

namespace MysqlPot
{
	public class EWSSender
	{
		public EWSSender ()
		{
		}
		
		
		
	/**
	 * returns the message to be send to the server
	 * @param token
	 * @param userName
	 * @param ip
	 * @param req
	 * @param time
	 * @param attackType
	 * @param ident
	 * @return
	 * @throws UnsupportedEncodingException
	 */
	public String getMessage(String token, String userName, String ip, String req, String time, String attackType, String ident, String host) 
	{
 
			Console.WriteLine("Info(getMessage): Generating message for user: " + userName + " and token: " + token);
			Console.WriteLine("Info(getMessage):                        ip: "+ ip + " request: " + req + " ident: " + ident);
			Console.WriteLine("Info(getMessage)                         and host: " + host);


        	String message =

                "<EWS-SimpleMessage version=\"1.0\">\n" +
                        "        <Authentication>\n" +
                        "                <username>" + userName + "</username>\n" +
                        "                <token>" + token + "</token>\n" +
                        "        </Authentication>\n" +
                        "        <Alert messageid=\""+ ident +"\">\n" +
                        "            <Analyzer id=\""+ident+"\" name=\""+ ident +"\"/>     \n" +
                        "                <CreateTime tz=\"0100\">"+ time +"</CreateTime>                                  \n" +
                        "                <Source>\n" +
                        "                        <Node>\n" +
                        "                                <Address category=\"ipv4-addr\"><address>" + ip + "</address></Address>       \n" +
                        "                        </Node>\n" +
                        "                </Source>                                      \n" +
                        "                <Target ident=\""+ host +"\">                           \n" +
                        "                        <Node>\n" +
                        "                                <Address category=\"ipv4-addr\">"+ ip +"</Address>             \n" +
                        "                        </Node>\n" +
                        "                        <Service ident=\"0\">                                             \n" +
                        "                                <dport>3306</dport>\n" +
                        "                        </Service>\n" +
                        "                </Target>\n" +
                        "                <Classification origin=\""+attackType+"\" ident=\""+ attackType +"\" text=\""+ attackType+"\"/> \n" +
                        "                <AdditionalData type=\"string\" meaning=\"user-agent\">Mozilla/5.0</AdditionalData>    \n" +
                        "                <AdditionalData type=\"string\" meaning=\"header\">"+ attackType +"</AdditionalData>  \n" +
                        "                <AdditionalData type=\"string\" meaning=\"request\">"+ req +"</AdditionalData>\n" +
                        "                <AdditionalData type=\"string\" meaning=\"domain\">evolution.hospedando.com</AdditionalData>   \n" +
                        "        </Alert>\n" +
                        "</EWS-SimpleMessage>";

        return message;
    }     // getMessage
		
	
	
	/*
	 * send data to the early warning system of DTAG
	 * 
	 */ 
	public void send(String token, String userName, String ip, String req, String time, String attackType, String ident, String host)
	{

			ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

			string str = getMessage(token, userName, ip, req, time, attackType, ident, host);
			byte[] byteArray = Encoding.UTF8.GetBytes (str);
			WebRequest request = WebRequest.Create("https://www.t-sec-radar.de:9443/ews-0.1/alert/postSimpleMessage");
			((HttpWebRequest)request).UserAgent = ".NET Framework Example Client";
			request.Method = "POST";
			request.ContentLength = byteArray.Length;
			request.ContentType = "text/xml; charset=utf-8";
			Stream dataStream = request.GetRequestStream ();
			dataStream.Write (byteArray, 0, byteArray.Length);
			dataStream.Close ();
			WebResponse response = request.GetResponse();
			Console.WriteLine (((HttpWebResponse)response).StatusDescription);
			Stream dataStream2 = response.GetResponseStream();

			StreamReader reader = new StreamReader (dataStream2);
            // Read the content.
            string responseFromServer = reader.ReadToEnd ();
       
			Console.WriteLine(responseFromServer);

			response.Close();

	}
}

}

