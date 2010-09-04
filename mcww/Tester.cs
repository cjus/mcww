#region GPL_FILE_HEADER
/* Tester.cs
   Copyright (C) 2005 Carlos Justiniano
   cjus@chessbrain.net, cjus34@yahoo.com, cjus@users.sourceforge.net

Tester.cs is free software; you can redistribute it and/or
modify it under the terms of the GNU General Public License as
published by the Free Software Foundation; either version 2 of the
License, or (at your option) any later version.

Tester.cs was developed by Carlos Justiniano for use on the
msgCourier project and the ChessBrain Project and is now distributed in
the hope that it will be useful, but WITHOUT ANY WARRANTY; without
even the implied warranty of MERCHANTABILITY or FITNESS FOR A
PARTICULAR PURPOSE.  See the GNU General Public License for more
details.

You should have received a copy of the GNU General Public License
along with Tester.cs; if not, write to the Free Software 
Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.
*/
#endregion

using System;
using System.Net;
using System.Windows.Forms;
using System.Threading;
using System.Collections;
using System.Text.RegularExpressions;
using Org.Mentalis.Network;

namespace mcww
{
	public class IndividualTest
	{
		public double responseTime;
		public bool fail;
	}

	public class TestDataElement
	{
		public string taskName;
		public ArrayList responseTimes = new ArrayList();
		public double avgResponseTime = 0.0;
		public double minResponseTime = double.MaxValue;
		public double maxResponseTime = double.MinValue;
		public double totalIterations;
		public int totPass;
		public int totFail;
	}

	/// <summary>
	/// Summary description for Tester.
	/// </summary>
	public class Tester
	{
		private MainForm mainForm;
		private TestTree testTree;
		private ListView listView;
		private ArrayList  testDataList;
		private ArrayList  threads = null;
		private int totalTasks = 0;
		private int completeTasks = 0;
		private object mutex = new object();
		private string currentTestName = "";

		public Tester(MainForm mainForm, TestTree testTree, ListView listView, ArrayList  testDataList)
		{
			this.mainForm = mainForm;
			this.testTree = testTree;
			this.listView = listView;
			this.testDataList = testDataList;
		}

