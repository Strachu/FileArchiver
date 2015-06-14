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
using System.IO;
using System.Threading;
using System.Threading.Tasks;

using FileArchiver.Core.Archive;

using Path = FileArchiver.Core.ValueTypes.Path;

namespace FileArchiver.Core.Loaders
{
	/// <summary>
	/// An interface for objects handling the loading of single archive format.
	/// </summary>
	[ContractClass(typeof(IArchiveFormatLoaderContractClass))]
	public interface IArchiveFormatLoader
	{
		/// <summary>
		/// Gets the information about handled archive format.
		/// </summary>
		ArchiveFormatInfo ArchiveFormatInfo { get; }

		/// <summary>
		/// Creates a new archive in format handled by this loader.
		/// </summary>
		/// <param name="archivePath">
		/// The path at which the archive will be created.
		/// </param>
		/// <param name="settings">
		/// The format specific settings of archive.
		/// </param>
		/// <exception cref="ArgumentException">
		/// Invalid type of settings object.
		/// </exception>
		IArchive CreateNew(Path archivePath, object settings);

		/// <summary>
		/// Determines whether the file represented by <c>path</c> is an archive which can be loaded by this loader.
		/// </summary>
		/// <param name="path">
		/// The path to a file which should be checked whether it can be opened by this loader.
		/// </param>
		bool IsSupportedArchive(Path path);

		/// <summary>
		/// Loads the archive located at specified path.
		/// </summary>
		/// <param name="path">
		/// The path of archive.
		/// </param>
		/// <param name="cancelToken">
		/// A token which indicates whether the loading should be canceled.
		/// </param>
		/// <param name="progress">
		/// An object indicating the progress of the loading.
		/// The progress value represents the percentage of work already done in the range [0.0, 1.0] or
		/// null if the implementation cannot reliably determine the progress.
		/// </param>
		/// <returns>
		/// The archive.
		/// </returns>
		/// <exception cref="IOException">
		/// An error occurred during the loading of the archive.
		/// </exception>
		/// <exception cref="OperationCanceledException">
		/// The operation has been canceled.
		/// </exception>
		Task<IArchive> LoadAsync(Path path, CancellationToken cancelToken, IProgress<double?> progress);
	}
}
