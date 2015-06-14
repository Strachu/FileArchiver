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
using FileArchiver.Core.Archive;
using FileArchiver.Core.ValueTypes;
using FileArchiver.Presentation.Commands.CommandSystem;
using FileArchiver.Presentation.FileListView;
using FileArchiver.Presentation.OtherViews;
using FileArchiver.Presentation.PerFileErrorHandlers;
using FileArchiver.Presentation.Progress;

using Lang = FileArchiver.Presentation.Properties.Resources;

namespace FileArchiver.Presentation.Commands
{
	public class ExtractFilesCommand : CommandBase
	{
		private readonly IFileListViewModel   mFileListViewModel;
		private readonly IDialogLauncher      mDialogLauncher;
		private readonly IProgressViewFactory mProgressViewFactory;

		private readonly IExtractionPerFileErrorPresenterFactory mExtractionErrorHandlerFactory;

		public ExtractFilesCommand(IDialogLauncher dialogLauncher,
		                           IProgressViewFactory progressViewFactory,
		                           IFileListViewModel fileListViewModel,
		                           IExtractionPerFileErrorPresenterFactory extractionErrorHandlerFactory)
		{
			Contract.Requires(dialogLauncher != null);
			Contract.Requires(progressViewFactory != null);
			Contract.Requires(fileListViewModel != null);
			Contract.Requires(extractionErrorHandlerFactory != null);

			mFileListViewModel             = fileListViewModel;
			mDialogLauncher                = dialogLauncher;
			mProgressViewFactory           = progressViewFactory;
			mExtractionErrorHandlerFactory = extractionErrorHandlerFactory;

			Enabled                        = false;

			mFileListViewModel.FilesInCurrentDirectory.ListChanged += FilesInCurrentDirectory_ListChanged;
		}

		public override async Task ExecuteAsync()
		{
			var destinationDirectory = mDialogLauncher.AskForDestinationDirectoryForExtraction();
			if(destinationDirectory == null)
				return;

			try
			{
				await ExtractSelectedFilesTo(destinationDirectory);
			}
			catch(OperationCanceledException)
			{
				// Nothing
			}
		}

		private async Task ExtractSelectedFilesTo(Path destinationDirectory)
		{
			var progressView = mProgressViewFactory.ShowProgressForNextOperation(Lang.ExtractProgressForm_Title,
			                                                                     Lang.ExtractProgressForm_Description);
			try
			{
				var filesToExtract = mFileListViewModel.FilesInCurrentDirectory.Where(file => file.Selected)
				                                                               .Select(GetFilePath).ToList();

				var errorPresenter = mExtractionErrorHandlerFactory.GetErrorPresenterForNextOperation();

				await mFileListViewModel.Archive.ExtractFilesAsync(destinationDirectory,
				                                                   filesToExtract,
				                                                   errorPresenter.ExceptionThrown,                              
				                                                   progressView.CancelToken,
				                                                   progressView.Progress);
			}
			finally
			{
				progressView.Hide();
			}
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
