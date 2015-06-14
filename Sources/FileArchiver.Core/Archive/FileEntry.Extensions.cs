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
using System.Linq;
using FileArchiver.Core.DirectoryTraversing;

namespace FileArchiver.Core.Archive
{
	public static class FileEntryExtensions
	{
		/// <summary>
		/// Enumerates all files recursively in depth first order.
		/// </summary>
		public static IEnumerable<FileEntry> EnumerateAllFilesRecursively(this FileEntry entry)
		{
			Contract.Requires(entry != null);

			var fileHierarchyTraverser = new FileEntryHierarchyTraverser();
			var flatteningVisitor      = new FileListFlatteningVisitor();

			fileHierarchyTraverser.Traverse(flatteningVisitor, entry);

			return flatteningVisitor.Files;
		}

		private class FileListFlatteningVisitor : DirectoryHierarchyVisitor<FileEntry>
		{
			private readonly List<FileEntry> mFiles = new List<FileEntry>();

			public override void VisitFile(FileEntry file)
			{
				mFiles.Add(file);
			}

			public override bool OnDirectoryEntering(FileEntry directory)
			{
				mFiles.Add(directory);
				return true;
			}

			public IEnumerable<FileEntry> Files
			{
				get { return mFiles.Skip(1); }
			}
		}
	}
}
