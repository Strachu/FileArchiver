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
using System.Diagnostics.Contracts;

namespace FileArchiver.Presentation.FileListView.Utils
{
	/// <summary>
	/// Class used for converting size in bytes to a string with the biggest unit postfix.
	/// </summary>
	internal static class UserFriendlySizeFormatter
	{
		private static readonly string[] SizeUnitsLookUpTable = { "B", "KiB", "MiB", "GiB" };

		/// <summary>
		/// Formats the specified size.
		/// </summary>
		/// <param name="size">
		/// The size to format.
		/// </param>
		/// <returns>
		/// String representing specified value with biggest unit postfix.
		/// For example:
		/// For 825, the function will return "825 B", for 1100, it will return "1.07 KB",
		/// for 3 gigabytes "3 GB" and so on.
		/// </returns>
		public static string Format(long size)
		{
			Contract.Ensures(Contract.Result<string>() != null);

			int   unitIndex = 0;
			float newSize   = size;

			while(newSize > 1000.0f && unitIndex < SizeUnitsLookUpTable.Length - 1)
			{
				newSize /= 1024.0f;
				unitIndex++;
			}

			return String.Format("{0:G3} {1}", newSize, SizeUnitsLookUpTable[unitIndex]);
		}
	}
}
