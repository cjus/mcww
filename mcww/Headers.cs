using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace mcww
{
	/// <summary>
	/// Summary description for Headers.
	/// </summary>
	public class Headers : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox textBoxHeaderName;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox textBoxHeaderValue;
		private System.Windows.Forms.Button buttonAddHeader;
		private System.Windows.Forms.ListBox listBoxHeaders;
		private System.Windows.Forms.Button buttonRemoveHeader;
		private System.Windows.Forms.Button buttonDone;
		private System.Windows.Forms.Button buttonCancel;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public Headers()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
		}

		public void PrepopulateItems(string headers)
		{
			string sHeader = headers.Remove(0,9);
			sHeader = sHeader.Replace("||","");
			string []sArray = sHeader.Split("|".ToCharArray());
			if (sArray.Length != 0)
			{
				foreach (string s in sArray)
				{
					if (s.Length >1 && s != "|")
						listBoxHeaders.Items.Add(s);
				}
			}
		}

		public string GetHeaders()
		{
			string sHeaders = "";
			foreach (string s in listBoxHeaders.Items)
			{
				sHeaders += s;
				sHeaders += "|";
			}
			return sHeaders.Replace("||","");
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
			this.label1 = new System.Windows.Forms.Label();
			this.textBoxHeaderName = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.textBoxHeaderValue = new System.Windows.Forms.TextBox();
			this.buttonAddHeader = new System.Windows.Forms.Button();
			this.listBoxHeaders = new System.Windows.Forms.ListBox();
			this.buttonRemoveHeader = new System.Windows.Forms.Button();
			this.buttonDone = new System.Windows.Forms.Button();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(8, 16);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(48, 16);
			this.label1.TabIndex = 0;
			this.label1.Text = "Header:";
			// 
			// textBoxHeaderName
			// 
			this.textBoxHeaderName.Location = new System.Drawing.Point(56, 13);
			this.textBoxHeaderName.Name = "textBoxHeaderName";
			this.textBoxHeaderName.Size = new System.Drawing.Size(184, 20);
			this.textBoxHeaderName.TabIndex = 1;
			this.textBoxHeaderName.Text = "";
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(8, 40);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(40, 16);
			this.label2.TabIndex = 2;
			this.label2.Text = "Value:";
			// 
			// textBoxHeaderValue
			// 
			this.textBoxHeaderValue.Location = new System.Drawing.Point(56, 37);
			this.textBoxHeaderValue.Name = "textBoxHeaderValue";
			this.textBoxHeaderValue.Size = new System.Drawing.Size(184, 20);
			this.textBoxHeaderValue.TabIndex = 2;
			this.textBoxHeaderValue.Text = "";
			// 
			// buttonAddHeader
			// 
			this.buttonAddHeader.Location = new System.Drawing.Point(165, 64);
			this.buttonAddHeader.Name = "buttonAddHeader";
			this.buttonAddHeader.TabIndex = 3;
			this.buttonAddHeader.Text = "Add";
			this.buttonAddHeader.Click += new System.EventHandler(this.buttonAddHeader_Click);
			// 
			// listBoxHeaders
			// 
			this.listBoxHeaders.Location = new System.Drawing.Point(8, 96);
			this.listBoxHeaders.Name = "listBoxHeaders";
			this.listBoxHeaders.Size = new System.Drawing.Size(232, 95);
			this.listBoxHeaders.TabIndex = 4;
			// 
			// buttonRemoveHeader
			// 
			this.buttonRemoveHeader.Location = new System.Drawing.Point(165, 199);
			this.buttonRemoveHeader.Name = "buttonRemoveHeader";
			this.buttonRemoveHeader.TabIndex = 5;
			this.buttonRemoveHeader.Text = "Remove";
			this.buttonRemoveHeader.Click += new System.EventHandler(this.buttonRemoveHeader_Click);
			// 
			// buttonDone
			// 
			this.buttonDone.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.buttonDone.Location = new System.Drawing.Point(32, 240);
			this.buttonDone.Name = "buttonDone";
			this.buttonDone.TabIndex = 6;
			this.buttonDone.Text = "OK";
			this.buttonDone.Click += new System.EventHandler(this.buttonDone_Click);
			// 
			// buttonCancel
			// 
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Location = new System.Drawing.Point(144, 240);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.TabIndex = 7;
			this.buttonCancel.Text = "Cancel";
			this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
			// 
			// Headers
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(250, 279);
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.buttonDone);
			this.Controls.Add(this.buttonRemoveHeader);
			this.Controls.Add(this.listBoxHeaders);
			this.Controls.Add(this.buttonAddHeader);
			this.Controls.Add(this.textBoxHeaderValue);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.textBoxHeaderName);
			this.Controls.Add(this.label1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "Headers";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Headers";
			this.ResumeLayout(false);

		}
		#endregion

		private void buttonAddHeader_Click(object sender, System.EventArgs e)
		{
			if (textBoxHeaderName.Text.Length == 0 || textBoxHeaderValue.Text.Length == 0)
			{
				MessageBox.Show(this, "Header and Value fields must contain text.", "MCWhirlWind",
					MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}

			if (textBoxHeaderName.Text.IndexOf(":") != -1 || textBoxHeaderValue.Text.IndexOf(":") != -1)
			{
				MessageBox.Show(this, "Header and Value fields can't contains colon symbols.", "MCWhirlWind",
					MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}

			listBoxHeaders.Items.Add(textBoxHeaderName.Text + ": " + textBoxHeaderValue.Text);
			textBoxHeaderName.Text = "";
			textBoxHeaderValue.Text = "";
		}

		private void buttonRemoveHeader_Click(object sender, System.EventArgs e)
		{
			if (listBoxHeaders.SelectedIndex == -1)
			{
				MessageBox.Show(this, "You must first select an item to remove.", "MCWhirlWind",
					MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}
			listBoxHeaders.Items.RemoveAt(listBoxHeaders.SelectedIndex);
		}

		private void buttonDone_Click(object sender, System.EventArgs e)
		{
			this.Close();
		}

		private void buttonCancel_Click(object sender, System.EventArgs e)
		{
			this.Close();
		}
	}
}
