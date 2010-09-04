#region GPL_FILE_HEADER
/* MainForm.cs
   Copyright (C) 2005 Carlos Justiniano
   cjus@chessbrain.net, cjus34@yahoo.com, cjus@users.sourceforge.net

MainForm.cs is free software; you can redistribute it and/or
modify it under the terms of the GNU General Public License as
published by the Free Software Foundation; either version 2 of the
License, or (at your option) any later version.

MainForm.cs was developed by Carlos Justiniano for use on the
msgCourier project and the ChessBrain Project and is now distributed in
the hope that it will be useful, but WITHOUT ANY WARRANTY; without
even the implied warranty of MERCHANTABILITY or FITNESS FOR A
PARTICULAR PURPOSE.  See the GNU General Public License for more
details.

You should have received a copy of the GNU General Public License
along with MainForm.cs; if not, write to the Free Software 
Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.
*/
#endregion

using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.IO;
using System.Text.RegularExpressions;

using mshtml;

namespace mcww
{
	/// <summary>
	/// Application mainform
	/// </summary>
	public class MainForm : System.Windows.Forms.Form
	{
		#region App variables
		private AppLogger appLog;
		private TestTree testTree;
		private Tester tester;
		private ArrayList testDataList = new ArrayList();
		private ReportGenerator reportGenerator;
		private bool modified = false;
		private bool tasksAborted = false;
		private bool timerOn = false;
		private DateTime testStartTime;
		private ListViewComparer listViewcomparer;
		private const int idColumn = 0;
		private const int taskNameColumn = 1;
		private const int percentCompleteColumn = 2;
		private const int minResColumn = 3;
		private const int maxResColumn = 4;
		private const int avgResColumn = 5;
		private const int passColumn = 6;
		private const int failColumn = 7;
		private const int statusColumn = 8;
		private string currentTestName = "";
		private bool useNagel = false;
		#endregion

		#region Form variables
		private System.Windows.Forms.StatusBar statusBar;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.TabControl tabControl;
		private System.Windows.Forms.TabPage tabPageMain;
		private System.Windows.Forms.Panel panel2;
		private System.Windows.Forms.Splitter splitterVert;
		private System.Windows.Forms.ToolBar toolBar;
		private System.Windows.Forms.TreeView treeViewXML;
		private System.Windows.Forms.ListBox listBoxAppLog;
		private System.Windows.Forms.ListView listViewThreads;
		private System.Windows.Forms.MenuItem menuItemFile;
		private System.Windows.Forms.MenuItem menuItemFileExit;
		private System.Windows.Forms.MenuItem menuItemFileLoadXML;
		private System.Windows.Forms.MenuItem menuItemFileSaveXML;
		private System.Windows.Forms.MenuItem menuItemHelp;
		private System.Windows.Forms.MenuItem menuItemHelpAbout;
		private System.Windows.Forms.MainMenu mainMenu;
		private System.Windows.Forms.OpenFileDialog openFileDialog;
		private System.Windows.Forms.SaveFileDialog saveFileDialog;
		private System.ComponentModel.IContainer components;

		private System.Windows.Forms.ImageList imageListTreeView;
		private System.Windows.Forms.ImageList imageListToolBar;
		private System.Windows.Forms.ToolBarButton toolBarButtonOpen;
		private System.Windows.Forms.ToolBarButton toolBarButtonSave;
		private System.Windows.Forms.ToolBarButton toolBarButtonSep;
		private System.Windows.Forms.ToolBarButton toolBarButtonPlayAll;
		private System.Windows.Forms.ToolBarButton toolBarButtonPlay;
		private System.Windows.Forms.ToolBarButton toolBarButtonStop;

		private System.Windows.Forms.ColumnHeader columnHeaderNum;
		private System.Windows.Forms.ColumnHeader columnHeaderTaskName;
		private System.Windows.Forms.ColumnHeader columnHeaderPercentComplete;
		private System.Windows.Forms.ColumnHeader columnHeaderMinRes;
		private System.Windows.Forms.ColumnHeader columnHeaderMaxRes;
		private System.Windows.Forms.ColumnHeader columnHeaderAvgRes;
		private System.Windows.Forms.ColumnHeader columnHeaderPass;
		private System.Windows.Forms.ColumnHeader columnHeaderFail;
		private System.Windows.Forms.ColumnHeader columnHeaderStatusMessage;

		private System.Windows.Forms.StatusBarPanel statusBarPanelTime;
		private System.Windows.Forms.StatusBarPanel statusBarPanelMessage;
		private System.Windows.Forms.StatusBarPanel statusBarPanelTimer;
		private System.Windows.Forms.MenuItem menuItemActions;
		private System.Windows.Forms.MenuItem menuItemActionsRunall;
		private System.Windows.Forms.MenuItem menuItemActionsRunSingle;
		private System.Windows.Forms.ToolBarButton toolBarButtonAddTest;
		private System.Windows.Forms.ToolBarButton toolBarButtonRemoveTest;
		private System.Windows.Forms.ToolBarButton toolBarButtonSep2;
		private System.Windows.Forms.MenuItem menuItemFileSep1;
		private System.Windows.Forms.MenuItem menuItemActionsStop;
		private System.Windows.Forms.MenuItem menuItemActionsAddTest;
		private System.Windows.Forms.MenuItem menuItemActionsRemoveTest;
		private System.Windows.Forms.ContextMenu contextMenuLogView;
		private System.Windows.Forms.MenuItem menuItemAppLogClearLog;
		private System.Windows.Forms.MenuItem menuItemAppLogExportLog;
		private System.Windows.Forms.ContextMenu contextMenuTreeViewXML;
		private System.Windows.Forms.MenuItem menuItemTreeViewAddTest;
		private System.Windows.Forms.MenuItem menuItemTreeViewRemoveTest;
		private System.Windows.Forms.MenuItem menuItemTreeViewRunAllTests;
		private System.Windows.Forms.MenuItem menuItemTreeViewRunSingleTest;
		private System.Windows.Forms.MenuItem menuItem7;
		private System.Windows.Forms.MenuItem menuItemTreeViewStopTests;
		private System.Windows.Forms.Splitter splitterHorz;
		private System.Windows.Forms.ContextMenu contextMenuMessageType;
		private System.Windows.Forms.MenuItem menuItemTCPMCP;
		private System.Windows.Forms.MenuItem menuItemUDPMCP;
		private System.Windows.Forms.TabPage tabPageReport;
		private System.Windows.Forms.MenuItem menuItemTreeViewCopyRawData;
		private System.Windows.Forms.MenuItem menuItem2;
		private System.Windows.Forms.MenuItem menuItemReportPrint;
		private System.Windows.Forms.ContextMenu contextMenuPayload;
		private System.Windows.Forms.MenuItem menuItemPayloadImport;
		private System.Windows.Forms.MenuItem menuItemPayloadRemove;
		private System.Windows.Forms.MenuItem menuItemPayloadExport;
		private System.Windows.Forms.OpenFileDialog openFileDialogPayload;
		private System.Windows.Forms.SaveFileDialog saveFileDialogPayload;
		private System.Windows.Forms.MenuItem menuItemImportXML;
		private System.Windows.Forms.MenuItem menuItemTCPHTTPGet;
		private System.Windows.Forms.MenuItem menuItemTCPHTTPPost;
		private AxSHDocVw.AxWebBrowser axWebBrowser;
		private System.Windows.Forms.RichTextBox richTextBox;
		private System.Windows.Forms.MenuItem menuItemReport;
		private System.Windows.Forms.MenuItem menuItemOptions;
		private System.Windows.Forms.MenuItem menuItemOptionsDisableNagel;
		private System.Windows.Forms.Timer timer;
		#endregion

		#region MainForm
		public MainForm(string param)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			// Setup report geneator
			reportGenerator = new ReportGenerator(this);

			// Setup AppLogger and testTree objects
			appLog = new AppLogger(listBoxAppLog);
			testTree = new TestTree(treeViewXML);
			tester = new Tester(this, testTree, listViewThreads, testDataList);
			testStartTime = new DateTime(DateTime.Now.Ticks);
			statusBarPanelTimer.Text = "00:00:00:000";

