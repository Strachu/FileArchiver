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
using System.Text;
using System.Threading.Tasks;

using FileArchiver.Core.ValueTypes;
using FileArchiver.Core.Utils;

namespace FileArchiver.Core.Archive
{
	/// <summary>
	/// A builder whose responsibility is to build a file hierarchy without worrying about creation of directories
	/// before the addition of files.
	/// </summary>
	public partial class FileEntryHierarchyBuilder
	{
		private readonly EntryAndItsFiles mRoot;

		public FileEntryHierarchyBuilder()
		{
			var dummyRootDirectory = new FileEntry.Builder().AsDirectory().WithName(new FileName("Root")).Build();

			mRoot = new EntryAndItsFiles(dummyRootDirectory);
		}

		/// <summary>
		/// Adds the file to the specified directory.
		/// </summary>
		/// <param name="destinationDirectory">
		/// The directory to add the file to. If the directory does not exist, it will be created.
		/// </param>
		/// <param name="file">
		/// The file to add.
		/// </param>
		/// <remarks>
		/// If the file already exists, it will be replaced.
		/// </remarks>
		public FileEntryHierarchyBuilder AddFile(Path destinationDirectory, FileEntry file)
		{
			Contract.Requires(destinationDirectory != null);
			Contract.Requires(file != null);

			var currentDir = GetOrCreateDestinationDirectory(destinationDirectory);

			AddOrReplaceFile(currentDir, file);
			return this;
		}

		private EntryAndItsFiles GetOrCreateDestinationDirectory(Path directoryPath)
		{
			var hierarchy  = GetPathHierarchy(directoryPath);
			var currentDir = mRoot;
			
			foreach(var path in hierarchy)
			{
				var previousDir = currentDir;
				currentDir      = currentDir.Files.SingleOrDefault(x => x.Entry.Name.Equals(path.FileName));

				if(currentDir == null)
				{
					var intermediateDirectory = new FileEntry.Builder().AsDirectory().WithName(path.FileName).Build();

					currentDir = new EntryAndItsFiles(intermediateDirectory);

					previousDir.Files.Add(currentDir);
				}
			}

			return currentDir;
		}

		private IEnumerable<Path> GetPathHierarchy(Path path)
		{
			var parentDirectoriesPaths = new List<Path>();
			while(path.ParentDirectory != null)
			{
				parentDirectoriesPaths.Add(path);

				path = path.ParentDirectory;
			}

			parentDirectoriesPaths.Reverse();

			return parentDirectoriesPaths;
		}

		private void AddOrReplaceFile(EntryAndItsFiles destinationDirectory, FileEntry file)
		{
			var newFileEntry = new EntryAndItsFiles(file);

			var existingFileEntry = destinationDirectory.Files.SingleOrDefault(x => x.Entry.Name.Equals(file.Name));
			if(existingFileEntry != null)
			{
				existingFileEntry.Files.CopyTo(newFileEntry.Files);

				destinationDirectory.Files.Remove(existingFileEntry);
			}

			destinationDirectory.Files.Add(newFileEntry);			
		}

		/// <summary>
		/// Builds the file hierarchy.
		/// </summary>
		public IEnumerable<FileEntry> Build()
		{
			return mRoot.Files.Select(InitializeDirectoryTree);
		}

		private FileEntry InitializeDirectoryTree(EntryAndItsFiles entry)
		{
			var directory = entry.Entry;
			if(entry.Files.Any())
			{
				var initializedChildren = entry.Files.Select(InitializeDirectoryTree).ToArray();

				directory = directory.BuildCopy().WithFiles(initializedChildren).Build();
			}
			return directory;
		}
	}
}
