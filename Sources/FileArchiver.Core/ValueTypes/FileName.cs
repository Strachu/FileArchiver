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
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Linq;

namespace FileArchiver.Core.ValueTypes
{
	/// <summary>
	/// An object representing a name of a file.
	/// </summary>
	/// <remarks>
	/// During comparisons the names are compared in a case insensitive fashion.
	/// </remarks>
	[TypeConverter(typeof(FileNameTypeConverter))]
	public class FileName : IComparable<FileName>, IComparable
	{
		private readonly static char[] ForbiddenCharacters = { '/', '\\' };

		private readonly string mName;

		public FileName(string name)
		{
			Contract.Requires(!String.IsNullOrWhiteSpace(name));
			Contract.Requires(IsValid(name));

			mName = name;
		}

		/// <summary>
		/// Validates whether passed string is a valid name, that is it is not empty nor it contains invalid characters such as path separators.
		/// </summary>
		[Pure]
		public static bool IsValid(string name)
		{
			if(String.IsNullOrWhiteSpace(name))
				return false;

			if(name.Any(character => ForbiddenCharacters.Contains(character)))
				return false;

			return true;
		}

		public override bool Equals(object obj)
		{
			var otherName = obj as FileName;
			if(otherName == null)
				return false;

			return mName.Equals(otherName.mName, StringComparison.OrdinalIgnoreCase);
		}

		public override int GetHashCode()
		{
			return mName.ToLower().GetHashCode();
		}

		public int CompareTo(FileName other)
		{
			if(other == null)
				return 1;

			return String.Compare(mName, other.mName, StringComparison.CurrentCultureIgnoreCase);
		}

		public int CompareTo(object obj)
		{
			if(obj == null)
				return 1;

			var otherName = obj as FileName;
			if(otherName == null)
				throw new ArgumentException("Passed object is not a FileName.");

			return this.CompareTo(otherName);
		}

		public static implicit operator string(FileName name)
		{
			return name.mName;
		}

		public static implicit operator Path(FileName name)
		{
			return new Path(name.mName);
		}

		public override string ToString()
		{
			return mName;
		}
	}
}
