#region GPL_FILE_HEADER
/* ThreadAgent.cs
   Copyright (C) 2005 Carlos Justiniano
   cjus@chessbrain.net, cjus34@yahoo.com, cjus@users.sourceforge.net

ThreadAgent.cs is free software; you can redistribute it and/or
modify it under the terms of the GNU General Public License as
published by the Free Software Foundation; either version 2 of the
License, or (at your option) any later version.

ThreadAgent.cs was developed by Carlos Justiniano for use on the
msgCourier project and the ChessBrain Project and is now distributed in
the hope that it will be useful, but WITHOUT ANY WARRANTY; without
even the implied warranty of MERCHANTABILITY or FITNESS FOR A
PARTICULAR PURPOSE.  See the GNU General Public License for more
details.

You should have received a copy of the GNU General Public License
along with ThreadAgent.cs; if not, write to the Free Software 
Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.
*/
#endregion

using System;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using Win32;

namespace mcww
{
	public class ThreadAgentMsgLog
	{
		public string goodSendHeaders;
		public string goodRecvHeaders;
		public string badRecvHeaders;
	}

	public class ThreadAgentInfo
	{
		public int threadCount;
		public double threadIterations;
		public int threadDelay;
		public int threadTimeout;
		public string messageType;
		public string messageCommand;
		public string messageHeaders;
		public string messageAddr;
		public string messagePort;
		public string messageContentType;
		public byte[]messagePayload;
		public ListViewItem item;
		public Tester tester;
		public TestDataElement testData;
		public ThreadAgentMsgLog msgLog;
		public Socket sock = null;
	}

	/// <summary>
	/// Summary description for ThreadAgent.
	/// </summary>
	public class ThreadAgent
	{
		private ThreadAgentInfo threadInfo;
		private int fail = 0;
		private int pass = 0;
		private HiPerfTimer perfTimer = new HiPerfTimer();

		public ThreadAgent(ThreadAgentInfo threadInfo)
		{
			this.threadInfo = threadInfo;
		}

