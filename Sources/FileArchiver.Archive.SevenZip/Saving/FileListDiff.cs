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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FileArchiver.Core.Archive;

namespace FileArchiver.Archive.SevenZip
{
	/// <summary>
	/// A class which compares 2 lists with files and tells the differences between them.
	/// </summary>
	internal partial class FileListDiff
	{
		private FileListDiff(IReadOnlyCollection<FileEntry> originalFileList, IReadOnlyCollection<FileEntry> currentFileList)
		{
			AddedFiles    = FindFilesNotInSecondList(originalFileList, currentFileList);
			RemovedFiles  = FindFilesNotInSecondList(currentFileList, originalFileList);

			ModifiedFiles = FindModifiedFiles(originalFileList, currentFileList);
		}

		/// <summary>
		/// Compares the specified lists and returns the differences between them.
		/// </summary>
		/// <param name="originalFileList">
		/// The original file list.
		/// </param>
		/// <param name="currentFileList">
		/// The changed file list.
		/// </param>
		public static FileListDiff Compare(IReadOnlyCollection<FileEntry> originalFileList,
		                                   IReadOnlyCollection<FileEntry> currentFileList)
		{
			return new FileListDiff(originalFileList, currentFileList);
		}

		private IEnumerable<FileEntry> FindFilesNotInSecondList(IReadOnlyCollection<FileEntry> originalFileList,
		                                                        IReadOnlyCollection<FileEntry> currentFileList)
		{
			foreach(var filesInSomeDirectory in EnumerateMatchingDirectories(originalFileList, currentFileList))
			{
				var currentFiles  = filesInSomeDirectory.CurrentFiles;
				var originalFiles = filesInSomeDirectory.OriginalFiles;

				foreach(var file in currentFiles.Except(originalFiles))
				{
					yield return file;
				}
			}
		}

		private IEnumerable<FileEntry> FindModifiedFiles(IReadOnlyCollection<FileEntry> originalFileList,
		                                                 IReadOnlyCollection<FileEntry> currentFileList)
		{
			foreach(var filesInSomeDirectory in EnumerateMatchingDirectories(originalFileList, currentFileList))
			{
				foreach(var file in filesInSomeDirectory.CurrentFiles)
				{
					var previousVersionOfFile = filesInSomeDirectory.OriginalFiles.SingleOrDefault(x => x.Equals(file));
					if(previousVersionOfFile == null)
						continue;

					if(IsModified(previousVersionOfFile, file))
					{
						yield return file;
					}
				}
			}
		}

		private static bool IsModified(FileEntry originalState, FileEntry currentState)
		{
			if(!currentState.Name.Equals(originalState.Name)                             ||
				currentState.ArchiveData          != originalState.ArchiveData            ||
			   currentState.IsDirectory          != originalState.IsDirectory            ||
			   currentState.LastModificationTime != originalState.LastModificationTime   ||
			  (currentState.DataFilePath == null && originalState.DataFilePath != null)  ||
			  (currentState.DataFilePath != null && currentState.DataFilePath.Equals(originalState.DataFilePath)))
			{
				return true;
			}

			// When files in directory change the directory should NOT be included in the list of modified files
			if(currentState.IsDirectory)
				return false;

			return currentState.Size  != originalState.Size ||
			       currentState.State == FileState.Modified;
		}

		/// <summary>
		/// Enumerates the directories in given lists in such a way that for every still existing directory
		/// a list of its original and current files is returned.
		/// </summary>
		private IEnumerable<DirectoryFiles> EnumerateMatchingDirectories(IReadOnlyCollection<FileEntry> originalFileList,
		                                                                 IReadOnlyCollection<FileEntry> currentFileList)
		{
			var results = new List<DirectoryFiles>();
			MatchFileListRecursively(originalFileList, currentFileList, results);
			return results;
		}

		private void MatchFileListRecursively(IReadOnlyCollection<FileEntry> originalFileList,
		                                      IReadOnlyCollection<FileEntry> currentFileList,
		                                      ICollection<DirectoryFiles> listToAddTo)
		{
			listToAddTo.Add(new DirectoryFiles(originalFileList, currentFileList));

			foreach(var originalDirectory in originalFileList.Where(file => file.IsDirectory))
			{
				var theDirectoryInNewList = currentFileList.SingleOrDefault(x => x.Equals(originalDirectory));
				if(theDirectoryInNewList == null)
					continue;

				MatchFileListRecursively(originalDirectory.Files.ToList(), theDirectoryInNewList.Files.ToList(), listToAddTo);
			}
		}

		/// <summary>
		/// Returns the list of added files.
		/// </summary>
		/// <remarks>
		/// If an entire directory with files has been added, only the directory will be listed.
		/// </remarks>
		public IEnumerable<FileEntry> AddedFiles
		{
			get;
			private set;
		}

		/// <summary>
		/// Returns the list of entries which have been modified.
		/// </summary>
		/// <remarks>
		/// It only check whether the given entry has been modified and ignores it's children.
		/// It means that if some file in a directory has been modified, only the file will be listed
		/// WITHOUT the directory.
		/// The directory will be in returned list only if its properties such as modification time has changed.
		/// </remarks>
		public IEnumerable<FileEntry> ModifiedFiles
		{
			get;
			private set;
		}

		/// <summary>
		/// Returns the list of removed files.
		/// </summary>
		/// <remarks>
		/// If an entire directory with files has been removed, only the directory will be listed.
		/// </remarks>
		public IEnumerable<FileEntry> RemovedFiles
		{
			get;
			private set;
		}
	}
}
