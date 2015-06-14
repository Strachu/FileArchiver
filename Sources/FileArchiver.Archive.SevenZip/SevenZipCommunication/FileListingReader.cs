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
using System.IO;
using System.Linq;

namespace FileArchiver.Archive.SevenZip.SevenZipCommunication
{
	/// <summary>
	/// A class used for reading file listings in the format returned
	/// by the invocation "7zr.exe l -slt -t7z archive" from 7-zip.
	/// </summary>
	/// <remarks>
	/// Sample listing can be found with the unit tests of this class.
	/// </remarks>
	internal class FileListingReader
	{
		private const string HeaderSeparator = "--";

		private readonly TextReader mSource;
		private bool                mHeaderRead = false;

		/// <summary>
		/// Initializes a new instance of the <see cref="FileListingReader"/> class.
		/// </summary>
		/// <param name="source">
		/// The reader with file listings in 7-zip format.
		/// </param>
		public FileListingReader(TextReader source)
		{
			Contract.Requires(source != null);

			mSource = source;
		}

		/// <summary>
		/// Reads the archive properties.
		/// </summary>
		/// <returns>
		/// Dictionary with properties of the archive.
		/// For the list of properties see sample listing located with unit tests.
		/// </returns>
		/// <exception cref="EndOfStreamException">
		/// When the stream has ended prematurely.
		/// </exception>
		/// <exception cref="IOException">
		/// When an I/O occurs in underlying text reader.
		/// </exception>
		public IDictionary<string, string> ReadArchiveProperties()
		{
			SkipHeader();

			var header = ReadNextBlock();

			SkipSeparatorAfterArchiveProperties();

			mHeaderRead = true;

			return header;
		}
		
		private void SkipSeparatorAfterArchiveProperties()
		{
			mSource.ReadLine();
		}

		/// <summary>
		/// Reads the properties of every file entry.
		/// </summary>
		/// <returns>
		/// The collection of dictionary with properties for each file entry.
		/// For the list of properties see sample listing located with unit tests.
		/// <remarks>
		/// The entries are read lazily.
		/// </remarks>
		/// </returns>
		/// <exception cref="EndOfStreamException">
		/// When the stream has ended prematurely.
		/// </exception>
		/// <exception cref="IOException">
		/// When an I/O occurs in underlying text reader.
		/// </exception>
		public IEnumerable<IDictionary<string, string>> ReadEntries()
		{
			if(!mHeaderRead)
			{
				ReadArchiveProperties();
			}

			while(true)
			{
				var nextBlockProperties = ReadNextBlock();

				if(!nextBlockProperties.Any())
					yield break;

				yield return nextBlockProperties;
			}
		}

		/// <summary>
		/// Skips the unneeded header located before archive properties.
		/// </summary>
		private void SkipHeader()
		{
			string line;
			do
			{
				line = mSource.ReadLine();

				if(line == null)
				{
					throw new EndOfStreamException();
				}
			}
			while(line != HeaderSeparator);
		}

		/// <summary>
		/// Reads next block of properties.
		/// </summary>
		/// <returns>
		/// Dictionary containing properties stored in just read block.
		/// </returns>
		/// <remarks>
		/// Blocks are separated by one empty line.
		/// </remarks>
		private IDictionary<string, string> ReadNextBlock()
		{
			var properties = new Dictionary<string, string>();

			while(true)
			{
				var line = mSource.ReadLine();

				if(line == String.Empty || (line == null && !properties.Any()))
				{
					break;
				}

				if(line == null)
				{
					throw new EndOfStreamException();
				}

				ParseLine(line, properties);
			}

			return properties;
		}

		private void ParseLine(string line, IDictionary<string, string> result)
		{
			var keyValueArray = line.Split('=');

			if(keyValueArray.Length != 2)
				throw new FormatException("Invalid format of an file listing. Has the format changed?");

			var key   = keyValueArray[0].Trim();
			var value = keyValueArray[1].Trim();

			result[key] = value;
		}
	}
}
