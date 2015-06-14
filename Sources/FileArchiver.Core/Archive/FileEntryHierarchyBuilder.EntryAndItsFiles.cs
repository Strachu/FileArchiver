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
		private class EntryAndItsFiles
		{
			public EntryAndItsFiles(FileEntry entry)
			{
				Entry = entry;
				Files = new List<EntryAndItsFiles>();
			}

			public FileEntry Entry
			{
				get;
				private set;
			}

			public ICollection<EntryAndItsFiles> Files
			{
				get;
				private set;
			}
		}
	}
}
