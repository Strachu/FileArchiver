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
using System.Threading;
using System.Threading.Tasks;
using FileArchiver.Core.Archive;
using FileArchiver.Core.ValueTypes;

namespace FileArchiver.Core.Loaders
{
	[ContractClassFor(typeof(IArchiveLoadingService))]
	internal abstract class IArchiveLoadingServiceContractClass : IArchiveLoadingService
	{
		IArchive IArchiveLoadingService.CreateNew(Path archivePath, IReadOnlyCollection<ArchiveFormatWithSettings> formats)
		{
			Contract.Requires(archivePath != null);
			Contract.Requires(formats != null);
			Contract.Requires(Contract.ForAll(formats, format => format != null));
			Contract.Ensures(Contract.Result<IArchive>() != null);

			throw new NotImplementedException();
		}

		Task<IArchive> IArchiveLoadingService.LoadAsync(Path path, CancellationToken cancelToken, IProgress<double?> progress)
		{
			Contract.Requires(path != null);

			throw new NotImplementedException();
		}

		IEnumerable<ArchiveFormatInfo> IArchiveLoadingService.SupportedFormats
		{
			get
			{
				Contract.Ensures(Contract.Result<IEnumerable<ArchiveFormatInfo>>() != null);
				Contract.Ensures(Contract.ForAll(Contract.Result<IEnumerable<ArchiveFormatInfo>>(), info => info != null));

				throw new NotImplementedException();
			}
		}
	}
}
