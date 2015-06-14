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

namespace FileArchiver.Core.Archive
{
	/// <summary>
	/// A class representing the pair of source and destination path.
	/// </summary>
	public class SourceDestinationPathPair
	{
		public SourceDestinationPathPair(Path sourcePath, Path destinationPath)
		{
			Contract.Requires(sourcePath != null);
			Contract.Requires(destinationPath != null);

			SourcePath      = sourcePath;
			DestinationPath = destinationPath;
		}

		public Path SourcePath
		{
			get;
			private set;
		}

		public Path DestinationPath
		{
			get;
			private set;
		}
	}
}
