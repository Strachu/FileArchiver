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

using FileArchiver.Core.Loaders;

using FileArchiver.Presentation.ArchiveSettings;
using FileArchiver.Presentation.Commands.CommandSystem;
using FileArchiver.Presentation.FileListView;
using FileArchiver.Presentation.Utils;

namespace FileArchiver.Presentation.Commands
{
	public class NewArchiveCommand : CommandBase
	{
		private readonly INewArchiveSettingsScreen mNewArchiveSettingsScreen;
		private readonly IArchiveLoadingService    mLoadingService;
		private readonly IFileListViewModel        mFileListViewModel;
		private readonly SaveArchiveCommand        mSaveCommand;

		public NewArchiveCommand(INewArchiveSettingsScreen newArchiveSettingsScreen,
		                         IArchiveLoadingService loadingService,
		                         IFileListViewModel fileListViewModel,
		                         SaveArchiveCommand saveCommand)
		{
			Contract.Requires(newArchiveSettingsScreen != null);
			Contract.Requires(loadingService != null);
			Contract.Requires(fileListViewModel != null);
			Contract.Requires(saveCommand != null);

			mNewArchiveSettingsScreen = newArchiveSettingsScreen;
			mLoadingService           = loadingService;
			mFileListViewModel        = fileListViewModel;
			mSaveCommand              = saveCommand;
		}

		public override async Task ExecuteAsync()
		{
			if(await mSaveCommand.ExecuteIfRequired() == CancelOrContinue.Cancel)
				return;

			var acceptedSettings = mNewArchiveSettingsScreen.Show();
			if(acceptedSettings == null)
				return;

			var newArchive = mLoadingService.CreateNew(acceptedSettings.DestinationPath,
			                                           acceptedSettings.ArchiveSettings.ToList());

			mFileListViewModel.SetArchive(newArchive, acceptedSettings.DestinationPath);
		}
	}
}
