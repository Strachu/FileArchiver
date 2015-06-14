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
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Globalization;

using FileArchiver.Presentation.Commands;
using FileArchiver.Presentation.FileListView;
using FileArchiver.Presentation.Properties;
using FileArchiver.Presentation.Settings;
using FileArchiver.Presentation.Utils;

using Path = FileArchiver.Core.ValueTypes.Path;

namespace FileArchiver.Presentation.Shell
{
	internal class MainPresenter
	{
		private readonly IMainView          mView;
		private readonly FileListViewModel  mFileListViewModel;
		private readonly OpenArchiveCommand mOpenCommand;
		private readonly SaveArchiveCommand mSaveCommand;

		private bool                        mViewShouldClose = false;

		public MainPresenter(IMainView mainView,
		                     FileListViewModel fileListViewModel,
		                     OpenArchiveCommand openCommand,
		                     SaveArchiveCommand saveCommand)
		{
			Contract.Requires(mainView != null);
			Contract.Requires(fileListViewModel != null);
			Contract.Requires(openCommand != null);
			Contract.Requires(saveCommand != null);

			mView              = mainView;
			mFileListViewModel = fileListViewModel;
			mOpenCommand       = openCommand;
			mSaveCommand       = saveCommand;

			InitializeView();

			mFileListViewModel.SubscribeToPropertyChanged(() => mFileListViewModel.ArchivePath, ArchiveChanged);
			mFileListViewModel.FilesInCurrentDirectory.ListChanged += FilesInCurrentDirectory_ListChanged;
		}

		private void InitializeView()
		{
			mView.Language = CultureInfo.CurrentUICulture.Name;

			if(ApplicationSettings.Instance.WindowLayout != null)
			{
				mView.LayoutSettings = ApplicationSettings.Instance.WindowLayout;
			}

			mView.ViewShown   += ViewShown;
			mView.ViewClosing += ViewClosing;
		}

		private async void ViewShown(object sender, EventArgs e)
		{
			var args = Environment.GetCommandLineArgs();
			if(args.Length == 2)
			{
				await mOpenCommand.OpenArchive(new Path(args[1]));
			}
		}

		private async void ViewClosing(object sender, CancelEventArgs e)
		{
			// A bit hackish workaround for the problem that we cannot "await" saving here (as the form will close) nor we can do it synchronously
			// (as the UI will freeze).
			// Instead we cancel the first closing event and "await" saving... when the saving ends the rest of function continue and we close the form
			// manually at the end.
			if(mViewShouldClose)
				return;

			e.Cancel = !mViewShouldClose;

			var action = await mSaveCommand.ExecuteIfRequired();
			if(action == CancelOrContinue.Cancel)
				return;

			ApplicationSettings.Instance.WindowLayout = mView.LayoutSettings;

			mViewShouldClose = true;

			mFileListViewModel.Archive.Close();

			mView.Close();
		}

		private void ArchiveChanged(object sender, EventArgs e)
		{
			mView.Title = String.Format("{0} - FileArchiver", mFileListViewModel.ArchivePath.FileName);
		}

		private void FilesInCurrentDirectory_ListChanged(object sender, ListChangedEventArgs e)
		{
			var filesInDirectoryCount = mFileListViewModel.FilesInCurrentDirectory.Count;

			mView.StatusText = String.Format(Resources.FilesInDirectoryStatusText, filesInDirectoryCount);
		}
	}
}
