using System.Diagnostics.Contracts;
using System.Windows.Forms;

using FileArchiver.Presentation.Commands.CommandSystem.Windows.Forms;

namespace FileArchiver.Presentation.FileListView.Windows.Forms
{
	partial class FileListPanel
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
			System.Windows.Forms.ContextMenuStrip FileContextMenu;
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FileListPanel));
			System.Windows.Forms.TableLayoutPanel MainLayoutPanel;
			System.Windows.Forms.TableLayoutPanel NavigationBarLayoutPanel;
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.ToolTip MainToolTip;
			this.mOpenFileContextMenuEntry = new CommandToolStripMenuItem();
			this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
			this.mCreateDirectoryContextMenuEntry = new CommandToolStripMenuItem();
			this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripSeparator();
			this.mExtractContextMenuEntry = new CommandToolStripMenuItem();
			this.mRenameContextMenuEntry = new CommandToolStripMenuItem();
			this.mRemoveContextMenuEntry = new CommandToolStripMenuItem();
			this.mNavigateUpButton = new System.Windows.Forms.Button();
			this.mAddressBar = new System.Windows.Forms.TextBox();
			this.mFileDataGrid = new FileGridView();
			this.mFileDataGrid_FileColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.mFileDataGrid_SizeColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.mFileDataGrid_ModificationDateColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.mFileDataGrid_FilesInDirectoryCount = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.PaddingColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.mFileEntryBindingSource = new System.Windows.Forms.BindingSource(this.components);
			this.mValidationErrorTooltip = new System.Windows.Forms.ToolTip(this.components);
			FileContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
			MainLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
			NavigationBarLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
			MainToolTip = new System.Windows.Forms.ToolTip(this.components);
			FileContextMenu.SuspendLayout();
			MainLayoutPanel.SuspendLayout();
			NavigationBarLayoutPanel.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.mFileDataGrid)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.mFileEntryBindingSource)).BeginInit();
			this.SuspendLayout();
			// 
			// FileContextMenu
			// 
			FileContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mOpenFileContextMenuEntry,
            this.toolStripMenuItem1,
            this.mCreateDirectoryContextMenuEntry,
            this.toolStripMenuItem3,
            this.mExtractContextMenuEntry,
            this.mRenameContextMenuEntry,
            this.mRemoveContextMenuEntry});
			FileContextMenu.Name = "contextMenuStrip1";
			FileContextMenu.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
			resources.ApplyResources(FileContextMenu, "FileContextMenu");
			// 
			// mOpenFileContextMenuEntry
			// 
			this.mOpenFileContextMenuEntry.Name = "mOpenFileContextMenuEntry";
			resources.ApplyResources(this.mOpenFileContextMenuEntry, "mOpenFileContextMenuEntry");
			// 
			// toolStripMenuItem1
			// 
			this.toolStripMenuItem1.Name = "toolStripMenuItem1";
			resources.ApplyResources(this.toolStripMenuItem1, "toolStripMenuItem1");
			// 
			// mCreateDirectoryContextMenuEntry
			// 
			this.mCreateDirectoryContextMenuEntry.Name = "mCreateDirectoryContextMenuEntry";
			resources.ApplyResources(this.mCreateDirectoryContextMenuEntry, "mCreateDirectoryContextMenuEntry");
			// 
			// toolStripMenuItem3
			// 
			this.toolStripMenuItem3.Name = "toolStripMenuItem3";
			resources.ApplyResources(this.toolStripMenuItem3, "toolStripMenuItem3");
			// 
			// mExtractContextMenuEntry
			// 
			this.mExtractContextMenuEntry.Name = "mExtractContextMenuEntry";
			resources.ApplyResources(this.mExtractContextMenuEntry, "mExtractContextMenuEntry");
			// 
			// mRenameContextMenuEntry
			// 
			this.mRenameContextMenuEntry.Name = "mRenameContextMenuEntry";
			resources.ApplyResources(this.mRenameContextMenuEntry, "mRenameContextMenuEntry");
			// 
			// mRemoveContextMenuEntry
			// 
			this.mRemoveContextMenuEntry.Name = "mRemoveContextMenuEntry";
			resources.ApplyResources(this.mRemoveContextMenuEntry, "mRemoveContextMenuEntry");
			// 
			// MainLayoutPanel
			// 
			resources.ApplyResources(MainLayoutPanel, "MainLayoutPanel");
			MainLayoutPanel.Controls.Add(NavigationBarLayoutPanel, 0, 0);
			MainLayoutPanel.Controls.Add(this.mFileDataGrid, 0, 1);
			MainLayoutPanel.Name = "MainLayoutPanel";
			// 
			// NavigationBarLayoutPanel
			// 
			resources.ApplyResources(NavigationBarLayoutPanel, "NavigationBarLayoutPanel");
			NavigationBarLayoutPanel.Controls.Add(this.mNavigateUpButton, 0, 0);
			NavigationBarLayoutPanel.Controls.Add(this.mAddressBar, 1, 0);
			NavigationBarLayoutPanel.Name = "NavigationBarLayoutPanel";
			// 
			// mNavigateUpButton
			// 
			resources.ApplyResources(this.mNavigateUpButton, "mNavigateUpButton");
			this.mNavigateUpButton.FlatAppearance.BorderSize = 0;
			this.mNavigateUpButton.Image = global::FileArchiver.Presentation.Properties.Resources.Icon_MoveUp;
			this.mNavigateUpButton.Name = "mNavigateUpButton";
			MainToolTip.SetToolTip(this.mNavigateUpButton, resources.GetString("mNavigateUpButton.ToolTip"));
			this.mNavigateUpButton.UseVisualStyleBackColor = true;
			this.mNavigateUpButton.Click += new System.EventHandler(this.NavigateUpButton_Click);
			// 
			// mAddressBar
			// 
			this.mAddressBar.BackColor = System.Drawing.SystemColors.ButtonHighlight;
			resources.ApplyResources(this.mAddressBar, "mAddressBar");
			this.mAddressBar.Name = "mAddressBar";
			this.mAddressBar.ReadOnly = true;
			// 
			// mFileDataGrid
			// 
			this.mFileDataGrid.AllowDrop = true;
			this.mFileDataGrid.AllowUserToAddRows = false;
			this.mFileDataGrid.AllowUserToDeleteRows = false;
			this.mFileDataGrid.AllowUserToOrderColumns = true;
			this.mFileDataGrid.AllowUserToResizeRows = false;
			dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(234)))), ((int)(((byte)(234)))), ((int)(((byte)(234)))));
			this.mFileDataGrid.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
			this.mFileDataGrid.AutoGenerateColumns = false;
			this.mFileDataGrid.BackgroundColor = System.Drawing.SystemColors.Window;
			this.mFileDataGrid.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.mFileDataGrid.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
			this.mFileDataGrid.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.Disable;
			this.mFileDataGrid.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
			dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
			dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Control;
			dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
			dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.WindowText;
			dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
			dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
			dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
			this.mFileDataGrid.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
			this.mFileDataGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
			this.mFileDataGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.mFileDataGrid_FileColumn,
            this.mFileDataGrid_SizeColumn,
            this.mFileDataGrid_ModificationDateColumn,
            this.mFileDataGrid_FilesInDirectoryCount,
            this.PaddingColumn});
			this.mFileDataGrid.ContextMenuStrip = FileContextMenu;
			this.mFileDataGrid.DataSource = this.mFileEntryBindingSource;
			resources.ApplyResources(this.mFileDataGrid, "mFileDataGrid");
			this.mFileDataGrid.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
			this.mFileDataGrid.GridColor = System.Drawing.SystemColors.Control;
			this.mFileDataGrid.Name = "mFileDataGrid";
			this.mFileDataGrid.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
			this.mFileDataGrid.RowHeadersVisible = false;
			this.mFileDataGrid.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
			this.mFileDataGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
			this.mFileDataGrid.ShowEditingIcon = false;
			this.mFileDataGrid.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.FileDoubleClick);
			this.mFileDataGrid.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.FileDataGrid_CellEndEdit);
			this.mFileDataGrid.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.FileDataGrid_CellFormatting);
			this.mFileDataGrid.CellValidating += new System.Windows.Forms.DataGridViewCellValidatingEventHandler(this.FileDataGrid_CellValidating);
			this.mFileDataGrid.Scroll += new System.Windows.Forms.ScrollEventHandler(this.FileDataGrid_Scroll);
			this.mFileDataGrid.SelectionChanged += new System.EventHandler(this.FileDataGrid_SelectionChanged);
			this.mFileDataGrid.DragDrop += new System.Windows.Forms.DragEventHandler(this.FileDataGrid_DragDrop);
			this.mFileDataGrid.DragEnter += new System.Windows.Forms.DragEventHandler(this.FileDataGrid_DragEnter);
			// 
			// mFileDataGrid_FileColumn
			// 
			this.mFileDataGrid_FileColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
			this.mFileDataGrid_FileColumn.DataPropertyName = "Name";
			resources.ApplyResources(this.mFileDataGrid_FileColumn, "mFileDataGrid_FileColumn");
			this.mFileDataGrid_FileColumn.Name = "mFileDataGrid_FileColumn";
			this.mFileDataGrid_FileColumn.Resizable = System.Windows.Forms.DataGridViewTriState.True;
			// 
			// mFileDataGrid_SizeColumn
			// 
			this.mFileDataGrid_SizeColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
			this.mFileDataGrid_SizeColumn.DataPropertyName = "Size";
			dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
			this.mFileDataGrid_SizeColumn.DefaultCellStyle = dataGridViewCellStyle3;
			resources.ApplyResources(this.mFileDataGrid_SizeColumn, "mFileDataGrid_SizeColumn");
			this.mFileDataGrid_SizeColumn.Name = "mFileDataGrid_SizeColumn";
			this.mFileDataGrid_SizeColumn.ReadOnly = true;
			// 
			// mFileDataGrid_ModificationDateColumn
			// 
			this.mFileDataGrid_ModificationDateColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
			this.mFileDataGrid_ModificationDateColumn.DataPropertyName = "LastModificationTime";
			dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
			dataGridViewCellStyle4.Format = "G";
			dataGridViewCellStyle4.NullValue = null;
			this.mFileDataGrid_ModificationDateColumn.DefaultCellStyle = dataGridViewCellStyle4;
			resources.ApplyResources(this.mFileDataGrid_ModificationDateColumn, "mFileDataGrid_ModificationDateColumn");
			this.mFileDataGrid_ModificationDateColumn.Name = "mFileDataGrid_ModificationDateColumn";
			this.mFileDataGrid_ModificationDateColumn.ReadOnly = true;
			this.mFileDataGrid_ModificationDateColumn.Resizable = System.Windows.Forms.DataGridViewTriState.True;
			// 
			// mFileDataGrid_FilesInDirectoryCount
			// 
			this.mFileDataGrid_FilesInDirectoryCount.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
			this.mFileDataGrid_FilesInDirectoryCount.DataPropertyName = "FileCount";
			resources.ApplyResources(this.mFileDataGrid_FilesInDirectoryCount, "mFileDataGrid_FilesInDirectoryCount");
			this.mFileDataGrid_FilesInDirectoryCount.Name = "mFileDataGrid_FilesInDirectoryCount";
			this.mFileDataGrid_FilesInDirectoryCount.ReadOnly = true;
			// 
			// PaddingColumn
			// 
			this.PaddingColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
			resources.ApplyResources(this.PaddingColumn, "PaddingColumn");
			this.PaddingColumn.Name = "PaddingColumn";
			this.PaddingColumn.ReadOnly = true;
			this.PaddingColumn.Resizable = System.Windows.Forms.DataGridViewTriState.False;
			this.PaddingColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
			// 
			// mFileEntryBindingSource
			// 
			this.mFileEntryBindingSource.DataSource = typeof(FileArchiver.Presentation.FileListView.FileEntryViewModel);
			// 
			// mValidationErrorTooltip
			// 
			this.mValidationErrorTooltip.ForeColor = System.Drawing.Color.Red;
			this.mValidationErrorTooltip.ToolTipIcon = System.Windows.Forms.ToolTipIcon.Error;
			// 
			// FileListPanel
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(MainLayoutPanel);
			this.Name = "FileListPanel";
			FileContextMenu.ResumeLayout(false);
			MainLayoutPanel.ResumeLayout(false);
			NavigationBarLayoutPanel.ResumeLayout(false);
			NavigationBarLayoutPanel.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.mFileDataGrid)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.mFileEntryBindingSource)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private TextBox mAddressBar;
		private Button mNavigateUpButton;
		private CommandToolStripMenuItem mOpenFileContextMenuEntry;
		private CommandToolStripMenuItem mExtractContextMenuEntry;
		private CommandToolStripMenuItem mRenameContextMenuEntry;
		private CommandToolStripMenuItem mRemoveContextMenuEntry;
		private ToolStripSeparator toolStripMenuItem1;
		private CommandToolStripMenuItem mCreateDirectoryContextMenuEntry;
		private ToolStripSeparator toolStripMenuItem3;
		private BindingSource mFileEntryBindingSource;
		private ToolTip mValidationErrorTooltip;
		private DataGridViewTextBoxColumn mFileDataGrid_FileColumn;
		private DataGridViewTextBoxColumn mFileDataGrid_SizeColumn;
		private DataGridViewTextBoxColumn mFileDataGrid_ModificationDateColumn;
		private DataGridViewTextBoxColumn mFileDataGrid_FilesInDirectoryCount;
		private DataGridViewTextBoxColumn PaddingColumn;
		private FileGridView mFileDataGrid;
	}
}

