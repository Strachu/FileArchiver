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

using FileArchiver.Core.Archive;
using FileArchiver.Core.Utils.File;
using FileArchiver.Core.ValueTypes;

using Path = FileArchiver.Core.ValueTypes.Path;

namespace FileArchiver.Core.Services
{
	public class TempFileProvider : IDisposable
	{
		private readonly Path                         mAppTempDirectory;
		private readonly IDictionary<FileEntry, Path> mTempFilesForEntries;

		public TempFileProvider()
		{
			mAppTempDirectory    = PathUtil.GetUniquePath(new Path(System.IO.Path.GetTempPath()));
			mTempFilesForEntries = new Dictionary<FileEntry, Path>(capacity: 10000);

			Directory.CreateDirectory(mAppTempDirectory);
		}

		/// <summary>
		/// Gets a unique path to a temporary file.
		/// </summary>
		/// <remarks>
		/// The file is NOT created automatically, only unique path is returned.
		/// </remarks>
		public Path GetUniqueTempFile()
		{
			return PathUtil.GetUniquePath(mAppTempDirectory);
		}

		/// <summary>
		/// An overload of <see cref="GetUniqueTempFile()"/> which ensures that the file has specified name.
		/// </summary>
		/// <param name="fileName">
		/// The name for the file.
		/// </param>
		public Path GetUniqueTempFile(FileName fileName)
		{
			Contract.Requires(fileName != null);

			var path = GetUniqueTempFile();

			Directory.CreateDirectory(path);

			return path.Combine(fileName);
		}

		/// <summary>
		/// Gets a path to a temporary file associated with given entry.
		/// </summary>
		/// <param name="entry">
		/// The entry for which to get the temporary file.
		/// </param>
		/// <returns>
		/// A path to a temporary file with the same name as the entry's name.
		/// </returns>
		/// <remarks>
		/// It is guaranteed that all future calls with the same entry will return the path to the same file.
		/// The file is NOT created automatically, only a path is returned to it.
		/// </remarks>
		public Path GetTempFileFor(FileEntry entry)
		{
			Contract.Requires(entry != null);

			AddTempFileIfDoesNotExist(entry);
			UpdateTempFileNameToMatchEntryName(entry);

			return mTempFilesForEntries[entry];
		}

		private void AddTempFileIfDoesNotExist(FileEntry entry)
		{
			if(mTempFilesForEntries.ContainsKey(entry))
				return;

			mTempFilesForEntries.Add(entry, GetUniqueTempFile(entry.Name));	
		}

		private void UpdateTempFileNameToMatchEntryName(FileEntry entry)
		{
			var tempFilePath = mTempFilesForEntries[entry];
			if(tempFilePath == null)
				return;

			if(tempFilePath.FileName.Equals(entry.Name))
				return;

			var newPath = tempFilePath.ChangeFileName(entry.Name);

			File.Move(tempFilePath, newPath);

			mTempFilesForEntries[entry] = newPath;
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposeManagedResources)
		{
			try
			{
				Directory.Delete(mAppTempDirectory, recursive: true);
			}
			catch(IOException)
			{
				// Probably some files in the directory are opened in external application - they cannot be deleted.
				// Ignore it...
			}
		}

		~TempFileProvider()
		{
			Dispose(false);
		}
	}
}
