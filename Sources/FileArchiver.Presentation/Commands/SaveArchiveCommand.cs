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

using FileArchiver.Presentation.Commands.CommandSystem;
using FileArchiver.Presentation.OtherViews;
using FileArchiver.Presentation.Progress;
using FileArchiver.Presentation.Utils;
using FileArchiver.Presentation.FileListView;

using Lang = FileArchiver.Presentation.Properties.Resources;

namespace FileArchiver.Presentation.Commands
{
	public class SaveArchiveCommand : CommandBase
	{
		private readonly IDialogLauncher      mDialogLauncher;
		private readonly IProgressViewFactory mProgressViewFactory;
		private readonly IFileListViewModel   mFileListViewModel;

		public SaveArchiveCommand(IDialogLauncher dialogLauncher,
		                          IProgressViewFactory progressViewFactory,
		                          IFileListViewModel fileListViewModel)
		{
			Contract.Requires(dialogLauncher != null);
			Contract.Requires(progressViewFactory != null);
			Contract.Requires(fileListViewModel != null);

			mDialogLauncher      = dialogLauncher;
			mProgressViewFactory = progressViewFactory;
			mFileListViewModel   = fileListViewModel;

			Enabled              = false;

			mFileListViewModel.SubscribeToPropertyChanged(() => mFileListViewModel.Archive, ArchiveChanged);
		}

		public override async Task ExecuteAsync()
		{
			await SaveArchive();
		}

		/// <summary>
		/// Executes the save command if the archive has been modified and user wants to save the changes.
		/// </summary>
		/// <returns>
		/// Whether the user ordered to cancel the command from which the function has been executed or not.
		/// </returns>
		public async Task<CancelOrContinue> ExecuteIfRequired()
		{
			if(!mFileListViewModel.Archive.IsModified)
				return CancelOrContinue.Continue;

			var action = mDialogLauncher.AskForSaveChangesAction(mFileListViewModel.ArchivePath.FileName);
			if(action == SaveChangesAction.Cancel)
				return CancelOrContinue.Cancel;

			if(action == SaveChangesAction.Save)
			{
				bool savedSuccessfully = await SaveArchive();

				return savedSuccessfully ? CancelOrContinue.Continue : CancelOrContinue.Cancel;
			}

			return CancelOrContinue.Continue;
		}

		private async Task<bool> SaveArchive()
		{
			try
			{
				var progressView = mProgressViewFactory.ShowProgressForNextOperation(Lang.SaveProgressForm_Title,
				                                                                     Lang.SaveProgressForm_Description);
				try
				{
					await mFileListViewModel.Archive.SaveAsync(progressView.CancelToken, progressView.Progress);
					return true;
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
			catch(UnauthorizedAccessException e)
			{
				// TODO SaveAs??
				mDialogLauncher.DisplayError(String.Format(Lang.SaveError, e.Message));
			}
			catch(IOException e)
			{
				mDialogLauncher.DisplayError(String.Format(Lang.SaveError, e.Message));
			}

			return false;
		}

		private void ArchiveChanged(object sender, EventArgs e)
		{
			Enabled = true;
		}
	}
}
