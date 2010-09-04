#region GPL_FILE_HEADER
/* XMLLoad.cs
   Copyright (C) 2005 Carlos Justiniano
   cjus@chessbrain.net, cjus34@yahoo.com, cjus@users.sourceforge.net

XMLLoad.cs is free software; you can redistribute it and/or
modify it under the terms of the GNU General Public License as
published by the Free Software Foundation; either version 2 of the
License, or (at your option) any later version.

XMLLoad.cs was developed by Carlos Justiniano for use on the
msgCourier project and the ChessBrain Project and is now distributed in
the hope that it will be useful, but WITHOUT ANY WARRANTY; without
even the implied warranty of MERCHANTABILITY or FITNESS FOR A
PARTICULAR PURPOSE.  See the GNU General Public License for more
details.

You should have received a copy of the GNU General Public License
along with XMLLoad.cs; if not, write to the Free Software 
Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.
*/
#endregion

using System;
using System.IO;
using System.Xml;
using System.Xml.XPath;
using System.Windows.Forms;
using System.Text;

namespace mcww
{
	/// <summary>
	/// XMLLoad handles loading of the XML text file 
	/// </summary>
	public class XMLLoad
	{
		private AppLogger appLog;
		private TestTree testTree;

		public XMLLoad(AppLogger appLog, TestTree testTree)
		{
			this.appLog = appLog;
			this.testTree = testTree;
		}

		public bool LoadTestXML(string fileName)
		{
			try
			{
				XmlDocument doc = new XmlDocument();
				XmlTextReader reader = new XmlTextReader(fileName);
				reader.WhitespaceHandling = WhitespaceHandling.None;
				reader.Read();
				doc.Load(reader);
				reader.Close();

				XmlNodeList nodes;
				nodes = doc.SelectNodes("mcww/testcase");
				if (nodes != null)
				{
					foreach(XmlNode node in nodes)
					{
						if (HandleTestCase(node) != true)
						{
							return false;
						}
					}
				}
			}
			catch(Exception ex)
			{
				appLog.Append(ex.Message);
				return false;
			}

			appLog.Append(fileName + " successfully loaded.");
			return true;
		}

		bool HandleTestCase(XmlNode testCaseNode)
		{
			string s;
			try
			{
				XmlAttribute nodeAttribute = testCaseNode.Attributes["description"];
				s = nodeAttribute.Value;
				testTree.AddTestBranch(s);

				if (testCaseNode.HasChildNodes == true)
				{
					for (int i=0; i< testCaseNode.ChildNodes.Count; i++)
					{
						XmlNode childNode = testCaseNode.ChildNodes[i];
						switch (childNode.Name)
						{
							case "threads":
								HandleThreadNode(childNode);
								break;
							case "message":
								HandleMessageNode(childNode);
								break;
							case "behaviors":
								HandleBehaviorsNode(childNode);
								break;
						};
					}
				}
			}
			catch(Exception ex)
			{
				appLog.Append("Error, HandleTestCase() failed. " + ex.Message);
				return false;
			}

			return true;
		}

		bool HandleThreadNode(XmlNode threadNode)
		{
			string count = "";
			string iterations = "";
			string delay = "";
			string timeout = "";
			XmlNode node = threadNode.SelectSingleNode("count");
			if (node != null)
				count = node.InnerText;
			node = threadNode.SelectSingleNode("iterations");
			if (node != null)
				iterations = node.InnerText;
			node = threadNode.SelectSingleNode("delay");
			if (node != null)
				delay = node.InnerText;
			node = threadNode.SelectSingleNode("timeout");
			if (node != null)
				timeout = node.InnerText;
			testTree.AddThreadInfo(count, iterations, delay, timeout);
			return true;
		}

		bool HandleMessageNode(XmlNode messageNode)
		{
			XmlAttribute nodeAttribute = messageNode.Attributes["protocol"];
			string sType = nodeAttribute.Value;

			string command = "";
			string headers = "";
			string address = "";
			string port = "";
			string payload = "";
			string mimeType = "";
			XmlNode node = messageNode.SelectSingleNode("command");
			if (node != null)
				command = node.InnerText;
			node = messageNode.SelectSingleNode("headers");
			if (node != null)
				headers = node.InnerText;
			node = messageNode.SelectSingleNode("address");
			if (node != null)
				address = node.InnerText;
			node = messageNode.SelectSingleNode("port");
			if (node != null)
				port = node.InnerText;
			node = messageNode.SelectSingleNode("payload");
			if (node != null)
			{
				if (node.Attributes.Count > 0)
				{
					payload = node.InnerText;
					mimeType = node.Attributes[0].InnerText;
				}
			}
			testTree.AddMessage(sType, command, headers, address, port, payload, mimeType);
			return true;
		}

		bool HandleBehaviorsNode(XmlNode behaviorNode)
		{
			StringBuilder sb = new StringBuilder();			
			XmlNode node = behaviorNode.SelectSingleNode("normal");
			if (node != null)
				sb.Append("normal ");
			node = behaviorNode.SelectSingleNode("delayed_disconnect");
			if (node != null)
				sb.Append("delayed_disconnect ");
			node = behaviorNode.SelectSingleNode("instant_disconnect");
			if (node != null)
				sb.Append("instant_disconnect ");
			node = behaviorNode.SelectSingleNode("corrupt_header");
			if (node != null)
				sb.Append("corrupt_header ");
			node = behaviorNode.SelectSingleNode("corrupt_payload");
			if (node != null)
				sb.Append("corrupt_payload ");
			testTree.AddBehaviors(sb.ToString());
			return true;
		}
	}
}
