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
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using FileArchiver.Core.Archive;
using FileArchiver.Core.Loaders;

using Path = FileArchiver.Core.ValueTypes.Path;

namespace FileArchiver.Core.Services
{
	internal class ArchiveExtractionService : IArchiveExtractionService
	{
		private readonly IArchiveLoadingService mLoadingService;

		public ArchiveExtractionService(IArchiveLoadingService loadingService)
		{
			Contract.Requires(loadingService != null);

			mLoadingService = loadingService;
		}

		public async Task ExtractArchiveAsync(Path archivePath,
		                                      FileExtractionErrorHandler errorHandler,
		                                      CancellationToken cancelToken,
		                                      IProgress<double?> progress)
		{
			var archive  = await mLoadingService.LoadAsync(archivePath, cancelToken, progress);
			var allFiles = archive.RootFiles.Select(file => file.Path).ToList();

			var destinationDirectory = archivePath.ParentDirectory;

			if(allFiles.Count() > 1)
			{
				destinationDirectory = destinationDirectory.Combine(archivePath.FileName).RemoveExtension();
			}

			await archive.ExtractFilesAsync(destinationDirectory, allFiles, errorHandler, cancelToken, progress);
		}
	}
}
