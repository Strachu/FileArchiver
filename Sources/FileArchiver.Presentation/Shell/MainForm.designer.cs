using System.Diagnostics.Contracts;
using System.Windows.Forms;

using FileArchiver.Presentation.Commands.CommandSystem.Windows.Forms;
using FileArchiver.Presentation.FileListView.Windows.Forms;

namespace FileArchiver.Presentation.Shell
{
	partial class MainForm
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
			if (disposing && (components != null))
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
		[ContractVerification(false)]
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
			System.Windows.Forms.MainMenu Menu;
			System.Windows.Forms.ToolStrip ToolBar;
			System.Windows.Forms.TableLayoutPanel MainLayoutPanel;
			System.Windows.Forms.ToolTip MainToolTip;
			this.mArchiveMenu = new System.Windows.Forms.MenuItem();
			this.mNewArchiveMenuEntry = new CommandMenuItem();
			this.mOpenArchiveMenuEntry = new CommandMenuItem();
			this.mSaveArchiveMenuEntry = new CommandMenuItem();
			this.mEditMenu = new System.Windows.Forms.MenuItem();
			this.mSelectAllMenuEntry = new System.Windows.Forms.MenuItem();
			this.mLanguageMenu = new System.Windows.Forms.MenuItem();
			this.mEnglishLanguageMenuEntry = new CommandMenuItem();
			this.mPolishLanguageMenuEntry = new CommandMenuItem();
			this.mAboutMenuEntry = new System.Windows.Forms.MenuItem();
			this.mNewToolbarButton = new CommandToolStripButton();
			this.mOpenToolbarButton = new CommandToolStripButton();
			this.mSaveToolbarButton = new CommandToolStripButton();
			this.mAddToolbarButton = new CommandToolStripButton();
			this.mExtractToolbarButton = new CommandToolStripButton();
			this.mRemoveToolbarButton = new CommandToolStripButton();
			this.mFileListRegion = new FileListRegion();
			this.mStatusBar = new System.Windows.Forms.StatusStrip();
			this.mStatusBarLabel = new System.Windows.Forms.ToolStripStatusLabel();
			toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			Menu = new System.Windows.Forms.MainMenu(this.components);
			ToolBar = new System.Windows.Forms.ToolStrip();
			MainLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
			MainToolTip = new System.Windows.Forms.ToolTip(this.components);
			ToolBar.SuspendLayout();
			MainLayoutPanel.SuspendLayout();
			this.mStatusBar.SuspendLayout();
			this.SuspendLayout();
			// 
			// toolStripSeparator1
			// 
			toolStripSeparator1.Name = "toolStripSeparator1";
			resources.ApplyResources(toolStripSeparator1, "toolStripSeparator1");
			// 
			// Menu
			// 
			Menu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.mArchiveMenu,
            this.mEditMenu,
            this.mLanguageMenu,
            this.mAboutMenuEntry});
			// 
			// mArchiveMenu
			// 
			this.mArchiveMenu.Index = 0;
			this.mArchiveMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.mNewArchiveMenuEntry,
            this.mOpenArchiveMenuEntry,
            this.mSaveArchiveMenuEntry});
			resources.ApplyResources(this.mArchiveMenu, "mArchiveMenu");
			// 
			// mNewArchiveMenuEntry
			// 
			this.mNewArchiveMenuEntry.Index = 0;
			resources.ApplyResources(this.mNewArchiveMenuEntry, "mNewArchiveMenuEntry");
			// 
			// mOpenArchiveMenuEntry
			// 
			this.mOpenArchiveMenuEntry.Index = 1;
			resources.ApplyResources(this.mOpenArchiveMenuEntry, "mOpenArchiveMenuEntry");
			// 
			// mSaveArchiveMenuEntry
			// 
			this.mSaveArchiveMenuEntry.Index = 2;
			resources.ApplyResources(this.mSaveArchiveMenuEntry, "mSaveArchiveMenuEntry");
			// 
			// mEditMenu
			// 
			this.mEditMenu.Index = 1;
			this.mEditMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.mSelectAllMenuEntry});
			resources.ApplyResources(this.mEditMenu, "mEditMenu");
			// 
			// mSelectAllMenuEntry
			// 
			this.mSelectAllMenuEntry.Index = 0;
			resources.ApplyResources(this.mSelectAllMenuEntry, "mSelectAllMenuEntry");
			this.mSelectAllMenuEntry.Click += new System.EventHandler(this.SelectAll_Click);
			// 
			// mLanguageMenu
			// 
			this.mLanguageMenu.Index = 2;
			this.mLanguageMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.mEnglishLanguageMenuEntry,
            this.mPolishLanguageMenuEntry});
			resources.ApplyResources(this.mLanguageMenu, "mLanguageMenu");
			// 
			// mEnglishLanguageMenuEntry
			// 
			this.mEnglishLanguageMenuEntry.Checked = true;
			this.mEnglishLanguageMenuEntry.Index = 0;
			this.mEnglishLanguageMenuEntry.RadioCheck = true;
			this.mEnglishLanguageMenuEntry.Tag = "en-US";
			resources.ApplyResources(this.mEnglishLanguageMenuEntry, "mEnglishLanguageMenuEntry");
			// 
			// mPolishLanguageMenuEntry
			// 
			this.mPolishLanguageMenuEntry.Index = 1;
			this.mPolishLanguageMenuEntry.RadioCheck = true;
			this.mPolishLanguageMenuEntry.Tag = "pl-PL";
			resources.ApplyResources(this.mPolishLanguageMenuEntry, "mPolishLanguageMenuEntry");
			// 
			// mAboutMenuEntry
			// 
			this.mAboutMenuEntry.Index = 3;
			resources.ApplyResources(this.mAboutMenuEntry, "mAboutMenuEntry");
			this.mAboutMenuEntry.Click += new System.EventHandler(this.About_Click);
			// 
			// ToolBar
			// 
			ToolBar.AllowMerge = false;
			ToolBar.BackColor = System.Drawing.SystemColors.Control;
			resources.ApplyResources(ToolBar, "ToolBar");
			ToolBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mNewToolbarButton,
            this.mOpenToolbarButton,
            this.mSaveToolbarButton,
            toolStripSeparator1,
            this.mAddToolbarButton,
            this.mExtractToolbarButton,
            this.mRemoveToolbarButton});
			ToolBar.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.HorizontalStackWithOverflow;
			ToolBar.Name = "ToolBar";
			ToolBar.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
			// 
			// mNewToolbarButton
			// 
			this.mNewToolbarButton.Image = global::FileArchiver.Presentation.Properties.Resources.Icon_New;
			resources.ApplyResources(this.mNewToolbarButton, "mNewToolbarButton");
			this.mNewToolbarButton.Name = "mNewToolbarButton";
			// 
			// mOpenToolbarButton
			// 
			this.mOpenToolbarButton.Image = global::FileArchiver.Presentation.Properties.Resources.Icon_Open;
			resources.ApplyResources(this.mOpenToolbarButton, "mOpenToolbarButton");
			this.mOpenToolbarButton.Name = "mOpenToolbarButton";
			// 
			// mSaveToolbarButton
			// 
			this.mSaveToolbarButton.Image = global::FileArchiver.Presentation.Properties.Resources.Icon_Save;
			resources.ApplyResources(this.mSaveToolbarButton, "mSaveToolbarButton");
			this.mSaveToolbarButton.Name = "mSaveToolbarButton";
			// 
			// mAddToolbarButton
			// 
			this.mAddToolbarButton.Image = global::FileArchiver.Presentation.Properties.Resources.Icon_Add;
			resources.ApplyResources(this.mAddToolbarButton, "mAddToolbarButton");
			this.mAddToolbarButton.Name = "mAddToolbarButton";
			// 
			// mExtractToolbarButton
			// 
			this.mExtractToolbarButton.Image = global::FileArchiver.Presentation.Properties.Resources.Icon_Extract;
			resources.ApplyResources(this.mExtractToolbarButton, "mExtractToolbarButton");
			this.mExtractToolbarButton.Name = "mExtractToolbarButton";
			// 
			// mRemoveToolbarButton
			// 
			this.mRemoveToolbarButton.Image = global::FileArchiver.Presentation.Properties.Resources.Icon_Remove;
			resources.ApplyResources(this.mRemoveToolbarButton, "mRemoveToolbarButton");
			this.mRemoveToolbarButton.Name = "mRemoveToolbarButton";
			// 
			// MainLayoutPanel
			// 
			resources.ApplyResources(MainLayoutPanel, "MainLayoutPanel");
			MainLayoutPanel.Controls.Add(ToolBar, 0, 0);
			MainLayoutPanel.Controls.Add(this.mFileListRegion, 0, 1);
			MainLayoutPanel.Controls.Add(this.mStatusBar, 0, 2);
			MainLayoutPanel.Name = "MainLayoutPanel";
			// 
			// mFileListRegion
			// 
			resources.ApplyResources(this.mFileListRegion, "mFileListRegion");
			this.mFileListRegion.Name = "mFileListRegion";
			// 
			// mStatusBar
			// 
			resources.ApplyResources(this.mStatusBar, "mStatusBar");
			this.mStatusBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mStatusBarLabel});
			this.mStatusBar.Name = "mStatusBar";
			this.mStatusBar.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
			// 
			// mStatusBarLabel
			// 
			this.mStatusBarLabel.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			resources.ApplyResources(this.mStatusBarLabel, "mStatusBarLabel");
			this.mStatusBarLabel.Margin = new System.Windows.Forms.Padding(0, 2, 0, 2);
			this.mStatusBarLabel.Name = "mStatusBarLabel";
			// 
			// MainForm
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(MainLayoutPanel);
			this.Menu = Menu;
			this.Name = "MainForm";
			ToolBar.ResumeLayout(false);
			ToolBar.PerformLayout();
			MainLayoutPanel.ResumeLayout(false);
			MainLayoutPanel.PerformLayout();
			this.mStatusBar.ResumeLayout(false);
			this.mStatusBar.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.ToolStripStatusLabel mStatusBarLabel;
		private MenuItem mLanguageMenu;
		private CommandToolStripButton mAddToolbarButton;
		private CommandToolStripButton mExtractToolbarButton;
		private CommandToolStripButton mRemoveToolbarButton;
		private CommandToolStripButton mOpenToolbarButton;
		private CommandToolStripButton mSaveToolbarButton;
		private CommandToolStripButton mNewToolbarButton;
		private MenuItem mSelectAllMenuEntry;
		private CommandMenuItem mPolishLanguageMenuEntry;
		private CommandMenuItem mEnglishLanguageMenuEntry;
		private MenuItem mArchiveMenu;
		private CommandMenuItem mNewArchiveMenuEntry;
		private CommandMenuItem mOpenArchiveMenuEntry;
		private CommandMenuItem mSaveArchiveMenuEntry;
		private MenuItem mEditMenu;
		private MenuItem mAboutMenuEntry;
		private StatusStrip mStatusBar;
		private FileListRegion mFileListRegion;
	}
}

