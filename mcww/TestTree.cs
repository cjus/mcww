#region GPL_FILE_HEADER
/* TestTree.cs
   Copyright (C) 2005 Carlos Justiniano
   cjus@chessbrain.net, cjus34@yahoo.com, cjus@users.sourceforge.net

TestTree.cs is free software; you can redistribute it and/or
modify it under the terms of the GNU General Public License as
published by the Free Software Foundation; either version 2 of the
License, or (at your option) any later version.

TestTree.cs was developed by Carlos Justiniano for use on the
msgCourier project and the ChessBrain Project and is now distributed in
the hope that it will be useful, but WITHOUT ANY WARRANTY; without
even the implied warranty of MERCHANTABILITY or FITNESS FOR A
PARTICULAR PURPOSE.  See the GNU General Public License for more
details.

You should have received a copy of the GNU General Public License
along with TestTree.cs; if not, write to the Free Software 
Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.
*/
#endregion

using System;
using System.Windows.Forms;
using System.Xml;

namespace mcww
{
	public class Payload
	{
		public string MIMEType;
		public byte []buffer = null;
	}

	public class TestTreeNodeTag
	{
		public string label;
		public object userData = null;
	}

	/// <summary>
	/// TestTree visual representation of test cases
	/// </summary>
	public class TestTree
	{
		private TreeView tree;
		private TreeNode testNode;
		private TreeNode currentNode;

		public TestTree(TreeView treeViewXML)
		{
			tree = treeViewXML;
		}

		public TreeNode GetTestNode()
		{
			return testNode;
		}

		public TreeNode GetSelectedNodesParent()
		{
			TreeNode node = tree.SelectedNode;
			if (node == null)
				return null;
			while (node.Parent != null)
			{
				node = node.Parent;
				tree.SelectedNode = node;
			}
			return node;
		}

		public void AddTestBranch(string description)
		{
			TreeNode newNode = new TreeNode(description);
			newNode.ImageIndex = 0;
			newNode.SelectedImageIndex = 0;
			TestTreeNodeTag nt = new TestTreeNodeTag();
			nt.label = "root";
			newNode.Tag = nt;
			tree.Nodes.Add(newNode);
			currentNode = newNode;
			testNode = currentNode;
		}

		public void AddThreadInfo(string count, string iterations, string delay, string timeout)
		{
			TreeNode newNode = currentNode.Nodes.Add("Threads");
			newNode.Tag = "label";
			newNode.ImageIndex = 1;
			newNode.SelectedImageIndex = 1;

			TreeNode newChildNode = newNode.Nodes.Add("Count: " + count);
			newChildNode.Tag = count;
			newChildNode.ImageIndex = 3;
			newChildNode.SelectedImageIndex = 3;

			newChildNode = newNode.Nodes.Add("Iterations: " + iterations);
			newChildNode.Tag = iterations;
			newChildNode.ImageIndex = 3;
			newChildNode.SelectedImageIndex = 3;

			newChildNode = newNode.Nodes.Add("Delay: " + delay);
			newChildNode.Tag = delay;
			newChildNode.ImageIndex = 3;
			newChildNode.SelectedImageIndex = 3;

			newChildNode = newNode.Nodes.Add("Timeout: " + timeout);
			newChildNode.Tag = timeout;
			newChildNode.ImageIndex = 3;
			newChildNode.SelectedImageIndex = 3;
		}

