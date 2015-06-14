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
using System.Globalization;
using System.Linq;

using FileArchiver.Core.Archive;
using FileArchiver.Core.ValueTypes;

namespace FileArchiver.Archive.SevenZip.SevenZipCommunication
{
	internal static class EntryParser
	{
		public static FileEntry ParseEntry(IDictionary<string, string> entryProperties)
		{
			Contract.Requires(entryProperties != null);

			var entryBuilder = new FileEntry.Builder();

			entryBuilder.WithName(new Path(entryProperties["Path"]).FileName)
			            .WithSize(Int32.Parse(entryProperties["Size"], CultureInfo.InvariantCulture))
			            .ModifiedOn(DateTime.Parse(entryProperties["Modified"]));

			if(entryProperties["Attributes"].First() == 'D')
			{
				entryBuilder.AsDirectory();
			}

			var customData = new SevenZipEntryData
			(
				fileId          : entryProperties["Path"],
				solidBlockIndex : (!String.IsNullOrWhiteSpace(entryProperties["Block"])) ? Int32.Parse(entryProperties["Block"])
				                                                                         : (int?)null				
			);

			entryBuilder.WithArchiveData(customData);

			return entryBuilder.Build();
		}
	}
}
