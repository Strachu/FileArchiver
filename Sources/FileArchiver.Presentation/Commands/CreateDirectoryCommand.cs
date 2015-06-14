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
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FileArchiver.Core;
using FileArchiver.Core.Archive;
using FileArchiver.Core.Services;
using FileArchiver.Core.ValueTypes;
using FileArchiver.Presentation.Commands.CommandSystem;
using FileArchiver.Presentation.Utils;
using FileArchiver.Presentation.FileListView;

using Lang = FileArchiver.Presentation.Properties.Resources;

namespace FileArchiver.Presentation.Commands
{
	public class CreateDirectoryCommand : CommandBase
	{
		private readonly IFileListView      mFileListView;
		private readonly IFileListViewModel mFileListViewModel;
		private readonly IFileNameGenerator mUniqueFileNameGenerator;

		public CreateDirectoryCommand(IFileListView fileListView,
		                              IFileListViewModel fileListViewModel,
		                              IFileNameGenerator nameGenerator)
		{
			Contract.Requires(fileListView != null);
			Contract.Requires(fileListViewModel != null);
			Contract.Requires(nameGenerator != null);

			mFileListView            = fileListView;
			mFileListViewModel       = fileListViewModel;
			mUniqueFileNameGenerator = nameGenerator;

			Enabled                  = false;

			mFileListViewModel.SubscribeToPropertyChanged(() => mFileListViewModel.Archive, ArchiveChanged);
		}

		public override Task ExecuteAsync()
		{
			var candidatePath = mFileListViewModel.CurrentDirectory.Combine(new FileName(Lang.NewFolder));
			var directoryPath = mUniqueFileNameGenerator.GenerateFreeFileName(candidatePath,
			                                                                  mFileListViewModel.Archive.FileExists);

			mFileListViewModel.Archive.CreateDirectory(directoryPath);

			mFileListView.StartFileRenaming(directoryPath.FileName);

			return Task.FromResult(0);
		}

		private void ArchiveChanged(object sender, EventArgs e)
		{
			Enabled = true;
		}
	}
}
