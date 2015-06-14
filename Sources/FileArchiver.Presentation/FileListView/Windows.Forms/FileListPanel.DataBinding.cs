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
using System.Linq;
using System.Windows.Forms;

using FileArchiver.Core.Utils;
using FileArchiver.Presentation.Utils;
using FileArchiver.Presentation.Utils.Windows.Forms;

namespace FileArchiver.Presentation.FileListView.Windows.Forms
{
	internal partial class FileListPanel
	{
		private void WireDataBinding()
		{
			mNavigateUpButton.DataBindings.Add("Enabled", mViewModel,
			                                   PropertyName.Of(() => mViewModel.NavigateToParentDirectoryEnabled));
		
			mViewModel.SubscribeToPropertyChanged(() => mViewModel.CurrentDirectoryFullAddress,
			                                             ViewModel_CurrentDirectoryChanged);

			mViewModel.SubscribeToPropertyChanged(() => mViewModel.FirstDisplayedFileIndex,
			                                             ViewModel_FirstDisplayedFileIndexChanged);

			mViewModel.FilesInCurrentDirectory.ListChanged += FilesInCurrentDirectory_ListChanged;
		}

		private void ViewModel_CurrentDirectoryChanged(object sender, EventArgs e)
		{
			if(mViewModel.CurrentDirectoryFullAddress == null)
				return;

			mAddressBar.Text = mViewModel.CurrentDirectoryFullAddress;
			mAddressBar.ScrollToEnd();

			mFileDataGrid.Focus();
		}
		
		private void ViewModel_FirstDisplayedFileIndexChanged(object sender, EventArgs e)
		{
			mFileDataGrid.FirstDisplayedScrollingRowIndex = mViewModel.FirstDisplayedFileIndex;
		}

		private void FileDataGrid_Scroll(object sender, ScrollEventArgs e)
		{
			mViewModel.FirstDisplayedFileIndex = mFileDataGrid.FirstDisplayedScrollingRowIndex;
		}

		private void FilesInCurrentDirectory_ListChanged(object sender, ListChangedEventArgs e)
		{
			foreach(var file in mViewModel.FilesInCurrentDirectory)
			{
				file.PropertyChanged -= File_PropertyChanged;
				file.PropertyChanged += File_PropertyChanged;
			}
		}

		private void File_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			var file        = (FileEntryViewModel)sender;
			var rowToSelect = mFileDataGrid.Rows.Cast<DataGridViewRow>().SingleOrDefault(row =>
			{
				var currentRowFileName = mViewModel.FilesInCurrentDirectory[row.Index].Name;

				return currentRowFileName.Equals(file.Name);
			});

			// Required when sorting the entries. After sorting selection changed event is raised while there are no rows.
			if(rowToSelect == null)
				return;

			rowToSelect.Selected = file.Selected;
		}

		private void FileDataGrid_SelectionChanged(object sender, EventArgs e)
		{
			var selectedRowIndices = mFileDataGrid.SelectedRows.Cast<DataGridViewRow>().Select(row => row.Index).ToList();

			for(int i = 0; i < mViewModel.FilesInCurrentDirectory.Count; ++i)
			{
				mViewModel.FilesInCurrentDirectory[i].Selected = selectedRowIndices.Contains(i);				
			}
		}
	}
}
