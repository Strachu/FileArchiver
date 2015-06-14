namespace FileArchiver.Presentation.PerFileErrorHandlers
{
	partial class FileExistsForm
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if(disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FileExistsForm));
			this.mErrorIconLabel = new System.Windows.Forms.Label();
			this.mMessageLabel = new System.Windows.Forms.Label();
			this.mOverwriteButton = new System.Windows.Forms.Button();
			this.mRenameButton = new System.Windows.Forms.Button();
			this.mSkipButton = new System.Windows.Forms.Button();
			this.mAbortButton = new System.Windows.Forms.Button();
			this.mApplyToAllCheckBox = new System.Windows.Forms.CheckBox();
			this.SuspendLayout();
			// 
			// mErrorIconLabel
			// 
			this.mErrorIconLabel.Image = global::FileArchiver.Presentation.Properties.Resources.Icon_Remove;
			resources.ApplyResources(this.mErrorIconLabel, "mErrorIconLabel");
			this.mErrorIconLabel.Name = "mErrorIconLabel";
			// 
			// mMessageLabel
			// 
			resources.ApplyResources(this.mMessageLabel, "mMessageLabel");
			this.mMessageLabel.Name = "mMessageLabel";
			// 
			// mOverwriteButton
			// 
			resources.ApplyResources(this.mOverwriteButton, "mOverwriteButton");
			this.mOverwriteButton.Name = "mOverwriteButton";
			this.mOverwriteButton.UseVisualStyleBackColor = true;
			this.mOverwriteButton.Click += new System.EventHandler(this.OverwriteButton_Click);
			// 
			// mRenameButton
			// 
			resources.ApplyResources(this.mRenameButton, "mRenameButton");
			this.mRenameButton.Name = "mRenameButton";
			this.mRenameButton.UseVisualStyleBackColor = true;
			this.mRenameButton.Click += new System.EventHandler(this.RenameButton_Click);
			// 
			// mSkipButton
			// 
			resources.ApplyResources(this.mSkipButton, "mSkipButton");
			this.mSkipButton.Name = "mSkipButton";
			this.mSkipButton.UseVisualStyleBackColor = true;
			this.mSkipButton.Click += new System.EventHandler(this.SkipButton_Click);
			// 
			// mAbortButton
			// 
			resources.ApplyResources(this.mAbortButton, "mAbortButton");
			this.mAbortButton.Name = "mAbortButton";
			this.mAbortButton.UseVisualStyleBackColor = true;
			this.mAbortButton.Click += new System.EventHandler(this.AbortButton_Click);
			// 
			// mApplyToAllCheckBox
			// 
			resources.ApplyResources(this.mApplyToAllCheckBox, "mApplyToAllCheckBox");
			this.mApplyToAllCheckBox.Name = "mApplyToAllCheckBox";
			this.mApplyToAllCheckBox.UseVisualStyleBackColor = true;
			// 
			// FileExistsForm
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.mApplyToAllCheckBox);
			this.Controls.Add(this.mAbortButton);
			this.Controls.Add(this.mSkipButton);
			this.Controls.Add(this.mRenameButton);
			this.Controls.Add(this.mOverwriteButton);
			this.Controls.Add(this.mMessageLabel);
			this.Controls.Add(this.mErrorIconLabel);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FileExistsForm";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label mErrorIconLabel;
		private System.Windows.Forms.Label mMessageLabel;
		private System.Windows.Forms.Button mOverwriteButton;
		private System.Windows.Forms.Button mRenameButton;
		private System.Windows.Forms.Button mSkipButton;
		private System.Windows.Forms.Button mAbortButton;
		private System.Windows.Forms.CheckBox mApplyToAllCheckBox;
	}
}