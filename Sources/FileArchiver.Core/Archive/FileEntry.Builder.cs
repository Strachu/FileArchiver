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
using System.IO.Abstractions;
using System.Linq;

using FileArchiver.Core.ValueTypes;

using Path = FileArchiver.Core.ValueTypes.Path;

namespace FileArchiver.Core.Archive
{
	public partial class FileEntry
	{
		public class Builder
		{
			private          Guid            mId;
			private readonly FileEntry       mParent;
			private          FileName        mName;
			private          long            mSize;
			private          DateTime?       mLastModificationTime;
			private          bool            mIsDirectory;
			private          Path            mDataFilePath;
			private          FileState       mFileState;
			private          List<FileEntry> mFiles;
			private readonly List<FileEntry> mRemovedFiles;
			private          object          mArchiveData;

			public Builder()
			{
				mId                   = Guid.NewGuid();
				mParent               = null;
				mName                 = null;
				mSize                 = 0;
				mLastModificationTime = null;
				mIsDirectory          = false;
				mDataFilePath         = null;
				mFileState            = FileState.Unchanged;
				mFiles                = new List<FileEntry>();
				mRemovedFiles         = new List<FileEntry>();
				mArchiveData          = null;
			}

			public Builder(FileEntry prototype)
			{
				Contract.Requires(prototype != null);

				mId                   = prototype.Id;
				mName                 = prototype.Name;
				mParent               = prototype.Parent;
				mSize                 = prototype.mFileDataSize;
				mLastModificationTime = prototype.LastModificationTime;
				mIsDirectory          = prototype.IsDirectory;
				mDataFilePath         = prototype.DataFilePath;
				mFileState            = prototype.mFileState;
				mFiles                = prototype.Files.ToList();
				mRemovedFiles         = prototype.mRemovedFiles;
				mArchiveData          = prototype.ArchiveData;
			}

			public Builder AsDirectory()
			{
				mIsDirectory = true;
				return this;
			}

			public Builder AsNew()
			{
				mId        = Guid.NewGuid();
				mFileState = FileState.New;
				return this;
			}

			public Builder AsModified()
			{
				mFileState = FileState.Modified;
				return this;
			}

			public Builder WithName(FileName name)
			{
				mName = name;
				return this;
			}

			public Builder WithSize(long size)
			{
				mSize = size;
				return this;
			}

			public Builder ModifiedOn(DateTime modificationTime)
			{
				mLastModificationTime = modificationTime;
				return this;
			}

			public Builder WithDataFromFile(FileInfoBase fileInfo)
			{
				Contract.Requires(fileInfo != null);
				Contract.Requires(!fileInfo.Attributes.HasFlag(FileAttributes.Directory));

				mDataFilePath = new Path(fileInfo.FullName);

				return this.WithSize(fileInfo.Length).ModifiedOn(fileInfo.LastWriteTime);
			}

			public Builder WithNewFile(FileEntry newFile)
			{
				mRemovedFiles.Remove(newFile);

				mFiles.Add(newFile);
				return this;
			}

			public Builder WithoutFileNamed(FileName name)
			{
				var fileToRemove = mFiles.Single(file => file.Name.Equals(name));

				mFiles.Remove(fileToRemove);
				mRemovedFiles.Add(fileToRemove);
				return this;
			}

			public Builder WithFiles(params FileEntry[] files)
			{
				Contract.Requires(files != null);
				Contract.Requires(Contract.ForAll(files, file => file != null));

				mFiles = files.ToList();
				return this;
			}

			public Builder WithArchiveData(object internalData)
			{
				Contract.Requires(internalData != null);

				mArchiveData = internalData;
				return this;
			}

			public FileEntry Build()
			{
				return new FileEntry(mId, mName, mParent, mFiles, mRemovedFiles, mSize, mLastModificationTime, mIsDirectory, mDataFilePath, mFileState, mArchiveData);
			}
		}
	}
}
