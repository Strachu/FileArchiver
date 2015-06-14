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

namespace FileArchiver.Core
{
	public class FileExistsException : ArgumentException
	{
		public FileExistsException(Path filePath, Exception innerException = null) : base("File " + filePath + " already exists.", innerException)
		{
			Contract.Requires(filePath != null);

			FilePath = filePath;
		}

		public Path FilePath
		{
			get;
			private set;
		}
	}
}
