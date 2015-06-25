#region Copyright
/*
 * Copyright (C) 2015 Patryk Strach
 * 
 * This file is part of FileArchiver.
 * 
 * FileArchiver is free software: you can redistribute it and/or modify it under the terms of
 * the GNU Lesser General Public License as published by the Free Software Foundation,
 * either version 3 of the License, or (at your option) any later version.
 * 
 * FileArchiver is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
 * without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
 * See the GNU Lesser General Public License for more details.
 * 
 * You should have received a copy of the GNU Lesser General Public License along with FileArchiver.
 * If not, see <http://www.gnu.org/licenses/>.
*/
#endregion

using System;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

using FileArchiver.Core;
using FileArchiver.Core.Utils;
using FileArchiver.Core.ValueTypes;
using FileArchiver.Presentation.Commands;
using FileArchiver.Presentation.FileListView.Utils;
using FileArchiver.Presentation.OtherViews;
using FileArchiver.Presentation.Settings;
using FileArchiver.Presentation.Utils;
using FileArchiver.Presentation.Utils.Windows.Forms;

namespace FileArchiver.Presentation.FileListView.Windows.Forms
{
	/// <summary>
	/// A panel which allows the user to navigate the directory hierarchy in the archive.
	/// </summary>
	internal partial class FileListPanel : UserControl, IFileListView
	{
		private readonly IFileListViewModel mViewModel;

		public FileListPanel(IFileListViewModel viewModel, IDialogLauncher dialogLauncher)
		{
			Contract.Requires(viewModel != null);
			Contract.Requires(dialogLauncher != null);

			InitializeComponent();

			mViewModel = viewModel;

			mFileEntryBindingSource.DataSource    = viewModel.FilesInCurrentDirectory;
			mFileDataGrid_FileColumn.CellTemplate = new DataGridViewFileNameWithIconCell();

			ViewModel_CurrentDirectoryChanged(this, EventArgs.Empty);

			WireDataBinding();

			mViewModel.ErrorOccured +=	(sender, e) => dialogLauncher.DisplayError(e.ErrorMessage);
		}