		public void Run()
		{
			#region vars
			ListViewItem.ListViewSubItem subItem;
			double elapsedResponseTimes = 0;
			double iterations = 0;
			double percentComplete = 0;
			double avgElapsedTime = 0;
			double loop = threadInfo.threadIterations;
			bool isHTTPGet = (threadInfo.messageType == "TCP/HTTP-GET") ? true : false;
			bool isHTTPPost = (threadInfo.messageType == "TCP/HTTP-POST") ? true : false;
			bool stickyError = false;
			Message msg = null;
			HTTPMessage httpMsg = null;
			string response;
			#endregion

			try
			{
				while (loop > 0)
				{
					if (isHTTPGet == false && isHTTPPost == false)
					{
						#region Do msgCourier
						msg = new Message();
						msg.SocketObj = threadInfo.sock;
						msg.CommandLine = threadInfo.messageCommand;						
						if (threadInfo.messageHeaders.Length > 0)
							msg.SetAdditionalFields(PrepareAdditionalFields(threadInfo.messageHeaders));
						msg.Port = Convert.ToInt32(threadInfo.messagePort);
						msg.ServerAddress = threadInfo.messageAddr;
						if (threadInfo.messageType == "TCP/MCP")
							msg.Transport = "tcp";
						else if (threadInfo.messageType == "UDP/MCP")
							msg.Transport = "udp";
	
						if (threadInfo.messagePayload != null && threadInfo.messageContentType != null)
							msg.SetPayload(threadInfo.messageContentType, threadInfo.messagePayload);

						msg.EnableNagel = threadInfo.tester.IsUseNagel();
						string additionalFields;
						additionalFields = "Options: reply-wait, notify-no\r\n";
						msg.SetAdditionalFields(additionalFields);
						#endregion
					}
					else
					{
						#region Do HTTP
						httpMsg = new HTTPMessage();
						httpMsg.SocketObj = threadInfo.sock;
						httpMsg.HTTPVerb = (isHTTPGet == true) ? "GET" : "POST";						
						httpMsg.CommandLine = threadInfo.messageCommand;
						if (threadInfo.messageHeaders.Length > 0)
							httpMsg.SetAdditionalFields(PrepareAdditionalFields(threadInfo.messageHeaders));
						httpMsg.Port = Convert.ToInt32(threadInfo.messagePort);
						httpMsg.ServerAddress = threadInfo.messageAddr;
						if (threadInfo.messagePayload != null && threadInfo.messageContentType != null)
							httpMsg.SetPayload(threadInfo.messageContentType, threadInfo.messagePayload);
						httpMsg.EnableNagel = threadInfo.tester.IsUseNagel();
						#endregion
					}

					bool success = false;
					perfTimer.Start();
	
					if (isHTTPGet == false && isHTTPPost == false)
					{
						msg.Send(Convert.ToInt32(threadInfo.threadTimeout));
						response = msg.GetResponse();
						success = (response.StartsWith("MCP")==true) ? true : false;
						if (success == true)
						{
							if (threadInfo.msgLog.goodRecvHeaders.Length == 0)
							{
								threadInfo.msgLog.goodSendHeaders = msg.SendHeaders;
								threadInfo.msgLog.goodRecvHeaders = response;
							}
						}
						else
						{
							if (threadInfo.msgLog.badRecvHeaders.Length == 0)
							{
								threadInfo.msgLog.goodSendHeaders = msg.SendHeaders;
								threadInfo.msgLog.badRecvHeaders = response;
							}
						}
						threadInfo.sock = msg.SocketObj;
					}
					else
					{
						httpMsg.Send(Convert.ToInt32(threadInfo.threadTimeout));
						response = httpMsg.GetResponse();
						if (response.StartsWith("HTTP/1.1 200")==true ||
							response.StartsWith("HTTP/1.1 304")==true)
						{
							success = true;
							if (threadInfo.msgLog.goodRecvHeaders == "")
							{
								threadInfo.msgLog.goodSendHeaders = httpMsg.SendHeaders;
								threadInfo.msgLog.goodRecvHeaders = response;
							}
						}
						else
						{
							success = false;
							if (threadInfo.msgLog.badRecvHeaders == "")
							{
								threadInfo.msgLog.goodSendHeaders = httpMsg.SendHeaders;
								threadInfo.msgLog.badRecvHeaders = response;
							}
						}
						threadInfo.sock = httpMsg.SocketObj;
					}

					#region Display results
					perfTimer.Stop();
					double dur = perfTimer.Duration * 1000.0;

//					bool bShow = threadInfo.tester.IsGridVisible();
//					if (++iterations >= threadInfo.threadIterations-1)
//						bShow = true;
					++iterations;
					bool bShow = true;
					IndividualTest it = new IndividualTest();
					it.responseTime = dur;
					it.fail = !success;
					threadInfo.testData.responseTimes.Add(it);

					elapsedResponseTimes += dur;
					if (dur < threadInfo.testData.minResponseTime)
						threadInfo.testData.minResponseTime = dur;
					if (dur > threadInfo.testData.maxResponseTime)
						threadInfo.testData.maxResponseTime = dur;

					avgElapsedTime = (elapsedResponseTimes / iterations);

					if (bShow == true)
					{
						threadInfo.item.SubItems[3].Text = string.Format("{0:f2}", threadInfo.testData.minResponseTime);
						threadInfo.item.SubItems[4].Text = string.Format("{0:f2}", threadInfo.testData.maxResponseTime);				
						subItem = threadInfo.item.SubItems[5];
						subItem.Text = string.Format("{0:f2}", avgElapsedTime);
					}

					if (success == false)
					{
						fail++;
						if (bShow == true)
						{
							threadInfo.item.SubItems[7].Text = fail.ToString();
							threadInfo.item.SubItems[8].Text = response;
						}
						stickyError = true;
					}
					else
					{
						pass++;
						if (bShow == true)
						{
							threadInfo.item.SubItems[6].Text = pass.ToString();
							if (stickyError == false)
								threadInfo.item.SubItems[8].Text = "Processed";
						}
					}

					percentComplete = ((iterations / threadInfo.threadIterations) * 100.0);

					if (bShow == true)
						threadInfo.item.SubItems[2].Text = percentComplete.ToString();

					threadInfo.testData.avgResponseTime = avgElapsedTime;
					threadInfo.testData.totalIterations = threadInfo.threadIterations;
					threadInfo.testData.totPass = pass;
					threadInfo.testData.totFail = fail;

					#endregion

					if (threadInfo.threadDelay > 0)
						Thread.Sleep(threadInfo.threadDelay);
					else
						Thread.Sleep(1);
					loop--;
				}
			}
			catch (Exception ex)
			{
				threadInfo.item.SubItems[8].Text = ex.Message;
			}

			threadInfo.testData.avgResponseTime = avgElapsedTime;
			threadInfo.testData.totalIterations = threadInfo.threadIterations;
			threadInfo.testData.totPass = pass;
			threadInfo.testData.totFail = fail;

			threadInfo.tester.IncTaskComplete();
		}

		private string PrepareAdditionalFields(string headers)
		{
			string sHeader = "";
			string []sArray = headers.Split("|".ToCharArray());
			if (sArray.Length != 0)
			{
				foreach (string s in sArray)
				{
					if (s.Length > 0)
					{
						sHeader += s;
						sHeader += "\r\n";
					}
				}
			}
			return sHeader;
		}

	}

}
