namespace FileArchiver.Presentation.OtherViews
{
	partial class AboutForm
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
			System.Windows.Forms.Label label1;
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AboutForm));
			System.Windows.Forms.Label label3;
			System.Windows.Forms.GroupBox groupBox2;
			this.panel1 = new System.Windows.Forms.Panel();
			this.mCreditsLabel = new System.Windows.Forms.LinkLabel();
			this.mAppInfoLabel = new System.Windows.Forms.Label();
			this.mOkButton = new System.Windows.Forms.Button();
			this.mApplicationWebsiteLink = new System.Windows.Forms.LinkLabel();
			label1 = new System.Windows.Forms.Label();
			label3 = new System.Windows.Forms.Label();
			groupBox2 = new System.Windows.Forms.GroupBox();
			groupBox2.SuspendLayout();
			this.panel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// label1
			// 
			resources.ApplyResources(label1, "label1");
			label1.Name = "label1";
			// 
			// label3
			// 
			resources.ApplyResources(label3, "label3");
			label3.Name = "label3";
			// 
			// groupBox2
			// 
			groupBox2.Controls.Add(this.panel1);
			resources.ApplyResources(groupBox2, "groupBox2");
			groupBox2.Name = "groupBox2";
			groupBox2.TabStop = false;
			// 
			// panel1
			// 
			resources.ApplyResources(this.panel1, "panel1");
			this.panel1.Controls.Add(this.mCreditsLabel);
			this.panel1.Name = "panel1";
			// 
			// mCreditsLabel
			// 
			resources.ApplyResources(this.mCreditsLabel, "mCreditsLabel");
			this.mCreditsLabel.Name = "mCreditsLabel";
			this.mCreditsLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.LinkInCredits_Clicked);
			// 
			// mAppInfoLabel
			// 
			resources.ApplyResources(this.mAppInfoLabel, "mAppInfoLabel");
			this.mAppInfoLabel.Name = "mAppInfoLabel";
			// 
			// mOkButton
			// 
			resources.ApplyResources(this.mOkButton, "mOkButton");
			this.mOkButton.Name = "mOkButton";
			this.mOkButton.UseVisualStyleBackColor = true;
			this.mOkButton.Click += new System.EventHandler(this.OKButton_Click);
			// 
			// mApplicationWebsiteLink
			// 
			resources.ApplyResources(this.mApplicationWebsiteLink, "mApplicationWebsiteLink");
			this.mApplicationWebsiteLink.Name = "mApplicationWebsiteLink";
			this.mApplicationWebsiteLink.TabStop = true;
			this.mApplicationWebsiteLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.ApplicationWebsiteLink_Clicked);
			// 
			// AboutForm
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.mOkButton);
			this.Controls.Add(this.mApplicationWebsiteLink);
			this.Controls.Add(groupBox2);
			this.Controls.Add(label3);
			this.Controls.Add(label1);
			this.Controls.Add(this.mAppInfoLabel);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "AboutForm";
			this.ShowInTaskbar = false;
			groupBox2.ResumeLayout(false);
			this.panel1.ResumeLayout(false);
			this.panel1.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Label mAppInfoLabel;
		private System.Windows.Forms.Button mOkButton;
		private System.Windows.Forms.LinkLabel mApplicationWebsiteLink;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.LinkLabel mCreditsLabel;
	}
}