#region GPL_FILE_HEADER
/* msgCourier.cs
   Copyright (C) 2005 Carlos Justiniano
   cjus@chessbrain.net, cjus34@yahoo.com, cjus@users.sourceforge.net

msgCourier.cs is free software; you can redistribute it and/or
modify it under the terms of the GNU General Public License as
published by the Free Software Foundation; either version 2 of the
License, or (at your option) any later version.

msgCourier.cs was developed by Carlos Justiniano for use on the
msgCourier project and the ChessBrain Project and is now distributed in
the hope that it will be useful, but WITHOUT ANY WARRANTY; without
even the implied warranty of MERCHANTABILITY or FITNESS FOR A
PARTICULAR PURPOSE.  See the GNU General Public License for more
details.

You should have received a copy of the GNU General Public License
along with msgCourier.cs; if not, write to the Free Software 
Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.
*/
#endregion

using System;
using System.Text;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace mcww
{
	public class Message
	{
		#region Accessors
		public int Port
		{
			get { return msgPort; }
			set { msgPort = value; }
		}
		public string ServerAddress
		{
			get { return serverAddress; }
			set { serverAddress = value; }
		}
		public string CommandLine
		{
			get { return commandLine; }
			set { commandLine = value; }
		}
		public string MessageID
		{
			get { return messageID; }
		}
		public string Transport
		{
			get { return messageTransport; }
			set { messageTransport = value; }
		}
		public string SendHeaders
		{
			get { return sentHeaders; }
			set { sentHeaders = value; }
		}
		public bool EnableNagel
		{
			get { return Nagel; }
			set { Nagel = value; }
		}
		public Socket SocketObj
		{
			get { return sock; }
			set { sock = value; }
		}
		#endregion

		#region Private Variables
		private int msgPort = 3400;
		private string serverAddress;
		private string messageID;
		private string commandLine;
		private string messageTransport = "tcp";
		private string priority = "priority-mid";
		private int contentLength = 0;
		private string contentType = "application/octet-stream";
		private byte[] msgdata;
		private byte[] payload;
		private	string msgHeader;
		private string msgUserHeaders;
		private string sentHeaders;
		private bool Nagel = true;
		Socket sock = null;
		bool keepAlive = true;
		#endregion

		public Message()
		{
			// generate message UUID
			messageID = System.Guid.NewGuid().ToString();
		}
		public void SetPayload(string mimeContentType, byte[]payloadData)
		{
			contentType = mimeContentType;
			payload = payloadData;
			contentLength = payloadData.Length;
		}
		public void SetAdditionalFields(string strHeader)
		{
			msgUserHeaders += strHeader;
			if (msgUserHeaders.IndexOf("Connection: close") != -1)
				keepAlive = false;
		}
		public string GetResponse()
		{
			
			string s;
			s = msgHeader;
			/*
			if (contentLength > 0)
			{
				System.Text.ASCIIEncoding enc = new System.Text.ASCIIEncoding();
				s += enc.GetString(payload);
			}
			*/
			return s;
		}

		public void Send(int timeOut)
		{
			msgHeader = commandLine;
			msgHeader += " MCP/1.0\r\n";

			msgHeader += "MsgID: ";
			msgHeader += messageID;
			msgHeader += "\r\n";

			//msgHeader += "Options: reply-wait\r\n";
			msgHeader += "Priority: ";
			msgHeader += priority;
			msgHeader += "\r\n";

			msgHeader += "To: ";
			msgHeader += serverAddress;
			msgHeader += ":";
			msgHeader += msgPort.ToString();
			msgHeader += "\r\n";

			msgHeader += "From: ";
			msgHeader += Dns.GetHostName();
			msgHeader += ":";
			msgHeader += msgPort.ToString();
			msgHeader += "\r\n";

			msgHeader += msgUserHeaders;

			if (contentLength > 0)
			{
				msgHeader += "Content-Length: ";
				msgHeader += contentLength.ToString();
				msgHeader += "\r\n";

				msgHeader += "Content-Type: ";
				msgHeader += contentType.ToString();
				msgHeader += "\r\n";
			}
			msgHeader += "\r\n";
			sentHeaders = msgHeader;

			if (contentLength == 0)
			{
				msgdata = Encoding.ASCII.GetBytes(msgHeader.ToString());
			}
			else
			{
				byte[] header = Encoding.ASCII.GetBytes(msgHeader.ToString());
				MemoryStream ms = new MemoryStream();
				ms.Write(header, 0, header.Length);
				ms.Write(payload, 0, contentLength);
				msgdata = ms.ToArray();
			}

			if (messageTransport == "tcp")
				SendRecieve(timeOut, false);
			else 
				SendRecieve(timeOut, true);
		}		

		public void SendRecieve(int timeOut, bool isUDP)
		{
			// prepare elapsed time
			long t1 = DateTime.Now.AddMilliseconds(timeOut).ToFileTimeUtc();

			if (sock == null)
			{
				try
				{
					if (isUDP)
					{
						sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
					}
					else
					{
						sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
						sock.Connect(new IPEndPoint(IPAddress.Parse(serverAddress), msgPort));
					}
					sock.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.NoDelay, (Nagel == true) ? 0 : 1);
				}
				catch (SocketException se)
				{
					sock.Close();
					msgHeader = se.Message;
					return;
				}
				catch (Exception e)
				{
					sock.Close();
					msgHeader = e.Message;
					return;
				}
			}

			// make socket nonblocking
			sock.Blocking = false;

			// send request
			int totalBytesSent = 0;
			while (totalBytesSent < msgdata.Length)
			{
				try
				{
					if (isUDP)
					{
						sock.SendTo(msgdata, 
							msgdata.Length, SocketFlags.None,
							new IPEndPoint(IPAddress.Parse(serverAddress), msgPort));
						break;
					}
					else
					{
						totalBytesSent += sock.Send(msgdata, 
							totalBytesSent, 
							msgdata.Length - totalBytesSent, 
							SocketFlags.None);
					}
				}
				catch (SocketException se)
				{
					if (se.ErrorCode != 10035) //WSAEWOULDBLOCK
					{
						sock.Close();
						msgHeader = "Unable to connect to application server";
						return;
					}
				}
				catch (Exception ex)
				{
					sock.Close();
					msgHeader = ex.Message;
					return;
				}
			}

			// reset variables
			msgHeader = "";
			payload = Encoding.ASCII.GetBytes("");

			// get response
			int totalBytesRecv = 0;
			int bytesRecv = 0;
			bool done = false;
			byte [] recvBuffer = new byte[4096];
			MemoryStream msRecv = new MemoryStream();
			while (true)
			{
				if (DateTime.Now.ToFileTimeUtc() > t1)
				{
					msgHeader = "Message timed out";
					break;
				}
		
				try
				{
					if (isUDP)
					{
						IPEndPoint remoteIPEndPoint = new IPEndPoint(IPAddress.Any, 0);
						EndPoint remote = (EndPoint)remoteIPEndPoint;
						sock.ReceiveFrom(recvBuffer, ref remote);
						bytesRecv = recvBuffer.Length;
					}
					else
					{
						bytesRecv = sock.Receive(recvBuffer, 0, 4096, SocketFlags.None);
					}

					if (bytesRecv != 0)
					{
						totalBytesRecv += bytesRecv;
						msRecv.Write(recvBuffer, 0, bytesRecv);
					
						// test if end of header
						byte []tmp = msRecv.ToArray();
						for (int i= 0; i < tmp.Length - 3; i++)
						{
							if (tmp[i] == '\r' && tmp[i+1] == '\n' && tmp[i+2] == '\r' && tmp[i+3] == '\n')
							{
								byte []head = new byte[i+4];
								Array.Copy(tmp,0, head, 0, i+4);

								ASCIIEncoding enc = new ASCIIEncoding();
								msgHeader = enc.GetString(head);

								// check for content length
								int idx = msgHeader.IndexOf("Content-Length: ");
								if (idx != -1)
								{
									int pos = idx + "Content-Length: ".Length;
									string val = msgHeader.Substring(pos, msgHeader.Length - pos - 4);
									int len = Convert.ToInt32(val);
									if (totalBytesRecv >= (len + msgHeader.Length))
									{
										payload = new byte[len];
										Array.Copy(tmp,msgHeader.Length, payload, 0, len);
										done = true;
										break;
									}
								}
								else
								{
									done = true;
									break;
								}
							}
						}
						if (done)
							break;
					}
				}
				catch (SocketException se)
				{
					if (se.ErrorCode != 10035) //WSAEWOULDBLOCK
					{
						msgHeader = se.Message;
						break;
					}
				}
				catch (Exception ex)
				{
					msgHeader = ex.Message;
					break;
				}
			}

			if (keepAlive == false)
			{
				sock.Close();
				sock = null;
			}
		}

	}
}
