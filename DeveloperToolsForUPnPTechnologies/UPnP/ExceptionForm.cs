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
using System.Windows.Forms;
using System.ComponentModel;

namespace OpenSource.Utilities
{
	/// <summary>
	/// Summary description for ExceptionForm.
	/// </summary>
	internal class ExceptionForm : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Button breakButton;
		private System.Windows.Forms.Button ignoreButton;
		private System.Windows.Forms.TextBox ErrorBox;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public ExceptionForm(Exception e)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			Text = e.Source;
			ErrorBox.Text = e.ToString();
			ErrorBox.SelectionLength = 0;
			//
			// TODO: Add any constructor code after InitializeComponent call
			//
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
			this.ErrorBox = new System.Windows.Forms.TextBox();
			this.breakButton = new System.Windows.Forms.Button();
			this.ignoreButton = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// ErrorBox
			// 
			this.ErrorBox.Anchor = (((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right);
			this.ErrorBox.Location = new System.Drawing.Point(8, 8);
			this.ErrorBox.Multiline = true;
			this.ErrorBox.Name = "ErrorBox";
			this.ErrorBox.ReadOnly = true;
			this.ErrorBox.Size = new System.Drawing.Size(416, 168);
			this.ErrorBox.TabIndex = 0;
			this.ErrorBox.Text = "";
			// 
			// breakButton
			// 
			this.breakButton.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right);
			this.breakButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.breakButton.Location = new System.Drawing.Point(272, 184);
			this.breakButton.Name = "breakButton";
			this.breakButton.TabIndex = 1;
			this.breakButton.Text = "Break";
			this.breakButton.Click += new System.EventHandler(this.breakButton_Click);
			// 
			// ignoreButton
			// 
			this.ignoreButton.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right);
			this.ignoreButton.Location = new System.Drawing.Point(352, 184);
			this.ignoreButton.Name = "ignoreButton";
			this.ignoreButton.TabIndex = 2;
			this.ignoreButton.Text = "Ignore";
			this.ignoreButton.Click += new System.EventHandler(this.ignoreButton_Click);
			// 
			// ExceptionForm
			// 
			this.AcceptButton = this.ignoreButton;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(432, 214);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.ignoreButton,
																		  this.breakButton,
																		  this.ErrorBox});
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
			this.Name = "ExceptionForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "ExceptionForm";
			this.ResumeLayout(false);

		}
		#endregion

		private void ignoreButton_Click(object sender, System.EventArgs e)
		{
			this.DialogResult = DialogResult.Cancel;
		}

		private void breakButton_Click(object sender, System.EventArgs e)
		{
			this.DialogResult = DialogResult.OK;
		}
	}
}
