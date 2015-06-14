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

using System.Collections.Generic;
using System.Diagnostics.Contracts;

using FileArchiver.Core.Loaders;
using FileArchiver.Core.ValueTypes;

namespace FileArchiver.Presentation.OtherViews
{
	public enum SaveChangesAction
	{
		Save,
		Discard,
		Cancel
	}

	[ContractClass(typeof(IDialogLauncherContractClass))]
	public interface IDialogLauncher
	{
		// TODO generalize the methods by naming it ShowOpenFileDialog() and taking the filter as parameter?
		//      The filter string can be specific to a UI technology, so instead of a string a custom class should
		//      be passed, similar to ArchiveFormatInfo but generalized.

		/// <summary>
		/// Asks for path to archive which should be opened.
		/// </summary>
		/// <param name="supportedArchives">
		/// The info about supported archive types.
		/// </param>
		/// <returns>
		/// The chosen path or null if the opening should be canceled.
		/// </returns>
		Path AskForArchiveOpenPath(IEnumerable<ArchiveFormatInfo> supportedArchives);

		/// <summary>
		/// Asks for a path to directory to which files will be extracted.
		/// </summary>
		/// <returns>
		/// The chosen path or null if the extraction should be canceled.
		/// </returns>		
		Path AskForDestinationDirectoryForExtraction();

		/// <summary>
		/// Asks for paths to files which should be added to the archive.
		/// </summary>
		/// <returns>
		/// The chosen paths or null if the adding should be canceled.
		/// </returns>	
		IEnumerable<Path> AskForFilesToAdd();

		SaveChangesAction AskForSaveChangesAction(FileName archiveName);

		void DisplayError(string message);
	}
}
