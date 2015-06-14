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
using System.IO.Abstractions;

using FileArchiver.Core.Archive;
using FileArchiver.Core.DirectoryTraversing;
using FileArchiver.Core.Utils;
using FileArchiver.Core.ValueTypes;

using Path = FileArchiver.Core.ValueTypes.Path;

namespace FileArchiver.Core
{
	/// <summary>
	/// A directory visitor which adds the files to an archive as they are encountered.
	/// </summary>
	internal class FromFileSystemFileAddingVisitor : DirectoryHierarchyVisitor<Path>
	{
		private readonly IFileSystem            mFileSystem;
		private readonly IArchive               mDestinationArchive;
		private readonly FileAddingErrorHandler mExceptionHandler;

		private Path mDestinationDirectoryForCurrentEntry;
		private Path mDestinationPathOverride;

		public FromFileSystemFileAddingVisitor(IFileSystem fileSystem,
		                                       IArchive destinationArchive,
		                                       Path destinationPath,
		                                       FileAddingErrorHandler errorHandler)
		{
			Contract.Requires(fileSystem != null);
			Contract.Requires(destinationArchive != null);
			Contract.Requires(destinationPath != null);
			Contract.Requires(errorHandler != null);

			mFileSystem                          = fileSystem;
			mDestinationArchive                  = destinationArchive;
			mExceptionHandler                    = errorHandler;

			mDestinationDirectoryForCurrentEntry = destinationPath.ParentDirectory;
			mDestinationPathOverride             = destinationPath;
		}

		public override void VisitFile(Path filePath)
		{
			AddEntryFromDisk(filePath, (name, file) =>
			{
				return new FileEntry.Builder().AsNew()
				                              .WithName(name)
														.WithDataFromFile(file)
			                                 .Build();
			});
		}

		private void AddEntryFromDisk(Path entryPath, Func<FileName, FileInfoBase, FileEntry> entryFactory)
		{
			var destinationPath = GetDestinationPathForEntry(entryPath);
			var fileInfo        = mFileSystem.FileInfo.FromFileName(entryPath);

			var newEntry        = entryFactory(destinationPath.FileName, fileInfo);

			mDestinationArchive.AddFile(destinationPath.ParentDirectory, newEntry);

			mDestinationPathOverride = null;
		}

		private Path GetDestinationPathForEntry(Path entry)
		{
			return mDestinationPathOverride ?? mDestinationDirectoryForCurrentEntry.Combine(entry.FileName);
		}

		public override bool OnDirectoryEntering(Path directoryPath)
		{
			FileName finalDirectoryName = null;

			AddEntryFromDisk(directoryPath, (name, directory) =>
			{
				finalDirectoryName = name;

				return new FileEntry.Builder().AsNew()
				                              .AsDirectory()
				                              .WithName(name)
				                              .ModifiedOn(directory.LastWriteTime)
														.Build();
			});

			mDestinationDirectoryForCurrentEntry = mDestinationDirectoryForCurrentEntry.Combine(finalDirectoryName);
			return true;
		}

		public override void OnDirectoryLeaving(Path directoryPath)
		{
			mDestinationDirectoryForCurrentEntry = mDestinationDirectoryForCurrentEntry.ParentDirectory;
		}

		public override RetryAction OnException(Path sourceEntryPath, Exception exception)
		{
			mDestinationPathOverride = GetDestinationPathForEntry(sourceEntryPath);

			return mExceptionHandler(ref mDestinationPathOverride, exception);
		}
	}
}
