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

using FileArchiver.Core.ValueTypes;

namespace FileArchiver.Core.Services
{
	/// <summary>
	/// A generator of new names for a file which ensures that generated is unique.
	/// </summary>
	[ContractClass(typeof(IFileNameGeneratorContractClass))]
	public interface IFileNameGenerator
	{
		/// <summary>
		/// Generates a new free name for the file.
		/// </summary>
		/// <param name="originalFileName">
		/// The path with original name of the file.
		/// </param>
		/// <param name="fileExistsPredicate">
		/// The predicate which checks whether file with specified path already exists.
		/// </param>
		/// <returns>
		/// New path with unique name for the file.
		/// </returns>
		Path GenerateFreeFileName(Path originalFileName, Predicate<Path> fileExistsPredicate);
	}
}
