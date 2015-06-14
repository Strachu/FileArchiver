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

using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.IO.Abstractions;
using System.Linq;

using Path = FileArchiver.Core.ValueTypes.Path;

namespace FileArchiver.Core.DirectoryTraversing
{
	public class FileSystemHierarchyTraverser : DirectoryHierarchyTraverser<Path>
	{
		private readonly IFileSystem mFileSystem;

		public FileSystemHierarchyTraverser(IFileSystem fileSystem)
		{
			Contract.Requires(fileSystem != null);
			
			mFileSystem = fileSystem;
		}

		protected override bool IsDirectory(Path entryPath)
		{
			return mFileSystem.File.GetAttributes(entryPath).HasFlag(FileAttributes.Directory);
		}

		protected override IEnumerable<Path> GetFiles(Path directoryPath)
		{
			return mFileSystem.Directory.EnumerateFileSystemEntries(directoryPath).Select(path => new Path(path));
		}
	}
}
