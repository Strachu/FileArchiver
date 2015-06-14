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
using System.Threading;
using System.Threading.Tasks;

using FileArchiver.Core.Archive;
using FileArchiver.Core.ValueTypes;

namespace FileArchiver.Core.Loaders
{
	[ContractClassFor(typeof(IArchiveFormatLoader))]
	internal abstract class IArchiveFormatLoaderContractClass : IArchiveFormatLoader
	{
		public ArchiveFormatInfo ArchiveFormatInfo
		{
			get
			{
				Contract.Ensures(Contract.Result<ArchiveFormatInfo>() != null);

				throw new NotImplementedException();
			}
		}

		IArchive IArchiveFormatLoader.CreateNew(Path archivePath, object settings)
		{
			Contract.Requires(archivePath != null);
			Contract.Ensures(Contract.Result<IArchive>() != null);

			throw new NotImplementedException();
		}

		bool IArchiveFormatLoader.IsSupportedArchive(Path path)
		{
			Contract.Requires(path != null);

			throw new NotImplementedException();
		}

		Task<IArchive> IArchiveFormatLoader.LoadAsync(Path path, CancellationToken cancelToken, IProgress<double?> progress)
		{
			Contract.Requires(path != null);
			Contract.Ensures(Contract.Result<Task<IArchive>>().Result != null);

			throw new NotImplementedException();
		}
	}
}
