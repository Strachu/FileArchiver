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
using System.Threading.Tasks;

using FileArchiver.Core;
using FileArchiver.Core.Loaders;
using FileArchiver.Presentation.ArchiveSettings;
using FileArchiver.Presentation.Properties;

using Path = FileArchiver.Core.ValueTypes.Path;

namespace FileArchiver.Presentation.CommandLine.Presenters
{
	/// <summary>
	/// A presenter for packing of files into an archive.
	/// </summary>
	public class PackIntoArchivePresenter
	{
		private readonly IPackIntoArchiveView             mView;
		private readonly INewArchiveSettingsScreen        mArchiveSettingsView;
		private readonly IArchiveLoadingService           mLoadingService;
		private readonly IFromFileSystemFileAddingService mFileAddingService;

		public PackIntoArchivePresenter(IPackIntoArchiveView view,
		                                INewArchiveSettingsScreen archiveSettingsScreen,
		                                IArchiveLoadingService loadingService,
		                                IFromFileSystemFileAddingService fileAddingService)
		{
			Contract.Requires(view != null);
			Contract.Requires(archiveSettingsScreen != null);
			Contract.Requires(loadingService != null);
			Contract.Requires(fileAddingService != null);

			mView                = view;
			mArchiveSettingsView = archiveSettingsScreen;
			mLoadingService      = loadingService;
			mFileAddingService   = fileAddingService;
		}

		/// <summary>
		/// Starts an asynchronous operation to packs the files into an archive.
		/// </summary>
		/// <param name="filePaths">
		/// The paths of files to pack into single archive.
		/// </param>
		public async Task PackFiles(params Path[] filePaths)
		{
			Contract.Requires(filePaths != null);
			Contract.Requires(filePaths.Any());
		//	Contract.Requires(Contract.ForAll(filePaths, path => path != null));
			Contract.Ensures(Contract.Result<Task>() != null);

			try
			{
				var archiveDestinationPath = DetermineDefaultArchivePath(filePaths);

				// TODO should ask whether to overwrite if the user accepts default path and the file already exists
				//  - or ask inside the new archive settings dialog as the path can be entered manually without using the file dialog?

				var archiveSettings = mArchiveSettingsView.Show(archiveDestinationPath);
				if(archiveSettings == null)
					return;

				var progressView = mView.ShowProgress(archiveSettings.DestinationPath);
				try
				{
					var archive = mLoadingService.CreateNew(archiveSettings.DestinationPath, archiveSettings.ArchiveSettings.ToList());

					mFileAddingService.AddFiles(archive, Path.Root, filePaths);

					await archive.SaveAsync(progressView.CancelToken, progressView.Progress);
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
				mView.DisplayError(String.Format(Resources.PackError, e.Message));
			}
			catch(IOException e)
			{
				mView.DisplayError(String.Format(Resources.PackError, e.Message));
			}
		}

		/// <summary>
		/// Determines the default archive path.
		/// </summary>
		/// <remarks>
		/// The default archive file name is a packed file name in the case of packing a single file
		/// or parent directory name when multiple files are packed.
		/// </remarks>
		private Path DetermineDefaultArchivePath(IReadOnlyCollection<Path> filePaths)
		{
			var parentDirectory = filePaths.First().ParentDirectory;
			var archiveFileName = (filePaths.Count == 1) ? filePaths.First().RemoveExtension() : parentDirectory.FileName;

			return parentDirectory.Combine(archiveFileName);
		}
	}
}
