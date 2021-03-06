﻿#region Copyright
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

using FileArchiver.Presentation.Commands.CommandSystem;
using FileArchiver.Presentation.OtherViews;
using FileArchiver.Presentation.Utils;
using FileArchiver.Presentation.FileListView;

namespace FileArchiver.Presentation.Commands
{
	public class AddFilesCommand : CommandBase
	{
		private readonly IDialogLauncher    mDialogLauncher;
		private readonly IFileListViewModel mFileListViewModel;

		public AddFilesCommand(IDialogLauncher dialogLauncher, IFileListViewModel fileListViewModel)
		{
			Contract.Requires(dialogLauncher != null);
			Contract.Requires(fileListViewModel != null);

			mDialogLauncher    = dialogLauncher;
			mFileListViewModel = fileListViewModel;

			Enabled            = false;

			mFileListViewModel.SubscribeToPropertyChanged(() => mFileListViewModel.AddFilesEnabled, AddFilesEnabledChanged);
		}

		public override Task ExecuteAsync()
		{
			var filesToAdd = mDialogLauncher.AskForFilesToAdd().ToList();
			if(!filesToAdd.Any())
				return Task.FromResult(0);

			mFileListViewModel.AddFiles(filesToAdd);

			return Task.FromResult(0);
		}

		private void AddFilesEnabledChanged(object sender, EventArgs e)
		{
			Enabled = mFileListViewModel.AddFilesEnabled;
		}
	}
}
