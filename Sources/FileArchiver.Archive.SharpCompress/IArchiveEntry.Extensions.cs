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

using FileArchiver.Core;
using FileArchiver.Core.Archive;
using FileArchiver.Core.ValueTypes;

using SharpCompress.Archive;

namespace FileArchiver.Archive.SharpCompress
{
	internal static class IArchiveEntryExtensions
	{
		public static FileEntry ToFileEntry(this IArchiveEntry sharpCompressEntry)
		{
			Contract.Requires(sharpCompressEntry != null);

			var entryBuilder = new FileEntry.Builder();

			entryBuilder.WithName(new Path(sharpCompressEntry.Key).FileName);
			entryBuilder.WithSize(sharpCompressEntry.Size);

			if(sharpCompressEntry.IsDirectory)
			{
				entryBuilder.AsDirectory();
			}

			if(sharpCompressEntry.LastModifiedTime.HasValue)
			{
				entryBuilder.ModifiedOn(sharpCompressEntry.LastModifiedTime.Value);
			}

			return entryBuilder.WithArchiveData(sharpCompressEntry).Build();
		}
	}
}
