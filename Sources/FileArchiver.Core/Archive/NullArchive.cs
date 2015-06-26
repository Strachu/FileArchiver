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
using System.Threading;
using System.Threading.Tasks;

using FileArchiver.Core.ValueTypes;

namespace FileArchiver.Core.Archive
{
	/// <summary>
	/// Null Object Pattern
	/// </summary>
	public sealed class NullArchive : IArchive
	{
		public bool SupportsMultipleFiles
		{
			get { return false; }
		}

		public void AddFile(Path destinationDirectoryPath, FileEntry newFile)
		{
		}

		public bool FileExists(Path path)
		{
			return false;
		}

		public bool FileExists(Guid fileId)
		{
			return false;
		}

		public FileEntry GetFile(Path path)
		{
			return new FileEntry.Builder().WithName(new FileName("NullFile")).Build();
		}

		public FileEntry GetFile(Guid fileId)
		{
			return new FileEntry.Builder().WithName(new FileName("NullFile")).Build();
		}

		public void RemoveFile(Path fileToRemove)
		{
		}

		public IEnumerable<FileEntry> RootFiles
		{
			get { return new FileEntry[0]; }
		}

		public bool IsModified
		{
			get { return false; }
		}

		public Task ExtractFilesAsync(IReadOnlyCollection<SourceDestinationPathPair> fileAndDestinationPathPairs,
		                              FileExtractionErrorHandler errorHandler,
		                              CancellationToken cancelToken,
		                              IProgress<double?> progress = null)
		{
			return Task.FromResult(0);
		}

		public Task SaveAsync(CancellationToken cancelToken, IProgress<double?> progress)
		{
			return Task.FromResult(0);
		}

		public void Dispose()
		{
		}

		public void Close()
		{
		}

		#pragma warning disable 67
		public event EventHandler<FileAddedEventArgs>   FileAdded;
		public event EventHandler<FileRemovedEventArgs> FileRemoved;
		#pragma warning restore 67
	}
}
