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
using System.Linq;
using System.Text.RegularExpressions;

namespace FileArchiver.Core.ValueTypes
{
	/// <summary>
	/// An object representing a file system path.
	/// </summary>
	/// <remarks>
	/// During comparisons the paths are compared in case insensitive fashion and the slashes
	/// are normalized by converting all of them to System.IO.Path.DirectorySeparatorChar.
	/// </remarks>
	public class Path
	{
		/// <summary>
		/// A path representing root directory in an archive.
		/// </summary>
		public static readonly Path Root = new Path(String.Empty);

		private readonly string mPath;

		public Path(string path)
		{
			Contract.Requires(path != null);

			mPath = NormalizeSeparators(path);
		}

		private string NormalizeSeparators(string path)
		{
			var normalizedPath = Regex.Replace(path, @"/|\\", System.IO.Path.DirectorySeparatorChar.ToString()).Trim();
			var finalPath      = normalizedPath.Trim(System.IO.Path.DirectorySeparatorChar);

			if(path.Any() && normalizedPath[0] == System.IO.Path.DirectorySeparatorChar)
			{
				finalPath = '/' + finalPath;
			}

			return finalPath;
		}

		public Path ChangeFileName(FileName newName)
		{
			Contract.Requires(newName != null);

			return ParentDirectory.Combine(newName);
		}

		public FileName FileName
		{
			get { return new FileName(System.IO.Path.GetFileName(mPath)); }
		}

		public Path ParentDirectory
		{
			get
			{
				if(this.Equals(Root))
					return null;

				var parentDirectory = System.IO.Path.GetDirectoryName(mPath);
				if(parentDirectory == null) // null is returned when GetDirectoryName is invoked with a hard disk letter
					return null;

				return new Path(parentDirectory);
			} 
		}

		/// <summary>
		/// Checks whether the path represented by this object can point to a directory
		/// which is an ancestor of a file pointed by given path.
		/// </summary>
		/// <param name="path">
		/// The path to use during checking.
		/// </param>
		/// <returns>
		/// True, if this path is an ancestor of given path, false otherwise.
		/// </returns>
		public bool IsAncestorOf(Path path)
		{
			Contract.Requires(path != null);

			var parent = path;
			while((parent = parent.ParentDirectory) != null)
			{
				if(parent.Equals(this))
				{
					return true;
				}
			}
			return false;
		}

		public Path Combine(Path path)
		{
			Contract.Requires(path != null);

			return new Path(System.IO.Path.Combine(mPath, path));
		}

		/// <summary>
		/// Changes the part of the path as if the file pointed by this path was moved from one directory to another.
		/// </summary>
		/// <param name="currentDirectory">
		/// The directory to move from. This denotes at which point of hierarchy the move should be done.
		/// </param>
		/// <param name="newDirectory">
		/// The new directory.
		/// </param>
		/// <example>
		/// <code>
		/// var path    = new Path("/directory/subdirectory/subdirectory2/file.txt");
		/// var newPath = path.ChangeDirectory(new Path("/directory/subdirectory"), new Path("/directory/newsubdir"));	
		/// 
		/// Assert.That(newPath, Is.EqualTo(new Path("/directory/newsubdir/subdirectory2/file.txt")));
		/// </code>
		/// </example>
		public Path ChangeDirectory(Path currentDirectory, Path newDirectory)
		{
			Contract.Requires(currentDirectory != null);
			Contract.Requires(newDirectory != null);

			var pathWithoutDirectory = !currentDirectory.Equals(Root) ? mPath.Substring(currentDirectory.mPath.Length + 1)
			                                                          : mPath;

			return newDirectory.Combine(new Path(pathWithoutDirectory));
		}

		public Path RemoveExtension()
		{
			var fileNameWithoutExtension = new Path(System.IO.Path.GetFileNameWithoutExtension(mPath));

			return ParentDirectory.Combine(fileNameWithoutExtension);
		}

		/// <summary>
		/// Removes the specified path from the end of current path.
		/// </summary>
		/// <param name="pathToRemove">
		/// The part of path suffix to remove.
		/// </param>
		public Path Remove(Path pathToRemove)
		{
			Contract.Requires(pathToRemove != null);

			var partToRemoveIndex = mPath.IndexOf(pathToRemove.mPath, StringComparison.OrdinalIgnoreCase);
			if(partToRemoveIndex == -1)
				throw new ArgumentException(pathToRemove + " not found in " + mPath);

			return new Path(mPath.Remove(partToRemoveIndex, pathToRemove.mPath.Length));
		}

		public override bool Equals(object obj)
		{
			var otherPath = obj as Path;
			if(otherPath == null)
				return false;

			return mPath.Equals(otherPath.mPath, StringComparison.OrdinalIgnoreCase);
		}

		public override int GetHashCode()
		{
			return mPath.ToLower().GetHashCode();
		}

		public static implicit operator string(Path path)
		{
			return path.mPath;
		}

		public override string ToString()
		{
			return mPath;
		}
	}
}
