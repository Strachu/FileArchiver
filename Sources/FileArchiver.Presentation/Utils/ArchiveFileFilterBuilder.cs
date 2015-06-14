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
using System.Text;

using FileArchiver.Core.Loaders;
using FileArchiver.Presentation.Properties;

namespace FileArchiver.Presentation.Utils
{
	/// <summary>
	/// An utility class which builds file filter for supported archives extensions for use with
	/// OpenFileDialog in Windows Forms.
	/// </summary>
	internal static class ArchiveFileFilterBuilder
	{
		/// <summary>
		/// Builds the file filter for specified supported archive list.
		/// </summary>
		/// <param name="supportedArchives">
		/// The supported archives list.
		/// </param>
		public static string BuildFilter(IList<ArchiveFormatInfo> supportedArchives)
		{
			var filterBuilder = new StringBuilder();

			filterBuilder.AppendSupportedArchivesFilter(supportedArchives);
			filterBuilder.AppendAllSingleExtensionFilters(supportedArchives);
			filterBuilder.AppendAllFilesFilter();

			return filterBuilder.ToString();
		}

		private static void AppendSupportedArchivesFilter(this StringBuilder filterBuilder, IEnumerable<ArchiveFormatInfo> supportedArchives)
		{
			filterBuilder.Append(Resources.Filter_SupportedArchives);
			filterBuilder.Append("|");
			filterBuilder.AppendAllExtensions(supportedArchives);
			filterBuilder.Append("|");
		}

		private static void AppendAllSingleExtensionFilters(this StringBuilder filterBuilder, IEnumerable<ArchiveFormatInfo> supportedArchives)
		{
			foreach(var archive in supportedArchives)
			{
				filterBuilder.Append(archive.LocalizedDescription);
				filterBuilder.Append("|");
				filterBuilder.Append("*");
				filterBuilder.Append(archive.Extension);

				filterBuilder.Append("|");
			}
		}

		private static void AppendAllFilesFilter(this StringBuilder filterBuilder)
		{
			filterBuilder.Append(Resources.Filter_AllFiles + "|*.*");
		}

		private static void AppendAllExtensions(this StringBuilder filterBuilder, IEnumerable<ArchiveFormatInfo> supportedArchives)
		{
			foreach(var archive in supportedArchives)
			{
				filterBuilder.Append("*");
				filterBuilder.Append(archive.Extension);
				filterBuilder.Append(";");
			}

			filterBuilder.Remove(filterBuilder.Length - 1, 1);
		}
	}
}
