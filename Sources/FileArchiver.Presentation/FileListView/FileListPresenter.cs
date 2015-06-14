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
using System.Threading.Tasks;

using FileArchiver.Core;
using FileArchiver.Core.Archive;
using FileArchiver.Core.Services;
using FileArchiver.Core.Utils;
using FileArchiver.Core.ValueTypes;
using FileArchiver.Presentation.Progress;
using FileArchiver.Presentation.Utils;

using Path = FileArchiver.Core.ValueTypes.Path;
using Lang = FileArchiver.Presentation.Properties.Resources;

namespace FileArchiver.Presentation.FileListView
{
	public class FileListPresenter
	{
		private readonly IFileListViewModel   mViewModel;
		private readonly IProgressViewFactory mProgressViewFactory;

		private readonly FileOpeningService   mFileOpeningService;

		public FileListPresenter(IFileListView view,
		                         IFileListViewModel viewModel,
		                         IProgressViewFactory progressViewFactory,
		                         FileOpeningService fileOpeningService)
		{
			Contract.Requires(view != null);
			Contract.Requires(viewModel != null);
			Contract.Requires(progressViewFactory != null);
			Contract.Requires(fileOpeningService != null);

			mProgressViewFactory = progressViewFactory;
			mFileOpeningService  = fileOpeningService;
			mViewModel           = viewModel;

			view.FileNameValidating += FileNameValidating;
			view.FileNameAccepted   += FileNameAccepted;

			mViewModel.FileOpeningRequested += ViewModel_FileOpeningRequested;
		}

		public async Task OpenFiles(IEnumerable<Path> files)
		{
			Contract.Requires(files != null);
		// TODO Bug in Code Contracts: https://social.msdn.microsoft.com/Forums/en-US/f0aaeb16-cbfd-4ad0-b883-71f3243fa8f3/invalidprogramexception-with-async-runtime-checks-small-repro?forum=codecontracts
		// Crashes in release mode.
		//	Contract.Requires(Contract.ForAll(files, file => file != null));

			try
			{
				var progressView = mProgressViewFactory.ShowProgressForNextOperation(Lang.ExtractProgressForm_Title,
				                                                                     Lang.ExtractProgressForm_Description);
				try
				{
					await mFileOpeningService.OpenFilesAsync(mViewModel.Archive,
					                                         files.Where(IsNotDirectory),
					                                         progressView.CancelToken,
					                                         progressView.Progress);
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
		}

		private bool IsNotDirectory(Path file)
		{
			return !mViewModel.Archive.GetFile(file).IsDirectory;
		}
		
		private async void ViewModel_FileOpeningRequested(object sender, FileOpenRequestEventArgs e)
		{
			await OpenFiles(e.FileToOpen.ToSingleElementList());
		}

		private void FileNameValidating(object sender, FileNameValidatingEventArgs e)
		{
			if(!FileName.IsValid(e.EnteredName))
			{
				e.ErrorMessage = Lang.InvalidNameValidationError;
				return;
			}

			var newName = new FileName(e.EnteredName);
			if(e.CurrentName.Equals(newName))
				return;

			if(mViewModel.Archive.FileExists(GetFilePath(newName)))
			{
				e.ErrorMessage = Lang.NameInUseValidationError;
				return;
			}
		}

		private void FileNameAccepted(object sender, FileNameAcceptedEventArgs e)
		{
			if(e.PreviousName.Equals(e.EnteredName))
				return;

			// The name is already changed by the view
			var oldFileIndex         = mViewModel.FilesInCurrentDirectory.FirstIndex(file => file.Name.Equals(e.EnteredName));
			var oldFirstDisplayIndex = mViewModel.FirstDisplayedFileIndex;

			mViewModel.FilesInCurrentDirectory[oldFileIndex].Name = e.PreviousName;

			mViewModel.Archive.RenameFile(GetFilePath(e.PreviousName), e.EnteredName);

			mViewModel.FilesInCurrentDirectory.ForEach(file => file.Selected = file.Name.Equals(e.EnteredName));

			var newFileIndex  = mViewModel.FilesInCurrentDirectory.FirstIndex(file => file.Name.Equals(e.EnteredName));
			var fileIndexDiff = newFileIndex - oldFileIndex;

			mViewModel.FirstDisplayedFileIndex = Math.Max(oldFirstDisplayIndex + fileIndexDiff, 0);
		}

		private Path GetFilePath(FileName fileName)
		{
			return mViewModel.CurrentDirectory.Combine(fileName);
		}
	}
}
