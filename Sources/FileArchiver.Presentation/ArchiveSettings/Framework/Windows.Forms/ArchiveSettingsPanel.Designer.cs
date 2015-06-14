namespace FileArchiver.Presentation.ArchiveSettings.Framework.Windows.Forms
{
	partial class ArchiveSettingsPanel
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

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.mLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
			this.SuspendLayout();
			// 
			// mLayoutPanel
			// 
			this.mLayoutPanel.AutoSize = true;
			this.mLayoutPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.mLayoutPanel.ColumnCount = 2;
			this.mLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.mLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.mLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.mLayoutPanel.Location = new System.Drawing.Point(0, 0);
			this.mLayoutPanel.Name = "mLayoutPanel";
			this.mLayoutPanel.RowCount = 1;
			this.mLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.mLayoutPanel.Size = new System.Drawing.Size(0, 0);
			this.mLayoutPanel.TabIndex = 4;
			// 
			// ArchiveSettingsPanel
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSize = true;
			this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.Controls.Add(this.mLayoutPanel);
			this.Name = "ArchiveSettingsPanel";
			this.Size = new System.Drawing.Size(0, 0);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TableLayoutPanel mLayoutPanel;
	}
}
