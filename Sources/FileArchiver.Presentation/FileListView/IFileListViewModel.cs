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
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Contracts;

using FileArchiver.Core;
using FileArchiver.Core.Archive;
using FileArchiver.Core.ValueTypes;

namespace FileArchiver.Presentation.FileListView
{
	[ContractClass(typeof(IFileListViewModelContractClass))]
	public interface IFileListViewModel : INotifyPropertyChanged
	{
		IArchive Archive
		{
			get;
		}

		Path ArchivePath
		{
			get;
		}

		BindingList<FileEntryViewModel> FilesInCurrentDirectory
		{
			get;
		}

		Path CurrentDirectory
		{
			get;
		}

		Path CurrentDirectoryFullAddress
		{
			get;
		}

		bool NavigateToParentDirectoryEnabled
		{
			get;
		}

		bool AddFilesEnabled
		{
			get;
		}

		/// <summary>
		/// Controls the current scroll position of the view by determining which entry is the first visible one.
		/// </summary>
		int FirstDisplayedFileIndex
		{
			get;
			set;
		}

		void SetArchive(IArchive archive, Path archivePath);

		/// <summary>
		/// Opens a file or directory in current directory.
		/// </summary>
		/// <param name="fileName">
		/// Name of the file or directory to open.
		/// </param>
		/// <remarks>
		/// If the name points to a directory, the view model will navigate to it.
		/// Otherwise the file will be opened in external application.
		/// </remarks>
		void Open(FileName fileName);

		void NavigateToParentDirectory();

		void AddFiles(IReadOnlyList<Path> files);

		event EventHandler<FileOpenRequestEventArgs> FileOpeningRequested;
		event EventHandler<ErrorEventArgs>           ErrorOccured;
	}
}