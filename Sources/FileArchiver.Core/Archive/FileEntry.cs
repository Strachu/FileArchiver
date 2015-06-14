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

using FileArchiver.Core.ValueTypes;

namespace FileArchiver.Core.Archive
{
	public enum FileState
	{
		New,
		Unchanged,
		Modified
	}

	/// <summary>
	/// A class containing all information about a file or a directory in an archive.
	/// </summary>
	public partial class FileEntry
	{
		private readonly long            mFileDataSize;
		private readonly FileState       mFileState;
		private readonly List<FileEntry> mRemovedFiles;

		private FileEntry(Guid id, FileName name, FileEntry parent, IEnumerable<FileEntry> files, IEnumerable<FileEntry> removedFiles, long size, DateTime? modificationTime, bool isDirectory, Path dataFilePath, FileState state, object archiveData)
		{
			Contract.Requires(name != null);
			Contract.Requires(files != null);
			Contract.Requires(Contract.ForAll(files, file => file != null));
			Contract.Requires(removedFiles != null);
			Contract.Requires(Contract.ForAll(removedFiles, file => file != null));
			Contract.Requires(isDirectory || !files.Any(), "Only directories can contain files.");
			Contract.Requires(size >= 0);

			Id                   = id;
			Name                 = name;
			Parent               = parent;
			Files                = files.ToList();
			mRemovedFiles        = removedFiles.ToList();
			mFileDataSize        = size;
			LastModificationTime = modificationTime;
			IsDirectory          = isDirectory;
			DataFilePath         = dataFilePath;
			mFileState           = state;
			ArchiveData          = archiveData;

			var filesWithDuplicatedNames = Files.GroupBy(file => file.Name).FirstOrDefault(group => group.Count() > 1);
			if(filesWithDuplicatedNames != null)
				throw new ArgumentException("Detected multiple files named " + filesWithDuplicatedNames.Key);

			// Strict immutability is a bit tricky in the case of tree structures where a reference to both a parent and children is needed.
			// If it is truly needed, the alternative to following code is to change "Parent" field to a "string Path" or construct the entire directory tree
			// starting from the root when a file is added to any directory.
			foreach(var file in Files)
			{
				file.Parent = this;
			}
		}

		public FileName Name
		{
			get;
			private set;
		}

		public Guid Id
		{
			get;
			private set;
		}

		public FileEntry Parent
		{
			get;
			private set;
		}

		public Path Path
		{
			get
			{
				if(Parent == null)
					return Name;

				return Parent.Path.Combine(Name);
			}
		}

		public long Size
		{
			get { return mFileDataSize + Files.Sum(file => file.Size); }
		}

		public DateTime? LastModificationTime
		{
			get;
			private set;
		}

		public bool IsDirectory
		{
			get;
			private set;
		}

		public Path DataFilePath
		{
			get;
			private set;
		}

		public FileState State
		{
			get
			{
				if(mFileState != FileState.Unchanged)
					return mFileState;

				if(Files.Any(file => file.State != FileState.Unchanged))
					return FileState.Modified;

				return mRemovedFiles.Any(file => file.State != FileState.New) ? FileState.Modified : FileState.Unchanged;
			}
		}

		public IEnumerable<FileEntry> Files
		{
			get;
			private set;
		}

		public object ArchiveData
		{
			get;
			private set;
		}

		public Builder BuildCopy()
		{
			return new Builder(this);
		}

		public override bool Equals(object obj)
		{
			var otherEntry = obj as FileEntry;
			if(otherEntry == null)
				return false;

			return Id.Equals(otherEntry.Id);
		}

		public override int GetHashCode()
		{
			return Id.GetHashCode();
		}

		public override string ToString()
		{
			return Path;
		}
	}
}
