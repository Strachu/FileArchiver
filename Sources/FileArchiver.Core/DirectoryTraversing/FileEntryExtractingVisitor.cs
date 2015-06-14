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
using FileArchiver.Core.Archive;
using FileArchiver.Core.Services;
using FileArchiver.Core.Utils;
using FileArchiver.Core.Utils.File;

using Path = FileArchiver.Core.ValueTypes.Path;

namespace FileArchiver.Core.DirectoryTraversing
{
	/// <summary>
	/// A base class for visitors which extracts files from archive as they are traversed.
	/// </summary>
	[ContractClass(typeof(FileEntryExtractingVisitorContractClass))]
	public abstract class FileEntryExtractingVisitor : DirectoryHierarchyVisitor<FileEntry>
	{
		private readonly TempFileProvider           mTempFileProvider;
		private readonly FileExtractionErrorHandler mExceptionHandler;
		private readonly CancellationToken          mCancelToken;
		private readonly CompositeFileProgress      mProgress;

		// TODO remove duplication with FromFileSystemFileAddingVisitor class
		private Path mDestinationDirectoryForCurrentEntry;
		private Path mDestinationPathOverride;

		protected FileEntryExtractingVisitor(TempFileProvider tempFileProvider,
		                                     Path destinationPath,
		                                     FileExtractionErrorHandler errorHandler,
		                                     CancellationToken cancelToken,
		                                     IProgress<long> progress = null)
		{
			Contract.Requires(tempFileProvider != null);
			Contract.Requires(destinationPath != null);
			Contract.Requires(errorHandler != null);

			mTempFileProvider                    = tempFileProvider;
			mExceptionHandler                    = errorHandler;
			mCancelToken                         = cancelToken;
			mProgress                            = (progress != null) ? new CompositeFileProgress(progress.Report)
			                                                          : new CompositeFileProgress(x => { });

			mDestinationDirectoryForCurrentEntry = destinationPath.ParentDirectory;
			mDestinationPathOverride             = destinationPath;
		}

		public override void VisitFile(FileEntry file)
		{
			var filePath = GetDestinationPathForEntry(file);

			ExtractFile(file, filePath);

			SetFileAttributes(file, filePath);

			mDestinationPathOverride = null;
		}

		public override bool OnDirectoryEntering(FileEntry directory)
		{
			var directoryPath = GetDestinationPathForEntry(directory);

			if(Directory.Exists(directoryPath))
				throw new FileExistsException(directoryPath);

			Directory.CreateDirectory(directoryPath);

			SetDirectoryAttributes(directory, directoryPath);

			mDestinationDirectoryForCurrentEntry = mDestinationDirectoryForCurrentEntry.Combine(directoryPath.FileName);
			mDestinationPathOverride             = null;
			return true;
		}

		public override void OnDirectoryLeaving(FileEntry directory)
		{
			mDestinationDirectoryForCurrentEntry = mDestinationDirectoryForCurrentEntry.ParentDirectory;
		}

		private Path GetDestinationPathForEntry(FileEntry entry)
		{
			return mDestinationPathOverride ?? mDestinationDirectoryForCurrentEntry.Combine(entry.Name);
		}

		/// <summary>
		/// Extracts the file.
		/// </summary>
		/// <param name="file">
		/// The file to extract.
		/// </param>
		/// <param name="destinationPath">
		/// The path to extract the file to.
		/// </param>
		/// <remarks>
		/// The default implementation copies the file to destination if its already on disk or
		/// calls <see cref="ExtractFileFromArchive"/> if the file needs to be extracted from archive.
		/// </remarks>
		protected virtual void ExtractFile(FileEntry file, Path destinationPath)
		{
			Contract.Requires(file != null);
			Contract.Requires(destinationPath != null);

			if(file.DataFilePath == null && !IsInTemp(file))
			{
				ExtractFileFromArchive(file, destinationPath);
			}
			else
			{
				CopyFileToDestination(file, destinationPath);
			}
		}

		/// <summary>
		/// Extracts the file from archive.
		/// </summary>
		/// <param name="file">
		/// The file to extract.
		/// </param>
		/// <param name="destinationPath">
		/// The path to extract the file to.
		/// </param>
		protected abstract void ExtractFileFromArchive(FileEntry file, Path destinationPath);

		protected void CopyFileToDestination(FileEntry file, Path destinationPath)
		{
			Contract.Requires(file != null);
			Contract.Requires(destinationPath != null);

			var sourcePath = IsInTemp(file) ? mTempFileProvider.GetTempFileFor(file) : file.DataFilePath;
			if(sourcePath.Equals(destinationPath))
				return;

			var sourceStream = File.Open(sourcePath, FileMode.Open, FileAccess.Read, FileShare.Read);

			sourceStream.CopyToNewFileWithProgress(destinationPath, mCancelToken, mProgress.GetProgressForNextFile());
		}

		/// <summary>
		/// Sets the attributes for extracted file.
		/// </summary>
		/// <param name="sourceEntry">
		/// The entry from which the file was extracted.
		/// </param>
		/// <param name="pathOnDisk">
		/// The path to which the file has been extracted.
		/// </param>
		protected virtual void SetFileAttributes(FileEntry sourceEntry, Path pathOnDisk)
		{
			Contract.Requires(sourceEntry != null);
			Contract.Requires(pathOnDisk != null);

			if(sourceEntry.LastModificationTime.HasValue)
			{
				File.SetLastWriteTime(pathOnDisk, sourceEntry.LastModificationTime.Value);
			}
		}

		/// <summary>
		/// Sets the attributes for created directory.
		/// </summary>
		/// <param name="sourceEntry">
		/// The entry for which the directory was created.
		/// </param>
		/// <param name="pathOnDisk">
		/// The path where the directory has been created.
		/// </param>
		protected virtual void SetDirectoryAttributes(FileEntry sourceEntry, Path pathOnDisk)
		{
			Contract.Requires(sourceEntry != null);
			Contract.Requires(pathOnDisk != null);

			if(sourceEntry.LastModificationTime.HasValue)
			{
				Directory.SetLastWriteTime(pathOnDisk, sourceEntry.LastModificationTime.Value);
			}
		}
		private bool IsInTemp(FileEntry file)
		{
			return File.Exists(mTempFileProvider.GetTempFileFor(file));
		}

		public override RetryAction OnException(FileEntry entry, Exception exception)
		{
			mDestinationPathOverride = GetDestinationPathForEntry(entry);

			return mExceptionHandler(ref mDestinationPathOverride, exception);
		}
	}
}
