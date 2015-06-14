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
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using FileArchiver.Core.ValueTypes;

namespace FileArchiver.Core.Archive
{
	[ContractClassFor(typeof(IArchive))]
	internal abstract class IArchiveContractClass : IArchive
	{
		void IArchive.AddFile(Path destinationDirectoryPath, FileEntry newFile)
		{
			Contract.Requires(destinationDirectoryPath != null);
			Contract.Requires(newFile != null);
		}

		bool IArchive.FileExists(Path path)
		{
			Contract.Requires(path != null);

			throw new NotImplementedException();
		}

		bool IArchive.FileExists(Guid fileId)
		{
			throw new NotImplementedException();
		}

		FileEntry IArchive.GetFile(Path path)
		{
			Contract.Requires(path != null);
			Contract.Ensures(Contract.Result<FileEntry>() != null);

			throw new NotImplementedException();
		}

		FileEntry IArchive.GetFile(Guid fileId)
		{
			Contract.Ensures(Contract.Result<FileEntry>() != null);

			throw new NotImplementedException();
		}

		void IArchive.RemoveFile(Path fileToRemove)
		{
			Contract.Requires(fileToRemove != null);

			throw new NotImplementedException();
		}

		IEnumerable<FileEntry> IArchive.RootFiles
		{
			get
			{
				Contract.Ensures(Contract.Result<IEnumerable<FileEntry>>() != null);

				throw new NotImplementedException();
			}
		}

		bool IArchive.IsModified
		{
			get { throw new NotImplementedException(); }
		}

		Task IArchive.ExtractFilesAsync(IReadOnlyCollection<SourceDestinationPathPair> fileAndDestinationPathPairs,
		                                FileExtractionErrorHandler errorHandler,
		                                CancellationToken cancelToken,
		                                IProgress<double?> progress)
		{
			Contract.Requires(fileAndDestinationPathPairs != null);
			Contract.Requires(Contract.ForAll(fileAndDestinationPathPairs, pair => pair != null));
			Contract.Requires(errorHandler != null);

			throw new NotImplementedException();
		}

		Task IArchive.SaveAsync(CancellationToken cancelToken, IProgress<double?> progress)
		{
			throw new NotImplementedException();
		}

		void IArchive.Close()
		{
			throw new NotImplementedException();
		}

		event EventHandler<FileAddedEventArgs> IArchive.FileAdded
		{
			add { throw new NotImplementedException(); }
			remove { throw new NotImplementedException(); }
		}

		event EventHandler<FileRemovedEventArgs> IArchive.FileRemoved
		{
			add { throw new NotImplementedException(); }
			remove { throw new NotImplementedException(); }
		}

		void IDisposable.Dispose()
		{
			throw new NotImplementedException();
		}
	}
}
