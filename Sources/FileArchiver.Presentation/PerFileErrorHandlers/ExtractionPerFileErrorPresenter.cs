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
using System.IO;
using System.IO.Abstractions;
using System.Threading;

using FileArchiver.Core;
using FileArchiver.Core.Services;
using FileArchiver.Core.Utils;
using FileArchiver.Presentation.Properties;

using Path = FileArchiver.Core.ValueTypes.Path;

namespace FileArchiver.Presentation.PerFileErrorHandlers
{
	/// <summary>
	/// A presenter for per file errors occurred during the extraction of files from the archive.
	/// </summary>
	public class ExtractionPerFileErrorPresenter : ArchivePerFileExceptionHandlerBase
	{
		private readonly IPerFileErrorView mView;
		private readonly IFileSystem       mFileSystem;

		public ExtractionPerFileErrorPresenter(IPerFileErrorView view,
		                                       IFileSystem fileSystem,
		                                       IFileNameGenerator fileNameGenerator,
		                                       SynchronizationContext uiThreadContext)
			: base(view, fileNameGenerator, uiThreadContext)
		{
			Contract.Requires(view != null);
			Contract.Requires(fileSystem != null);

			mView       = view;
			mFileSystem = fileSystem;
		}

		protected override RetryAction ShowErrorMessageAndAskForRetryAction(string errorMessage)
		{
			var message = String.Format(Resources.ExtractError, errorMessage);

			return mView.AskForRetryAction(message);
		}

		protected override void DeleteDestinationFile(Path filePath)
		{
			RetryLogicHelper.Try(() =>
			{
				if(mFileSystem.FileInfo.FromFileName(filePath).Attributes.HasFlag(FileAttributes.Directory))
				{
					mFileSystem.Directory.Delete(filePath, recursive: true);
				}
				else
				{
					mFileSystem.File.Delete(filePath);
				}
			},
			exception =>
			{
				// File.Delete() will throw an exception on Windows if the file is in use.
				if(exception is IOException)
				{
					return ShowErrorMessageAndAskForRetryAction(exception.Message);
				}

				return RetryAction.RethrowException;
			});
		}

		protected override bool CheckDestinationFileExists(Path filePath)
		{
			return mFileSystem.File.Exists(filePath) || mFileSystem.Directory.Exists(filePath);
		}
	}
}
