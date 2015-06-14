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
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FileArchiver.Presentation.Commands.CommandSystem;
using FileArchiver.Presentation.FileListView;

namespace FileArchiver.Presentation.Commands
{
	public class RenameFileCommand : CommandBase
	{
		private readonly IFileListView      mFileListView;
		private readonly IFileListViewModel mFileListViewModel;

		public RenameFileCommand(IFileListView fileListView, IFileListViewModel fileListViewModel)
		{
			Contract.Requires(fileListView != null);
			Contract.Requires(fileListViewModel != null);

			mFileListView      = fileListView;
			mFileListViewModel = fileListViewModel;

			Enabled            = false;

			mFileListViewModel.FilesInCurrentDirectory.ListChanged += FilesInCurrentDirectory_ListChanged;
		}

		public override Task ExecuteAsync()
		{
			var fileToRename = mFileListViewModel.FilesInCurrentDirectory.First(file => file.Selected);

			mFileListView.StartFileRenaming(fileToRename.Name);

			return Task.FromResult(0);
		}

		private void FilesInCurrentDirectory_ListChanged(object sender, ListChangedEventArgs e)
		{
			Enabled = mFileListViewModel.FilesInCurrentDirectory.Any();
		}
	}
}
