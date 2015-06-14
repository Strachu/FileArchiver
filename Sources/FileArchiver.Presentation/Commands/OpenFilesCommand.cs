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

using FileArchiver.Core;
using FileArchiver.Core.ValueTypes;
using FileArchiver.Presentation.Commands.CommandSystem;
using FileArchiver.Presentation.FileListView;

namespace FileArchiver.Presentation.Commands
{
	public class OpenFilesCommand : CommandBase
	{
		private readonly IFileListViewModel mFileListViewModel;
		private readonly FileListPresenter  mFileListPresenter;

		public OpenFilesCommand(IFileListViewModel fileListViewModel,
		                        FileListPresenter fileListPresenter)
		{
			Contract.Requires(fileListViewModel != null);
			Contract.Requires(fileListPresenter != null);

			mFileListViewModel = fileListViewModel;
			mFileListPresenter = fileListPresenter;

			Enabled            = false;

			mFileListViewModel.FilesInCurrentDirectory.ListChanged += FilesInCurrentDirectory_ListChanged;
		}

		public override async Task ExecuteAsync()
		{
			var filesToExtract = mFileListViewModel.FilesInCurrentDirectory.Where(file => file.Selected)
				                                                            .Select(GetFilePath).ToList();

			await mFileListPresenter.OpenFiles(filesToExtract);
		}

		private Path GetFilePath(FileEntryViewModel file)
		{
			return mFileListViewModel.CurrentDirectory.Combine(file.Name);
		}

		private void FilesInCurrentDirectory_ListChanged(object sender, ListChangedEventArgs e)
		{
			Enabled = mFileListViewModel.FilesInCurrentDirectory.Any();
		}
	}
}
