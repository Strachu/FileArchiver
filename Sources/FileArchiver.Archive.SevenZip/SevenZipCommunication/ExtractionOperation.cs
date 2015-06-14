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

using System.Text.RegularExpressions;

namespace FileArchiver.Archive.SevenZip.SevenZipCommunication
{
	internal enum Action
	{
		Extracting,
		Skipping
	}

	internal class ExtractionOperationLine
	{
		private ExtractionOperationLine(Action action, string fileId)
		{
			Action = action;
			FileId = fileId;
		}

		public static ExtractionOperationLine Parse(string line)
		{
			var operationAndFilePair = RemoveExcessSpaces(line).Split(new char[] { ' ' }, 2);

			return new ExtractionOperationLine
			(
				action: operationAndFilePair[0] == "Skipping" ? Action.Skipping : Action.Extracting,
				fileId: operationAndFilePair[1]
			);
		}

		private static string RemoveExcessSpaces(string line)
		{
			return Regex.Replace(line, " +", " ");
		}

		public Action Action
		{
			get;
			private set;
		}

		public string FileId
		{
			get;
			private set;
		}
	}
}