		protected override void OnLoad(EventArgs e)
		{
			if(this.IsInDesignMode())
				return;

			mOpenFileContextMenuEntry.Command        = DependencyInjectionContainer.Instance.Get<OpenFilesCommand>();
			mCreateDirectoryContextMenuEntry.Command = DependencyInjectionContainer.Instance.Get<CreateDirectoryCommand>();
			mExtractContextMenuEntry.Command         = DependencyInjectionContainer.Instance.Get<ExtractFilesCommand>();
			mRemoveContextMenuEntry.Command          = DependencyInjectionContainer.Instance.Get<RemoveFilesCommand>();
			mRenameContextMenuEntry.Command          = DependencyInjectionContainer.Instance.Get<RenameFileCommand>();
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public FileListPanelLayoutSettings LayoutSettings
		{
			get
			{
				return new FileListPanelLayoutSettings
				{
					FileColumnWidth                    = mFileDataGrid_FileColumn.Width,
					SizeColumnWidth                    = mFileDataGrid_SizeColumn.Width,
					ModificationDateColumnWidth        = mFileDataGrid_ModificationDateColumn.Width,
					FilesInDirectoryCountWidth         = mFileDataGrid_FilesInDirectoryCount.Width,

					FileColumnDisplayIndex             = mFileDataGrid_FileColumn.DisplayIndex,
					SizeColumnDisplayIndex             = mFileDataGrid_SizeColumn.DisplayIndex,
					ModificationDateColumnDisplayIndex = mFileDataGrid_ModificationDateColumn.DisplayIndex,
					FilesInDirectoryCountDisplayIndex  = mFileDataGrid_FilesInDirectoryCount.DisplayIndex
				};
			}

			set
			{
				mFileDataGrid_FileColumn.Width                    = value.FileColumnWidth;
				mFileDataGrid_SizeColumn.Width                    = value.SizeColumnWidth;
				mFileDataGrid_ModificationDateColumn.Width        = value.ModificationDateColumnWidth;
				mFileDataGrid_FilesInDirectoryCount.Width         = value.FilesInDirectoryCountWidth;

				mFileDataGrid_FileColumn.DisplayIndex             = value.FileColumnDisplayIndex;
				mFileDataGrid_SizeColumn.DisplayIndex             = value.SizeColumnDisplayIndex;
				mFileDataGrid_ModificationDateColumn.DisplayIndex = value.ModificationDateColumnDisplayIndex;
				mFileDataGrid_FilesInDirectoryCount.DisplayIndex  = value.FilesInDirectoryCountDisplayIndex;
			}
		}

		private void NavigateUpButton_Click(object sender, EventArgs e)
		{
			mViewModel.NavigateToParentDirectory();
		}

		private FileName mFileNameBeforeRename;

		public void StartFileRenaming(FileName fileName)
		{
			mFileNameBeforeRename = fileName;

			var rowToEdit = mFileDataGrid.Rows.Cast<DataGridViewRow>().Single(row =>
			{
				return mViewModel.FilesInCurrentDirectory[row.Index].Name.Equals(fileName);
			});

			mFileDataGrid.CurrentCell = rowToEdit.Cells[mFileDataGrid_FileColumn.Index];
			mFileDataGrid.BeginEdit(true);
		}

		private void FileDataGrid_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
		{
			if(e.ColumnIndex != mFileDataGrid_FileColumn.Index)
				return;

			var eventArgs = new FileNameValidatingEventArgs(currentName:      mViewModel.FilesInCurrentDirectory[e.RowIndex].Name,
			                                                newNameCandidate: e.FormattedValue as string);

			FileNameValidating.SafeRaise(this, eventArgs);

			if(!String.IsNullOrEmpty(eventArgs.ErrorMessage))
			{
				mValidationErrorTooltip.Show(eventArgs.ErrorMessage, mFileDataGrid.EditingControl, duration: 2000);

				e.Cancel = true;
			}
		}

		private void FileDataGrid_CellEndEdit(object sender, DataGridViewCellEventArgs e)
		{
			var enteredName = mViewModel.FilesInCurrentDirectory[e.RowIndex].Name;

			FileNameAccepted.SafeRaise(this, new FileNameAcceptedEventArgs(mFileNameBeforeRename, enteredName));
		}

		public void SelectAll()
		{
			mFileDataGrid.SelectAll();
		}

		private void FileDoubleClick(object sender, DataGridViewCellEventArgs e)
		{
			if(IsIndexOfHeader(e.RowIndex))
				return;

			mViewModel.Open(mViewModel.FilesInCurrentDirectory[e.RowIndex].Name);
		}

		private static bool IsIndexOfHeader(int rowIndex)
		{
			return rowIndex < 0;
		}

		private void FileDataGrid_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
		{
			if(e.ColumnIndex == mFileDataGrid_FileColumn.Index)
			{
				if(mViewModel.FilesInCurrentDirectory[e.RowIndex].Modified)
				{
					e.CellStyle.Font = new Font(e.CellStyle.Font, FontStyle.Bold);
				}
			}

			if(e.ColumnIndex == mFileDataGrid_SizeColumn.Index)
			{
				e.Value             = UserFriendlySizeFormatter.Format((long)e.Value);
				e.FormattingApplied = true;
			}
		}

		private void FileDataGrid_DragEnter(object sender, DragEventArgs e)
		{
			if(!e.Data.GetDataPresent(DataFormats.FileDrop) || !mViewModel.AddFilesEnabled)
				return;

			e.Effect = DragDropEffects.Copy;
		}

		private void FileDataGrid_DragDrop(object sender, DragEventArgs e)
		{
			if(!e.Data.GetDataPresent(DataFormats.FileDrop) || !mViewModel.AddFilesEnabled)
				return;

			var droppedFiles = (string[])e.Data.GetData(DataFormats.FileDrop);

			mViewModel.AddFiles(droppedFiles.Select(path => new Path(path)).ToList());

			var parentForm = base.FindForm();
			if(parentForm != null)
			{
				parentForm.Activate();
			}

			e.Effect = DragDropEffects.Link;
		}

		public event EventHandler<FileNameValidatingEventArgs> FileNameValidating;
		public event EventHandler<FileNameAcceptedEventArgs>   FileNameAccepted;
	}
}
