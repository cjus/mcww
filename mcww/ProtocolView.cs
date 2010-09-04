#region GPL_FILE_HEADER
/* ProtocolView.cs
   Copyright (C) 2006 Carlos Justiniano
   cjus@chessbrain.net, cjus34@yahoo.com, cjus@users.sourceforge.net

ProtocolView.cs is free software; you can redistribute it and/or
modify it under the terms of the GNU General Public License as
published by the Free Software Foundation; either version 2 of the
License, or (at your option) any later version.

ProtocolView.cs was developed by Carlos Justiniano for use on the
msgCourier project and the ChessBrain Project and is now distributed in
the hope that it will be useful, but WITHOUT ANY WARRANTY; without
even the implied warranty of MERCHANTABILITY or FITNESS FOR A
PARTICULAR PURPOSE.  See the GNU General Public License for more
details.

You should have received a copy of the GNU General Public License
along with ProtocolView.cs; if not, write to the Free Software 
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
	/// Summary description for ProtocolView.
	/// </summary>
	public class ProtocolView : System.Windows.Forms.Form
	{
		private System.Windows.Forms.RichTextBox richTextBox;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public ProtocolView()
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
			this.richTextBox = new System.Windows.Forms.RichTextBox();
			this.SuspendLayout();
			// 
			// richTextBox
			// 
			this.richTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
			this.richTextBox.Font = new System.Drawing.Font("Courier New", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.richTextBox.Location = new System.Drawing.Point(0, 0);
			this.richTextBox.Name = "richTextBox";
			this.richTextBox.ReadOnly = true;
			this.richTextBox.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.ForcedBoth;
			this.richTextBox.Size = new System.Drawing.Size(520, 189);
			this.richTextBox.TabIndex = 0;
			this.richTextBox.Text = "";
			this.richTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.richTextBox_KeyDown);
			// 
			// ProtocolView
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(520, 189);
			this.Controls.Add(this.richTextBox);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ProtocolView";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Protocol View";
			this.ResumeLayout(false);

		}
		#endregion

		public void SetContent(string goodSend, string goodRecv, string badRecv)
		{
			richTextBox.Text = goodSend;
			richTextBox.Select(0, goodSend.Length);
			richTextBox.SelectionColor = Color.DarkBlue;
				
			int iLoc = goodSend.Length - 4;

			if (goodRecv.Length > 0)
			{
				richTextBox.Text += goodRecv;
				richTextBox.Select(iLoc, richTextBox.Text.Length);
				richTextBox.SelectionColor = Color.DarkGreen;
				iLoc += badRecv.Length - 4;
			}

			if (badRecv.Length > 0)
			{
				richTextBox.Text += badRecv;
				richTextBox.Select(iLoc, richTextBox.Text.Length);
				richTextBox.SelectionColor = Color.DarkRed;
				iLoc += badRecv.Length - 4;
			}
			richTextBox.Select(0,0);
		}

		private void richTextBox_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Escape)
				this.Close();
		}
	}
}
