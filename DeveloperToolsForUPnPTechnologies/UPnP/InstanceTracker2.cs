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
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace OpenSource.Utilities
{
	/// <summary>
	/// Summary description for InstanceTracker2.
	/// </summary>
	internal class InstanceTracker2 : System.Windows.Forms.Form
	{
		private ArrayList TheData;
		private int Current = 0;
		private System.Windows.Forms.TextBox TextBox;
		private System.Windows.Forms.Button PreviousButton;
		private System.Windows.Forms.Button NextButton;
		private System.Windows.Forms.Label Status;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public InstanceTracker2(ArrayList DataList)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			TheData = DataList;
			ShowStatus();
			//
			// TODO: Add any constructor code after InitializeComponent call
			//
		}

		private void ShowStatus()
		{
			Status.Text = ((int)(Current+1)).ToString() + " of " + TheData.Count.ToString();
			TextBox.Text = (string)TheData[Current];
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
			this.TextBox = new System.Windows.Forms.TextBox();
			this.PreviousButton = new System.Windows.Forms.Button();
			this.NextButton = new System.Windows.Forms.Button();
			this.Status = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// TextBox
			// 
			this.TextBox.Anchor = (((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right);
			this.TextBox.Location = new System.Drawing.Point(8, 8);
			this.TextBox.Multiline = true;
			this.TextBox.Name = "TextBox";
			this.TextBox.ReadOnly = true;
			this.TextBox.Size = new System.Drawing.Size(344, 232);
			this.TextBox.TabIndex = 0;
			this.TextBox.Text = "";
			// 
			// PreviousButton
			// 
			this.PreviousButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.PreviousButton.Location = new System.Drawing.Point(8, 248);
			this.PreviousButton.Name = "PreviousButton";
			this.PreviousButton.TabIndex = 1;
			this.PreviousButton.Text = "<<";
			this.PreviousButton.Click += new System.EventHandler(this.PreviousButton_Click);
			// 
			// NextButton
			// 
			this.NextButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.NextButton.Location = new System.Drawing.Point(280, 248);
			this.NextButton.Name = "NextButton";
			this.NextButton.TabIndex = 2;
			this.NextButton.Text = ">>";
			this.NextButton.Click += new System.EventHandler(this.NextButton_Click);
			// 
			// Status
			// 
			this.Status.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.Status.Location = new System.Drawing.Point(128, 248);
			this.Status.Name = "Status";
			this.Status.TabIndex = 3;
			this.Status.Text = "1 of 1";
			this.Status.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// InstanceTracker2
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(360, 278);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.Status,
																		  this.NextButton,
																		  this.PreviousButton,
																		  this.TextBox});
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
			this.Name = "InstanceTracker2";
			this.Text = "InstanceTracker2";
			this.ResumeLayout(false);

		}
		#endregion

		private void PreviousButton_Click(object sender, System.EventArgs e)
		{
			--Current;
			if (Current<0) Current = 0;
			ShowStatus();
		}

		private void NextButton_Click(object sender, System.EventArgs e)
		{
			++Current;
			if (Current>TheData.Count-1) Current = TheData.Count-1;
			ShowStatus();
		}
	}
}
