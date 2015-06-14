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
using System.Text;

using FileArchiver.Core.Archive;
using FileArchiver.Core.Utils.File;

using Path = FileArchiver.Core.ValueTypes.Path;

namespace FileArchiver.Archive.SevenZip
{
	/// <summary>
	/// A class which assigns a temporary path for every file on the same disk as the destination path to ensure
	/// a free move operation.
	/// </summary>
	/// <remarks>
	/// Every file of a directory has assigned a temporary path inside a temporary directory of its parent. 
	/// </remarks>
	internal class TempDirectoryProvider : IDisposable
	{
		private readonly Dictionary<FileEntry, Path> mFileTempPathIndex = new Dictionary<FileEntry, Path>();

		/// <summary>
		/// Initializes a new instance of the <see cref="TempDirectoryProvider"/> class.
		/// </summary>
		/// <param name="fileDestinationPathPairs">
		/// The files and their destination path for which to generate temp paths for.
		/// The destination path is used to ensure that the temporary path will be on the same drive as
		/// the path to which the file will be extracted.
		/// </param>
		public TempDirectoryProvider(IEnumerable<FileDestinationPair> fileDestinationPathPairs)
		{
			foreach(var pair in fileDestinationPathPairs)
			{
				var tempDirectoryOnTheSameDisk = PathUtil.GetUniquePath(pair.DestinationPath.ParentDirectory);

				foreach(var fileInDirectory in pair.File.EnumerateAllFilesRecursively())
				{
					mFileTempPathIndex[fileInDirectory] = tempDirectoryOnTheSameDisk.Combine(fileInDirectory.Path);
				}

				mFileTempPathIndex[pair.File] = tempDirectoryOnTheSameDisk.Combine(pair.File.Path);
			}
		}

		public Path GetTemporaryPathFor(FileEntry file)
		{
			Contract.Requires(file != null);

			return mFileTempPathIndex[file];
		}

		/// <summary>
		/// Gets the top most temporary directory in which given file is stored.
		/// </summary>
		public Path GetTopMostTemporaryDirectoryOf(FileEntry file)
		{
			Contract.Requires(file != null);

			return mFileTempPathIndex[file].Remove(file.Path);
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposeManagedResources)
		{
			foreach(var fileTempPath in mFileTempPathIndex)
			{
				var topMostTemporaryDirectory = GetTopMostTemporaryDirectoryOf(fileTempPath.Key);

				if(Directory.Exists(topMostTemporaryDirectory))
				{
					Directory.Delete(topMostTemporaryDirectory, recursive: true);
				}
			}
		}

		~TempDirectoryProvider()
		{
		   Dispose(false);
		}
	}
}
