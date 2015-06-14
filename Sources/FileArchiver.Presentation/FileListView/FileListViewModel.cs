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
using System.Threading;
using System.Threading.Tasks;

using FileArchiver.Core;
using FileArchiver.Core.Archive;
using FileArchiver.Core.Utils;
using FileArchiver.Core.ValueTypes;
using FileArchiver.Presentation.PerFileErrorHandlers;
using FileArchiver.Presentation.Utils;

namespace FileArchiver.Presentation.FileListView
{
	internal class FileListViewModel : NotifyPropertyChangedHelper, IFileListViewModel
	{
		private const int FILE_DISPLAY_INDEX_AFTER_SCROLLING = 1;

		private readonly IFileIconProvider                       mFileIconProvider;
		private readonly IFromFileSystemFileAddingService        mFileAddingService;
		private readonly IFileAddingPerFileErrorPresenterFactory mFileAddingErrorHandlerFactory;
		private readonly SynchronizationContext                  mUIThread;

		private readonly FileEntryBindingList                    mFilesInCurrentDirectory = new FileEntryBindingList();

		public FileListViewModel(IFileIconProvider fileIconProvider,
		                         IFromFileSystemFileAddingService fileAddingService,
		                         IFileAddingPerFileErrorPresenterFactory fileAddingErrorHandlerFactory)
		{
			Contract.Requires(fileIconProvider != null);
			Contract.Requires(fileAddingService != null);
			Contract.Requires(fileAddingErrorHandlerFactory != null);

			mFileIconProvider              = fileIconProvider;
			mFileAddingService             = fileAddingService;
			mFileAddingErrorHandlerFactory = fileAddingErrorHandlerFactory;
			mUIThread                      = SynchronizationContext.Current;
		}

		private IArchive _mArchive = new NullArchive();
		public IArchive Archive
		{
			get { return _mArchive; }
			private set { base.SetFieldWithNotification(ref _mArchive, value); }
		}

		private Path _mArchivePath = Path.Root;
		public Path ArchivePath
		{
			get { return _mArchivePath; }
			private set { base.SetFieldWithNotification(ref _mArchivePath, value); }
		}

		public BindingList<FileEntryViewModel> FilesInCurrentDirectory
		{
			get { return mFilesInCurrentDirectory; }
		}
		
		private Path _mCurrentDirectory = Path.Root;
		public Path CurrentDirectory
		{
			get { return _mCurrentDirectory; }
			private set { base.SetFieldWithNotification(ref _mCurrentDirectory, value); }
		}

		public Path CurrentDirectoryFullAddress
		{
			get { return ArchivePath.Combine(CurrentDirectory); }
		}

		private bool _mNavigateToParentDirectoryEnabled = false;
		public bool NavigateToParentDirectoryEnabled
		{
			get { return _mNavigateToParentDirectoryEnabled; }
			private set { base.SetFieldWithNotification(ref _mNavigateToParentDirectoryEnabled, value); }
		}

		private bool _mAddFilesEnabled = false;
		public bool AddFilesEnabled
		{
			get { return _mAddFilesEnabled; }
			private set { base.SetFieldWithNotification(ref _mAddFilesEnabled, value); }
		}

		private int _mFirstDisplayedFileIndex = 0;
		public int FirstDisplayedFileIndex
		{
			get { return _mFirstDisplayedFileIndex; }
			set { base.SetFieldWithNotification(ref _mFirstDisplayedFileIndex, value); }
		}

		public void SetArchive(IArchive archive, Path archivePath)
		{
			Archive.Close();

			Archive     = archive;
			ArchivePath = archivePath;

			AddFilesEnabled = true;

			ChangeDirectory(Path.Root);

			Archive.FileAdded   += Archive_FileAdded;
			Archive.FileRemoved += Archive_FileRemoved;
		}

		public void Open(FileName fileName)
		{
			var fileEntry = GetFilesInCurrentDirectory().Single(file => file.Name.Equals(fileName));

			if(fileEntry.IsDirectory)
			{
				ChangeDirectory(fileEntry.Path);
			}
			else
			{
				FileOpeningRequested.SafeRaise(this, new FileOpenRequestEventArgs(fileEntry.Path));
			}
		}

		public void NavigateToParentDirectory()
		{
			ChangeDirectory(CurrentDirectory.ParentDirectory);
		}

