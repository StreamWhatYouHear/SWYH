/*   
Copyright 2006 - 2010 Intel Corporation

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

   http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/

using System;
using System.IO;
using System.Text;
using System.Drawing;
using System.Diagnostics;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace OpenSource.Utilities
{
	/// <summary>
	/// Summary description for InstanceTracker.
	/// </summary>
	public sealed class InstanceTracker : System.Windows.Forms.Form
	{
//		private struct EventData
//		{
//			object origin;
//			string trace;
//			string information;
//		}

        public static string VersionString = "0.01";

        public delegate void TrackerHandler(object obj);
        public delegate void UpdateHandler(object name);

		public struct InstanceStruct
		{
			public WeakReference WR;
			public string StackList;
		}

		private class InstanceTrackerSorter : IComparer
		{
			public int Compare(object x, object y)
			{
				ListViewItem ItemX = (ListViewItem)x;
				ListViewItem ItemY = (ListViewItem)y;

				return(ItemX.SubItems[1].Text.CompareTo(ItemY.SubItems[1].Text));
			}
		}

		public static void StartTimer()
		{
			TimeValue = DateTime.Now.Ticks;
		}
		public static void StopTimer()
		{
			StopTimer("");
		}
		public static void StopTimer(string s)
		{
			MessageBox.Show("Time to execute: " + s + " = " + new TimeSpan(DateTime.Now.Ticks-TimeValue).TotalMilliseconds.ToString());
		}

		public new static bool Enabled = false;

		private static long TimeValue;

		private static Hashtable WeakTable = new Hashtable();
		private static Hashtable DataTable = new Hashtable();
		private static Hashtable instancetable = new Hashtable();
		private static InstanceTracker tracker = null;

		private System.Windows.Forms.StatusBar statusBar;
		private System.Windows.Forms.MainMenu mainMenu;
		private System.Windows.Forms.MenuItem menuItem1;
		private System.Windows.Forms.MenuItem gcMenuItem;
		private System.Windows.Forms.MenuItem menuItem3;
		private System.Windows.Forms.MenuItem closeMenuItem;
		private System.Windows.Forms.MenuItem fullGcMenuItem;
		private System.Windows.Forms.ContextMenu DetailMenu;
		private System.Windows.Forms.MenuItem DetailMenuItem;
		private System.Windows.Forms.TabControl tabControl1;
		private System.Windows.Forms.TabPage tabPage1;
		private System.Windows.Forms.TabPage tabPage2;
		private System.Windows.Forms.ListView instanceListView;
		private System.Windows.Forms.ColumnHeader columnHeader1;
		private System.Windows.Forms.ColumnHeader columnHeader2;
		private System.Windows.Forms.ListView EventListView;
		private System.Windows.Forms.ColumnHeader columnHeader3;
		private System.Windows.Forms.Splitter splitter1;
		private System.Windows.Forms.TextBox EventText;
		private System.Windows.Forms.ColumnHeader columnHeader4;
		private System.Windows.Forms.MenuItem menuItem2;
		private System.Windows.Forms.MenuItem ClearEventMenuItem;
		private System.Windows.Forms.MenuItem menuItem5;
		private System.Windows.Forms.MenuItem haltOnExceptionsMenuItem;
		private System.Windows.Forms.MenuItem menuItem6;
		private System.Windows.Forms.MenuItem enableInstanceTrackingMenuItem;
		private System.Windows.Forms.MenuItem menuItem4;
		private System.Windows.Forms.ToolBar eventToolBar;
		private System.Windows.Forms.ToolBarButton showExceptionsToolBarButton;
		private System.Windows.Forms.ToolBarButton toolBarButton1;
		private System.Windows.Forms.ToolBarButton clearEventLogToolBarButton;
		private System.Windows.Forms.ToolBarButton toolBarButton2;
		private System.Windows.Forms.ToolBarButton gcToolBarButton;
		private System.Windows.Forms.ToolBarButton toolBarButton3;
		private System.Windows.Forms.ImageList ToolsIconList;
		private System.Windows.Forms.ImageList iconImageList;
		private System.Windows.Forms.MenuItem saveAsMenuItem;
		private System.Windows.Forms.MenuItem menuItem8;
		private System.Windows.Forms.SaveFileDialog saveLogFileDialog;
		private System.Windows.Forms.ToolBarButton showWarningEventsToolBarButton;
		private System.Windows.Forms.ToolBarButton showInformationEventsToolBarButton;
		private System.Windows.Forms.ToolBarButton showAuditEventsToolBarButton;
		private System.Windows.Forms.MenuItem showExceptionsMenuItem;
		private System.Windows.Forms.MenuItem showWarningsMenuItem;
		private System.Windows.Forms.MenuItem showInformationMenuItem;
		private System.Windows.Forms.MenuItem showAuditMenuItem;
		private System.ComponentModel.IContainer components;

		public InstanceTracker()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			this.instanceListView.ListViewItemSorter = new InstanceTrackerSorter();

			OpenSource.Utilities.EventLogger.OnEvent += new OpenSource.Utilities.EventLogger.EventHandler(OnEventSink);
			OpenSource.Utilities.EventLogger.Enabled = true;

			if (OpenSource.Utilities.EventLogger.ShowAll == true) 
			{
				this.showInformationMenuItem.Checked = true;
				this.showWarningsMenuItem.Checked = true;
				this.showExceptionsMenuItem.Checked = true;
				this.showAuditMenuItem.Checked = true;
			}
		}
        private void OnEventSink(EventLogEntryType LogType, object origin, string trace, string information)
        {
            this.BeginInvoke(new OpenSource.Utilities.EventLogger.EventHandler(OnEventSinkEx), new object[4] { LogType, origin, trace, information });
        }
		private void OnEventSinkEx(EventLogEntryType LogType,object origin, string trace,string information)
		{
			int ImageIndex = 0;
			lock(this)
			{
				switch(LogType)
				{
					case EventLogEntryType.Information:
						ImageIndex = 0;
						if (!this.showInformationMenuItem.Checked){return;}
						break;
					case EventLogEntryType.Warning:
						ImageIndex = 1;
						if (!this.showWarningsMenuItem.Checked){return;}
						break;
					case EventLogEntryType.Error:
						ImageIndex = 2;
						if (!this.showExceptionsMenuItem.Checked){return;}
						break;
					case EventLogEntryType.SuccessAudit:
						ImageIndex = 3;
						if (!this.showAuditMenuItem.Checked){return;}
						break;
					default:
						ImageIndex = 4;
						break;
				}

				string originString = "";
				if (origin.GetType()==typeof(string))
				{
					originString = (string)origin;
				}
				else
				{
					originString = origin.GetType().Name + " [" + origin.GetHashCode().ToString()+"]";
				}
				
				ListViewItem i = new ListViewItem(new string[2]{originString,information},ImageIndex);
				if (origin.GetType()==typeof(string))
				{
					originString = (string)origin;
				}
				else
				{
					originString = origin.GetType().FullName + " [" + origin.GetHashCode().ToString()+"]";
				}
				if (trace!="")
				{
					i.Tag = "Origin: " + originString + "\r\nTime: " + DateTime.Now.ToString() + "\r\n\r\n" + information + "\r\n\r\nTRACE:\r\n" + trace;
				}
				else
				{
					i.Tag = "Origin: " + originString + "\r\nTime: " + DateTime.Now.ToString() + "\r\n\r\n" + information;
				}
				EventListView.Items.Insert(0,i);
			}
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if ( disposing )
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(InstanceTracker));
            this.DetailMenu = new System.Windows.Forms.ContextMenu();
            this.DetailMenuItem = new System.Windows.Forms.MenuItem();
            this.statusBar = new System.Windows.Forms.StatusBar();
            this.mainMenu = new System.Windows.Forms.MainMenu(this.components);
            this.menuItem1 = new System.Windows.Forms.MenuItem();
            this.saveAsMenuItem = new System.Windows.Forms.MenuItem();
            this.menuItem8 = new System.Windows.Forms.MenuItem();
            this.gcMenuItem = new System.Windows.Forms.MenuItem();
            this.fullGcMenuItem = new System.Windows.Forms.MenuItem();
            this.menuItem4 = new System.Windows.Forms.MenuItem();
            this.enableInstanceTrackingMenuItem = new System.Windows.Forms.MenuItem();
            this.menuItem3 = new System.Windows.Forms.MenuItem();
            this.closeMenuItem = new System.Windows.Forms.MenuItem();
            this.menuItem2 = new System.Windows.Forms.MenuItem();
            this.showExceptionsMenuItem = new System.Windows.Forms.MenuItem();
            this.showWarningsMenuItem = new System.Windows.Forms.MenuItem();
            this.showInformationMenuItem = new System.Windows.Forms.MenuItem();
            this.showAuditMenuItem = new System.Windows.Forms.MenuItem();
            this.menuItem5 = new System.Windows.Forms.MenuItem();
            this.haltOnExceptionsMenuItem = new System.Windows.Forms.MenuItem();
            this.menuItem6 = new System.Windows.Forms.MenuItem();
            this.ClearEventMenuItem = new System.Windows.Forms.MenuItem();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.EventText = new System.Windows.Forms.TextBox();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.EventListView = new System.Windows.Forms.ListView();
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.iconImageList = new System.Windows.Forms.ImageList(this.components);
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.instanceListView = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ToolsIconList = new System.Windows.Forms.ImageList(this.components);
            this.eventToolBar = new System.Windows.Forms.ToolBar();
            this.toolBarButton3 = new System.Windows.Forms.ToolBarButton();
            this.showExceptionsToolBarButton = new System.Windows.Forms.ToolBarButton();
            this.showWarningEventsToolBarButton = new System.Windows.Forms.ToolBarButton();
            this.showInformationEventsToolBarButton = new System.Windows.Forms.ToolBarButton();
            this.showAuditEventsToolBarButton = new System.Windows.Forms.ToolBarButton();
            this.toolBarButton1 = new System.Windows.Forms.ToolBarButton();
            this.clearEventLogToolBarButton = new System.Windows.Forms.ToolBarButton();
            this.toolBarButton2 = new System.Windows.Forms.ToolBarButton();
            this.gcToolBarButton = new System.Windows.Forms.ToolBarButton();
            this.saveLogFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.tabControl1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.SuspendLayout();
            // 
            // DetailMenu
            // 
            this.DetailMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.DetailMenuItem});
            // 
            // DetailMenuItem
            // 
            this.DetailMenuItem.Index = 0;
            this.DetailMenuItem.Text = "Details";
            this.DetailMenuItem.Click += new System.EventHandler(this.DetailMenuItem_Click);
            // 
            // statusBar
            // 
            this.statusBar.Location = new System.Drawing.Point(0, 441);
            this.statusBar.Name = "statusBar";
            this.statusBar.Size = new System.Drawing.Size(480, 16);
            this.statusBar.TabIndex = 1;
            // 
            // mainMenu
            // 
            this.mainMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItem1,
            this.menuItem2});
            // 
            // menuItem1
            // 
            this.menuItem1.Index = 0;
            this.menuItem1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.saveAsMenuItem,
            this.menuItem8,
            this.gcMenuItem,
            this.fullGcMenuItem,
            this.menuItem4,
            this.enableInstanceTrackingMenuItem,
            this.menuItem3,
            this.closeMenuItem});
            this.menuItem1.Text = "&File";
            // 
            // saveAsMenuItem
            // 
            this.saveAsMenuItem.Index = 0;
            this.saveAsMenuItem.Text = "Save As...";
            this.saveAsMenuItem.Click += new System.EventHandler(this.saveAsMenuItem_Click);
            // 
            // menuItem8
            // 
            this.menuItem8.Index = 1;
            this.menuItem8.Text = "-";
            // 
            // gcMenuItem
            // 
            this.gcMenuItem.Index = 2;
            this.gcMenuItem.Text = "&Garbage Collect";
            this.gcMenuItem.Click += new System.EventHandler(this.gcMenuItem_Click);
            // 
            // fullGcMenuItem
            // 
            this.fullGcMenuItem.Index = 3;
            this.fullGcMenuItem.Text = "&Full Garbage Collect";
            this.fullGcMenuItem.Click += new System.EventHandler(this.fullGcMenuItem_Click);
            // 
            // menuItem4
            // 
            this.menuItem4.Index = 4;
            this.menuItem4.Text = "-";
            // 
            // enableInstanceTrackingMenuItem
            // 
            this.enableInstanceTrackingMenuItem.Index = 5;
            this.enableInstanceTrackingMenuItem.Text = "&Enable Object Tracking";
            this.enableInstanceTrackingMenuItem.Click += new System.EventHandler(this.enableInstanceTrackingMenuItem_Click);
            // 
            // menuItem3
            // 
            this.menuItem3.Index = 6;
            this.menuItem3.Text = "-";
            // 
            // closeMenuItem
            // 
            this.closeMenuItem.Index = 7;
            this.closeMenuItem.Text = "&Close";
            this.closeMenuItem.Click += new System.EventHandler(this.closeMenuItem_Click);
            // 
            // menuItem2
            // 
            this.menuItem2.Index = 1;
            this.menuItem2.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.showExceptionsMenuItem,
            this.showWarningsMenuItem,
            this.showInformationMenuItem,
            this.showAuditMenuItem,
            this.menuItem5,
            this.haltOnExceptionsMenuItem,
            this.menuItem6,
            this.ClearEventMenuItem});
            this.menuItem2.Text = "Events";
            // 
            // showExceptionsMenuItem
            // 
            this.showExceptionsMenuItem.Index = 0;
            this.showExceptionsMenuItem.Text = "Show &Exception Messages";
            this.showExceptionsMenuItem.Click += new System.EventHandler(this.showExceptionsMenuItem_Click);
            // 
            // showWarningsMenuItem
            // 
            this.showWarningsMenuItem.Index = 1;
            this.showWarningsMenuItem.Text = "Show &Warning Messages";
            this.showWarningsMenuItem.Click += new System.EventHandler(this.showWarningsMenuItem_Click);
            // 
            // showInformationMenuItem
            // 
            this.showInformationMenuItem.Index = 2;
            this.showInformationMenuItem.Text = "Show &Information Messages";
            this.showInformationMenuItem.Click += new System.EventHandler(this.showInformationMenuItem_Click);
            // 
            // showAuditMenuItem
            // 
            this.showAuditMenuItem.Index = 3;
            this.showAuditMenuItem.Text = "Show &Audit Messages";
            this.showAuditMenuItem.Click += new System.EventHandler(this.showAuditMenuItem_Click);
            // 
            // menuItem5
            // 
            this.menuItem5.Index = 4;
            this.menuItem5.Text = "-";
            // 
            // haltOnExceptionsMenuItem
            // 
            this.haltOnExceptionsMenuItem.Index = 5;
            this.haltOnExceptionsMenuItem.Text = "&Halt On Exceptions";
            this.haltOnExceptionsMenuItem.Click += new System.EventHandler(this.haltOnExceptionsMenuItem_Click);
            // 
            // menuItem6
            // 
            this.menuItem6.Index = 6;
            this.menuItem6.Text = "-";
            // 
            // ClearEventMenuItem
            // 
            this.ClearEventMenuItem.Index = 7;
            this.ClearEventMenuItem.Text = "&Clear Event Log";
            this.ClearEventMenuItem.Click += new System.EventHandler(this.ClearEventMenuItem_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Alignment = System.Windows.Forms.TabAlignment.Bottom;
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 28);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(480, 413);
            this.tabControl1.TabIndex = 2;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.EventText);
            this.tabPage2.Controls.Add(this.splitter1);
            this.tabPage2.Controls.Add(this.EventListView);
            this.tabPage2.Location = new System.Drawing.Point(4, 4);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Size = new System.Drawing.Size(472, 387);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Events";
            // 
            // EventText
            // 
            this.EventText.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.EventText.Dock = System.Windows.Forms.DockStyle.Fill;
            this.EventText.Font = new System.Drawing.Font("Lucida Console", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.EventText.Location = new System.Drawing.Point(0, 179);
            this.EventText.Multiline = true;
            this.EventText.Name = "EventText";
            this.EventText.ReadOnly = true;
            this.EventText.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.EventText.Size = new System.Drawing.Size(472, 208);
            this.EventText.TabIndex = 1;
            this.EventText.TabStop = false;
            this.EventText.TextChanged += new System.EventHandler(this.EventText_TextChanged);
            // 
            // splitter1
            // 
            this.splitter1.BackColor = System.Drawing.SystemColors.ControlDark;
            this.splitter1.Dock = System.Windows.Forms.DockStyle.Top;
            this.splitter1.Location = new System.Drawing.Point(0, 176);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(472, 3);
            this.splitter1.TabIndex = 2;
            this.splitter1.TabStop = false;
            // 
            // EventListView
            // 
            this.EventListView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.EventListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader4,
            this.columnHeader3});
            this.EventListView.Dock = System.Windows.Forms.DockStyle.Top;
            this.EventListView.FullRowSelect = true;
            this.EventListView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.EventListView.LargeImageList = this.iconImageList;
            this.EventListView.Location = new System.Drawing.Point(0, 0);
            this.EventListView.MultiSelect = false;
            this.EventListView.Name = "EventListView";
            this.EventListView.Size = new System.Drawing.Size(472, 176);
            this.EventListView.SmallImageList = this.iconImageList;
            this.EventListView.TabIndex = 0;
            this.EventListView.UseCompatibleStateImageBehavior = false;
            this.EventListView.View = System.Windows.Forms.View.Details;
            this.EventListView.SelectedIndexChanged += new System.EventHandler(this.EventListView_SelectedIndexChanged);
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "Origin";
            this.columnHeader4.Width = 150;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Message";
            this.columnHeader3.Width = 300;
            // 
            // iconImageList
            // 
            this.iconImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("iconImageList.ImageStream")));
            this.iconImageList.TransparentColor = System.Drawing.Color.Transparent;
            this.iconImageList.Images.SetKeyName(0, "");
            this.iconImageList.Images.SetKeyName(1, "");
            this.iconImageList.Images.SetKeyName(2, "");
            this.iconImageList.Images.SetKeyName(3, "");
            this.iconImageList.Images.SetKeyName(4, "");
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.instanceListView);
            this.tabPage1.Location = new System.Drawing.Point(4, 4);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Size = new System.Drawing.Size(472, 387);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Instances";
            // 
            // instanceListView
            // 
            this.instanceListView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.instanceListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2});
            this.instanceListView.ContextMenu = this.DetailMenu;
            this.instanceListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.instanceListView.FullRowSelect = true;
            this.instanceListView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.instanceListView.Location = new System.Drawing.Point(0, 0);
            this.instanceListView.MultiSelect = false;
            this.instanceListView.Name = "instanceListView";
            this.instanceListView.Size = new System.Drawing.Size(472, 387);
            this.instanceListView.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.instanceListView.TabIndex = 1;
            this.instanceListView.UseCompatibleStateImageBehavior = false;
            this.instanceListView.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Count";
            this.columnHeader1.Width = 81;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Object";
            this.columnHeader2.Width = 370;
            // 
            // ToolsIconList
            // 
            this.ToolsIconList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ToolsIconList.ImageStream")));
            this.ToolsIconList.TransparentColor = System.Drawing.Color.Transparent;
            this.ToolsIconList.Images.SetKeyName(0, "");
            this.ToolsIconList.Images.SetKeyName(1, "");
            this.ToolsIconList.Images.SetKeyName(2, "");
            this.ToolsIconList.Images.SetKeyName(3, "");
            this.ToolsIconList.Images.SetKeyName(4, "");
            this.ToolsIconList.Images.SetKeyName(5, "");
            this.ToolsIconList.Images.SetKeyName(6, "");
            // 
            // eventToolBar
            // 
            this.eventToolBar.Appearance = System.Windows.Forms.ToolBarAppearance.Flat;
            this.eventToolBar.Buttons.AddRange(new System.Windows.Forms.ToolBarButton[] {
            this.toolBarButton3,
            this.showExceptionsToolBarButton,
            this.showWarningEventsToolBarButton,
            this.showInformationEventsToolBarButton,
            this.showAuditEventsToolBarButton,
            this.toolBarButton1,
            this.clearEventLogToolBarButton,
            this.toolBarButton2,
            this.gcToolBarButton});
            this.eventToolBar.DropDownArrows = true;
            this.eventToolBar.ImageList = this.ToolsIconList;
            this.eventToolBar.Location = new System.Drawing.Point(0, 0);
            this.eventToolBar.Name = "eventToolBar";
            this.eventToolBar.ShowToolTips = true;
            this.eventToolBar.Size = new System.Drawing.Size(480, 28);
            this.eventToolBar.TabIndex = 4;
            this.eventToolBar.ButtonClick += new System.Windows.Forms.ToolBarButtonClickEventHandler(this.eventToolBar_ButtonClick);
            // 
            // toolBarButton3
            // 
            this.toolBarButton3.Name = "toolBarButton3";
            this.toolBarButton3.Style = System.Windows.Forms.ToolBarButtonStyle.Separator;
            // 
            // showExceptionsToolBarButton
            // 
            this.showExceptionsToolBarButton.ImageIndex = 3;
            this.showExceptionsToolBarButton.Name = "showExceptionsToolBarButton";
            this.showExceptionsToolBarButton.Style = System.Windows.Forms.ToolBarButtonStyle.ToggleButton;
            this.showExceptionsToolBarButton.ToolTipText = "Show Exception Messages";
            // 
            // showWarningEventsToolBarButton
            // 
            this.showWarningEventsToolBarButton.ImageIndex = 2;
            this.showWarningEventsToolBarButton.Name = "showWarningEventsToolBarButton";
            this.showWarningEventsToolBarButton.Style = System.Windows.Forms.ToolBarButtonStyle.ToggleButton;
            this.showWarningEventsToolBarButton.ToolTipText = "Show Warnings Messages";
            // 
            // showInformationEventsToolBarButton
            // 
            this.showInformationEventsToolBarButton.ImageIndex = 1;
            this.showInformationEventsToolBarButton.Name = "showInformationEventsToolBarButton";
            this.showInformationEventsToolBarButton.Style = System.Windows.Forms.ToolBarButtonStyle.ToggleButton;
            this.showInformationEventsToolBarButton.ToolTipText = "Show Information Messages";
            // 
            // showAuditEventsToolBarButton
            // 
            this.showAuditEventsToolBarButton.ImageIndex = 6;
            this.showAuditEventsToolBarButton.Name = "showAuditEventsToolBarButton";
            this.showAuditEventsToolBarButton.Style = System.Windows.Forms.ToolBarButtonStyle.ToggleButton;
            this.showAuditEventsToolBarButton.ToolTipText = "Show Audit Messages";
            // 
            // toolBarButton1
            // 
            this.toolBarButton1.Name = "toolBarButton1";
            this.toolBarButton1.Style = System.Windows.Forms.ToolBarButtonStyle.Separator;
            // 
            // clearEventLogToolBarButton
            // 
            this.clearEventLogToolBarButton.ImageIndex = 4;
            this.clearEventLogToolBarButton.Name = "clearEventLogToolBarButton";
            this.clearEventLogToolBarButton.ToolTipText = "Clear Event Log";
            // 
            // toolBarButton2
            // 
            this.toolBarButton2.ImageIndex = 5;
            this.toolBarButton2.Name = "toolBarButton2";
            this.toolBarButton2.Style = System.Windows.Forms.ToolBarButtonStyle.Separator;
            // 
            // gcToolBarButton
            // 
            this.gcToolBarButton.ImageIndex = 5;
            this.gcToolBarButton.Name = "gcToolBarButton";
            this.gcToolBarButton.ToolTipText = "Force Garbage Collection";
            // 
            // saveLogFileDialog
            // 
            this.saveLogFileDialog.DefaultExt = "log";
            this.saveLogFileDialog.FileName = "DebugInformation.log";
            this.saveLogFileDialog.Filter = "Log Files|*.log";
            this.saveLogFileDialog.Title = "Save Debug Information Log";
            // 
            // InstanceTracker
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(480, 457);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.eventToolBar);
            this.Controls.Add(this.statusBar);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Menu = this.mainMenu;
            this.Name = "InstanceTracker";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Debug Information";
            this.Closed += new System.EventHandler(this.InstanceTracker_Closed);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.InstanceTracker_FormClosing);
            this.Load += new System.EventHandler(this.InstanceTracker_Load);
            this.tabControl1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.tabPage1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

		/// <summary>
		/// Add one to the counter for the type specified by object o.
		/// </summary>
		/// <param name="o"></param>
		public static void Add(object o) 
		{
			if (Enabled == false) return;
			lock (instancetable) 
			{
				System.Diagnostics.StackTrace ST = new System.Diagnostics.StackTrace();
				System.Diagnostics.StackFrame SF;
				int IDX = 0;
				StringBuilder sb = new StringBuilder();

				do
				{
					SF = ST.GetFrame(IDX);
					if (SF!=null)
					{
						if (sb.Length==0)
						{
							sb.Append(SF.GetMethod().DeclaringType.FullName + " : " + SF.GetMethod().Name);
						}
						else
						{
							sb.Append("\r\n");
							sb.Append(SF.GetMethod().DeclaringType.FullName + " : " + SF.GetMethod().Name);
						}
					}
					++IDX;
				}
                while(SF!=null&&IDX!=7);
				
				string name = o.GetType().FullName;
				if (DataTable.ContainsKey(name) == false) DataTable[name] = new ArrayList();
				InstanceStruct iss = new InstanceStruct();
				iss.WR = new WeakReference(o);
				iss.StackList = sb.ToString();
				((ArrayList)DataTable[name]).Add(iss);

				if (tracker != null)
				{
					//tracker.UpdateDisplayEntry(name);
                    tracker.instanceListView.BeginInvoke(new UpdateHandler(HandlerUpdate), new object[1] { name });
		            tracker.statusBar.BeginInvoke(new TrackerHandler(HandleTracker), new object[1] { o.GetType().FullName });
				}
			}
		}

        public static void HandleTracker(object name)
	    {
	    	tracker.statusBar.Text = "Add: " + (string)name;
	    }

        public static void HandlerUpdate(object name)
        {
            tracker.UpdateDisplayEntry((string) name);
        }

		/// <summary>
		/// Remove one to the counter for the type specified by object o.
		/// </summary>
		/// <param name="o"></param>
		public static void Remove(object o) 
		{
			if (Enabled == false) return;
			lock (instancetable)
			{
				if (tracker != null)
				{
					tracker.UpdateDisplayEntry(o.GetType().FullName);
					tracker.statusBar.Text = "Remove: " + o.GetType().FullName;
				}
			}
		}

		private void InstanceTracker_Closed(object sender, System.EventArgs e)
		{
			tracker.Dispose();
			tracker = null;
		}

		/// <summary>
		/// Display the tracking debug window to the user. If an instance of the windows
		/// is already shown, it will pop it back to front.
		/// </summary>
		public static void Display() 
		{
			if (tracker != null) 
			{
				tracker.Activate();
			} 
			else 
			{
				tracker = new InstanceTracker();
				tracker.Show();
			}
		}

        private void UpdateDisplay()
        {
            lock (instancetable)
            {
                instanceListView.Items.Clear();
                foreach (string name in instancetable.Keys)
                {
                    instanceListView.Items.Add(new ListViewItem(new string[] { instancetable[name].ToString(), name }));
                }
            }
            showExceptionsToolBarButton.Pushed = showExceptionsMenuItem.Checked;
            showWarningEventsToolBarButton.Pushed = showWarningsMenuItem.Checked;
            showInformationEventsToolBarButton.Pushed = showInformationMenuItem.Checked;
            showAuditEventsToolBarButton.Pushed = showAuditMenuItem.Checked;
        }

		private void UpdateDisplayEntry(string name) 
		{
			Recalculate(name);
			lock (instancetable) 
			{
				foreach (ListViewItem li in instanceListView.Items)
				{
					if (li.SubItems[1].Text == name) 
					{
						li.SubItems[0].Text = instancetable[name].ToString();
						return;
					}
				}

				instanceListView.Items.Add(new ListViewItem(new string[] {instancetable[name].ToString(),name}));
			}
		}

		private void InstanceTracker_Load(object sender, System.EventArgs e)
		{
			UpdateDisplay();
		}

		private void gcMenuItem_Click(object sender, System.EventArgs e)
		{
			GC.Collect();
			Recalculate();
			this.UpdateDisplay();
		}

		private void Recalculate(string name)
		{
			lock(instancetable)
			{
				if (DataTable.ContainsKey(name))
				{
					ArrayList A = (ArrayList)DataTable[name];
					ArrayList RemoveList = new ArrayList();
					foreach(InstanceStruct iss in A)
					{
						if (iss.WR.IsAlive==false)
						{
							RemoveList.Add(iss);
						}
					}
					foreach(InstanceStruct iss in RemoveList)
					{
						A.Remove(iss);
					}
					instancetable[name] = (long)A.Count;
				}
				else
				{
					instancetable[name] = (long)0;
				}
			}
		}
		private void Recalculate()
		{
			lock(instancetable)
			{
				IDictionaryEnumerator en = instancetable.GetEnumerator();
				ArrayList KeyList = new ArrayList();
				while(en.MoveNext())
				{
					KeyList.Add(en.Key);
				}
				foreach(string ObjectName in KeyList)
				{
					if (DataTable.ContainsKey(ObjectName))
					{
						ArrayList A = (ArrayList)DataTable[ObjectName];
						ArrayList RemoveList = new ArrayList();
						foreach(InstanceStruct iss in A)
						{
							if (iss.WR.IsAlive==false)
							{
								RemoveList.Add(iss);
							}
						}
						foreach(InstanceStruct iss in RemoveList)
						{
							A.Remove(iss);
						}
						instancetable[ObjectName] = (long)A.Count;
					}
				}
			}
		}

		private void closeMenuItem_Click(object sender, System.EventArgs e)
		{
			Close();
		}

		private void fullGcMenuItem_Click(object sender, System.EventArgs e)
		{
			long mem = GC.GetTotalMemory(true);
			statusBar.Text = "Memory: " + mem.ToString();
		}

		private void DetailMenuItem_Click(object sender, System.EventArgs e)
		{
			ListViewItem lvi = instanceListView.SelectedItems[0];
			string CompName = lvi.SubItems[1].Text;
			ArrayList a = (ArrayList)DataTable[CompName];
			ArrayList DL = new ArrayList();
			foreach(InstanceStruct iss in a)
			{
				if (iss.WR.IsAlive)
					DL.Add(iss.StackList);
			}
			if (DL.Count>0)
			{
				InstanceTracker2 it2 = new InstanceTracker2(DL);
				it2.Text = CompName;
				it2.ShowDialog();
				it2.Dispose();
			}
			else
			{
				MessageBox.Show("No details for this item");
			}
		}

		private void ClearEventMenuItem_Click(object sender, System.EventArgs e)
		{
			EventText.Text = "";
			EventListView.Items.Clear();
		}

		private void haltOnExceptionsMenuItem_Click(object sender, System.EventArgs e)
		{
			haltOnExceptionsMenuItem.Checked = !haltOnExceptionsMenuItem.Checked;
			EventLogger.SetOnExceptionAction(haltOnExceptionsMenuItem.Checked);
		}

		private void enableInstanceTrackingMenuItem_Click(object sender, System.EventArgs e)
		{
			enableInstanceTrackingMenuItem.Checked = !enableInstanceTrackingMenuItem.Checked;
			Enabled = enableInstanceTrackingMenuItem.Checked;
		}

		private void eventToolBar_ButtonClick(object sender, System.Windows.Forms.ToolBarButtonClickEventArgs e)
		{
			if (e.Button == showExceptionsToolBarButton)		showExceptionsMenuItem_Click(this,null);
			if (e.Button == showWarningEventsToolBarButton)		showWarningsMenuItem_Click(this,null);
			if (e.Button == showInformationEventsToolBarButton) showInformationMenuItem_Click(this,null);
			if (e.Button == showAuditEventsToolBarButton)		showAuditMenuItem_Click(this,null);
			if (e.Button == clearEventLogToolBarButton)			ClearEventMenuItem_Click(this,null);
			if (e.Button == gcToolBarButton)					gcMenuItem_Click(this,null);
		}

		private void saveAsMenuItem_Click(object sender, System.EventArgs e)
		{
			if (saveLogFileDialog.ShowDialog(this) == DialogResult.OK) 
			{
				StreamWriter file = File.CreateText(saveLogFileDialog.FileName);
				foreach (ListViewItem l in EventListView.Items) 
				{
					file.Write(l.Tag.ToString());
					file.Write("\r\n\r\n-------------------------------------------------\r\n");
				}
				file.Close();
			}
		}

		private void EventListView_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if (EventListView.SelectedItems.Count > 0)
			{
				string Tag = (string)EventListView.SelectedItems[0].Tag;
				EventText.Text = Tag;
			}
		}

		private void showExceptionsMenuItem_Click(object sender, System.EventArgs e)
		{
			showExceptionsMenuItem.Checked = !showExceptionsMenuItem.Checked;
			showExceptionsToolBarButton.Pushed = showExceptionsMenuItem.Checked;
			OpenSource.Utilities.EventLogger.ShowAll = showAuditMenuItem.Checked || showInformationMenuItem.Checked || showExceptionsMenuItem.Checked || showExceptionsMenuItem.Checked;
		}

		private void showWarningsMenuItem_Click(object sender, System.EventArgs e)
		{
			showWarningsMenuItem.Checked = !showWarningsMenuItem.Checked;
			showWarningEventsToolBarButton.Pushed = showWarningsMenuItem.Checked;
			OpenSource.Utilities.EventLogger.ShowAll = showAuditMenuItem.Checked || showInformationMenuItem.Checked || showExceptionsMenuItem.Checked || showExceptionsMenuItem.Checked;
		}

		private void showInformationMenuItem_Click(object sender, System.EventArgs e)
		{
			showInformationMenuItem.Checked = !showInformationMenuItem.Checked;
			showInformationEventsToolBarButton.Pushed = showInformationMenuItem.Checked;
			OpenSource.Utilities.EventLogger.ShowAll = showAuditMenuItem.Checked || showInformationMenuItem.Checked || showExceptionsMenuItem.Checked || showExceptionsMenuItem.Checked;
		}

		private void showAuditMenuItem_Click(object sender, System.EventArgs e)
		{
			showAuditMenuItem.Checked = !showAuditMenuItem.Checked;
			showAuditEventsToolBarButton.Pushed = showAuditMenuItem.Checked;
			OpenSource.Utilities.EventLogger.ShowAll = showAuditMenuItem.Checked || showInformationMenuItem.Checked || showExceptionsMenuItem.Checked || showExceptionsMenuItem.Checked;
		}

        public static void GenerateExceptionFile(string filename, string exception, string versionInfo)
        {
            try
            {
                FileStream fs = new FileStream(filename, FileMode.Append, FileAccess.Write);
                StringBuilder s = new StringBuilder();
                if (fs.Length == 0) s.Append("Please e-mail these exceptions to Ylian Saint-Hilaire, ylian.saint-hilaire@intel.com.").Append("\r\n\r\n");
                s.AppendFormat("********** {0}\r\n{1}\r\n\r\n", DateTime.Now.ToString(), exception);
                s.Append(versionInfo).Append("\r\n");
                //s.Append(GetLastEvents(10)).Append("\r\n\r\n");
                byte[] bytes = UTF8Encoding.UTF8.GetBytes(s.ToString());
                fs.Write(bytes, 0, bytes.Length);
                fs.Close();
                MessageBox.Show(string.Format("Exception error logged in: {0}\r\n\r\n{1}", fs.Name, "Please e-mail these exceptions to Ylian Saint-Hilaire, ylian.saint-hilaire@intel.com."), "Application Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex) 
            {
                OpenSource.Utilities.EventLogger.Log(ex);
            }
        }

        private void InstanceTracker_FormClosing(object sender, FormClosingEventArgs e)
        {
            OpenSource.Utilities.EventLogger.OnEvent -= new OpenSource.Utilities.EventLogger.EventHandler(OnEventSink);
        }

        private void EventText_TextChanged(object sender, EventArgs e)
        {

        }

	}
}
