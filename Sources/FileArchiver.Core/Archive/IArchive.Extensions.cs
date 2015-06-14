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
using System.Threading;
using System.Threading.Tasks;

using FileArchiver.Core.Utils;
using FileArchiver.Core.Utils.File;
using FileArchiver.Core.ValueTypes;

namespace FileArchiver.Core.Archive
{
	public static class IArchiveExtensions
	{
		/// <summary>
		/// Updates the file in the archive after it has been modified.
		/// </summary>
		/// <param name="archive">
		/// The archive.
		/// </param>
		/// <param name="updatedFile">
		/// The updated file.
		/// </param>
		public static void UpdateFile(this IArchive archive, FileEntry updatedFile)
		{
			Contract.Requires(archive != null);
			Contract.Requires(updatedFile != null);

			// The path in updatedFile can be stale as is in the case of file renaming.
			var currentPath = archive.GetFile(updatedFile.Id).Path;

			archive.RemoveFile(currentPath);
			archive.AddFile(GetParentPath(updatedFile), updatedFile);
		}

		private static Path GetParentPath(FileEntry file)
		{
			return (file.Parent != null) ? file.Parent.Path : Path.Root;
		}

		public static void CreateDirectory(this IArchive archive, Path directoryPath)
		{
			Contract.Requires(archive != null);
			Contract.Requires(directoryPath != null);

			var directoryEntry = new FileEntry.Builder().AsDirectory()
			                                            .AsNew()
			                                            .WithName(directoryPath.FileName)
			                                            .ModifiedOn(DateTime.Now)
			                                            .Build();

			archive.AddFile(directoryPath.ParentDirectory, directoryEntry);
		}

		public static void RenameFile(this IArchive archive, Path filePath, FileName newName)
		{
			Contract.Requires(archive != null);
			Contract.Requires(filePath != null);
			Contract.Requires(newName != null);

			var previousEntry = archive.GetFile(filePath);
			var modifiedEntry = previousEntry.BuildCopy()
			                                 .AsModified()
			                                 .WithName(newName)
			                                 .Build();

			archive.UpdateFile(modifiedEntry);
		}

		/// <summary>
		/// The same as <see cref="IArchive.ExtractFilesAsync(IReadOnlyCollection{SourceDestinationPathPair}, FileExtractionErrorHandler, CancellationToken, IProgress{double?})">
		/// ExtractFilesAsync()</see> but without per file error handler support.
		/// </summary>
		public static Task ExtractFilesAsync(this IArchive archive,
		                                     IReadOnlyCollection<SourceDestinationPathPair> fileAndDestinationPathPairs,
		                                     CancellationToken cancelToken,
		                                     IProgress<double?> progress = null)
		{
			Contract.Requires(archive != null);

			return archive.ExtractFilesAsync(fileAndDestinationPathPairs, NullFileErrorHandler, cancelToken, progress);
		}

		private static RetryAction NullFileErrorHandler(ref Path destinationPath, Exception exception)
		{
			return RetryAction.RethrowException;
		}

		/// <summary>
		/// The overload of <see cref="IArchive.ExtractFilesAsync(IReadOnlyCollection{SourceDestinationPathPair}, FileExtractionErrorHandler, CancellationToken, IProgress{double?})">
		/// ExtractFilesAsync()</see> which extracts the given files under common directory.
		/// </summary>
		/// <param name="destinationDirectory">
		/// The path of directory to extract the files to.
		/// </param>
		/// <param name="filesToExtract">
		/// The paths of files to extract.
		/// </param>
		public static Task ExtractFilesAsync(this IArchive archive,
		                                     Path destinationDirectory,
		                                     IReadOnlyCollection<Path> filesToExtract,
		                                     FileExtractionErrorHandler errorHandler,
		                                     CancellationToken cancelToken,
		                                     IProgress<double?> progress = null)
		{
			Contract.Requires(archive != null);
			Contract.Requires(destinationDirectory != null);
			Contract.Requires(filesToExtract != null);
			Contract.Requires(Contract.ForAll(filesToExtract, file => file != null));

			if(!filesToExtract.Any())
				return Task.FromResult(0);

			var commonParentDirectoryPath   = PathUtil.GetParentDirectory(filesToExtract);
			var fileAndDestinationPathPairs = filesToExtract.Select(filePath =>
			{
				return new SourceDestinationPathPair
				(
					sourcePath      : filePath,
					destinationPath : (commonParentDirectoryPath != null) ? filePath.ChangeDirectory(commonParentDirectoryPath, destinationDirectory)
						                                                   : destinationDirectory.Combine(filePath)
				);
			});

			return archive.ExtractFilesAsync(fileAndDestinationPathPairs.ToList(), errorHandler, cancelToken, progress);
		}
	}
}
