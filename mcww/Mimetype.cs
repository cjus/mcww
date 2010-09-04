#region GPL_FILE_HEADER
/* Mimetype.cs
   Copyright (C) 2005 Carlos Justiniano
   cjus@chessbrain.net, cjus34@yahoo.com, cjus@users.sourceforge.net

Mimetype.cs is free software; you can redistribute it and/or
modify it under the terms of the GNU General Public License as
published by the Free Software Foundation; either version 2 of the
License, or (at your option) any later version.

Mimetype.cs was developed by Carlos Justiniano for use on the
msgCourier project and the ChessBrain Project and is now distributed in
the hope that it will be useful, but WITHOUT ANY WARRANTY; without
even the implied warranty of MERCHANTABILITY or FITNESS FOR A
PARTICULAR PURPOSE.  See the GNU General Public License for more
details.

You should have received a copy of the GNU General Public License
along with Mimetype.cs; if not, write to the Free Software 
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
	/// Summary description for Mimetype.
	/// </summary>
	public class Mimetype : System.Windows.Forms.Form
	{
		private Hashtable mimeTypesTable;
		private string searchString;
		private string selectedMimeString;
		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ListView listViewMIMETypes;
		private System.Windows.Forms.ColumnHeader columnHeaderFileExt;
		private System.Windows.Forms.ColumnHeader columnHeaderMIMEType;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public string MIMETypeString
		{
			get { return selectedMimeString; }
		}

		#region ListViewComparer
		private class ListViewComparer : IComparer
		{
			private ListView listView;
			public ListViewComparer(ListView listView)
			{
				this.listView = listView;
			}
			public ListView ListView
			{
				get { return this.listView; }
			}

			private int column = 0;
			public int SortColumn
			{
				get { return column; }
				set { column = value; }
			}

			public int Compare(object a, object b)
			{
				ListViewItem item1 = (ListViewItem)a;
				ListViewItem item2 = (ListViewItem)b;

				if (listView.Sorting == SortOrder.Descending)
				{
					ListViewItem tmp = item1;
					item1 = item2;
					item2 = tmp;
				}

				ListViewItem.ListViewSubItem sub1 = item1.SubItems[0];
				ListViewItem.ListViewSubItem sub2 = item2.SubItems[0];
				return CaseInsensitiveComparer.Default.Compare(sub1.Text, sub2.Text);
			}
		}
		#endregion

		public Mimetype()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			InitTable();
		}

		public void LookupType(string fileName)
		{
			searchString = fileName;
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
			this.buttonOK = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.listViewMIMETypes = new System.Windows.Forms.ListView();
			this.columnHeaderFileExt = new System.Windows.Forms.ColumnHeader();
			this.columnHeaderMIMEType = new System.Windows.Forms.ColumnHeader();
			this.SuspendLayout();
			// 
			// buttonOK
			// 
			this.buttonOK.Location = new System.Drawing.Point(107, 200);
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.TabIndex = 2;
			this.buttonOK.Text = "OK";
			this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(8, 8);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(272, 48);
			this.label1.TabIndex = 3;
			this.label1.Text = "An attempt has been made to autoselect the MIME type for the data you\'re trying t" +
				"o import.   Change the MIME type to override the detected type.";
			// 
			// listViewMIMETypes
			// 
			this.listViewMIMETypes.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
																								this.columnHeaderFileExt,
																								this.columnHeaderMIMEType});
			this.listViewMIMETypes.FullRowSelect = true;
			this.listViewMIMETypes.GridLines = true;
			this.listViewMIMETypes.HideSelection = false;
			this.listViewMIMETypes.Location = new System.Drawing.Point(8, 56);
			this.listViewMIMETypes.MultiSelect = false;
			this.listViewMIMETypes.Name = "listViewMIMETypes";
			this.listViewMIMETypes.Size = new System.Drawing.Size(272, 128);
			this.listViewMIMETypes.Sorting = System.Windows.Forms.SortOrder.Ascending;
			this.listViewMIMETypes.TabIndex = 1;
			this.listViewMIMETypes.View = System.Windows.Forms.View.Details;

			// 
			// columnHeaderFileExt
			// 
			this.columnHeaderFileExt.Text = "FileExt";
			// 
			// columnHeaderMIMEType
			// 
			this.columnHeaderMIMEType.Text = "MIME Type";
			this.columnHeaderMIMEType.Width = 200;
			// 
			// Mimetype
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(288, 234);
			this.Controls.Add(this.listViewMIMETypes);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.buttonOK);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.Name = "Mimetype";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Select Mime type";
			this.Load += new System.EventHandler(this.Mimetype_Load);
			this.ResumeLayout(false);

		}
		#endregion

		private void Mimetype_Load(object sender, System.EventArgs e)
		{
			int idx = searchString.LastIndexOf(".");
			if (idx != 0)
			{
				string ext = (searchString.Substring(idx+1)).ToLower();
				string MIMEType = mimeTypesTable[ext] as string;				
				if (MIMEType == null)
					ext = "bin";
				// Perform binary search for ext
				int left = 0;
				int right = listViewMIMETypes.Items.Count;
				int mid;
				int compResult;
				while (left <= right)
				{
					mid = (left + right) / 2;
					ListViewItem item = listViewMIMETypes.Items[mid];
					compResult = string.Compare(item.Text, ext, true);
					if (compResult == 0)
					{
						listViewMIMETypes.Focus();
						listViewMIMETypes.EnsureVisible(mid);
						item.Selected = true;
						selectedMimeString = item.SubItems[1].Text;
						break;
					}
					else if (compResult == -1)
					{
						left = mid + 1;
					}
					else if (compResult == 1)
					{
						right = mid - 1;
					}
				}
			}
		}

		private void InitTable()
		{
			mimeTypesTable = new Hashtable();
			#region mimeTypesTable Table Entries
			mimeTypesTable["3dmf"] = "x-world/x-3dmf";
			mimeTypesTable["3dm"] = "x-world/x-3dmf";
			mimeTypesTable["a"] = "application/octet-stream";
			mimeTypesTable["aab"] = "application/x-authorware-bin";
			mimeTypesTable["aam"] = "application/x-authorware-map";
			mimeTypesTable["aas"] = "application/x-authorware-seg";
			mimeTypesTable["abc"] = "text/vnd.abc";
			mimeTypesTable["acgi"] = "text/html";
			mimeTypesTable["afl"] = "video/animaflex";
			mimeTypesTable["ai"] = "application/postscript";
			mimeTypesTable["aif"] = "audio/x-aiff";
			mimeTypesTable["aifc"] = "audio/x-aiff";
			mimeTypesTable["aiff"] = "audio/x-aiff";
			mimeTypesTable["aim"] = "application/x-aim";
			mimeTypesTable["aip"] = "text/x-audiosoft-intra";
			mimeTypesTable["ani"] = "application/x-navi-animation";
			mimeTypesTable["aos"] = "application/x-nokia-9000-communicator-add-on-software";
			mimeTypesTable["aps"] = "application/mime";
			mimeTypesTable["arc"] = "application/octet-stream";
			mimeTypesTable["arj"] = "application/octet-stream";
			mimeTypesTable["art"] = "image/x-jg";
			mimeTypesTable["asf"] = "video/x-ms-asf";
			mimeTypesTable["asm"] = "text/x-asm";
			mimeTypesTable["asp"] = "text/asp";
			mimeTypesTable["asx"] = "video/x-ms-asf";
			mimeTypesTable["au"] = "audio/x-au";
			mimeTypesTable["avi"] = "video/x-msvideo";
			mimeTypesTable["avs"] = "video/avs-video";
			mimeTypesTable["bcpio"] = "application/x-bcpio";
			mimeTypesTable["bin"] = "application/octet-stream";
			mimeTypesTable["bm"] = "image/bmp";
			mimeTypesTable["bmp"] = "image/x-windows-bmp";
			mimeTypesTable["boo"] = "application/book";
			mimeTypesTable["book"] = "application/book";
			mimeTypesTable["boz"] = "application/x-bzip2";
			mimeTypesTable["bsh"] = "application/x-bsh";
			mimeTypesTable["bz"] = "application/x-bzip";
			mimeTypesTable["bz2"] = "application/x-bzip2";
			mimeTypesTable["c"] = "text/plain";
			mimeTypesTable["c++"] = "text/plain";
			mimeTypesTable["cat"] = "application/vnd.ms-pki.seccat";
			mimeTypesTable["cc"] = "text/plain";
			mimeTypesTable["ccad"] = "application/clariscad";
			mimeTypesTable["cco"] = "application/x-cocoa";
			mimeTypesTable["cdf"] = "application/x-cdf";
			mimeTypesTable["cer"] = "application/x-x509-ca-cert";
			mimeTypesTable["cha"] = "application/x-chat";
			mimeTypesTable["chat"] = "application/x-chat";
			mimeTypesTable["class"] = "application/x-java-class";
			mimeTypesTable["com"] = "application/octet-stream";
			mimeTypesTable["conf"] = "text/plain";
			mimeTypesTable["cpio"] = "application/x-cpio";
			mimeTypesTable["cpp"] = "text/x-c";
			mimeTypesTable["cpt"] = "application/x-compactpro";
			mimeTypesTable["crl"] = "application/pkix-crl";
			mimeTypesTable["crt"] = "application/x-x509-ca-cert";
			mimeTypesTable["csh"] = "text/x-script.csh";
			mimeTypesTable["css"] = "text/css";
			mimeTypesTable["cxx"] = "text/plain";
			mimeTypesTable["dcr"] = "application/x-director";
			mimeTypesTable["deepv"] = "application/x-deepv";
			mimeTypesTable["def"] = "text/plain";
			mimeTypesTable["der"] = "application/x-x509-ca-cert";
			mimeTypesTable["dif"] = "video/x-dv";
			mimeTypesTable["dir"] = "application/x-director";
			mimeTypesTable["dl"] = "video/x-dl";
			mimeTypesTable["doc"] = "application/msword";
			mimeTypesTable["dot"] = "application/msword";
			mimeTypesTable["dp"] = "application/commonground";
			mimeTypesTable["drw"] = "application/drafting";
			mimeTypesTable["dump"] = "application/octet-stream";
			mimeTypesTable["dv"] = "video/x-dv";
			mimeTypesTable["dvi"] = "application/x-dvi";
			mimeTypesTable["dwf"] = "model/vnd.dwf";
			mimeTypesTable["dwg"] = "image/x-dwg";
			mimeTypesTable["dxf"] = "image/x-dwg";
			mimeTypesTable["dxr"] = "application/x-director";
			mimeTypesTable["el"] = "text/x-script.elisp";
			mimeTypesTable["elc"] = "application/x-elc";
			mimeTypesTable["env"] = "application/x-envoy";
			mimeTypesTable["eps"] = "application/postscript";
			mimeTypesTable["es"] = "application/x-esrehber";
			mimeTypesTable["etx"] = "text/x-setext";
			mimeTypesTable["evy"] = "application/x-envoy";
			mimeTypesTable["exe"] = "application/octet-stream";
			mimeTypesTable["f"] = "text/plain";
			mimeTypesTable["f"] = "text/x-fortran";
			mimeTypesTable["f77"] = "text/x-fortran";
			mimeTypesTable["f90"] = "text/plain";
			mimeTypesTable["f90"] = "text/x-fortran";
			mimeTypesTable["fdf"] = "application/vnd.fdf";
			mimeTypesTable["fif"] = "image/fif";
			mimeTypesTable["fli"] = "video/x-fli";
			mimeTypesTable["flo"] = "image/florian";
			mimeTypesTable["flx"] = "text/vnd.fmi.flexstor";
			mimeTypesTable["fmf"] = "video/x-atomic3d-feature";
			mimeTypesTable["for"] = "text/plain";
			mimeTypesTable["form"] = "application/x-www-form-urlencoded";
			mimeTypesTable["fpx"] = "image/vnd.net-fpx";
			mimeTypesTable["frl"] = "application/freeloader";
			mimeTypesTable["funk"] = "audio/make";
			mimeTypesTable["g"] = "text/plain";
			mimeTypesTable["g3"] = "image/g3fax";
			mimeTypesTable["gif"] = "image/gif";
			mimeTypesTable["gl"] = "video/x-gl";
			mimeTypesTable["gsd"] = "audio/x-gsm";
			mimeTypesTable["gsm"] = "audio/x-gsm";
			mimeTypesTable["gsp"] = "application/x-gsp";
			mimeTypesTable["gss"] = "application/x-gss";
			mimeTypesTable["gtar"] = "application/x-gtar";
			mimeTypesTable["gz"] = "application/x-gzip";
			mimeTypesTable["gzip"] = "application/x-gzip";
			mimeTypesTable["h"] = "text/plain";
			mimeTypesTable["hdf"] = "application/x-hdf";
			mimeTypesTable["help"] = "application/x-helpfile";
			mimeTypesTable["hgl"] = "application/vnd.hp-HPGL";
			mimeTypesTable["hh"] = "text/plain";
			mimeTypesTable["hlb"] = "text/x-script";
			mimeTypesTable["hlp"] = "application/x-winhelp";
			mimeTypesTable["hpg"] = "application/vnd.hp-HPGL";
			mimeTypesTable["hpgl"] = "application/vnd.hp-HPGL";
			mimeTypesTable["hqx"] = "application/x-mac-binhex40";
			mimeTypesTable["hta"] = "application/hta";
			mimeTypesTable["htm"] = "text/html";
			mimeTypesTable["html"] = "text/html";
			mimeTypesTable["htmls"] = "text/html";
			mimeTypesTable["htt"] = "text/webviewhtml";
			mimeTypesTable["htx"] = "text/html";
			mimeTypesTable["ice"] = "x-conference/x-cooltalk";
			mimeTypesTable["ico"] = "image/x-icon";
			mimeTypesTable["idc"] = "text/plain";
			mimeTypesTable["ief"] = "image/ief";
			mimeTypesTable["iefs"] = "image/ief";
			mimeTypesTable["iges"] = "application/iges";
			mimeTypesTable["igs"] = "application/iges";
			mimeTypesTable["ima"] = "application/x-ima";
			mimeTypesTable["imap"] = "application/x-httpd-imap";
			mimeTypesTable["inf"] = "application/inf";
			mimeTypesTable["ins"] = "application/x-internett-signup";
			mimeTypesTable["ip"] = "application/x-ip2";
			mimeTypesTable["isu"] = "video/x-isvideo";
			mimeTypesTable["it"] = "audio/it";
			mimeTypesTable["iv"] = "application/x-inventor";
			mimeTypesTable["ivr"] = "i-world/i-vrml";
			mimeTypesTable["ivy"] = "application/x-livescreen";
			mimeTypesTable["jam"] = "audio/x-jam";
			mimeTypesTable["jav"] = "text/plain";
			mimeTypesTable["java"] = "text/plain";
			mimeTypesTable["jcm"] = "application/x-java-commerce";
			mimeTypesTable["jfif"] = "image/jpeg";
			mimeTypesTable["jfif-tbnl"] = "image/jpeg";
			mimeTypesTable["jpe"] = "image/jpeg";
			mimeTypesTable["jpeg"] = "image/jpeg";
			mimeTypesTable["jpg"] = "image/jpeg";
			mimeTypesTable["jps"] = "image/x-jps";
			mimeTypesTable["js"] = "application/x-javascript";
			mimeTypesTable["jut"] = "image/jutvision";
			mimeTypesTable["kar"] = "music/x-karaoke";
			mimeTypesTable["ksh"] = "text/x-script.ksh";
			mimeTypesTable["la"] = "audio/x-nspaudio";
			mimeTypesTable["lam"] = "audio/x-liveaudio";
			mimeTypesTable["latex"] = "application/x-latex";
			mimeTypesTable["lha"] = "application/octet-stream";
			mimeTypesTable["lhx"] = "application/octet-stream";
			mimeTypesTable["list"] = "text/plain";
			mimeTypesTable["lma"] = "audio/x-nspaudio";
			mimeTypesTable["log"] = "text/plain";
			mimeTypesTable["lsp"] = "text/x-script.lisp";
			mimeTypesTable["lst"] = "text/plain";
			mimeTypesTable["lsx"] = "text/x-la-asf";
			mimeTypesTable["ltx"] = "application/x-latex";
			mimeTypesTable["lzh"] = "application/octet-stream";
			mimeTypesTable["lzx"] = "application/octet-stream";
			mimeTypesTable["m"] = "text/plain";
			mimeTypesTable["m1v"] = "video/mpeg";
			mimeTypesTable["m2a"] = "audio/mpeg";
			mimeTypesTable["m2v"] = "video/mpeg";
			mimeTypesTable["m3u"] = "audio/x-mpequrl";
			mimeTypesTable["man"] = "application/x-troff-man";
			mimeTypesTable["map"] = "application/x-navimap";
			mimeTypesTable["mar"] = "text/plain";
			mimeTypesTable["mbd"] = "application/mbedlet";
			mimeTypesTable["mc$"] = "application/x-magic-cap-package-1.0";
			mimeTypesTable["mcd"] = "application/x-mathcad";
			mimeTypesTable["mcf"] = "text/mcf";
			mimeTypesTable["mcp"] = "application/netmc";
			mimeTypesTable["me"] = "application/x-troff-me";
			mimeTypesTable["mht"] = "message/rfc822";
			mimeTypesTable["mhtml"] = "message/rfc822";
			mimeTypesTable["mid"] = "x-music/x-midi";
			mimeTypesTable["midi"] = "audio/x-midi";
			mimeTypesTable["mif"] = "application/x-mif";
			mimeTypesTable["mime"] = "www/mime";
			mimeTypesTable["mjf"] = "audio/x-vnd.AudioExplosion.MjuiceMediaFile";
			mimeTypesTable["mjpg"] = "video/x-motion-jpeg";
			mimeTypesTable["mm"] = "application/base64";
			mimeTypesTable["mme"] = "application/base64";
			mimeTypesTable["mod"] = "audio/x-mod";
			mimeTypesTable["moov"] = "video/quicktime";
			mimeTypesTable["mov"] = "video/quicktime";
			mimeTypesTable["movie"] = "video/x-sgi-movie";
			mimeTypesTable["mp2"] = "audio/x-mpeg";
			mimeTypesTable["mp3"] = "video/x-mpeg";
			mimeTypesTable["mpa"] = "audio/mpeg";
			mimeTypesTable["mpc"] = "application/x-project";
			mimeTypesTable["mpe"] = "video/mpeg";
			mimeTypesTable["mpeg"] = "video/mpeg";
			mimeTypesTable["mpg"] = "video/mpeg";
			mimeTypesTable["mpga"] = "audio/mpeg";
			mimeTypesTable["mpp"] = "application/vnd.ms-project";
			mimeTypesTable["mpt"] = "application/x-project";
			mimeTypesTable["mpv"] = "application/x-project";
			mimeTypesTable["mpx"] = "application/x-project";
			mimeTypesTable["mrc"] = "application/marc";
			mimeTypesTable["ms"] = "application/x-troff-ms";
			mimeTypesTable["mv"] = "video/x-sgi-movie";
			mimeTypesTable["my"] = "audio/make";
			mimeTypesTable["mzz"] = "application/x-vnd.AudioExplosion.mzz";
			mimeTypesTable["nap"] = "image/naplps";
			mimeTypesTable["naplps"] = "image/naplps";
			mimeTypesTable["nc"] = "application/x-netcdf";
			mimeTypesTable["ncm"] = "application/vnd.nokia.configuration-message";
			mimeTypesTable["nif"] = "image/x-niff";
			mimeTypesTable["niff"] = "image/x-niff";
			mimeTypesTable["nix"] = "application/x-mix-transfer";
			mimeTypesTable["nsc"] = "application/x-conference";
			mimeTypesTable["nvd"] = "application/x-navidoc";
			mimeTypesTable["o"] = "application/octet-stream";
			mimeTypesTable["oda"] = "application/oda";
			mimeTypesTable["omc"] = "application/x-omc";
			mimeTypesTable["omcd"] = "application/x-omcdatamaker";
			mimeTypesTable["omcr"] = "application/x-omcregerator";
			mimeTypesTable["p"] = "text/x-pascal";
			mimeTypesTable["p10"] = "application/x-pkcs10";
			mimeTypesTable["p12"] = "application/x-pkcs12";
			mimeTypesTable["p7a"] = "application/x-pkcs7-signature";
			mimeTypesTable["p7c"] = "application/x-pkcs7-mime";
			mimeTypesTable["p7m"] = "application/x-pkcs7-mime";
			mimeTypesTable["p7r"] = "application/x-pkcs7-certreqresp";
			mimeTypesTable["p7s"] = "application/pkcs7-signature";
			mimeTypesTable["part"] = "application/pro_eng";
			mimeTypesTable["pas"] = "text/pascal";
			mimeTypesTable["pbm"] = "image/x-portable-bitmap";
			mimeTypesTable["pcl"] = "application/x-pcl";
			mimeTypesTable["pct"] = "image/x-pict";
			mimeTypesTable["pcx"] = "image/x-pcx";
			mimeTypesTable["pdb "] = "chemical/x-pdb";
			mimeTypesTable["pdf"] = "application/pdf";
			mimeTypesTable["pfunk"] = "audio/make";
			mimeTypesTable["pfunk"] = "audio/make.my.funk";
			mimeTypesTable["pgm"] = "image/x-portable-graymap";
			mimeTypesTable["pgm"] = "image/x-portable-greymap";
			mimeTypesTable["pic"] = "image/pict";
			mimeTypesTable["pict"] = "image/pict";
			mimeTypesTable["pkg"] = "application/x-newton-compatible-pkg";
			mimeTypesTable["pko"] = "application/vnd.ms-pki.pko";
			mimeTypesTable["pl"] = "text/plain";
			mimeTypesTable["plx"] = "application/x-PiXCLscript";
			mimeTypesTable["pm"] = "image/x-xpixmap";
			mimeTypesTable["pm4"] = "application/x-pagemaker";
			mimeTypesTable["pm5"] = "application/x-pagemaker";
			mimeTypesTable["png"] = "image/png";
			mimeTypesTable["pnm"] = "application/x-portable-anymap";
			mimeTypesTable["pot"] = "application/vnd.ms-powerpoint";
			mimeTypesTable["pov"] = "model/x-pov";
			mimeTypesTable["ppa"] = "application/vnd.ms-powerpoint";
			mimeTypesTable["ppm"] = "image/x-portable-pixmap";
			mimeTypesTable["pps"] = "application/vnd.ms-powerpoint";
			mimeTypesTable["ppt"] = "application/vnd.ms-powerpoint";
			mimeTypesTable["ppz"] = "application/mspowerpoint";
			mimeTypesTable["pre"] = "application/x-freelance";
			mimeTypesTable["prt"] = "application/pro_eng";
			mimeTypesTable["ps"] = "application/postscript";
			mimeTypesTable["psd"] = "application/octet-stream";
			mimeTypesTable["pvu"] = "paleovu/x-pv";
			mimeTypesTable["pwz"] = "application/vnd.ms-powerpoint";
			mimeTypesTable["py"] = "text/x-script.phyton";
			mimeTypesTable["pyc"] = "applicaiton/x-bytecode.python";
			mimeTypesTable["qcp"] = "audio/vnd.qcelp";
			mimeTypesTable["qd3"] = "x-world/x-3dmf";
			mimeTypesTable["qd3d"] = "x-world/x-3dmf";
			mimeTypesTable["qif"] = "image/x-quicktime";
			mimeTypesTable["qt"] = "video/quicktime";
			mimeTypesTable["qtc"] = "video/x-qtc";
			mimeTypesTable["qti"] = "image/x-quicktime";
			mimeTypesTable["qtif"] = "image/x-quicktime";
			mimeTypesTable["ra"] = "audio/x-realaudio";
			mimeTypesTable["ram"] = "audio/x-pn-realaudio";
			mimeTypesTable["ras"] = "image/x-cmu-raster";
			mimeTypesTable["rast"] = "image/cmu-raster";
			mimeTypesTable["rexx"] = "text/x-script.rexx";
			mimeTypesTable["rf"] = "image/vnd.rn-realflash";
			mimeTypesTable["rgb"] = "image/x-rgb";
			mimeTypesTable["rm"] = "application/vnd.rn-realmedia";
			mimeTypesTable["rmi"] = "audio/mid";
			mimeTypesTable["rmm"] = "audio/x-pn-realaudio";
			mimeTypesTable["rmp"] = "audio/x-pn-realaudio";
			mimeTypesTable["rng"] = "application/ringing-tones";
			mimeTypesTable["rnx"] = "application/vnd.rn-realplayer";
			mimeTypesTable["roff"] = "application/x-troff";
			mimeTypesTable["rp"] = "image/vnd.rn-realpix";
			mimeTypesTable["rpm"] = "audio/x-pn-realaudio-plugin";
			mimeTypesTable["rt"] = "text/vnd.rn-realtext";
			mimeTypesTable["rtf"] = "text/richtext";
			mimeTypesTable["rtx"] = "text/richtext";
			mimeTypesTable["rv"] = "video/vnd.rn-realvideo";
			mimeTypesTable["s"] = "text/x-asm";
			mimeTypesTable["s3m"] = "audio/s3m";
			mimeTypesTable["saveme"] = "application/octet-stream";
			mimeTypesTable["sbk"] = "application/x-tbook";
			mimeTypesTable["scm"] = "text/x-script.scheme";
			mimeTypesTable["sdml"] = "text/plain";
			mimeTypesTable["sdp"] = "application/sdp";
			mimeTypesTable["sdp"] = "application/x-sdp";
			mimeTypesTable["sdr"] = "application/sounder";
			mimeTypesTable["sea"] = "application/sea";
			mimeTypesTable["sea"] = "application/x-sea";
			mimeTypesTable["set"] = "application/set";
			mimeTypesTable["sexpr"] = "text/s-expression";	//CJ: I added this.. I'm not sure if this is official!
			mimeTypesTable["sgm"] = "text/x-sgml";
			mimeTypesTable["sgml"] = "text/x-sgml";
			mimeTypesTable["sh"] = "text/x-script.sh";
			mimeTypesTable["shar"] = "application/x-bsh";
			mimeTypesTable["shtml"] = "text/x-server-parsed-html";
			mimeTypesTable["sid"] = "audio/x-psid";
			mimeTypesTable["sit"] = "application/x-sit";
			mimeTypesTable["skd"] = "application/x-koan";
			mimeTypesTable["skm"] = "application/x-koan";
			mimeTypesTable["skp"] = "application/x-koan";
			mimeTypesTable["skt"] = "application/x-koan";
			mimeTypesTable["sl"] = "application/x-seelogo";
			mimeTypesTable["smi"] = "application/smil";
			mimeTypesTable["smil"] = "application/smil";
			mimeTypesTable["snd"] = "audio/x-adpcm";
			mimeTypesTable["sol"] = "application/solids";
			mimeTypesTable["spc"] = "text/x-speech";
			mimeTypesTable["spl"] = "application/futuresplash";
			mimeTypesTable["spr"] = "application/x-sprite";
			mimeTypesTable["sprite"] = "application/x-sprite";
			mimeTypesTable["src"] = "application/x-wais-source";
			mimeTypesTable["ssi"] = "text/x-server-parsed-html";
			mimeTypesTable["ssm"] = "application/streamingmedia";
			mimeTypesTable["sst"] = "application/vnd.ms-pki.certstore";
			mimeTypesTable["step"] = "application/step";
			mimeTypesTable["stl"] = "application/x-navistyle";
			mimeTypesTable["stp"] = "application/step";
			mimeTypesTable["sv4cpio"] = "application/x-sv4cpio";
			mimeTypesTable["sv4crc"] = "application/x-sv4crc";
			mimeTypesTable["svf"] = "image/x-dwg";
			mimeTypesTable["svr"] = "x-world/x-svr";
			mimeTypesTable["swf"] = "application/x-shockwave-flash";
			mimeTypesTable["t"] = "application/x-troff";
			mimeTypesTable["talk"] = "text/x-speech";
			mimeTypesTable["tar"] = "application/x-tar";
			mimeTypesTable["tbk"] = "application/x-tbook";
			mimeTypesTable["tcl"] = "text/x-script.tcl";
			mimeTypesTable["tcsh"] = "text/x-script.tcsh";
			mimeTypesTable["tex"] = "application/x-tex";
			mimeTypesTable["texi"] = "application/x-texinfo";
			mimeTypesTable["texinfo"] = "application/x-texinfo";
			mimeTypesTable["text"] = "text/plain";
			mimeTypesTable["tgz"] = "application/x-compressed";
			mimeTypesTable["tif"] = "image/x-tiff";
			mimeTypesTable["tiff"] = "image/x-tiff";
			mimeTypesTable["tr"] = "application/x-troff";
			mimeTypesTable["tsi"] = "audio/tsp-audio";
			mimeTypesTable["tsp"] = "audio/tsplayer";
			mimeTypesTable["tsv"] = "text/tab-separated-values";
			mimeTypesTable["turbot"] = "image/florian";
			mimeTypesTable["txt"] = "text/plain";
			mimeTypesTable["uil"] = "text/x-uil";
			mimeTypesTable["uni"] = "text/uri-list";
			mimeTypesTable["unis"] = "text/uri-list";
			mimeTypesTable["unv"] = "application/i-deas";
			mimeTypesTable["uri"] = "text/uri-list";
			mimeTypesTable["uris"] = "text/uri-list";
			mimeTypesTable["ustar"] = "application/x-ustar";
			mimeTypesTable["uu"] = "text/x-uuencode";
			mimeTypesTable["uue"] = "text/x-uuencode";
			mimeTypesTable["vcd"] = "application/x-cdlink";
			mimeTypesTable["vcs"] = "text/x-vCalendar";
			mimeTypesTable["vda"] = "application/vda";
			mimeTypesTable["vdo"] = "video/vdo";
			mimeTypesTable["vew"] = "application/groupwise";
			mimeTypesTable["viv"] = "video/vnd.vivo";
			mimeTypesTable["vivo"] = "video/vnd.vivo";
			mimeTypesTable["vmd"] = "application/vocaltec-media-desc";
			mimeTypesTable["vmf"] = "application/vocaltec-media-file";
			mimeTypesTable["voc"] = "audio/voc";
			mimeTypesTable["voc"] = "audio/x-voc";
			mimeTypesTable["vos"] = "video/vosaic";
			mimeTypesTable["vox"] = "audio/voxware";
			mimeTypesTable["vqe"] = "audio/x-twinvq-plugin";
			mimeTypesTable["vqf"] = "audio/x-twinvq";
			mimeTypesTable["vql"] = "audio/x-twinvq-plugin";
			mimeTypesTable["vrml"] = "application/x-vrml";
			mimeTypesTable["vrt"] = "x-world/x-vrt";
			mimeTypesTable["vsd"] = "application/x-visio";
			mimeTypesTable["vst"] = "application/x-visio";
			mimeTypesTable["vsw"] = "application/x-visio";
			mimeTypesTable["w60"] = "application/wordperfect6.0";
			mimeTypesTable["w61"] = "application/wordperfect6.1";
			mimeTypesTable["w6w"] = "application/msword";
			mimeTypesTable["wav"] = "audio/x-wav";
			mimeTypesTable["wb1"] = "application/x-qpro";
			mimeTypesTable["wbmp"] = "image/vnd.wap.wbmp";
			mimeTypesTable["web"] = "application/vnd.xara";
			mimeTypesTable["wiz"] = "application/msword";
			mimeTypesTable["wk1"] = "application/x-123";
			mimeTypesTable["wmf"] = "windows/metafile";
			mimeTypesTable["wml"] = "text/vnd.wap.wml";
			mimeTypesTable["wmlc"] = "application/vnd.wap.wmlc";
			mimeTypesTable["wmls"] = "text/vnd.wap.wmlscript";
			mimeTypesTable["wmlsc"] = "application/vnd.wap.wmlscriptc";
			mimeTypesTable["word"] = "application/msword";
			mimeTypesTable["wp"] = "application/wordperfect";
			mimeTypesTable["wp5"] = "application/wordperfect";
			mimeTypesTable["wp6"] = "application/wordperfect";
			mimeTypesTable["wpd"] = "application/wordperfect";
			mimeTypesTable["wq1"] = "application/x-lotus";
			mimeTypesTable["wri"] = "application/mswrite";
			mimeTypesTable["wrl"] = "x-world/x-vrml";
			mimeTypesTable["wrz"] = "x-world/x-vrml";
			mimeTypesTable["wsc"] = "text/scriplet";
			mimeTypesTable["wsrc"] = "application/x-wais-source";
			mimeTypesTable["wtk"] = "application/x-wintalk";
			mimeTypesTable["x-png"] = "image/png";
			mimeTypesTable["xbm"] = "image/x-xbitmap";
			mimeTypesTable["xdr"] = "video/x-amt-demorun";
			mimeTypesTable["xgz"] = "xgl/drawing";
			mimeTypesTable["xif"] = "image/vnd.xiff";
			mimeTypesTable["xl"] = "application/excel";
			mimeTypesTable["xla"] = "application/x-msexcel";
			mimeTypesTable["xlb"] = "application/vnd.ms-excel";
			mimeTypesTable["xlc"] = "application/vnd.ms-excel";
			mimeTypesTable["xld"] = "application/x-excel";
			mimeTypesTable["xlk"] = "application/x-excel";
			mimeTypesTable["xll"] = "application/vnd.ms-excel";
			mimeTypesTable["xlm"] = "application/x-excel";
			mimeTypesTable["xls"] = "application/vnd.ms-excel";
			mimeTypesTable["xls"] = "application/x-msexcel";
			mimeTypesTable["xlt"] = "application/x-excel";
			mimeTypesTable["xlv"] = "application/x-excel";
			mimeTypesTable["xlw"] = "application/vnd.ms-excel";
			mimeTypesTable["xm"] = "audio/xm";
			mimeTypesTable["xml"] = "text/xml";
			mimeTypesTable["xmz"] = "xgl/movie";
			mimeTypesTable["xpix"] = "application/x-vnd.ls-xpix";
			mimeTypesTable["xpm"] = "image/x-xpixmap";
			mimeTypesTable["xsr"] = "video/x-amt-showrun";
			mimeTypesTable["xwd"] = "image/x-xwd";
			mimeTypesTable["xyz"] = "chemical/x-pdb";
			mimeTypesTable["z"] = "application/x-compress";
			mimeTypesTable["zip"] = "application/x-zip-compressed";
			mimeTypesTable["zoo"] = "application/octet-stream";
			mimeTypesTable["zsh"] = "text/x-script.zsh";
			#endregion

			IDictionaryEnumerator it = mimeTypesTable.GetEnumerator();
			while (it.MoveNext())
			{
				ListViewItem item = new ListViewItem();
				item.Text = it.Key.ToString();
				item.SubItems.Add(it.Value.ToString());
				listViewMIMETypes.Items.Add(item);
			}
		}

		private void buttonOK_Click(object sender, System.EventArgs e)
		{
			ListViewItem item = listViewMIMETypes.SelectedItems[0];
			selectedMimeString = item.SubItems[1].Text;
			this.Close();
		}

	}
}
