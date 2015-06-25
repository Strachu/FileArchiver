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
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FileArchiver.Core.Archive;
using FileArchiver.Core.DirectoryTraversing;
using FileArchiver.Core.ValueTypes;
using FileArchiver.Core.Utils.File;

using Lang = FileArchiver.Core.Properties.Resources;

namespace FileArchiver.Core
{
	internal class FromFileSystemFileAddingService : IFromFileSystemFileAddingService
	{
		private readonly IFileSystem mFileSystem;

		public FromFileSystemFileAddingService(IFileSystem fileSystem)
		{
			Contract.Requires(fileSystem != null);

			mFileSystem = fileSystem;
		}

		public void AddFiles(IArchive archive,
		                     Path destinationDirectory,
		                     IReadOnlyCollection<Path> pathsOnFileSystem,
		                     FileAddingErrorHandler errorHandler)
		{
			CheckFilesCanBeAdded(archive, pathsOnFileSystem);

			foreach(var pathOnFileSystem in pathsOnFileSystem)
			{
				var traverser = new FileSystemHierarchyTraverser(mFileSystem);
				var visitor   = new FromFileSystemFileAddingVisitor(mFileSystem, archive,
				                                                    destinationDirectory.Combine(pathOnFileSystem.FileName),
				                                                    errorHandler);

				traverser.Traverse(visitor, pathOnFileSystem);
			}
		}

		private void CheckFilesCanBeAdded(IArchive archive, IReadOnlyCollection<Path> pathsOnFileSystem)
		{
			if(archive.SupportsMultipleFiles)
				return;

			if(archive.RootFiles.Any()     ||
			   pathsOnFileSystem.Count > 1 || pathsOnFileSystem.Any(mFileSystem.FileInfo.IsDirectory))
			{
				throw new InvalidOperationException(Lang.ArchiveCannotContainMultipleFiles);
			}			
		}
	}
}