		private void ChangeDirectory(Path newDirectoryPath)
		{
			CurrentDirectory                 = newDirectoryPath;
			NavigateToParentDirectoryEnabled = !CurrentDirectory.Equals(Path.Root);
			FirstDisplayedFileIndex          = 0;

			ReloadFileList();
		}

		private void ReloadFileList()
		{
			var files = GetFilesInCurrentDirectory().Select(file => new FileEntryViewModel(file, mFileIconProvider))
			                                        .ToList();

			mFilesInCurrentDirectory.Clear();

			files.CopyTo(mFilesInCurrentDirectory);

			mFilesInCurrentDirectory.Sort();			
		}

		private IEnumerable<FileEntry> GetFilesInCurrentDirectory()
		{
			return CurrentDirectory.Equals(Path.Root) ? Archive.RootFiles : Archive.GetFile(CurrentDirectory).Files;
		}

		public void AddFiles(IReadOnlyList<Path> files)
		{
			try
			{
				var oldFileList = FilesInCurrentDirectory.ToList();

				var errorPresenter = mFileAddingErrorHandlerFactory.GetErrorPresenterForNextOperation(Archive);

				mFileAddingService.AddFiles(Archive, CurrentDirectory, files, errorPresenter.ExceptionThrown);

				var addedFiles = FilesInCurrentDirectory.Except(oldFileList).ToList();

				if(addedFiles.Any())
				{
					FilesInCurrentDirectory.ForEach(file => file.Selected = addedFiles.Contains(file));

					ScrollTo(FilesInCurrentDirectory.First(addedFiles.Contains));
				}
			}
			catch(OperationCanceledException)
			{
				// Nothing
			}
		}

		private void Archive_FileAdded(object sender, FileAddedEventArgs e)
		{
			if(!e.AddedFile.Path.ParentDirectory.Equals(CurrentDirectory))
				return;

			mUIThread.Send(() =>
			{
				var newFileViewModel = new FileEntryViewModel(e.AddedFile, mFileIconProvider);
				var newFileIndex = mFilesInCurrentDirectory.FindOrderedIndex(newFileViewModel);

				FilesInCurrentDirectory.Insert(newFileIndex, newFileViewModel);

				FilesInCurrentDirectory.ForEach(file => file.Selected = file.Name.Equals(e.AddedFile.Name));

				ScrollTo(newFileViewModel);
			});
		}

		private void ScrollTo(FileEntryViewModel fileToCenterAt)
		{
			int fileIndex = FilesInCurrentDirectory.IndexOf(fileToCenterAt);

			FirstDisplayedFileIndex = Math.Max(fileIndex - FILE_DISPLAY_INDEX_AFTER_SCROLLING, 0);			
		}

		private void Archive_FileRemoved(object sender, FileRemovedEventArgs e)
		{
			if(!e.RemovedFile.Path.ParentDirectory.Equals(CurrentDirectory))
				return;

			mUIThread.Send(() =>
			{
				// RemoveAt() causes the view to scroll to the removed file overwriting the current FirstDisplayedFileIndex.
				var oldFirstDisplayedFileIndex = FirstDisplayedFileIndex;
				var removedFileIndex = FilesInCurrentDirectory.FirstIndex(file => file.Name.Equals(e.RemovedFile.Name));

				FilesInCurrentDirectory.RemoveAt(removedFileIndex);

				if(FilesInCurrentDirectory.Any() && !FilesInCurrentDirectory.Any(x => x.Selected))
				{
					var fileToSelect = Math.Min(removedFileIndex, FilesInCurrentDirectory.Count - 1);

					FilesInCurrentDirectory[fileToSelect].Selected = true;
				}

				if(removedFileIndex <= FirstDisplayedFileIndex && FirstDisplayedFileIndex > 0)
				{
					FirstDisplayedFileIndex = oldFirstDisplayedFileIndex - 1;
				}
				else
				{
					FirstDisplayedFileIndex = oldFirstDisplayedFileIndex;
				}
			});
		}

		protected override void OnPropertyChanged(string propertyName)
		{
			base.OnPropertyChanged(propertyName);

			if(propertyName == PropertyName.Of(() => ArchivePath) ||
			   propertyName == PropertyName.Of(() => CurrentDirectory))
			{
				OnPropertyChanged(PropertyName.Of(() => CurrentDirectoryFullAddress));
			}
		}

		public event EventHandler<FileOpenRequestEventArgs> FileOpeningRequested;
	}
}
