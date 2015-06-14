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
using System.Text.RegularExpressions;

using FileArchiver.Core.ValueTypes;

using Path = FileArchiver.Core.ValueTypes.Path;

namespace FileArchiver.Core.Utils.File
{
	public static class PathUtil
	{
		/// <summary>
		/// Gets the common parent directory for specified files.
		/// </summary>
		/// <remarks>
		/// For example, for files "C:/Dir/Dir/file.txt", "C:/Dir/Dir/a.txt", "C:/Dir/OtherDir/file"
		/// the function will return: "C:/Dir".
		/// </remarks>
		/// <param name="paths">
		/// The paths to get common directory for.
		/// </param>
		/// <returns>
		/// The path of common directory for all files or null if there is no common directory.
		/// </returns>
		public static Path GetParentDirectory(IReadOnlyCollection<Path> paths)
		{
			Contract.Requires(paths != null);
			Contract.Requires(paths.Any());
			Contract.Requires(Contract.ForAll(paths, path => path != null));

			var prefix = paths.First().ToString();
			foreach(var path in paths)
			{
				var prefixLength = GetPrefixLength(prefix, path);
				if(prefixLength < prefix.Length)
				{
					prefix = prefix.Substring(0, prefixLength);
				}
			}

			if(prefix == String.Empty)
				return new Path(String.Empty);

			var root = System.IO.Path.GetDirectoryName(prefix);

			// GetDirectoryName returns null when prefix is a file system root
			if(root == null)
				return new Path(prefix);

			return new Path(root);
		}

		private static int GetPrefixLength(string first, string second)
		{
			return (first.Length <= second.Length) ? first.TakeWhile((character, index) => character == second[index]).Count()
			                                       : second.TakeWhile((character, index) => character == first[index]).Count();
		}

		/// <summary>
		/// Returns path to a file with random name guaranteed to be unique in specified directory.
		/// </summary>
		/// <param name="parentDirectory">
		/// The parent directory for the file.
		/// </param>
		public static Path GetUniquePath(Path parentDirectory)
		{
			Contract.Requires(parentDirectory != null);

			while(true)
			{
				var path = parentDirectory.Combine(new FileName(System.IO.Path.GetRandomFileName()));

				if(!Directory.Exists(path) && !System.IO.File.Exists(path))
				{
					return path;
				}
			}
		}
	}
}
