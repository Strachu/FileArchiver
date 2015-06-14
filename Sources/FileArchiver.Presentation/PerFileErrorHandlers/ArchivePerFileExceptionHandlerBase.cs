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
using System.Threading;

using FileArchiver.Core;
using FileArchiver.Core.Services;
using FileArchiver.Core.Utils;

using FileArchiver.Presentation.Utils;

using Path = FileArchiver.Core.ValueTypes.Path;

namespace FileArchiver.Presentation.PerFileErrorHandlers
{
	/// <summary>
	/// Base class with shared implementation for handling per file errors in archive operations.
	/// </summary>
	[ContractClass(typeof(ArchivePerFileExceptionHandlerBaseContractClass))]
	public abstract class ArchivePerFileExceptionHandlerBase
	{
		private readonly IPerFileErrorView      mView;
		private readonly IFileNameGenerator     mUniqueFileNameGenerator;
		private readonly SynchronizationContext mUIThread;

		private FileExistsAction?               mActionToApplyToAllOnFileExists = null;

		protected ArchivePerFileExceptionHandlerBase(IPerFileErrorView view,
		                                             IFileNameGenerator fileNameGenerator,
		                                             SynchronizationContext uiThreadContext)
		{
			Contract.Requires(view != null);
			Contract.Requires(fileNameGenerator != null);
			Contract.Requires(uiThreadContext != null);

			mView                    = view;
			mUniqueFileNameGenerator = fileNameGenerator;
			mUIThread                = uiThreadContext;
		}

		/// <summary>
		/// The handler for per file exceptions.
		/// </summary>
		public RetryAction ExceptionThrown(ref Path destinationPath, Exception exception)
		{
			if(exception is OperationCanceledException)
			{
				return RetryAction.Abort;
			}

			if(exception is FileExistsException)
			{
				return OnFileExists(ref destinationPath, exception);
			}

			if(exception is IOException || exception is UnauthorizedAccessException)
			{
				return mUIThread.Send(() => ShowErrorMessageAndAskForRetryAction(exception.Message));
			}

			return RetryAction.RethrowException;
		}

		private RetryAction OnFileExists(ref Path destinationPath, Exception exception)
		{
			var renameCandidate = mUniqueFileNameGenerator.GenerateFreeFileName(destinationPath, CheckDestinationFileExists);

			switch(GetAction(destinationPath, renameCandidate))
			{
				case FileExistsAction.Overwrite:
					DeleteDestinationFile(destinationPath);
					return RetryAction.Retry;

				case FileExistsAction.Rename:
					destinationPath = renameCandidate;
					return RetryAction.Retry;

				case FileExistsAction.Skip:
					return RetryAction.Ignore;

				case FileExistsAction.Abort:
					return RetryAction.Abort;

				default:
					throw new InvalidOperationException("Invalid enum value.");
			}
		}

		private FileExistsAction GetAction(Path destinationPath, Path renameCandidate)
		{
			if(mActionToApplyToAllOnFileExists != null)
				return mActionToApplyToAllOnFileExists.Value;

			bool applyToAll = false;
			var action = mUIThread.Send(() => mView.AskForFileExistsAction(destinationPath, renameCandidate, out applyToAll));

			if(applyToAll)
			{
				mActionToApplyToAllOnFileExists = action;
			}

			return action;
		}

		protected abstract RetryAction ShowErrorMessageAndAskForRetryAction(string errorMessage);

		protected abstract bool CheckDestinationFileExists(Path filePath);

		protected abstract void DeleteDestinationFile(Path filePath);
	}
}
