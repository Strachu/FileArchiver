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

using Path = FileArchiver.Core.ValueTypes.Path;

namespace FileArchiver.Core.Services
{
	public class NumberedFileNameGenerator : IFileNameGenerator
	{
		public Path GenerateFreeFileName(Path originalFileName, Predicate<Path> fileExistsPredicate)
		{
			if(!fileExistsPredicate(originalFileName))
				return originalFileName;

			string generatedName;

			var fileDirectory = System.IO.Path.GetDirectoryName(originalFileName);
			var fileName      = System.IO.Path.GetFileNameWithoutExtension(originalFileName);
			var fileExtension = System.IO.Path.GetExtension(originalFileName);

			int number = 2;
			do
			{
				generatedName = System.IO.Path.Combine(fileDirectory, fileName + " - " + number + fileExtension);
				number++;
			}
			while(fileExistsPredicate(new Path(generatedName)));

			return new Path(generatedName);
		}
	}
}
