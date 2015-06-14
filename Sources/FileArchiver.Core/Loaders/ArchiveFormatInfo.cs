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

namespace FileArchiver.Core.Loaders
{
	// TODO maybe make it as a metadata for use with MEF?
	/// <summary>
	/// Metadata about an archive format.
	/// </summary>
	public class ArchiveFormatInfo
	{
		public ArchiveFormatInfo(string extension, string localizedDescription, bool supportsCompression)
		{
			Contract.Requires(!String.IsNullOrWhiteSpace(extension));
			Contract.Requires(extension.StartsWith("."));
			Contract.Requires(!String.IsNullOrWhiteSpace(localizedDescription));

			Extension            = extension;
			LocalizedDescription = localizedDescription;
			SupportsCompression  = supportsCompression;
		}

		/// <summary>
		/// The extension used by files in this archive format.
		/// </summary>
		/// <remarks>
		/// Returned string has to include leading dot.
		/// </remarks>
		public string Extension
		{
			get;
			private set;
		}

		/// <summary>
		/// Localized description of this archive format such as "Zip archive" for displaying purposes.
		/// </summary>
		public string LocalizedDescription
		{
			get;
			private set;
		}

		/// <summary>
		/// Indicates whether the archive supports compression.
		/// </summary>
		/// <remarks>
		/// It is used to decide whether there is a benefit in wrapping the archive in this format inside another archive which supports compression.
		/// For example, the descriptor for TAR archive should return false to allow it to be embedded into a GZip stream.
		/// </remarks>
		public bool SupportsCompression
		{
			get;
			private set;
		}
	}
}
