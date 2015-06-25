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
using System.IO;
using System.Threading;
using System.Threading.Tasks;

using FileArchiver.Core.Utils;

using Path = FileArchiver.Core.ValueTypes.Path;

namespace FileArchiver.Core.Archive
{
	/// <summary>
	/// A class helping with saving an archive nested inside another archive, 
	/// as is in the case of .tar.gz - a .tar archive saved in a .gz file.
	/// </summary>
	public class ArchiveInArchiveDecorator : IArchive
	{
		private IArchive      mParentArchive;
		private IArchive      mEmbeddedArchive;
		private readonly Path mEmbeddedArchivePath;

		public ArchiveInArchiveDecorator(IArchive parentArchive,
		                                 IArchive embeddedArchive,
		                                 Path embeddedArchivePath)
		{
			Contract.Requires(parentArchive != null);
			Contract.Requires(embeddedArchive != null);
			Contract.Requires(embeddedArchivePath != null);

			mParentArchive       = parentArchive;
			mEmbeddedArchive     = embeddedArchive;
			mEmbeddedArchivePath = embeddedArchivePath;

			mEmbeddedArchive.FileAdded   += (sender, e) => FileAdded.SafeRaise(this, e);
			mEmbeddedArchive.FileRemoved += (sender, e) => FileRemoved.SafeRaise(this, e);
		}

		public async Task SaveAsync(CancellationToken cancelToken, IProgress<double?> progress)
		{
			await mEmbeddedArchive.SaveAsync(cancelToken, progress);

			foreach(var file in mParentArchive.RootFiles.ToList())
			{
				mParentArchive.RemoveFile(file.Path);
			}

			var embeddedArchiveEntry = new FileEntry.Builder().AsNew()
			                                                  .WithName(mEmbeddedArchivePath.FileName)
			                                                  .WithDataFromFile(new FileInfo(mEmbeddedArchivePath))
			                                                  .Build();

			mParentArchive.AddFile(Path.Root, embeddedArchiveEntry);

			await mParentArchive.SaveAsync(cancelToken, progress);
		}

		public bool SupportsMultipleFiles
		{
			get { return mEmbeddedArchive.SupportsMultipleFiles; }
		}

		public void AddFile(Path destinationDirectoryPath, FileEntry newFile)
		{
			mEmbeddedArchive.AddFile(destinationDirectoryPath, newFile);
		}

		public bool FileExists(Path path)
		{
			return mEmbeddedArchive.FileExists(path);
		}

		public bool FileExists(Guid fileId)
		{
			return mEmbeddedArchive.FileExists(fileId);
		}

		public FileEntry GetFile(Path path)
		{
			return mEmbeddedArchive.GetFile(path);
		}

		public FileEntry GetFile(Guid fileId)
		{
			return mEmbeddedArchive.GetFile(fileId);
		}

		public void RemoveFile(Path fileToRemove)
		{
			mEmbeddedArchive.RemoveFile(fileToRemove);
		}

		public IEnumerable<FileEntry> RootFiles
		{
			get { return mEmbeddedArchive.RootFiles; }
		}

		public bool IsModified
		{
			get { return mEmbeddedArchive.IsModified; }
		}

		public Task ExtractFilesAsync(IReadOnlyCollection<SourceDestinationPathPair> fileAndDestinationPathPairs,
		                              FileExtractionErrorHandler errorHandler,
		                              CancellationToken cancelToken,
		                              IProgress<double?> progress = null)
		{
			return mEmbeddedArchive.ExtractFilesAsync(fileAndDestinationPathPairs, errorHandler, cancelToken, progress);
		}

		public void Close()
		{
			Dispose();
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposeManagedResources)
		{
			if(disposeManagedResources)
			{
				if(mEmbeddedArchive != null)
				{
					mEmbeddedArchive.Close();
					mEmbeddedArchive = null;
				}

				if(mParentArchive != null)
				{
					mParentArchive.Close();
					mParentArchive = null;
				}
			}
		}

		public event EventHandler<FileAddedEventArgs>   FileAdded;
		public event EventHandler<FileRemovedEventArgs> FileRemoved;
	}
}
