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

using FileArchiver.Core.ValueTypes;

namespace FileArchiver.Presentation.ArchiveSettings
{
	// TODO Maybe just a INewArchivePresenter returning newly created archive instead of just archive settings??
	/// <summary>
	/// A screen allowing the user to enter properties of archive to be created such as its format, location and format specific options.
	/// </summary>
	public interface INewArchiveSettingsScreen
	{
		/// <summary>
		/// Shows the screen.
		/// </summary>
		/// <param name="defaultDestinationPath">
		/// The default destination path of archive without the extension or null if no default shown be set.
		/// </param>
		/// <param name="allowSingleFileArchives">
		/// Controls whether the user will be able to choose an archive type which can contain only single file.
		/// </param>
		/// <returns>
		/// Settings accepted by the user or <b>null</b> if the user decided to cancel.
		/// </returns>
		NewArchiveSettings Show(Path defaultDestinationPath = null, bool allowSingleFileArchives = true);
	}
}
