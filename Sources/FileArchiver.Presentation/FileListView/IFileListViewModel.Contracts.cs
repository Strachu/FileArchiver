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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FileArchiver.Core;
using FileArchiver.Core.Archive;
using FileArchiver.Core.ValueTypes;

namespace FileArchiver.Presentation.FileListView
{
	[ContractClassFor(typeof(IFileListViewModel))]
	internal abstract class IFileListViewModelContractClass : IFileListViewModel
	{
		IArchive IFileListViewModel.Archive
		{
			get
			{
				Contract.Ensures(Contract.Result<IArchive>() != null);

				throw new NotImplementedException();
			}
		}

		Path IFileListViewModel.ArchivePath
		{
			get
			{
				Contract.Ensures(Contract.Result<Path>() != null);

				throw new NotImplementedException();
			}
		}

		BindingList<FileEntryViewModel> IFileListViewModel.FilesInCurrentDirectory
		{
			get
			{
				Contract.Ensures(Contract.Result<BindingList<FileEntryViewModel>>() != null);
				Contract.Ensures(Contract.ForAll(Contract.Result<BindingList<FileEntryViewModel>>(), viewModel => viewModel != null));

				throw new NotImplementedException();
			}
		}

		Path IFileListViewModel.CurrentDirectory
		{
			get
			{
				Contract.Ensures(Contract.Result<Path>() != null);

				throw new NotImplementedException();
			}
		}

		Path IFileListViewModel.CurrentDirectoryFullAddress
		{
			get
			{
				Contract.Ensures(Contract.Result<Path>() != null);

				throw new NotImplementedException();
			}
		}

		bool IFileListViewModel.NavigateToParentDirectoryEnabled
		{
			get { throw new NotImplementedException(); }
		}

		bool IFileListViewModel.AddFilesEnabled
		{
			get { throw new NotImplementedException(); }
		}

		int IFileListViewModel.FirstDisplayedFileIndex
		{
			get
			{
				Contract.Ensures(Contract.Result<int>() >= 0);

				throw new NotImplementedException();
			}
			set
			{
				Contract.Requires(value >= 0);

				throw new NotImplementedException();
			}
		}

		void IFileListViewModel.SetArchive(IArchive archive, Path archivePath)
		{
			Contract.Requires(archive != null);
			Contract.Requires(archivePath != null);
		}

		void IFileListViewModel.Open(FileName fileName)
		{
			Contract.Requires(fileName != null);
		}

		void IFileListViewModel.NavigateToParentDirectory()
		{
			throw new NotImplementedException();
		}

		void IFileListViewModel.AddFiles(IReadOnlyList<Path> files)
		{
			Contract.Requires(files != null);
			Contract.Requires(Contract.ForAll(files, file => file != null));
		}

		event EventHandler<FileOpenRequestEventArgs> IFileListViewModel.FileOpeningRequested
		{
			add { throw new NotImplementedException(); }
			remove { throw new NotImplementedException(); }
		}

		event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
		{
			add { throw new NotImplementedException(); }
			remove { throw new NotImplementedException(); }
		}
	}
}
