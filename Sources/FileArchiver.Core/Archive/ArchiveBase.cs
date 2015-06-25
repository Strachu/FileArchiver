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
using System.Threading;
using System.Threading.Tasks;

using FileArchiver.Core.Utils;

using Path = FileArchiver.Core.ValueTypes.Path;

namespace FileArchiver.Core.Archive
{
	/// <summary>
	/// A base class for archive implementations which relieves the implementor
	/// from implementing the management of in memory file list.
	/// </summary>
	public abstract class ArchiveBase : IArchive
	{
		private readonly List<FileEntry>             mRootFiles        = new List<FileEntry>();
		private readonly List<FileEntry>             mRemovedRootFiles = new List<FileEntry>();
		private readonly Dictionary<Path, FileEntry> mPathFileIndex    = new Dictionary<Path, FileEntry>(capacity: 100000);
		private readonly Dictionary<Guid, FileEntry> mIdFileIndex      = new Dictionary<Guid, FileEntry>(capacity: 100000);

		public virtual bool SupportsMultipleFiles
		{
			get { return true; }
		}

		public virtual void AddFile(Path destinationDirectoryPath, FileEntry newFile)
		{
			if(destinationDirectoryPath.Equals(Path.Root))
			{
				AddRootFile(newFile);
			}
			else
			{
				AddFileToDirectory(destinationDirectoryPath, newFile);
			}

			OnFileAdded(newFile);
		}

		protected void AddRootFile(FileEntry newFile)
		{
			Contract.Requires(newFile != null);

			if(mRootFiles.Any(file => file.Name.Equals(newFile.Name)))
				throw new FileExistsException(newFile.Path);

			mRemovedRootFiles.Remove(newFile);
			mRootFiles.Add(newFile);

			AddToIndex(newFile);		
		}

		protected void AddFileToDirectory(Path destinationDirectoryPath, FileEntry newFile)
		{
			Contract.Requires(destinationDirectoryPath != null);
			Contract.Requires(newFile != null);

			var destinationDirectory = GetFile(destinationDirectoryPath);
			if(!destinationDirectory.IsDirectory)
				throw new ArgumentException(String.Format("\"{0}\" is not a directory.", destinationDirectoryPath));
			
			if(destinationDirectory.Files.Any(file => file.Name.Equals(newFile.Name)))
				throw new FileExistsException(newFile.Path);

			var updatedDestinationDirectory = destinationDirectory.BuildCopy().WithNewFile(newFile).Build();

			UpdateReferenceInIndex(updatedDestinationDirectory);

			UpdateParentsWithNewFileReference(destinationDirectory, updatedDestinationDirectory);

			AddToIndex(newFile);	
		}

		/// <summary>
		/// Updates the references to modified entry in its parents, all the way to the root.
		/// </summary>
		/// <param name="oldEntry">
		/// The old version of entry.
		/// </param>
		/// <param name="entry">
		/// The new version of entry.
		/// </param>
		private void UpdateParentsWithNewFileReference(FileEntry oldEntry, FileEntry entry)
		{
			if(entry.Parent == null)
			{
				mRootFiles.Remove(oldEntry);
				mRootFiles.Add(entry);
				return;
			}

			var updatedParent = entry.Parent.BuildCopy().WithoutFileNamed(oldEntry.Name)
																	  .WithNewFile(entry).Build();

			UpdateReferenceInIndex(updatedParent);

			UpdateParentsWithNewFileReference(oldEntry.Parent, updatedParent);
		}

		private void AddToIndex(FileEntry newEntry)
		{
			mPathFileIndex.Add(newEntry.Path, newEntry);
			mIdFileIndex.Add(newEntry.Id, newEntry);

			foreach(var file in newEntry.EnumerateAllFilesRecursively())
			{
				mPathFileIndex.Add(file.Path, file);
				mIdFileIndex.Add(file.Id, file);
			}
		}

		private void UpdateReferenceInIndex(FileEntry entry)
		{
			mPathFileIndex[entry.Path] = entry;
			mIdFileIndex[entry.Id]     = entry;
		}

		public bool FileExists(Path path)
		{
			return mPathFileIndex.ContainsKey(path);
		}

		public bool FileExists(Guid fileId)
		{
			return mIdFileIndex.ContainsKey(fileId);
		}

		public FileEntry GetFile(Path path)
		{
			if(!FileExists(path))
				throw new FileNotFoundException(String.Format("The file \"{0}\" does not exist.", path));

			return mPathFileIndex[path];
		}

		public FileEntry GetFile(Guid fileId)
		{
			if(!mIdFileIndex.ContainsKey(fileId))
				throw new FileNotFoundException(String.Format("The file with id \"{0}\" does not exist.", fileId));

			return mIdFileIndex[fileId];
		}

		public void RemoveFile(Path fileToRemove)
		{
			if(!FileExists(fileToRemove))
				return;

			var file = GetFile(fileToRemove);

			if(file.Parent == null)
			{
				mRootFiles.Remove(file);
				mRemovedRootFiles.Add(file);
			}
			else
			{
				var updatedParent = file.Parent.BuildCopy()
				                               .WithoutFileNamed(file.Name)
				                               .Build();

				UpdateReferenceInIndex(updatedParent);

				UpdateParentsWithNewFileReference(file.Parent, updatedParent);
			}

			RemoveFromIndex(file);

			OnFileRemoved(file);
		}

		private void RemoveFromIndex(FileEntry removedEntry)
		{
			mPathFileIndex.Remove(removedEntry.Path);
			mIdFileIndex.Remove(removedEntry.Id);

			foreach(var childFile in removedEntry.EnumerateAllFilesRecursively())
			{
				mPathFileIndex.Remove(childFile.Path);
				mIdFileIndex.Remove(childFile.Id);
			}
		}

		public IEnumerable<FileEntry> RootFiles
		{
			get { return mRootFiles; }
		}

		public bool IsModified
		{
			get
			{
				return mRootFiles.Any(entry => entry.State != FileState.Unchanged) ||
				       mRemovedRootFiles.Any(file => file.State != FileState.New);
			}
		}

		/// <summary>
		/// Resets all internal state of archive such as if it was just created.
		/// </summary>
		protected virtual void ResetState()
		{
			foreach(var file in RootFiles.ToList())
			{
				RemoveFile(file.Path);
			}

			mRemovedRootFiles.Clear();
			mRootFiles.Clear();
			mPathFileIndex.Clear();
			mIdFileIndex.Clear();
		}

		public event EventHandler<FileAddedEventArgs>   FileAdded;
		public event EventHandler<FileRemovedEventArgs> FileRemoved;

		protected virtual void OnFileAdded(FileEntry addedFile)
		{
			Contract.Requires(addedFile != null);

			FileAdded.SafeRaise(this, new FileAddedEventArgs(addedFile));
		}

		protected virtual void OnFileRemoved(FileEntry removedFile)
		{
			Contract.Requires(removedFile != null);

			FileRemoved.SafeRaise(this, new FileRemovedEventArgs(removedFile));
		}

		void IDisposable.Dispose()
		{
			Close();
		}

		public void Close()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposeManagedResources)
		{
		}

		public abstract Task ExtractFilesAsync(IReadOnlyCollection<SourceDestinationPathPair> fileAndDestinationPathPairs, FileExtractionErrorHandler errorHandler, CancellationToken cancelToken, IProgress<double?> progress = null);
		public abstract Task SaveAsync(CancellationToken cancelToken, IProgress<double?> progress);
	}
}