		public void ExecuteSingleTest(string testName)
		{
			currentTestName = testName;
			TreeNode node = testTree.GetSelectedNodesParent();
			int threadCount = 0;
			int threadIterations = 0;
			int threadDelay = 0;
			int threadDelayRange1 = -1;
			int threadDelayRange2 = -1;
			int threadTimeout = 0;
			string messageType = "";
			string messageCommand = "";
			string messageHeaders = "";
			string messageAddr = "";
			string messagePort = "";
			string messageContentType = "";
			byte[]messagePayload = null;

			string taskName = node.Text;

			TreeNode nodeThreads = node.Nodes[0];
			TreeNode nodeMessage = node.Nodes[1];

			int nodeCount = node.Nodes.Count;
			for (int i=0; i< nodeCount; i++)
			{
				if (node.Nodes[i].Text == "Threads")
				{
					foreach (TreeNode threadChildNode in node.Nodes[i].Nodes)
					{
						if (threadChildNode.Text.IndexOf("Count:") != -1)
							threadCount = Convert.ToInt32(threadChildNode.Tag.ToString());
						else if (threadChildNode.Text.IndexOf("Iterations:") != -1)
							threadIterations = Convert.ToInt32(threadChildNode.Tag.ToString());
						else if (threadChildNode.Text.IndexOf("Delay:") != -1)
						{
							Regex expr = new Regex(@"-");
							string []array = expr.Split(threadChildNode.Tag.ToString());
							if (array.Length == 2)
							{
								threadDelay = -1;
								threadDelayRange1 = Convert.ToInt32(array[0].ToString().Trim());
								threadDelayRange2 = Convert.ToInt32(array[1].ToString().Trim());
							}
							else
							{
								threadDelay = Convert.ToInt32(threadChildNode.Tag.ToString().Trim());
								threadDelayRange1 = -1;
								threadDelayRange2 = -1;
							}
						}
						else if (threadChildNode.Text.IndexOf("Timeout:") != -1)
							threadTimeout = Convert.ToInt32(threadChildNode.Tag.ToString());
					}
				}
				else if (node.Nodes[i].Text == "Message")
				{
					foreach (TreeNode messageChildNode in node.Nodes[i].Nodes)
					{
						if (messageChildNode.Text.IndexOf("Command:") != -1)
							messageCommand = messageChildNode.Tag.ToString();
						else if (messageChildNode.Text.IndexOf("Headers:") != -1)
							messageHeaders = messageChildNode.Tag.ToString();
						else if (messageChildNode.Text.IndexOf("Address:") != -1)
							messageAddr = messageChildNode.Tag.ToString();
						else if (messageChildNode.Text.IndexOf("Port:") != -1)
							messagePort = messageChildNode.Tag.ToString();
						else if (messageChildNode.Text.IndexOf("Type:") != -1)
							messageType = messageChildNode.Tag.ToString();
						else if (messageChildNode.Text.IndexOf("Payload:") != -1)
						{
							TestTreeNodeTag tag = messageChildNode.Tag as TestTreeNodeTag;
							if (tag != null)
							{
								Payload payload = tag.userData as Payload;
								if (payload != null)
								{
									messageContentType = payload.MIMEType;
									messagePayload = payload.buffer;
								}
							}
						}
					}
				}
			}

			if (messageType.IndexOf("TCP/HTTP") != -1)
			{
				Url url = new Url(messageCommand);
				if (url.Protocol.Length != 0)
				{
					try
					{
						IPHostEntry hostInfo = Dns.Resolve(url.Host);
						IPAddress[] address = hostInfo.AddressList;
						messageAddr = address[0].ToString();
						messagePort = (url.Port == 0) ? "80" : url.Port.ToString();
						//messageCommand = url.Protocol + "://" + url.Host + "/" + url.Path;
					}
					catch (Exception ex)
					{
						MessageBox.Show(null, ex.Message, "MCWhirlWind",
							MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
					}
				}
			}

			// remove all items
			foreach (ListViewItem item in listView.Items)
				item.Remove();

			testDataList.Clear();
			threads = new ArrayList();
			Random rand = new Random();
			for (int i=0; i<threadCount; i++)
			{
				ListViewItem item = new ListViewItem();
				item.Text = i.ToString();		// 0 #
				item.SubItems.Add(taskName);	// 1 TaskName
				item.SubItems.Add("0");			// 2 %Complete
				item.SubItems.Add("0");			// 3 MinRes
				item.SubItems.Add("0");			// 4 MaxRes
				item.SubItems.Add("0");			// 5 AvgRes
				item.SubItems.Add("0");			// 6 Pass
				item.SubItems.Add("0");			// 7 Fail
				item.SubItems.Add("Pending..."); // 8 Status Message
				
				listView.Items.Add(item);

				ThreadAgentInfo threadInfo = new ThreadAgentInfo();
				item.Tag = threadInfo;

				threadInfo.threadCount = threadCount;
				threadInfo.threadIterations = threadIterations;
				if (threadDelay == -1)
					threadInfo.threadDelay = rand.Next(threadDelayRange1, threadDelayRange2); 
				else
					threadInfo.threadDelay = threadDelay;
				threadInfo.threadTimeout = threadTimeout;
				threadInfo.messageType = messageType;
				threadInfo.messageCommand = messageCommand;
				threadInfo.messageHeaders = messageHeaders;
				threadInfo.messageAddr = messageAddr;
				threadInfo.messagePort = messagePort;
				threadInfo.messageContentType = messageContentType;
				threadInfo.messagePayload = messagePayload;
				threadInfo.item = item;
				threadInfo.tester = this;
				threadInfo.testData = new TestDataElement();
				threadInfo.testData.taskName = node.Text;

				testDataList.Add(threadInfo.testData);
				threadInfo.msgLog = new ThreadAgentMsgLog();
				threadInfo.msgLog.goodSendHeaders = "";
				threadInfo.msgLog.goodRecvHeaders = "";
				threadInfo.msgLog.badRecvHeaders = "";
				ThreadAgent threadAgent = new ThreadAgent(threadInfo);
				Thread t = new Thread(new ThreadStart(threadAgent.Run));
				threads.Add(t);
				t.Start();
			}

			Thread.Sleep(0);

			totalTasks = threadCount;
			completeTasks = 0;
		}

		public void ExecuteAllTest()
		{
		}

		public void StopTest()
		{
			foreach (Thread t in threads)
				t.Abort();
		}

		public void IncTaskComplete()
		{
			lock (mutex)
			{
				completeTasks++;
				if (completeTasks == totalTasks)
				{
					mainForm.TasksCompleted();
				}
			}
		}

		public bool IsGridVisible()
		{
			return mainForm.IsGridVisible();
		}

		public bool IsUseNagel()
		{
			return mainForm.IsUseNagel();
		}

	}
}
