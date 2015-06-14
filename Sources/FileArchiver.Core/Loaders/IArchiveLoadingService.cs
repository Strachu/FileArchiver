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
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using FileArchiver.Core.Archive;

using Path = FileArchiver.Core.ValueTypes.Path;

namespace FileArchiver.Core.Loaders
{
	[ContractClass(typeof(IArchiveLoadingServiceContractClass))]
	public interface IArchiveLoadingService
	{
		/// <summary>
		/// Creates new archive at specified path
		/// </summary>
		/// <param name="archivePath">
		/// The path where the archive will be saved.
		/// </param>
		/// <param name="formats">
		/// The list of formats. If more than 1 entry will be specified the function will create multiple archives
		/// each one nested in another. The order in the case of nesting is that the first entry will be the most
		/// nested. For example, if you pass there a collection where 0th element is equal to tar and 1st to gz, the
		/// function will create a tar archive compressed with GZip stream.
		/// </param>
		/// <exception cref="ArgumentException">
		/// Passed invalid type of settings object to some format.
		/// </exception>
		/// <returns>
		/// New archive.
		/// </returns>
		IArchive CreateNew(Path archivePath, IReadOnlyCollection<ArchiveFormatWithSettings> formats);

		/// <summary>
		/// Loads the archive from specified path.
		/// </summary>
		/// <param name="path">
		/// The path of archive.
		/// </param>
		/// <param name="cancelToken">
		/// A token which indicates whether the loading should be canceled.
		/// </param>
		/// <param name="progress">
		/// An object indicating the progress of the loading.
		/// </param>
		/// <returns>
		/// A newly loaded archive.
		/// </returns>
		/// <exception cref="OperationCanceledException">
		/// The loading has been canceled.
		/// </exception>
		/// <exception cref="NotSupportedFormatException">
		/// The file is in format which is not supported.
		/// </exception>
		/// <exception cref="IOException">
		/// An error occurred during the loading of an archive.
		/// </exception>
		Task<IArchive> LoadAsync(Path path, CancellationToken cancelToken, IProgress<double?> progress = null);

		IEnumerable<ArchiveFormatInfo> SupportedFormats { get; }
	}
}
