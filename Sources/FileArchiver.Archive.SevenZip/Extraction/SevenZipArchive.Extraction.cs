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
using System.IO;
using System.Linq;
using System.Threading;

using FileArchiver.Core.Archive;
using FileArchiver.Core.DirectoryTraversing;
using FileArchiver.Core.Utils.File;

namespace FileArchiver.Archive.SevenZip
{
	public partial class SevenZipArchive
	{
		private void ExtractFiles(IEnumerable<SourceDestinationPathPair> fileAndDestinationPathPairs,
		                          FileExtractionErrorHandler errorHandler,
		                          CancellationToken cancelToken,
		                          IProgress<double?> progress = null)
		{
			var extractOperations = fileAndDestinationPathPairs.Select(pair => new FileDestinationPair
			(
				file            : base.GetFile(pair.SourcePath),
				destinationPath : pair.DestinationPath
			)).ToList();

			progress               = progress ?? new Progress<double?>();
			long bytesToExtract    = GetTotalBytesToExtract(extractOperations.Select(x => x.File).ToList());
			var extractionProgress = new CompositeFileProgress(bytes => progress.Report((double)bytes / bytesToExtract));

			using(var temporaryDirectory = new TempDirectoryProvider(extractOperations))
			{
				var filesNotOnDiskYet = GetFilesNotOnDiskYet(extractOperations.Select(x => x.File)).ToList();

				ExtractFilesToTemp(filesNotOnDiskYet, temporaryDirectory, cancelToken, extractionProgress);
				CopyFilesToDestination(extractOperations, temporaryDirectory, errorHandler, cancelToken, extractionProgress);
			}
		}

		private long GetTotalBytesToExtract(IReadOnlyCollection<FileEntry> filesToExtract)
		{
			long bytesToExtract = filesToExtract.Sum(file => file.Size);

			if(IsSolid)
			{
				var filesNotOnDiskYet       = GetFilesNotOnDiskYet(filesToExtract).ToList();
				var linkedFiles             = GetAllFilesInSolidBlocksOf(filesNotOnDiskYet).ToList();
				var linkedFilesNotOnDiskYet = linkedFiles.Where(file => !IsAlreadyOnDisk(file)).ToList();

				bytesToExtract += linkedFilesNotOnDiskYet.Sum(file => file.Size);
			}

			return bytesToExtract;
		}

		private IEnumerable<FileEntry> GetFilesNotOnDiskYet(IEnumerable<FileEntry> files)
		{
			return files.Flatten().Where(file => !file.IsDirectory).Where(file => !IsAlreadyOnDisk(file));
		}

		private bool IsAlreadyOnDisk(FileEntry file)
		{
			bool isInTemp = File.Exists(mTempFileProvider.GetTempFileFor(file));

			return file.DataFilePath != null || isInTemp;
		}

		private void ExtractFilesToTemp(IReadOnlyCollection<FileEntry> filesToExtract,
		                                TempDirectoryProvider temporaryDirectory,
		                                CancellationToken cancelToken,
		                                CompositeFileProgress progress)
		{
			if(!filesToExtract.Any())
				return;

			if(!IsSolid)
			{
				var fileDestinationPairs = filesToExtract.Select(fileToExtract => new FileDestinationPair
				(
					file            : fileToExtract,
					destinationPath : temporaryDirectory.GetTemporaryPathFor(fileToExtract)
				));

				mSevenZipApplication.ExtractFromArchive(mArchivePath, fileDestinationPairs.ToList(), cancelToken,
				                                        progress.GetProgressForNextFile());
			}
			else
			{
				// Files are extracted to system temporary directory, because in archive with solid compression
				// only entire blocks can be extracted even if single file is wanted.
				// Extracting to temp makes future extractions a lot faster which is important especially
				// when opening single files in external application.

				var linkedFiles             = GetAllFilesInSolidBlocksOf(filesToExtract).ToList();
				var linkedFilesNotOnDiskYet = linkedFiles.Where(file => !IsAlreadyOnDisk(file)).ToList();
				var fileDestinationPairs    = linkedFilesNotOnDiskYet.Select(fileToExtract => new FileDestinationPair
				(
					file            : fileToExtract,
					destinationPath : mTempFileProvider.GetTempFileFor(fileToExtract)
				));

				mSevenZipApplication.ExtractFromArchive(mArchivePath, fileDestinationPairs.ToList(), cancelToken,
				                                        progress.GetProgressForNextFile());
			}
		}

		private void CopyFilesToDestination(IEnumerable<FileDestinationPair> fileAndDestinationPathPairs,
		                                    TempDirectoryProvider temporaryDirectories,
		                                    FileExtractionErrorHandler errorHandler,
		                                    CancellationToken cancelToken,
		                                    CompositeFileProgress progress)
		{
			foreach(var copyOperation in fileAndDestinationPathPairs)
			{
				var sourceDirectory = temporaryDirectories.GetTopMostTemporaryDirectoryOf(copyOperation.File);

				var hierarchyTraverser = new FileEntryHierarchyTraverser();
				var extractingVisitor  = new FileCopyingVisitor(mTempFileProvider, sourceDirectory,
				                                                copyOperation.DestinationPath,
				                                                errorHandler, cancelToken,
				                                                progress.GetProgressForNextFile());

				hierarchyTraverser.Traverse(extractingVisitor, copyOperation.File);
			}
		}
	}
}
