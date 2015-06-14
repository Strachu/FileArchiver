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

using FileArchiver.Core;
using FileArchiver.Core.Utils;
using FileArchiver.Core.ValueTypes;

namespace FileArchiver.Presentation.PerFileErrorHandlers
{
	public enum FileExistsAction
	{
		Overwrite,
		Rename,
		Skip,
		Abort
	}

	public interface IPerFileErrorView
	{
		/// <summary>
		/// Shows an dialog box asking the user whether to retry, ignore or abort failed action.
		/// </summary>
		/// <param name="errorMessage">
		/// The error message to display to user.
		/// </param>
		/// <returns>
		/// The chosen action.
		/// </returns>
		RetryAction AskForRetryAction(string errorMessage);

		/// <summary>
		/// Shows an dialog box asking the user what to do in the case of file name collision.
		/// </summary>
		/// <param name="filePath">
		/// The path of already existing file.
		/// </param>
		/// <param name="renameCandidate">
		/// Proposed new name for the file.
		/// </param>
		/// <param name="applyForAll">
		/// Indicates whether chosen Action should apply to all future collisions.
		/// </param>
		/// <returns>
		/// The chosen action.
		/// </returns>
		FileExistsAction AskForFileExistsAction(Path filePath, Path renameCandidate, out bool applyForAll);
	}
}
