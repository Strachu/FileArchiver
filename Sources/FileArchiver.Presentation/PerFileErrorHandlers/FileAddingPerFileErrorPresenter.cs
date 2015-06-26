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
using System.Diagnostics.Contracts;
using System.Threading;

using FileArchiver.Core;
using FileArchiver.Core.Archive;
using FileArchiver.Core.Services;
using FileArchiver.Core.Utils;
using FileArchiver.Core.ValueTypes;
using FileArchiver.Presentation.Properties;

namespace FileArchiver.Presentation.PerFileErrorHandlers
{
	/// <summary>
	/// A presenter for per file errors occurred during the addition of files to the archive.
	/// </summary>
	public class FileAddingPerFileErrorPresenter : ArchivePerFileExceptionHandlerBase
	{
		private readonly IPerFileErrorView mView;
		private readonly IArchive          mArchive;

		public FileAddingPerFileErrorPresenter(IPerFileErrorView view,
		                                       IArchive archive,
		                                       IFileNameGenerator fileNameGenerator,
		                                       SynchronizationContext uiThreadContext)
			: base(view, fileNameGenerator, uiThreadContext)
		{
			Contract.Requires(view != null);
			Contract.Requires(archive != null);

			mView    = view;
			mArchive = archive;
		}

		protected override RetryAction ShowErrorMessageAndAskForRetryAction(string errorMessage)
		{
			var message = String.Format(Resources.AddFilesError, errorMessage);

			return mView.AskForRetryAction(message);
		}

		protected override void DeleteDestinationFile(Path filePath)
		{
			mArchive.RemoveFile(filePath);
		}

		protected override bool CheckDestinationFileExists(Path filePath)
		{
			return mArchive.FileExists(filePath);
		}
	}
}