		public void AddMessage(string sType, string command, string headers, string address, string port, string payload, string mimeType)
		{
			TreeNode newNode = currentNode.Nodes.Add("Message");
			newNode.ImageIndex = 2;
			newNode.Tag = "label";
			newNode.SelectedImageIndex = 2;

			TreeNode newChildNode = newNode.Nodes.Add("Type: " + sType);
			newChildNode.Tag = sType;
			newChildNode.ImageIndex = 3;
			newChildNode.SelectedImageIndex = 3;

			newChildNode = newNode.Nodes.Add("Command: " + command);
			newChildNode.Tag = command;
			newChildNode.ImageIndex = 3;
			newChildNode.SelectedImageIndex = 3;

			newChildNode = newNode.Nodes.Add("Headers: " + headers);
			newChildNode.Tag = headers;
			newChildNode.ImageIndex = 3;
			newChildNode.SelectedImageIndex = 3;

			newChildNode = newNode.Nodes.Add("Address: " + address);
			newChildNode.Tag = address;
			newChildNode.ImageIndex = 3;
			newChildNode.SelectedImageIndex = 3;

			newChildNode = newNode.Nodes.Add("Port: " + port);
			newChildNode.Tag = port;
			newChildNode.ImageIndex = 3;
			newChildNode.SelectedImageIndex = 3;

			if (payload == "")
			{
				newChildNode = newNode.Nodes.Add("Payload: {empty}");
			}
			else
			{		
				TestTreeNodeTag tag = new TestTreeNodeTag();
				Payload payloadContent = new Payload();
				payloadContent.buffer = Convert.FromBase64String(payload);
				payloadContent.MIMEType = mimeType ;
				string prettyNumber = string.Format("{0:0,0}", payloadContent.buffer.Length);
				newChildNode = newNode.Nodes.Add("Payload: " + mimeType + " {" + prettyNumber + " bytes}");
				tag.userData = payloadContent;
				newChildNode.Tag = tag;
			}
			newChildNode.ImageIndex = 3;
			newChildNode.SelectedImageIndex = 3;
			newChildNode.Parent.Parent.Expand();
		}

		public void AddBehaviors(string behaviors)
		{
			TreeNode newNode = currentNode.Nodes.Add("Behaviors");
			newNode.ImageIndex = 2;
			newNode.Tag = "label";
			newNode.SelectedImageIndex = 2;

			string s;
			if (behaviors.Length == 0)
				s = "List: {unassigned}";
			else
				s = "List: {assigned}";
			TreeNode newChildNode = newNode.Nodes.Add(s);
			newChildNode.Tag = s;
			newChildNode.ImageIndex = 3;
			newChildNode.SelectedImageIndex = 3;
		}

