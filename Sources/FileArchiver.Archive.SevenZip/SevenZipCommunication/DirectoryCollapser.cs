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

using FileArchiver.Core.Archive;

namespace FileArchiver.Archive.SevenZip.SevenZipCommunication
{
	/// <summary>
	/// An utility class responsible for optimally collapsing file lists.
	/// </summary>
	public class DirectoryCollapser
	{
		/// <summary>
		/// Collapses the specified file list such that when it is possible only single entry for a directory
		/// is left when all its files are on the list.
		/// </summary>
		/// <param name="files">
		/// The file list to collapse.
		/// </param>
		/// <returns>
		/// Collapsed file list.
		/// </returns>
		public static IEnumerable<FileEntry> Collapse(IReadOnlyCollection<FileEntry> files)
		{
			Contract.Requires(files != null);
			Contract.Requires(Contract.ForAll(files, file => file != null));

			var collapsed = new HashSet<FileEntry>(files);
			var passCount = GetPassCount(files);

			for(int i = 0; i < passCount; ++i)
			{
				CollapseSingleLevel(collapsed);
			}

			return collapsed.ToList();
		}

		private static void CollapseSingleLevel(ICollection<FileEntry> files)
		{
			foreach(var file in files.ToList())
			{
				if(file.Parent == null)
					continue;

				if(files.Contains(file.Parent))
				{
					files.Remove(file);
				}

				bool allFilesPresent = file.Parent.Files.All(files.Contains);
				if(allFilesPresent)
				{
					files.Remove(file);
					files.Add(file.Parent);
				}
			}
		}

		private static int GetPassCount(IEnumerable<FileEntry> files)
		{
			return files.Select(GetAncestorDirectoriesCount).Max();
		}

		private static int GetAncestorDirectoriesCount(FileEntry entry)
		{
			int count = 0;
			while(entry.Parent != null)
			{
				count++;

				entry = entry.Parent;
			}
			return count;
		}
	}
}
