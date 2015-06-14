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

using Path = FileArchiver.Core.ValueTypes.Path;

namespace FileArchiver.Core.Services
{
	[ContractClass(typeof(IArchiveExtractionServiceContractClass))]
	public interface IArchiveExtractionService
	{
		/// <summary>
		/// Extracts the entire archive asynchronously.
		/// </summary>
		/// <param name="archivePath">
		/// The path of archive to extract.
		/// </param>
		/// <param name="errorHandler">
		/// A handler which will be able to handle exceptions per file without aborting the extraction.
		/// Every exception which is listed in the of exceptions can be handler with this handler.
		/// The most common situation to handle is when destination file already exists.
		/// </param>
		/// <param name="cancelToken">
		/// The cancel token.
		/// </param>
		/// <param name="progress">
		/// The progress.
		/// The progress value represents the percentage of work already done in the range [0.0, 1.0] or
		/// null if the implementation cannot reliably determine the progress.
		/// </param>
		/// <exception cref="OperationCanceledException">
		/// The extraction has been canceled.
		/// </exception>
		/// <exception cref="FileExistsException">
		/// Some file already exists in destination path.
		/// </exception>
		/// <exception cref="UnauthorizedAccessException">
		/// The parent directory of the archive is read only.
		/// </exception>
		/// <remarks>
		/// The extracted files will be placed inside a subdirectory of directory in which the archives are located.
		/// </remarks>
		Task ExtractArchiveAsync(Path archivePath,
		                         FileExtractionErrorHandler errorHandler,
		                         CancellationToken cancelToken,
		                         IProgress<double?> progress = null);
	}
}