		public void ExportTree(string fileName)
		{
			try
			{
				XmlDocument xmlDoc = new XmlDocument();
				XmlNode xmlNode = xmlDoc.CreateNode(XmlNodeType.XmlDeclaration,"","");
				xmlDoc.AppendChild(xmlNode);

				XmlElement xmlElem = xmlDoc.CreateElement("","mcww","");
				xmlElem.SetAttribute("version","","1.0");
				XmlNode xmlTopNode = xmlDoc.AppendChild(xmlElem);
				XmlNode xmlTestNode;
				
				TreeNode node = tree.Nodes[0];
				while (node != null)
				{
					xmlElem = xmlDoc.CreateElement("", "testcase", "");
					xmlElem.SetAttribute("description", "", node.Text);
					xmlTestNode = xmlTopNode.AppendChild(xmlElem);

					ProcessTestBranch(xmlDoc, xmlTestNode, node);
					node = node.NextNode;
				}

				xmlDoc.Save(fileName); 
			}
			catch (Exception ex)
			{
				MessageBox.Show(tree, "Problem exporting XML test document. " + ex.Message, "MCWhirlWind",
					MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void ProcessTestBranch(XmlDocument xmlDoc, XmlNode xmlNode, TreeNode node)
		{
			XmlElement xmlElem;
			foreach (TreeNode n in node.Nodes)
			{
				if (n.Text == "Threads")
				{
					xmlElem = xmlDoc.CreateElement("", "threads", "");
					XmlNode xmlThreadsNode = xmlNode.AppendChild(xmlElem);
					ProcessThreadsBranch(xmlDoc, xmlThreadsNode, n);
				}
				if (n.Text == "Message")
				{
					XmlNode xmlMessageNode = UpdateMessageNodeAttributes(xmlDoc, xmlNode, n.FirstNode.Text);
					ProcessMessageBranch(xmlDoc, xmlMessageNode, n);
				}
				if (n.Text == "Behaviors")
				{
					xmlElem = xmlDoc.CreateElement("", "behaviors", "");
					XmlNode xmlThreadsNode = xmlNode.AppendChild(xmlElem);
					ProcessBehaviorsBranch(xmlDoc, xmlThreadsNode, n);
				}
			}
		}

		private void ProcessThreadsBranch(XmlDocument xmlDoc, XmlNode xmlNode, TreeNode node)
		{
			XmlElement xmlElem;
			string keyName = "";
			string keyValue = "";
			foreach (TreeNode n in node.Nodes)
			{
				ParseNode(n.Text, ref keyName, ref keyValue);
				switch (keyName)
				{
					case "count":
						xmlElem = xmlDoc.CreateElement("", "count", "");
						xmlElem.InnerText = keyValue;
						xmlNode.AppendChild(xmlElem);
						break;
					case "iterations":
						xmlElem = xmlDoc.CreateElement("", "iterations", "");
						xmlElem.InnerText = keyValue;
						xmlNode.AppendChild(xmlElem);
						break;
					case "delay":
						xmlElem = xmlDoc.CreateElement("", "delay", "");
						xmlElem.InnerText = keyValue;
						xmlNode.AppendChild(xmlElem);
						break;
					case "timeout":
						xmlElem = xmlDoc.CreateElement("", "timeout", "");
						xmlElem.InnerText = keyValue;
						xmlNode.AppendChild(xmlElem);
						break;
				}	
			}
		}

		private void ProcessMessageBranch(XmlDocument xmlDoc, XmlNode xmlNode, TreeNode node)
		{
			XmlElement xmlElem;
			string keyName = "";
			string keyValue = "";
			foreach (TreeNode n in node.Nodes)
			{
				ParseNode(n.Text, ref keyName, ref keyValue);
				switch (keyName)
				{
					case "command":
						xmlElem = xmlDoc.CreateElement("", "command", "");
						xmlElem.InnerText = keyValue;
						xmlNode.AppendChild(xmlElem);
						break;
					case "headers":
						xmlElem = xmlDoc.CreateElement("", "headers", "");
						xmlElem.InnerText = keyValue;
						xmlNode.AppendChild(xmlElem);
						break;
					case "address":
						xmlElem = xmlDoc.CreateElement("", "address", "");
						xmlElem.InnerText = keyValue;
						xmlNode.AppendChild(xmlElem);
						break;
					case "port":
						xmlElem = xmlDoc.CreateElement("", "port", "");
						xmlElem.InnerText = keyValue;
						xmlNode.AppendChild(xmlElem);
						break;
					case "payload":
						xmlElem = xmlDoc.CreateElement("", "payload", "");
						if (n.Text.IndexOf("bytes") != -1)
						{
							TestTreeNodeTag tag = n.Tag as TestTreeNodeTag;
							Payload payload = tag.userData as Payload;
							if (payload != null)
							{
								xmlElem.SetAttribute("mimetype","",payload.MIMEType);
								xmlElem.InnerXml = "<base64>" + Convert.ToBase64String(payload.buffer) + "</base64>";
							}
						}
						xmlNode.AppendChild(xmlElem);
						break;
				}	
			}
		}

		private void ProcessBehaviorsBranch(XmlDocument xmlDoc, XmlNode xmlNode, TreeNode node)
		{
		}

		private void ParseNode(string s, ref string keyName, ref string keyValue)
		{
			int sep = s.IndexOf(":");
			if (sep != -1)
			{
				keyName = s.Substring(0, sep).ToLower();
				keyValue = s.Substring(sep+2);
			}
		}

		private XmlNode UpdateMessageNodeAttributes(XmlDocument xmlDoc, XmlNode xmlNode, string type)
		{
			string keyName = "";
			string keyValue = "";
			int sep = type.IndexOf(":");
			if (sep != -1)
			{
				ParseNode(type, ref keyName, ref keyValue);
				sep = keyValue.IndexOf("/");
				if (sep != -1)
				{				
					XmlElement xmlElem = xmlDoc.CreateElement("", "message", "");
					xmlElem.SetAttribute("protocol", "", keyValue);
					return xmlNode.AppendChild(xmlElem);
				}
			}
			return null;
		}
	}
}

