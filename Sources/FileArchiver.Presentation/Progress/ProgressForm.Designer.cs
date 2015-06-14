namespace FileArchiver.Presentation.Progress
{
	partial class ProgressForm
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProgressForm));
			this.mLabel = new System.Windows.Forms.Label();
			this.mCancelButton = new System.Windows.Forms.Button();
			this.mProgressBar = new System.Windows.Forms.ProgressBar();
			this.SuspendLayout();
			// 
			// mLabel
			// 
			resources.ApplyResources(this.mLabel, "mLabel");
			this.mLabel.Name = "mLabel";
			// 
			// mCancelButton
			// 
			resources.ApplyResources(this.mCancelButton, "mCancelButton");
			this.mCancelButton.Name = "mCancelButton";
			this.mCancelButton.UseVisualStyleBackColor = true;
			this.mCancelButton.Click += new System.EventHandler(this.Cancel_Click);
			// 
			// mProgressBar
			// 
			resources.ApplyResources(this.mProgressBar, "mProgressBar");
			this.mProgressBar.Maximum = 10000;
			this.mProgressBar.Name = "mProgressBar";
			this.mProgressBar.Step = 100;
			// 
			// ProgressForm
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.mLabel);
			this.Controls.Add(this.mProgressBar);
			this.Controls.Add(this.mCancelButton);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ProgressForm";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Close_Click);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ProgressBar mProgressBar;
		private System.Windows.Forms.Button mCancelButton;
		private System.Windows.Forms.Label mLabel;
	}
}