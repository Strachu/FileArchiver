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
using FileArchiver.Core.Utils.File;
using FileArchiver.Core.ValueTypes;

namespace FileArchiver.Archive.SevenZip
{
	public partial class SevenZipArchive
	{
		private void Save(CancellationToken cancelToken, IProgress<double?> progress)
		{
			var diffList      = FileListDiff.Compare(mOriginalFiles, base.RootFiles.ToList());
			var modifiedFiles = RemoveFileEntriesIfTheirParentsAreInList(diffList.ModifiedFiles.ToList()).ToList();

			// Removing modifiedFiles is not required, as adding will replace it but it can speed up the saving in
			// the case of solid archives and it is also required when renaming a file, especially a directory.
			var filesToDelete = diffList.RemovedFiles.Concat(modifiedFiles).ToList();
			var filesToAdd    = diffList.AddedFiles.Concat(modifiedFiles).ToList();

			var savingProgress = new SavingProgress(this, progress, filesToAdd, filesToDelete);

			var tempDirectoryForFiles = mTempFileProvider.GetUniqueTempFile();
			var newArchivePath        = mArchivePath;

			try
			{	
				if(filesToAdd.Any())
				{
					var filesToExtract = filesToAdd.Select(file => new SourceDestinationPathPair
					(
						sourcePath      : file.Path,
						destinationPath : tempDirectoryForFiles.Combine(file.Path)
					));

					ExtractFiles(filesToExtract, DefaultFileExtractionErrorHandlers.RethrowEveryException, cancelToken,
					             savingProgress.ExtractionProgress);
				}

				if(filesToDelete.Any())
				{
					newArchivePath                = PathUtil.GetUniquePath(mArchivePath.ParentDirectory);
					var filesToDeleteIdsInArchive = filesToDelete.Select(file => file.GetArchiveEntryData().IdInArchive);

					mSevenZipApplication.RemoveFilesFromArchive(mArchivePath, newArchivePath, filesToDeleteIdsInArchive,
					                                            cancelToken, savingProgress.RemovalProgress);
				}

				if(filesToAdd.Any())
				{
					var rootDirectories  = filesToAdd.Select(GetRootDirectoryName).Distinct();
					var directoriesToAdd = rootDirectories.Select(dirName => tempDirectoryForFiles.Combine(dirName));

					mSevenZipApplication.AddFilesToArchive(newArchivePath, directoriesToAdd, cancelToken,
					                                       savingProgress.AdditionProgress);
				}

				if(!newArchivePath.Equals(mArchivePath))
				{
					ReplaceArchive(newArchivePath);
				}
			}
			finally
			{
				if(Directory.Exists(tempDirectoryForFiles))
				{
					Directory.Delete(tempDirectoryForFiles, recursive: true);
				}
			}

			ReloadArchive();	
		}

		private IEnumerable<FileEntry> RemoveFileEntriesIfTheirParentsAreInList(IReadOnlyCollection<FileEntry> files)
		{
			return files.Where(x => !files.Any(file => IsAncestorOf(x, file)));
		}

		private bool IsAncestorOf(FileEntry thisEntry, FileEntry second)
		{
			var parent = second;
			while((parent = parent.Parent) != null)
			{
				if(parent.Equals(thisEntry))
				{
					return true;
				}
			}
			return false;
		}

		private FileName GetRootDirectoryName(FileEntry entry)
		{
			var parent = entry;
			while(parent.Parent != null)
			{
				parent = parent.Parent;
			}
			return parent.Name;
		}

		private void ReplaceArchive(string tempFilePath)
		{
			File.Delete(mArchivePath);
			File.Move(tempFilePath, mArchivePath);
		}

		private void ReloadArchive()
		{
			ResetState();

			ReadEntries(CancellationToken.None);
		}

		protected override void ResetState()
		{
			base.ResetState();

			mSolidBlockFileIdsIndex.Clear();
			mOriginalFiles.Clear();
		}
	}
}