			if (param.Length > 3)
			{
				XMLLoad loader = new XMLLoad(appLog, testTree);
				loader.LoadTestXML(param);

				menuItemActionsRunSingle.Enabled = true;
				
				EnableTreeNodeControlItems(true);
				treeViewXML.Select();
			}
		}

		protected override void OnLoad(EventArgs e)
		{
			listViewcomparer = new ListViewComparer(listViewThreads);
			listViewThreads.ListViewItemSorter = listViewcomparer;
			listViewThreads.Sorting = SortOrder.Ascending;
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
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
			this.components = new System.ComponentModel.Container();
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(MainForm));
			this.statusBar = new System.Windows.Forms.StatusBar();
			this.statusBarPanelMessage = new System.Windows.Forms.StatusBarPanel();
			this.statusBarPanelTimer = new System.Windows.Forms.StatusBarPanel();
			this.statusBarPanelTime = new System.Windows.Forms.StatusBarPanel();
			this.mainMenu = new System.Windows.Forms.MainMenu();
			this.menuItemFile = new System.Windows.Forms.MenuItem();
			this.menuItemFileLoadXML = new System.Windows.Forms.MenuItem();
			this.menuItemImportXML = new System.Windows.Forms.MenuItem();
			this.menuItemFileSaveXML = new System.Windows.Forms.MenuItem();
			this.menuItemFileSep1 = new System.Windows.Forms.MenuItem();
			this.menuItemFileExit = new System.Windows.Forms.MenuItem();
			this.menuItemActions = new System.Windows.Forms.MenuItem();
			this.menuItemActionsRunall = new System.Windows.Forms.MenuItem();
			this.menuItemActionsRunSingle = new System.Windows.Forms.MenuItem();
			this.menuItemActionsStop = new System.Windows.Forms.MenuItem();
			this.menuItem2 = new System.Windows.Forms.MenuItem();
			this.menuItemActionsAddTest = new System.Windows.Forms.MenuItem();
			this.menuItemActionsRemoveTest = new System.Windows.Forms.MenuItem();
			this.menuItemReport = new System.Windows.Forms.MenuItem();
			this.menuItemReportPrint = new System.Windows.Forms.MenuItem();
			this.menuItemOptions = new System.Windows.Forms.MenuItem();
			this.menuItemOptionsDisableNagel = new System.Windows.Forms.MenuItem();
			this.menuItemHelp = new System.Windows.Forms.MenuItem();
			this.menuItemHelpAbout = new System.Windows.Forms.MenuItem();
			this.toolBar = new System.Windows.Forms.ToolBar();
			this.toolBarButtonOpen = new System.Windows.Forms.ToolBarButton();
			this.toolBarButtonSave = new System.Windows.Forms.ToolBarButton();
			this.toolBarButtonSep = new System.Windows.Forms.ToolBarButton();
			this.toolBarButtonAddTest = new System.Windows.Forms.ToolBarButton();
			this.toolBarButtonRemoveTest = new System.Windows.Forms.ToolBarButton();
			this.toolBarButtonSep2 = new System.Windows.Forms.ToolBarButton();
			this.toolBarButtonPlayAll = new System.Windows.Forms.ToolBarButton();
			this.toolBarButtonPlay = new System.Windows.Forms.ToolBarButton();
			this.toolBarButtonStop = new System.Windows.Forms.ToolBarButton();
			this.imageListToolBar = new System.Windows.Forms.ImageList(this.components);
			this.treeViewXML = new System.Windows.Forms.TreeView();
			this.contextMenuTreeViewXML = new System.Windows.Forms.ContextMenu();
			this.menuItemTreeViewAddTest = new System.Windows.Forms.MenuItem();
			this.menuItemTreeViewRemoveTest = new System.Windows.Forms.MenuItem();
			this.menuItem7 = new System.Windows.Forms.MenuItem();
			this.menuItemTreeViewRunAllTests = new System.Windows.Forms.MenuItem();
			this.menuItemTreeViewRunSingleTest = new System.Windows.Forms.MenuItem();
			this.menuItemTreeViewStopTests = new System.Windows.Forms.MenuItem();
			this.menuItemTreeViewCopyRawData = new System.Windows.Forms.MenuItem();
			this.imageListTreeView = new System.Windows.Forms.ImageList(this.components);
			this.panel1 = new System.Windows.Forms.Panel();
			this.tabControl = new System.Windows.Forms.TabControl();
			this.tabPageMain = new System.Windows.Forms.TabPage();
			this.panel2 = new System.Windows.Forms.Panel();
			this.listViewThreads = new System.Windows.Forms.ListView();
			this.columnHeaderNum = new System.Windows.Forms.ColumnHeader();
			this.columnHeaderTaskName = new System.Windows.Forms.ColumnHeader();
			this.columnHeaderPercentComplete = new System.Windows.Forms.ColumnHeader();
			this.columnHeaderMinRes = new System.Windows.Forms.ColumnHeader();
			this.columnHeaderMaxRes = new System.Windows.Forms.ColumnHeader();
			this.columnHeaderAvgRes = new System.Windows.Forms.ColumnHeader();
			this.columnHeaderPass = new System.Windows.Forms.ColumnHeader();
			this.columnHeaderFail = new System.Windows.Forms.ColumnHeader();
			this.columnHeaderStatusMessage = new System.Windows.Forms.ColumnHeader();
			this.richTextBox = new System.Windows.Forms.RichTextBox();
			this.splitterHorz = new System.Windows.Forms.Splitter();
			this.listBoxAppLog = new System.Windows.Forms.ListBox();
			this.contextMenuLogView = new System.Windows.Forms.ContextMenu();
			this.menuItemAppLogClearLog = new System.Windows.Forms.MenuItem();
			this.menuItemAppLogExportLog = new System.Windows.Forms.MenuItem();
			this.tabPageReport = new System.Windows.Forms.TabPage();
			this.axWebBrowser = new AxSHDocVw.AxWebBrowser();
			this.splitterVert = new System.Windows.Forms.Splitter();
			this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
			this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
			this.timer = new System.Windows.Forms.Timer(this.components);
			this.contextMenuMessageType = new System.Windows.Forms.ContextMenu();
			this.menuItemTCPMCP = new System.Windows.Forms.MenuItem();
			this.menuItemUDPMCP = new System.Windows.Forms.MenuItem();
			this.menuItemTCPHTTPGet = new System.Windows.Forms.MenuItem();
			this.menuItemTCPHTTPPost = new System.Windows.Forms.MenuItem();
			this.contextMenuPayload = new System.Windows.Forms.ContextMenu();
			this.menuItemPayloadImport = new System.Windows.Forms.MenuItem();
			this.menuItemPayloadRemove = new System.Windows.Forms.MenuItem();
			this.menuItemPayloadExport = new System.Windows.Forms.MenuItem();
			this.openFileDialogPayload = new System.Windows.Forms.OpenFileDialog();
			this.saveFileDialogPayload = new System.Windows.Forms.SaveFileDialog();
			((System.ComponentModel.ISupportInitialize)(this.statusBarPanelMessage)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.statusBarPanelTimer)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.statusBarPanelTime)).BeginInit();
			this.panel1.SuspendLayout();
			this.tabControl.SuspendLayout();
			this.tabPageMain.SuspendLayout();
			this.panel2.SuspendLayout();
			this.tabPageReport.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.axWebBrowser)).BeginInit();
			this.SuspendLayout();
			// 
			// statusBar
			// 
			this.statusBar.Location = new System.Drawing.Point(0, 455);
			this.statusBar.Name = "statusBar";
			this.statusBar.Panels.AddRange(new System.Windows.Forms.StatusBarPanel[] {
																						 this.statusBarPanelMessage,
																						 this.statusBarPanelTimer,
																						 this.statusBarPanelTime});
			this.statusBar.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.statusBar.ShowPanels = true;
			this.statusBar.Size = new System.Drawing.Size(712, 22);
			this.statusBar.TabIndex = 0;
			// 
			// statusBarPanelMessage
			// 
			this.statusBarPanelMessage.AutoSize = System.Windows.Forms.StatusBarPanelAutoSize.Spring;
			this.statusBarPanelMessage.BorderStyle = System.Windows.Forms.StatusBarPanelBorderStyle.None;
			this.statusBarPanelMessage.Text = "Ready";
			this.statusBarPanelMessage.Width = 676;
			// 
			// statusBarPanelTimer
			// 
			this.statusBarPanelTimer.Alignment = System.Windows.Forms.HorizontalAlignment.Center;
			this.statusBarPanelTimer.AutoSize = System.Windows.Forms.StatusBarPanelAutoSize.Contents;
			this.statusBarPanelTimer.Width = 10;
			// 
			// statusBarPanelTime
			// 
			this.statusBarPanelTime.Alignment = System.Windows.Forms.HorizontalAlignment.Center;
			this.statusBarPanelTime.AutoSize = System.Windows.Forms.StatusBarPanelAutoSize.Contents;
			this.statusBarPanelTime.Width = 10;
			// 
			// mainMenu
			// 
			this.mainMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					 this.menuItemFile,
																					 this.menuItemActions,
																					 this.menuItemReport,
																					 this.menuItemOptions,
																					 this.menuItemHelp});
			// 
			// menuItemFile
			// 
			this.menuItemFile.Index = 0;
			this.menuItemFile.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																						 this.menuItemFileLoadXML,
																						 this.menuItemImportXML,
																						 this.menuItemFileSaveXML,
																						 this.menuItemFileSep1,
																						 this.menuItemFileExit});
			this.menuItemFile.Shortcut = System.Windows.Forms.Shortcut.CtrlF;
			this.menuItemFile.Text = "File";
			// 
			// menuItemFileLoadXML
			// 
			this.menuItemFileLoadXML.Index = 0;
			this.menuItemFileLoadXML.Shortcut = System.Windows.Forms.Shortcut.CtrlL;
			this.menuItemFileLoadXML.Text = "Load";
			this.menuItemFileLoadXML.Click += new System.EventHandler(this.menuItemFileLoadXML_Click);
			// 
			// menuItemImportXML
			// 
			this.menuItemImportXML.Index = 1;
			this.menuItemImportXML.Shortcut = System.Windows.Forms.Shortcut.CtrlI;
			this.menuItemImportXML.Text = "Import";
			this.menuItemImportXML.Click += new System.EventHandler(this.menuItemImportXML_Click);
			// 
			// menuItemFileSaveXML
			// 
			this.menuItemFileSaveXML.Index = 2;
			this.menuItemFileSaveXML.Shortcut = System.Windows.Forms.Shortcut.CtrlS;
			this.menuItemFileSaveXML.Text = "Save";
			this.menuItemFileSaveXML.Click += new System.EventHandler(this.menuItemFileSaveXML_Click);
			// 
			// menuItemFileSep1
			// 
			this.menuItemFileSep1.Index = 3;
			this.menuItemFileSep1.Text = "-";
			// 
			// menuItemFileExit
			// 
			this.menuItemFileExit.Index = 4;
			this.menuItemFileExit.Shortcut = System.Windows.Forms.Shortcut.CtrlX;
			this.menuItemFileExit.Text = "Exit";
			this.menuItemFileExit.Click += new System.EventHandler(this.menuItemFileExit_Click);
			// 
			// menuItemActions
			// 
			this.menuItemActions.Index = 1;
			this.menuItemActions.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																							this.menuItemActionsRunall,
																							this.menuItemActionsRunSingle,
																							this.menuItemActionsStop,
																							this.menuItem2,
																							this.menuItemActionsAddTest,
																							this.menuItemActionsRemoveTest});
			this.menuItemActions.Text = "Actions";
			// 
			// menuItemActionsRunall
			// 
			this.menuItemActionsRunall.Enabled = false;
			this.menuItemActionsRunall.Index = 0;
			this.menuItemActionsRunall.Shortcut = System.Windows.Forms.Shortcut.CtrlA;
			this.menuItemActionsRunall.Text = "Run all tests";
			this.menuItemActionsRunall.Click += new System.EventHandler(this.menuItemActionsRunall_Click);
			// 
			// menuItemActionsRunSingle
			// 
			this.menuItemActionsRunSingle.Enabled = false;
			this.menuItemActionsRunSingle.Index = 1;
			this.menuItemActionsRunSingle.Shortcut = System.Windows.Forms.Shortcut.CtrlR;
			this.menuItemActionsRunSingle.Text = "Run single test";
			this.menuItemActionsRunSingle.Click += new System.EventHandler(this.menuItemActionsRunSingle_Click);
			// 
			// menuItemActionsStop
			// 
			this.menuItemActionsStop.Enabled = false;
			this.menuItemActionsStop.Index = 2;
			this.menuItemActionsStop.Shortcut = System.Windows.Forms.Shortcut.CtrlB;
			this.menuItemActionsStop.Text = "Stop test";
			this.menuItemActionsStop.Click += new System.EventHandler(this.menuItemActionsStop_Click);
			// 
			// menuItem2
			// 
			this.menuItem2.Index = 3;
			this.menuItem2.Text = "-";
			// 
			// menuItemActionsAddTest
			// 
			this.menuItemActionsAddTest.Index = 4;
			this.menuItemActionsAddTest.Shortcut = System.Windows.Forms.Shortcut.CtrlI;
			this.menuItemActionsAddTest.Text = "Add test";
			this.menuItemActionsAddTest.Click += new System.EventHandler(this.menuItemActionsAddTest_Click);
			// 
			// menuItemActionsRemoveTest
			// 
			this.menuItemActionsRemoveTest.Enabled = false;
			this.menuItemActionsRemoveTest.Index = 5;
			this.menuItemActionsRemoveTest.Shortcut = System.Windows.Forms.Shortcut.CtrlD;
			this.menuItemActionsRemoveTest.Text = "Remove test";
			this.menuItemActionsRemoveTest.Click += new System.EventHandler(this.menuItemActionsRemoveTest_Click);
			// 
			// menuItemReport
			// 
			this.menuItemReport.Index = 2;
			this.menuItemReport.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																						   this.menuItemReportPrint});
			this.menuItemReport.Text = "Report";
			// 
			// menuItemReportPrint
			// 
			this.menuItemReportPrint.Enabled = false;
			this.menuItemReportPrint.Index = 0;
			this.menuItemReportPrint.Shortcut = System.Windows.Forms.Shortcut.CtrlP;
			this.menuItemReportPrint.Text = "Print";
			this.menuItemReportPrint.Click += new System.EventHandler(this.menuItemReportPrint_Click);
			// 
			// menuItemOptions
			// 
			this.menuItemOptions.Index = 3;
			this.menuItemOptions.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																							this.menuItemOptionsDisableNagel});
			this.menuItemOptions.Text = "Options";
			// 
			// menuItemOptionsDisableNagel
			// 
			this.menuItemOptionsDisableNagel.Checked = true;
			this.menuItemOptionsDisableNagel.Index = 0;
			this.menuItemOptionsDisableNagel.Text = "Disable Nagel";
			this.menuItemOptionsDisableNagel.Click += new System.EventHandler(this.menuItemOptionsDisableNagel_Click);
			// 
			// menuItemHelp
			// 
			this.menuItemHelp.Index = 4;
			this.menuItemHelp.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																						 this.menuItemHelpAbout});
			this.menuItemHelp.Shortcut = System.Windows.Forms.Shortcut.CtrlH;
			this.menuItemHelp.Text = "Help";
			// 
			// menuItemHelpAbout
			// 
			this.menuItemHelpAbout.Index = 0;
			this.menuItemHelpAbout.Shortcut = System.Windows.Forms.Shortcut.F1;
			this.menuItemHelpAbout.Text = "About";
			this.menuItemHelpAbout.Click += new System.EventHandler(this.menuItemHelpAbout_Click);
			// 
			// toolBar
			// 
			this.toolBar.Appearance = System.Windows.Forms.ToolBarAppearance.Flat;
			this.toolBar.AutoSize = false;
			this.toolBar.Buttons.AddRange(new System.Windows.Forms.ToolBarButton[] {
																					   this.toolBarButtonOpen,
																					   this.toolBarButtonSave,
																					   this.toolBarButtonSep,
																					   this.toolBarButtonAddTest,
																					   this.toolBarButtonRemoveTest,
																					   this.toolBarButtonSep2,
																					   this.toolBarButtonPlayAll,
																					   this.toolBarButtonPlay,
																					   this.toolBarButtonStop});
			this.toolBar.ButtonSize = new System.Drawing.Size(18, 18);
			this.toolBar.DropDownArrows = true;
			this.toolBar.ImageList = this.imageListToolBar;
			this.toolBar.Location = new System.Drawing.Point(0, 0);
			this.toolBar.Name = "toolBar";
			this.toolBar.ShowToolTips = true;
			this.toolBar.Size = new System.Drawing.Size(712, 24);
			this.toolBar.TabIndex = 1;
			this.toolBar.ButtonClick += new System.Windows.Forms.ToolBarButtonClickEventHandler(this.toolBar_ButtonClick);
			// 
			// toolBarButtonOpen
			// 
			this.toolBarButtonOpen.ImageIndex = 0;
			this.toolBarButtonOpen.ToolTipText = "Load Test";
			// 
			// toolBarButtonSave
			// 
			this.toolBarButtonSave.ImageIndex = 1;
			this.toolBarButtonSave.ToolTipText = "Save Test";
			// 
			// toolBarButtonSep
			// 
			this.toolBarButtonSep.Style = System.Windows.Forms.ToolBarButtonStyle.Separator;
			// 
			// toolBarButtonAddTest
			// 
			this.toolBarButtonAddTest.ImageIndex = 5;
			// 
			// toolBarButtonRemoveTest
			// 
			this.toolBarButtonRemoveTest.Enabled = false;
			this.toolBarButtonRemoveTest.ImageIndex = 6;
			// 
			// toolBarButtonSep2
			// 
			this.toolBarButtonSep2.Style = System.Windows.Forms.ToolBarButtonStyle.Separator;
			// 
			// toolBarButtonPlayAll
			// 
			this.toolBarButtonPlayAll.Enabled = false;
			this.toolBarButtonPlayAll.ImageIndex = 3;
			this.toolBarButtonPlayAll.ToolTipText = "Run all test";
			// 
			// toolBarButtonPlay
			// 
			this.toolBarButtonPlay.Enabled = false;
			this.toolBarButtonPlay.ImageIndex = 2;
			this.toolBarButtonPlay.ToolTipText = "Run Test";
			// 
			// toolBarButtonStop
			// 
			this.toolBarButtonStop.Enabled = false;
			this.toolBarButtonStop.ImageIndex = 4;
			this.toolBarButtonStop.ToolTipText = "Stop tests";
			// 
			// imageListToolBar
			// 
			this.imageListToolBar.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
			this.imageListToolBar.ImageSize = new System.Drawing.Size(16, 16);
			this.imageListToolBar.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageListToolBar.ImageStream")));
			this.imageListToolBar.TransparentColor = System.Drawing.Color.Magenta;
			// 
			// treeViewXML
			// 
			this.treeViewXML.ContextMenu = this.contextMenuTreeViewXML;
			this.treeViewXML.Dock = System.Windows.Forms.DockStyle.Left;
			this.treeViewXML.HideSelection = false;
			this.treeViewXML.ImageList = this.imageListTreeView;
			this.treeViewXML.Indent = 19;
			this.treeViewXML.LabelEdit = true;
			this.treeViewXML.Location = new System.Drawing.Point(0, 24);
			this.treeViewXML.Name = "treeViewXML";
			this.treeViewXML.Size = new System.Drawing.Size(152, 431);
			this.treeViewXML.TabIndex = 2;
			this.treeViewXML.KeyDown += new System.Windows.Forms.KeyEventHandler(this.treeViewXML_KeyDown);
			this.treeViewXML.AfterLabelEdit += new System.Windows.Forms.NodeLabelEditEventHandler(this.treeViewXML_AfterLabelEdit);
			this.treeViewXML.BeforeLabelEdit += new System.Windows.Forms.NodeLabelEditEventHandler(this.treeViewXML_BeforeLabelEdit);
			// 
			// contextMenuTreeViewXML
			// 
			this.contextMenuTreeViewXML.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																								   this.menuItemTreeViewAddTest,
																								   this.menuItemTreeViewRemoveTest,
																								   this.menuItem7,
																								   this.menuItemTreeViewRunAllTests,
																								   this.menuItemTreeViewRunSingleTest,
																								   this.menuItemTreeViewStopTests,
																								   this.menuItemTreeViewCopyRawData});

			// 
			// menuItemTreeViewAddTest
			// 
			this.menuItemTreeViewAddTest.Index = 0;
			this.menuItemTreeViewAddTest.Text = "Add Test";
			this.menuItemTreeViewAddTest.Click += new System.EventHandler(this.menuItemActionsAddTest_Click);
			// 
			// menuItemTreeViewRemoveTest
			// 
			this.menuItemTreeViewRemoveTest.Enabled = false;
			this.menuItemTreeViewRemoveTest.Index = 1;
			this.menuItemTreeViewRemoveTest.Text = "Remove Test";
			this.menuItemTreeViewRemoveTest.Click += new System.EventHandler(this.menuItemActionsRemoveTest_Click);
			// 
			// menuItem7
			// 
			this.menuItem7.Index = 2;
			this.menuItem7.Text = "-";
			// 
			// menuItemTreeViewRunAllTests
			// 
			this.menuItemTreeViewRunAllTests.Enabled = false;
			this.menuItemTreeViewRunAllTests.Index = 3;
			this.menuItemTreeViewRunAllTests.Text = "Run all tests";
			// 
			// menuItemTreeViewRunSingleTest
			// 
			this.menuItemTreeViewRunSingleTest.Enabled = false;
			this.menuItemTreeViewRunSingleTest.Index = 4;
			this.menuItemTreeViewRunSingleTest.Text = "Run single test";
			this.menuItemTreeViewRunSingleTest.Click += new System.EventHandler(this.menuItemActionsRunSingle_Click);
			// 
			// menuItemTreeViewStopTests
			// 
			this.menuItemTreeViewStopTests.Enabled = false;
			this.menuItemTreeViewStopTests.Index = 5;
			this.menuItemTreeViewStopTests.Text = "Stop tests";
			// 
			// menuItemTreeViewCopyRawData
			// 
			this.menuItemTreeViewCopyRawData.Enabled = false;
			this.menuItemTreeViewCopyRawData.Index = 6;
			this.menuItemTreeViewCopyRawData.Text = "Copy raw data";
			this.menuItemTreeViewCopyRawData.Click += new System.EventHandler(this.menuItemCopyRawData_Click);
			// 
			// imageListTreeView
			// 
			this.imageListTreeView.ImageSize = new System.Drawing.Size(16, 16);
			this.imageListTreeView.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageListTreeView.ImageStream")));
			this.imageListTreeView.TransparentColor = System.Drawing.Color.Magenta;
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.tabControl);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel1.Location = new System.Drawing.Point(152, 24);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(560, 431);
			this.panel1.TabIndex = 3;
			// 
			// tabControl
			// 
			this.tabControl.Controls.Add(this.tabPageMain);
			this.tabControl.Controls.Add(this.tabPageReport);
			this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tabControl.Location = new System.Drawing.Point(0, 0);
			this.tabControl.Name = "tabControl";
			this.tabControl.SelectedIndex = 0;
			this.tabControl.Size = new System.Drawing.Size(560, 431);
			this.tabControl.TabIndex = 0;
			// 
			// tabPageMain
			// 
			this.tabPageMain.Controls.Add(this.panel2);
			this.tabPageMain.Location = new System.Drawing.Point(4, 22);
			this.tabPageMain.Name = "tabPageMain";
			this.tabPageMain.Size = new System.Drawing.Size(552, 405);
			this.tabPageMain.TabIndex = 0;
			this.tabPageMain.Text = "Main";
			// 
			// panel2
			// 
			this.panel2.Controls.Add(this.listViewThreads);
			this.panel2.Controls.Add(this.richTextBox);
			this.panel2.Controls.Add(this.splitterHorz);
			this.panel2.Controls.Add(this.listBoxAppLog);
			this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel2.Location = new System.Drawing.Point(0, 0);
			this.panel2.Name = "panel2";
			this.panel2.Size = new System.Drawing.Size(552, 405);
			this.panel2.TabIndex = 0;
			// 
			// listViewThreads
			// 
			this.listViewThreads.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
																							  this.columnHeaderNum,
																							  this.columnHeaderTaskName,
																							  this.columnHeaderPercentComplete,
																							  this.columnHeaderMinRes,
																							  this.columnHeaderMaxRes,
																							  this.columnHeaderAvgRes,
																							  this.columnHeaderPass,
																							  this.columnHeaderFail,
																							  this.columnHeaderStatusMessage});
			this.listViewThreads.Dock = System.Windows.Forms.DockStyle.Fill;
			this.listViewThreads.FullRowSelect = true;
			this.listViewThreads.GridLines = true;
			this.listViewThreads.Location = new System.Drawing.Point(0, 0);
			this.listViewThreads.Name = "listViewThreads";
			this.listViewThreads.Size = new System.Drawing.Size(552, 333);
			this.listViewThreads.TabIndex = 0;
			this.listViewThreads.View = System.Windows.Forms.View.Details;
			this.listViewThreads.KeyDown += new System.Windows.Forms.KeyEventHandler(this.listViewThreads_KeyDown);
			this.listViewThreads.DoubleClick += new System.EventHandler(this.listViewThreads_DoubleClick);
			this.listViewThreads.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.listViewThreads_ColumnClick);
			// 
			// columnHeaderNum
			// 
			this.columnHeaderNum.Text = "#";
			this.columnHeaderNum.Width = 45;
			// 
			// columnHeaderTaskName
			// 
			this.columnHeaderTaskName.Text = "Task Name";
			this.columnHeaderTaskName.Width = 140;
			// 
			// columnHeaderPercentComplete
			// 
			this.columnHeaderPercentComplete.Text = "% Complete";
			this.columnHeaderPercentComplete.Width = 70;
			// 
			// columnHeaderMinRes
			// 
			this.columnHeaderMinRes.Text = "MinRes";
			// 
			// columnHeaderMaxRes
			// 
			this.columnHeaderMaxRes.Text = "MaxRes";
			// 
			// columnHeaderAvgRes
			// 
			this.columnHeaderAvgRes.Text = "AvgRes";
			// 
			// columnHeaderPass
			// 
			this.columnHeaderPass.Text = "Pass";
			// 
			// columnHeaderFail
			// 
			this.columnHeaderFail.Text = "Fail";
			// 
			// columnHeaderStatusMessage
			// 
			this.columnHeaderStatusMessage.Text = "Status Message";
			this.columnHeaderStatusMessage.Width = 300;
			// 
			// richTextBox
			// 
			this.richTextBox.BackColor = System.Drawing.SystemColors.ActiveBorder;
			this.richTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
			this.richTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.richTextBox.ForeColor = System.Drawing.SystemColors.MenuText;
			this.richTextBox.Location = new System.Drawing.Point(0, 0);
			this.richTextBox.Name = "richTextBox";
			this.richTextBox.Size = new System.Drawing.Size(552, 333);
			this.richTextBox.TabIndex = 2;
			this.richTextBox.Text = "Test is currently executing...\nOnce complete, the results view and report view wi" +
				"ll be visible.\nTo abort this test use the square (stop) button or the Actions=>S" +
				"top Test menu item.";
			// 
			// splitterHorz
			// 
			this.splitterHorz.BackColor = System.Drawing.SystemColors.Control;
			this.splitterHorz.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.splitterHorz.Cursor = System.Windows.Forms.Cursors.HSplit;
			this.splitterHorz.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.splitterHorz.Location = new System.Drawing.Point(0, 333);
			this.splitterHorz.Name = "splitterHorz";
			this.splitterHorz.Size = new System.Drawing.Size(552, 3);
			this.splitterHorz.TabIndex = 1;
			this.splitterHorz.TabStop = false;
			// 
			// listBoxAppLog
			// 
			this.listBoxAppLog.ContextMenu = this.contextMenuLogView;
			this.listBoxAppLog.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.listBoxAppLog.HorizontalScrollbar = true;
			this.listBoxAppLog.Location = new System.Drawing.Point(0, 336);
			this.listBoxAppLog.Name = "listBoxAppLog";
			this.listBoxAppLog.SelectionMode = System.Windows.Forms.SelectionMode.None;
			this.listBoxAppLog.Size = new System.Drawing.Size(552, 69);
			this.listBoxAppLog.TabIndex = 0;
			// 
			// contextMenuLogView
			// 
			this.contextMenuLogView.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																							   this.menuItemAppLogClearLog,
																							   this.menuItemAppLogExportLog});
			// 
			// menuItemAppLogClearLog
			// 
			this.menuItemAppLogClearLog.Index = 0;
			this.menuItemAppLogClearLog.Text = "Clear log";
			// 
			// menuItemAppLogExportLog
			// 
			this.menuItemAppLogExportLog.Index = 1;
			this.menuItemAppLogExportLog.Text = "Export log";
			// 
			// tabPageReport
			// 
			this.tabPageReport.Controls.Add(this.axWebBrowser);
			this.tabPageReport.Location = new System.Drawing.Point(4, 22);
			this.tabPageReport.Name = "tabPageReport";
			this.tabPageReport.Size = new System.Drawing.Size(552, 405);
			this.tabPageReport.TabIndex = 2;
			this.tabPageReport.Text = "Report";
			// 
			// axWebBrowser
			// 
			this.axWebBrowser.ContainingControl = this;
			this.axWebBrowser.Dock = System.Windows.Forms.DockStyle.Fill;
			this.axWebBrowser.Enabled = true;
			this.axWebBrowser.Location = new System.Drawing.Point(0, 0);
			this.axWebBrowser.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axWebBrowser.OcxState")));
			this.axWebBrowser.Size = new System.Drawing.Size(552, 405);
			this.axWebBrowser.TabIndex = 0;
			// 
			// splitterVert
			// 
			this.splitterVert.BackColor = System.Drawing.SystemColors.Control;
			this.splitterVert.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.splitterVert.Location = new System.Drawing.Point(152, 24);
			this.splitterVert.MinExtra = 100;
			this.splitterVert.Name = "splitterVert";
			this.splitterVert.Size = new System.Drawing.Size(3, 431);
			this.splitterVert.TabIndex = 4;
			this.splitterVert.TabStop = false;
			// 
			// openFileDialog
			// 
			this.openFileDialog.DefaultExt = "xml";
			this.openFileDialog.Filter = "Test XML files (*.xml)|*.xml";
			this.openFileDialog.Title = "Load test script";
			// 
			// saveFileDialog
			// 
			this.saveFileDialog.DefaultExt = "xml";
			this.saveFileDialog.Filter = "Test XML files (*.xml)|*.xml";
			this.saveFileDialog.Title = "Save current test script";
			// 
			// timer
			// 
			this.timer.Enabled = true;
			this.timer.Interval = 1000;
			this.timer.Tick += new System.EventHandler(this.timer_Tick);
			// 
			// contextMenuMessageType
			// 
			this.contextMenuMessageType.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																								   this.menuItemTCPMCP,
																								   this.menuItemUDPMCP,
																								   this.menuItemTCPHTTPGet,
																								   this.menuItemTCPHTTPPost});
			// 
			// menuItemTCPMCP
			// 
			this.menuItemTCPMCP.Index = 0;
			this.menuItemTCPMCP.Text = "TCP/MCP";
			this.menuItemTCPMCP.Click += new System.EventHandler(this.menuItemTCPMCP_Click);
			// 
			// menuItemUDPMCP
			// 
			this.menuItemUDPMCP.Index = 1;
			this.menuItemUDPMCP.Text = "UDP/MCP";
			this.menuItemUDPMCP.Click += new System.EventHandler(this.menuItemUDPMCP_Click);
			// 
			// menuItemTCPHTTPGet
			// 
			this.menuItemTCPHTTPGet.Index = 2;
			this.menuItemTCPHTTPGet.Text = "TCP/HTTP-GET";
			this.menuItemTCPHTTPGet.Click += new System.EventHandler(this.menuItemTCPHTTPGet_Click);
			// 
			// menuItemTCPHTTPPost
			// 
			this.menuItemTCPHTTPPost.Index = 3;
			this.menuItemTCPHTTPPost.Text = "TCP/HTTP-POST";
			this.menuItemTCPHTTPPost.Click += new System.EventHandler(this.menuItemTCPHTTPPost_Click);
			// 
			// contextMenuPayload
			// 
			this.contextMenuPayload.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																							   this.menuItemPayloadImport,
																							   this.menuItemPayloadRemove,
																							   this.menuItemPayloadExport});
			// 
			// menuItemPayloadImport
			// 
			this.menuItemPayloadImport.Index = 0;
			this.menuItemPayloadImport.Text = "Import...";
			this.menuItemPayloadImport.Click += new System.EventHandler(this.menuItemPayloadImport_Click);
			// 
			// menuItemPayloadRemove
			// 
			this.menuItemPayloadRemove.Enabled = false;
			this.menuItemPayloadRemove.Index = 1;
			this.menuItemPayloadRemove.Text = "Remove";
			this.menuItemPayloadRemove.Click += new System.EventHandler(this.menuItemPayloadRemove_Click);
			// 
			// menuItemPayloadExport
			// 
			this.menuItemPayloadExport.Enabled = false;
			this.menuItemPayloadExport.Index = 2;
			this.menuItemPayloadExport.Text = "Export...";
			this.menuItemPayloadExport.Click += new System.EventHandler(this.menuItemPayloadExport_Click);
			// 
			// openFileDialogPayload
			// 
			this.openFileDialogPayload.Filter = "All files (*.*)|*.*";
			this.openFileDialogPayload.Title = "Import Message Payload Data";
			// 
			// saveFileDialogPayload
			// 
			this.saveFileDialogPayload.Filter = "All files (*.*)|*.*|Base64 (*.b64)|*.b64";
			this.saveFileDialogPayload.Title = "Export payload data";
			// 
			// MainForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(712, 477);
			this.Controls.Add(this.splitterVert);
			this.Controls.Add(this.panel1);
			this.Controls.Add(this.treeViewXML);
			this.Controls.Add(this.toolBar);
			this.Controls.Add(this.statusBar);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Menu = this.mainMenu;
			this.Name = "MainForm";
			this.Text = "MCWhirlWind";
			((System.ComponentModel.ISupportInitialize)(this.statusBarPanelMessage)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.statusBarPanelTimer)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.statusBarPanelTime)).EndInit();
			this.panel1.ResumeLayout(false);
			this.tabControl.ResumeLayout(false);
			this.tabPageMain.ResumeLayout(false);
			this.panel2.ResumeLayout(false);
			this.tabPageReport.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.axWebBrowser)).EndInit();
			this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main(string[] args) 
		{
			string param = "";
			if (args.Length > 0 && args[0].IndexOf(".xml") != -1)
				param = args[0];
			Application.Run(new MainForm(param));
		}
		#endregion

		#region MenuItem handlers
		private void menuItemFileExit_Click(object sender, System.EventArgs e)
		{
			this.Close();	
		}

		private void menuItemHelpAbout_Click(object sender, System.EventArgs e)
		{
			About about = new About();
			about.ShowDialog(this);
		}

		private void menuItemFileLoadXML_Click(object sender, System.EventArgs e)
		{
			LoadXML(false);
		}

		private void menuItemImportXML_Click(object sender, System.EventArgs e)
		{
			LoadXML(true);
		}

		private void menuItemFileSaveXML_Click(object sender, System.EventArgs e)
		{
			SaveXML();
		}

		private void menuItemActionsRunall_Click(object sender, System.EventArgs e)
		{
			//RunAll();
		}

		private void menuItemTreeViewSave_Click(object sender, System.EventArgs e)
		{
			SaveXML();
		}

		private void menuItemTreeViewLoad_Click(object sender, System.EventArgs e)
		{
			LoadXML(false);
		}

		private void menuItemActionsRunSingle_Click(object sender, System.EventArgs e)
		{
			RunSingle();
		}

		private void menuItemActionsStop_Click(object sender, System.EventArgs e)
		{
			StopTest();		
		}

		private void menuItemActionsAddTest_Click(object sender, System.EventArgs e)
		{
			AddTest();
		}

		private void menuItemActionsRemoveTest_Click(object sender, System.EventArgs e)
		{
			DeleteSelectedTest();
		}

		private void menuItemTCPMCP_Click(object sender, System.EventArgs e)
		{			
			changeSelectedTreeNodeLabel("TCP/MCP");	
		}

		private void menuItemUDPMCP_Click(object sender, System.EventArgs e)
		{
			changeSelectedTreeNodeLabel("UDP/MCP");		
		}

		private void menuItemTCPHTTPGet_Click(object sender, System.EventArgs e)
		{
			changeSelectedTreeNodeLabel("TCP/HTTP-GET");
		}

		private void menuItemTCPHTTPPost_Click(object sender, System.EventArgs e)
		{
			changeSelectedTreeNodeLabel("TCP/HTTP-POST");
		}

		private void menuItemCopyRawData_Click(object sender, System.EventArgs e)
		{
			if (testDataList.Count == 0)
			{
				MessageBox.Show(this, "You must first execute a test case.", "MCWhirlWind",
					MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				return;
			}
			string s = "";
			foreach (TestDataElement item in testDataList)
			{
				for (int i=0; i<item.responseTimes.Count; i++)
				{
					IndividualTest d = item.responseTimes[i] as IndividualTest;
					s += d.responseTime.ToString();
					if (i != item.responseTimes.Count-1)
						s += "\t";
				}
				s += "\r\n";
			}
			Clipboard.SetDataObject(s);
		}

		private void menuItemReportPrint_Click(object sender, System.EventArgs e)
		{
			IHTMLDocument2 doc = axWebBrowser.Document as IHTMLDocument2;
			doc.execCommand("Print", true, 0);	
		}

		private void menuItemPayloadImport_Click(object sender, System.EventArgs e)
		{
			ImportPayloadData();
		}

		private void menuItemPayloadRemove_Click(object sender, System.EventArgs e)
		{
			RemovePayloadData();
		}

		private void menuItemPayloadExport_Click(object sender, System.EventArgs e)
		{
			ExportPayloadData();
		}

		private void menuItemOptionsDisableNagel_Click(object sender, System.EventArgs e)
		{
			useNagel = !useNagel;
			menuItemOptionsDisableNagel.Checked = !useNagel;	
		}

		#endregion

		#region Toolbar handlers
		private void toolBar_ButtonClick(object sender, System.Windows.Forms.ToolBarButtonClickEventArgs e)
		{
			if (e.Button == toolBarButtonOpen)
			{
				LoadXML(false);
			}
			else if (e.Button == toolBarButtonSave)
			{
				SaveXML();
			}
			else if (e.Button == toolBarButtonPlayAll)
			{
				//RunAll();
			}
			else if (e.Button == toolBarButtonPlay)
			{
				RunSingle();
			}
			else if (e.Button == toolBarButtonAddTest)
			{
				AddTest();
			}
			else if (e.Button == toolBarButtonRemoveTest)
			{
				DeleteSelectedTest();
			}
			else if (e.Button == toolBarButtonStop)
			{
				StopTest();
			}
		}
		#endregion

		#region TreeNode handlers
		private void treeViewXML_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			if (e.KeyCode == Keys.F2)
			{
				if (treeViewXML.SelectedNode != null)
				{
					string type = treeViewXML.SelectedNode.Tag as string;
					if (type == "label")
					{
						MessageBox.Show(this, "Thread and Message labels can't be edited.", "MCWhirlWind",
							MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
						return;
					}
					treeViewXML.SelectedNode.BeginEdit();
					e.Handled = true;
				}
			}
			if (e.KeyCode == Keys.Enter)
			{
				RunSingle();
			}
		}

		private void treeViewXML_BeforeLabelEdit(object sender, System.Windows.Forms.NodeLabelEditEventArgs e)
		{
			if (e.Node.Text.IndexOf("Type:") == 0)
			{
				contextMenuMessageType.Show(treeViewXML, new Point(e.Node.Bounds.Location.X, e.Node.Bounds.Location.Y));
				e.CancelEdit = true;
			}
			else if (e.Node.Text.IndexOf("Headers:") == 0)
			{
				Headers dialog = new Headers();
				dialog.PrepopulateItems(e.Node.Text);
				DialogResult dr = dialog.ShowDialog(this);
				if (dr == DialogResult.OK)
				{
					changeSelectedTreeNodeLabel("Headers: " + dialog.GetHeaders());
				}
				e.CancelEdit = true;
			}
			else if (e.Node.Text.IndexOf("Payload:") > -1)
			{
				if (treeViewXML.SelectedNode != null)
				{
					if (treeViewXML.SelectedNode.Text.IndexOf("bytes") > 0)
					{
						menuItemPayloadRemove.Enabled = true;
						menuItemPayloadExport.Enabled = true;
					}
					else
					{
						menuItemPayloadRemove.Enabled = false;
						menuItemPayloadExport.Enabled = false;
					}
				}

				contextMenuPayload.Show(treeViewXML, new Point(e.Node.Bounds.Location.X, e.Node.Bounds.Location.Y));
				e.CancelEdit = true;
			}
		}

		private void treeViewXML_AfterLabelEdit(object sender, System.Windows.Forms.NodeLabelEditEventArgs e)
		{
			if (e.Label == null)
			{
				// cancelled by user
				e.CancelEdit = true;
				return;
			}

			string type = e.Node.Tag as string;
			if (type == "label")
			{
				e.CancelEdit = true;
				MessageBox.Show(this, "Thread, Message labels can't be edited.", "MCWhirlWind",
					MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				return;
			}

			changeSelectedTreeNodeLabel(e.Label);
			e.CancelEdit = true;
		}

		private void changeSelectedTreeNodeLabel(string newValue)
		{
			string chset = ": ";
			string oldText = treeViewXML.SelectedNode.Text;
			int len = oldText.Length;
			string sourceLabel = "";
			string sourceValue = "";
			int iLabelIndex = oldText.IndexOf(":",0,len);
			if (iLabelIndex > 0)
			{
				sourceLabel = oldText.Substring(0, iLabelIndex+1);
				sourceValue = oldText.Substring(iLabelIndex, len-iLabelIndex);
				sourceValue = sourceValue.Trim(chset.ToCharArray());
			}

			// check if node is a labelled node or a propery node
			if (iLabelIndex == 0)
			{
				// Labelled node
				treeViewXML.SelectedNode.Text = newValue;
			}
			else
			{
				// Property node
				string s = newValue;
				int sepIndex = s.IndexOf(":", 0, s.Length);
				if (sepIndex == -1)
				{
					string val = s.Trim(chset.ToCharArray());
					treeViewXML.SelectedNode.Text = sourceLabel + " " + val;
					treeViewXML.SelectedNode.Tag = val;
				}
				else
				{
					string val = s.Substring(sepIndex, s.Length - sepIndex);
					val = val.Trim(chset.ToCharArray());
					treeViewXML.SelectedNode.Text = sourceLabel + " " + val;
					treeViewXML.SelectedNode.Tag = val;
				}		
			}
			modified = true;
		}
		#endregion 

		#region Timer handler
		private void timer_Tick(object sender, System.EventArgs e)
		{	
			DateTime dt = new DateTime(DateTime.Now.Ticks);

			string TimeInString = "";
			int hour = dt.Hour;
			if (hour > 12) hour -= 12;
			int min = dt.Minute;
			int sec = dt.Second;
			TimeInString = (hour < 10)? "0" + hour.ToString() :hour.ToString();
			TimeInString += ":" + ((min<10)?"0" + min.ToString() :min.ToString());
			TimeInString += ":" + ((sec<10)?"0" + sec.ToString() :sec.ToString());
			statusBarPanelTime.Text = TimeInString;

			if (timerOn == true)
			{
				DateTime elapsed = new DateTime(dt.Ticks - testStartTime.Ticks);
				hour = elapsed.Hour;
				if (hour > 12) hour -= 12;
				min = elapsed.Minute;
				sec = elapsed.Second;
				TimeInString = (hour < 10)? "0" + hour.ToString() : hour.ToString();
				TimeInString += ":" + ((min<10)?"0" + min.ToString() : min.ToString());
				TimeInString += ":" + ((sec<10)?"0" + sec.ToString() : sec.ToString());
				TimeInString += ":" + string.Format("{0:00#}", elapsed.Millisecond);
				statusBarPanelTimer.Text = TimeInString;
			}
		}
		#endregion

		#region Main View Comparer class
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

				ListViewItem.ListViewSubItem sub1 = item1.SubItems[SortColumn];
				ListViewItem.ListViewSubItem sub2 = item2.SubItems[SortColumn];
				switch (SortColumn)
				{
					case MainForm.taskNameColumn:
					case MainForm.statusColumn:
						return CaseInsensitiveComparer.Default.Compare(sub1.Text, sub2.Text);
				}

				double x1 = Convert.ToDouble(sub1.Text);
				double x2 = Convert.ToDouble(sub2.Text);
				if (x1 < x2)
					return -1;
				else if (x1 == x2)
					return 0;
				return 1;
			}
		}
		#endregion

		#region ListViewThreads
		private void listViewThreads_ColumnClick(object sender, System.Windows.Forms.ColumnClickEventArgs e)
		{
			SortOrder prevOrder = listViewThreads.Sorting;
			listViewThreads.Sorting = SortOrder.None;
			if (e.Column == listViewcomparer.SortColumn)
			{
				if (prevOrder == SortOrder.Ascending)
					listViewThreads.Sorting = SortOrder.Descending;
				else 
					listViewThreads.Sorting = SortOrder.Ascending;
			}
			else
			{
				listViewcomparer.SortColumn = e.Column;
				listViewThreads.Sorting = SortOrder.Ascending;
			}
		}
		private void listViewThreads_DoubleClick(object sender, System.EventArgs e)
		{
			DisplayProtocolDialog();
		}	

		private void listViewThreads_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter)
			{
				DisplayProtocolDialog();
			}
		}
		#endregion

		private void LoadXML(bool bImport)
		{
			if (openFileDialog.ShowDialog() == DialogResult.OK)
			{
				if (bImport == false)
				{
					//listViewThreads.Clear();
					TreeNode node;
					while ((node = testTree.GetSelectedNodesParent()) != null)
						node.Remove();
				}
				appLog.Reset();
				XMLLoad loader = new XMLLoad(appLog, testTree);
				loader.LoadTestXML(openFileDialog.FileName.ToString());
				//menuItemActionsRunall.Enabled = true;
				menuItemActionsRunSingle.Enabled = true;
				
				EnableTreeNodeControlItems(true);
				treeViewXML.Select();
			}
		}

		private void SaveXML()
		{
			if (saveFileDialog.ShowDialog() == DialogResult.OK)
			{
				testTree.ExportTree(saveFileDialog.FileName);
			}
		}

		private void RunAll()
		{
			if (treeViewXML.GetNodeCount(true) == 0)
			{
				MessageBox.Show(this, "No tests loaded.", "MCWhirlWind",
					MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				return;
			}
			menuItemActionsStop.Enabled = true;
			menuItemTreeViewStopTests.Enabled = true;
			toolBarButtonStop.Enabled = true;
			tasksAborted = false;
		}

		private void RunSingle()
		{
			if (TestValidSelectedNode() == true)
			{
				DialogResult result;
				TreeNode node = treeViewXML.SelectedNode;
				if (treeViewXML.SelectedNode.Tag == null || treeViewXML.SelectedNode.Tag.ToString() != "mcww.TestTreeNodeTag")
					node = testTree.GetSelectedNodesParent();
				currentTestName = node.Text;
				result = MessageBox.Show(this, "Run test: \"" + currentTestName + "\"?", "MCWhirlWind",
					MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
				if (result ==  DialogResult.Yes)
				{
					listViewThreads.Visible = false;
					testDataList.Clear();
					UpdateRunTestGUI();
					tester.ExecuteSingleTest(currentTestName);
				}
			}
		}

		private void UpdateRunTestGUI()
		{
			timerOn = true;
			menuItemActionsStop.Enabled = true;
			menuItemTreeViewStopTests.Enabled = true;
			toolBarButtonStop.Enabled = true;
			tasksAborted = false;
			testStartTime = new DateTime(DateTime.Now.Ticks);
		}

		private bool TestValidSelectedNode()
		{
			if (treeViewXML.GetNodeCount(true) == 0)
			{
				MessageBox.Show(this, "No tests loaded.", "MCWhirlWind",
					MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				return false;
			}
			if (treeViewXML.SelectedNode == null)
			{
				MessageBox.Show(this, "No test node selected in tree view.", "MCWhirlWind",
					MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				return false;
			}
			return true;
		}

		private void AddTest()
		{
			modified = true;
			EnableTreeNodeControlItems(true);

			testTree.AddTestBranch("New test");
			testTree.AddThreadInfo("1","1","0","300000");
			testTree.AddMessage("TCP/HTTP-GET", "http://www.google.com", "", "72.14.207.99", "80", "", "");
			TreeNode node = testTree.GetTestNode();
			node.Expand();
			node.BeginEdit();		
		}

		private void DeleteSelectedTest()
		{
			if (TestValidSelectedNode() == true)
			{
				TreeNode node = testTree.GetSelectedNodesParent();
				DialogResult result;
				result = MessageBox.Show(this, "Remove test: \"" + node.Text + "\"?", "MCWhirlWind",
					MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
				if (result ==  DialogResult.Yes)
				{
					appLog.Append("Test \"" + node.Text + "\" removed.");
					node.Remove();
					if (treeViewXML.GetNodeCount(true) == 0)
					{
						EnableTreeNodeControlItems(false);
					}
				}
			}
		}

		private void EnableTreeNodeControlItems(bool enabled)
		{
			//toolBarButtonPlayAll.Enabled = enabled;
			toolBarButtonPlay.Enabled = enabled;
			toolBarButtonRemoveTest.Enabled = enabled;
			menuItemActionsRemoveTest.Enabled = enabled;
			menuItemActionsRunSingle.Enabled = enabled;
			menuItemTreeViewAddTest.Enabled = enabled;
			menuItemTreeViewRemoveTest.Enabled = enabled;
			menuItemTreeViewRunAllTests.Enabled = enabled;
			menuItemTreeViewRunSingleTest.Enabled = enabled;
		}

		private void StopTest()
		{
			listViewThreads.Visible = true;
			tasksAborted = true;
			tester.StopTest();
			timerOn = false;
			menuItemActionsStop.Enabled = false;
			menuItemTreeViewStopTests.Enabled = false;
			toolBarButtonStop.Enabled = false;			
			appLog.Append("Test \"" + currentTestName + "\" aborted.");
		}

		public void TasksCompleted()
		{
			listViewThreads.Visible = true;
			timerOn = false;
			menuItemActionsStop.Enabled = false;
			menuItemTreeViewStopTests.Enabled = false;
			toolBarButtonStop.Enabled = false;
			if (tasksAborted == true)
			{
				MessageBox.Show(this, "Tasks aborted by user.", "MCWhirlWind",
					MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			}
			else
			{
				GenerateReport();
				appLog.Append("Tests completed in " + statusBarPanelTimer.Text);
				MessageBox.Show(this, "Tasks completed.", "MCWhirlWind",
					MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				menuItemTreeViewCopyRawData.Enabled = true;
			}
		}

		private void GenerateReport()
		{
			if (testDataList.Count == 0)
			{
				MessageBox.Show(this, "You must first execute a test case.", "MCWhirlWind",
					MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				return;
			}
			reportGenerator.SetRawData(testDataList);
			reportGenerator.GenerateReport(currentTestName);
			menuItemReportPrint.Enabled = true;
		}

		public void DisplayInWebPage(string pageLocation)
		{
			Object o = null;
			axWebBrowser.Navigate(pageLocation,ref o,ref o,ref o,ref o);
		}

		private void ImportPayloadData()
		{
			Payload payload = new Payload();
			if (openFileDialogPayload.ShowDialog() == DialogResult.OK)
			{
				Mimetype dialog = new Mimetype();
				dialog.LookupType(openFileDialogPayload.FileName);
				dialog.ShowDialog(this);

				string MIMEType = dialog.MIMETypeString;

				// if MIMEType is valid load binary data.
				if (MIMEType != null)
				{
					try
					{
						int bytesRead;
						byte []buffer = new byte[32000];
						MemoryStream ms = new MemoryStream();
						Stream st = File.OpenRead(openFileDialogPayload.FileName);
						while ((bytesRead = st.Read(buffer, 0, 32000)) > 0)
						{
							ms.Write(buffer,0,bytesRead);
						}
						payload.MIMEType = MIMEType;
						payload.buffer = ms.ToArray();
						st.Close();
						ms.Close();
					}
					catch
					{
						MessageBox.Show(this, "File import failed.", "MCWhirlWind",
							MessageBoxButtons.OK, MessageBoxIcon.Error);
					}
				}

				if (treeViewXML.SelectedNode != null)
				{
					string payloadLabel = treeViewXML.SelectedNode.Text;
					if (payloadLabel.IndexOf("Payload:") != 0)
					{
						MessageBox.Show(this, "A 'Payload:' node on the tree view must be selected.", "MCWhirlWind",
							MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
						return;
					}
					string prettyNumber = string.Format("{0:0,0}", payload.buffer.Length);
					treeViewXML.SelectedNode.Text = "Payload: " + payload.MIMEType + " {" + prettyNumber + " bytes}";

					TestTreeNodeTag tag = new TestTreeNodeTag();
					tag.label = "payload";
					tag.userData = payload;
					treeViewXML.SelectedNode.Tag = tag;
				}
			}
		}

		private void RemovePayloadData()
		{
			if (treeViewXML.SelectedNode != null)
			{
				string payloadLabel = treeViewXML.SelectedNode.Text;
				if (payloadLabel.IndexOf("Payload:") != 0)
				{
					MessageBox.Show(this, "A 'Payload:' node on the tree view must be selected.", "MCWhirlWind",
						MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
					return;
				}
				treeViewXML.SelectedNode.Text = "Payload: {empty}";
				treeViewXML.SelectedNode.Tag = null;
			}
		}

		private void DisplayProtocolDialog()
		{
			ListViewItem item = listViewThreads.SelectedItems[0];
			ThreadAgentInfo threadInfo = item.Tag as ThreadAgentInfo;
			if (threadInfo == null)
			{
				MessageBox.Show(this, "You must first execute a test case.", "MCWhirlWind",
					MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				return;
			}
			ProtocolView dialog = new ProtocolView();
			dialog.SetContent(threadInfo.msgLog.goodSendHeaders, threadInfo.msgLog.goodRecvHeaders, threadInfo.msgLog.badRecvHeaders);
			dialog.ShowDialog(this);
		}

		private void ExportPayloadData()
		{
			if (treeViewXML.SelectedNode != null)
			{
				string payloadLabel = treeViewXML.SelectedNode.Text;
				if (payloadLabel.IndexOf("Payload:") != 0)
				{
					MessageBox.Show(this, "A 'Payload:' node on the tree view must be selected.", "MCWhirlWind",
						MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
					return;
				}
			}

			if (saveFileDialogPayload.ShowDialog() == DialogResult.OK)
			{
				try
				{
					TestTreeNodeTag tag = treeViewXML.SelectedNode.Tag as TestTreeNodeTag;
					Payload payload = tag.userData as Payload;					

					if (saveFileDialogPayload.FileName.IndexOf(".b64") > 0)
					{
						string base64;
						base64 = Convert.ToBase64String(payload.buffer);
						StreamWriter writer = new StreamWriter(saveFileDialogPayload.FileName, false);
						writer.Write(base64);
						writer.Close();
					}
					else
					{
						Stream st = File.OpenWrite(saveFileDialogPayload.FileName);
						st.Write(payload.buffer, 0, payload.buffer.Length);
						st.Close();
					}
				}
				catch(Exception ex)
				{
					MessageBox.Show(this, "Unable to export data." + ex.Message, "MCWhirlWind",
						MessageBoxButtons.OK, MessageBoxIcon.Error);
					appLog.Append(ex.Message);    
				}
			}
		}

		public bool IsGridVisible()
		{
			return listViewThreads.Visible;
		}

		public bool IsUseNagel()
		{
			return useNagel;
		}

	}
}
