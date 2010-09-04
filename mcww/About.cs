#region GPL_FILE_HEADER
/* About.cs
   Copyright (C) 2005 Carlos Justiniano
   cjus@chessbrain.net, cjus34@yahoo.com, cjus@users.sourceforge.net

About.cs is free software; you can redistribute it and/or
modify it under the terms of the GNU General Public License as
published by the Free Software Foundation; either version 2 of the
License, or (at your option) any later version.

About.cs was developed by Carlos Justiniano for use on the
msgCourier project and the ChessBrain Project and is now distributed in
the hope that it will be useful, but WITHOUT ANY WARRANTY; without
even the implied warranty of MERCHANTABILITY or FITNESS FOR A
PARTICULAR PURPOSE.  See the GNU General Public License for more
details.

You should have received a copy of the GNU General Public License
along with About.cs; if not, write to the Free Software 
Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.
*/
#endregion

using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace mcww
{
	/// <summary>
	/// Summary description for About.
	/// </summary>
	public class About : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.LinkLabel linkLabelMgsCourierCom;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.PictureBox pictureBoxAboutBox;
		private System.Windows.Forms.RichTextBox richTextBoxAboutBox;
		private System.Windows.Forms.LinkLabel linkLabelZedGraph;
		private System.Windows.Forms.LinkLabel linkLabelGPL;
		private System.Windows.Forms.LinkLabel linkLabelEmailToAuthor;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public About()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(About));
			this.buttonOK = new System.Windows.Forms.Button();
			this.linkLabelMgsCourierCom = new System.Windows.Forms.LinkLabel();
			this.label2 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.pictureBoxAboutBox = new System.Windows.Forms.PictureBox();
			this.richTextBoxAboutBox = new System.Windows.Forms.RichTextBox();
			this.linkLabelZedGraph = new System.Windows.Forms.LinkLabel();
			this.linkLabelGPL = new System.Windows.Forms.LinkLabel();
			this.linkLabelEmailToAuthor = new System.Windows.Forms.LinkLabel();
			this.SuspendLayout();
			// 
			// buttonOK
			// 
			this.buttonOK.Location = new System.Drawing.Point(312, 104);
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.TabIndex = 0;
			this.buttonOK.Text = "OK";
			this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
			// 
			// linkLabelMgsCourierCom
			// 
			this.linkLabelMgsCourierCom.Cursor = System.Windows.Forms.Cursors.Hand;
			this.linkLabelMgsCourierCom.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
			this.linkLabelMgsCourierCom.Location = new System.Drawing.Point(144, 96);
			this.linkLabelMgsCourierCom.Name = "linkLabelMgsCourierCom";
			this.linkLabelMgsCourierCom.Size = new System.Drawing.Size(88, 16);
			this.linkLabelMgsCourierCom.TabIndex = 1;
			this.linkLabelMgsCourierCom.TabStop = true;
			this.linkLabelMgsCourierCom.Text = "msgCourier.com";
			this.linkLabelMgsCourierCom.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelMgsCourierCom_LinkClicked);
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(104, 96);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(48, 16);
			this.label2.TabIndex = 2;
			this.label2.Text = "Links:";
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(104, 8);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(192, 16);
			this.label4.TabIndex = 4;
			this.label4.Text = "MCWW version 1.0.0  (c) 2005-2006";
			// 
			// pictureBoxAboutBox
			// 
			this.pictureBoxAboutBox.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.pictureBoxAboutBox.Image = ((System.Drawing.Image)(resources.GetObject("pictureBoxAboutBox.Image")));
			this.pictureBoxAboutBox.Location = new System.Drawing.Point(8, 8);
			this.pictureBoxAboutBox.Name = "pictureBoxAboutBox";
			this.pictureBoxAboutBox.Size = new System.Drawing.Size(88, 112);
			this.pictureBoxAboutBox.TabIndex = 5;
			this.pictureBoxAboutBox.TabStop = false;
			// 
			// richTextBoxAboutBox
			// 
			this.richTextBoxAboutBox.BackColor = System.Drawing.SystemColors.Menu;
			this.richTextBoxAboutBox.Location = new System.Drawing.Point(104, 24);
			this.richTextBoxAboutBox.Name = "richTextBoxAboutBox";
			this.richTextBoxAboutBox.Size = new System.Drawing.Size(280, 64);
			this.richTextBoxAboutBox.TabIndex = 8;
			this.richTextBoxAboutBox.Text = "* MsgCourier WhirlWind is a testing framework for the msgCourier application serv" +
				"er.\n* MCWW uses ZedGraph (c) 2005 John Champion\n* MCWW is released under the GNU" +
				" Public License";
			// 
			// linkLabelZedGraph
			// 
			this.linkLabelZedGraph.Cursor = System.Windows.Forms.Cursors.Hand;
			this.linkLabelZedGraph.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
			this.linkLabelZedGraph.Location = new System.Drawing.Point(232, 96);
			this.linkLabelZedGraph.Name = "linkLabelZedGraph";
			this.linkLabelZedGraph.Size = new System.Drawing.Size(72, 16);
			this.linkLabelZedGraph.TabIndex = 9;
			this.linkLabelZedGraph.TabStop = true;
			this.linkLabelZedGraph.Text = "ZedGraph";
			this.linkLabelZedGraph.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelZedGraph_LinkClicked);
			// 
			// linkLabelGPL
			// 
			this.linkLabelGPL.Cursor = System.Windows.Forms.Cursors.Hand;
			this.linkLabelGPL.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
			this.linkLabelGPL.Location = new System.Drawing.Point(144, 112);
			this.linkLabelGPL.Name = "linkLabelGPL";
			this.linkLabelGPL.Size = new System.Drawing.Size(152, 16);
			this.linkLabelGPL.TabIndex = 10;
			this.linkLabelGPL.TabStop = true;
			this.linkLabelGPL.Text = "GNU General Public License";
			this.linkLabelGPL.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelGPL_LinkClicked);
			// 
			// linkLabelEmailToAuthor
			// 
			this.linkLabelEmailToAuthor.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
			this.linkLabelEmailToAuthor.Location = new System.Drawing.Point(288, 8);
			this.linkLabelEmailToAuthor.Name = "linkLabelEmailToAuthor";
			this.linkLabelEmailToAuthor.Size = new System.Drawing.Size(96, 16);
			this.linkLabelEmailToAuthor.TabIndex = 11;
			this.linkLabelEmailToAuthor.TabStop = true;
			this.linkLabelEmailToAuthor.Text = "Carlos Justiniano";
			this.linkLabelEmailToAuthor.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelEmailToAuthor_LinkClicked);
			// 
			// About
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(394, 135);
			this.Controls.Add(this.linkLabelEmailToAuthor);
			this.Controls.Add(this.linkLabelGPL);
			this.Controls.Add(this.linkLabelZedGraph);
			this.Controls.Add(this.richTextBoxAboutBox);
			this.Controls.Add(this.pictureBoxAboutBox);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.linkLabelMgsCourierCom);
			this.Controls.Add(this.buttonOK);
			this.Controls.Add(this.label2);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "About";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "About MsgCourier WhirlWind";
			this.ResumeLayout(false);

		}
		#endregion

		private void buttonOK_Click(object sender, System.EventArgs e)
		{
			this.Close();
		}

		private void linkLabelMgsCourierCom_LinkClicked(object sender, System.Windows.Forms.LinkLabelLinkClickedEventArgs e)
		{
			System.Diagnostics.Process.Start("IExplore","http://www.msgCourier.com");
			linkLabelMgsCourierCom.LinkVisited = true;
		}

		private void linkLabelZedGraph_LinkClicked(object sender, System.Windows.Forms.LinkLabelLinkClickedEventArgs e)
		{
			System.Diagnostics.Process.Start("IExplore","http://zedgraph.sourceforge.net/");
			linkLabelZedGraph.LinkVisited = true;	
		}

		private void linkLabelGPL_LinkClicked(object sender, System.Windows.Forms.LinkLabelLinkClickedEventArgs e)
		{		
			System.Diagnostics.Process.Start("IExplore","http://www.gnu.org/licenses/gpl.txt");
			linkLabelGPL.LinkVisited = true;	
		}

		private void linkLabelEmailToAuthor_LinkClicked(object sender, System.Windows.Forms.LinkLabelLinkClickedEventArgs e)
		{
			System.Diagnostics.Process.Start("IExplore","mailto://cjus@chessbrain.net");
			linkLabelEmailToAuthor.LinkVisited = true;		
		}
	}
}
