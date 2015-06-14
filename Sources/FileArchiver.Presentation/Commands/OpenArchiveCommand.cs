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
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FileArchiver.Core.Archive;
using FileArchiver.Core.Loaders;

using FileArchiver.Presentation.Commands.CommandSystem;
using FileArchiver.Presentation.FileListView;
using FileArchiver.Presentation.OtherViews;
using FileArchiver.Presentation.Progress;
using FileArchiver.Presentation.Utils;

using Path = FileArchiver.Core.ValueTypes.Path;
using Lang = FileArchiver.Presentation.Properties.Resources;

namespace FileArchiver.Presentation.Commands
{
	public class OpenArchiveCommand : CommandBase
	{
		private readonly IDialogLauncher        mDialogLauncher;
		private readonly IProgressViewFactory   mProgressViewFactory;
		private readonly IFileListViewModel     mFileListViewModel;

		private readonly IArchiveLoadingService mLoadingService;
		private readonly SaveArchiveCommand     mSaveCommand;

		public OpenArchiveCommand(IDialogLauncher dialogLauncher,
		                          IProgressViewFactory progressViewFactory,
		                          IFileListViewModel fileListViewModel,
		                          IArchiveLoadingService loadingService,
		                          SaveArchiveCommand saveCommand)
		{
			Contract.Requires(dialogLauncher != null);
			Contract.Requires(progressViewFactory != null);
			Contract.Requires(fileListViewModel != null);
			Contract.Requires(loadingService != null);
			Contract.Requires(saveCommand != null);

			mDialogLauncher      = dialogLauncher;
			mProgressViewFactory = progressViewFactory;
			mFileListViewModel   = fileListViewModel;
			mLoadingService      = loadingService;
			mSaveCommand         = saveCommand;
		}

		public override async Task ExecuteAsync()
		{
			if(await mSaveCommand.ExecuteIfRequired() == CancelOrContinue.Cancel)
				return;

			var archivePath = mDialogLauncher.AskForArchiveOpenPath(mLoadingService.SupportedFormats);
			if(archivePath == null)
				return;

			await OpenArchive(archivePath);
		}

		/// <summary>
		/// Opens the archive from a path delivered from a different source than the user.
		/// </summary>
		public async Task OpenArchive(Path archivePath)
		{
			Contract.Requires(archivePath != null);

			var newArchive = await LoadArchive(archivePath);
			if(newArchive == null)
				return;

			mFileListViewModel.SetArchive(newArchive, archivePath);			
		}

		private async Task<IArchive> LoadArchive(Path archivePath)
		{
			try
			{
				// Try...finally instead of using() because Form.Dispose() makes a parent window flicker once.
				var progressView = mProgressViewFactory.ShowProgressForNextOperation(Lang.OpenProgressForm_Title,
				                                                                     Lang.OpenProgressForm_Description);
				try
				{
					return await mLoadingService.LoadAsync(archivePath, progressView.CancelToken, progressView.Progress);
				}
				finally
				{
					progressView.Hide();
				}
			}
			catch(OperationCanceledException)
			{
				// Nothing
			}
			catch(NotSupportedFormatException)
			{
				mDialogLauncher.DisplayError(String.Format(Lang.NotSupportedFormatError, archivePath.FileName));
			}
			catch(IOException e)
			{
				mDialogLauncher.DisplayError(String.Format(Lang.ExtractError, e.Message));
			}

			return null;
		}
	}
}
