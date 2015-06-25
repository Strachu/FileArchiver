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

using Path = FileArchiver.Core.ValueTypes.Path;

namespace FileArchiver.Core.Archive
{
	[ContractClass(typeof(IArchiveContractClass))]
	public interface IArchive : IDisposable
	{
		/// <summary>
		/// Indicates whether this archive supports containing more than one file.
		/// </summary>
		bool SupportsMultipleFiles
		{
			get;
		}

		/// <summary>
		/// Adds a file under specified directory.
		/// </summary>
		/// <param name="destinationDirectoryPath">
		/// The path of directory to place the file in.
		/// </param>
		/// <param name="newFile">
		/// The file to add.
		/// </param>
		/// <exception cref="FileNotFoundException">
		/// When there is no file with path specified as destination directory.
		/// </exception>
		/// <exception cref="ArgumentException">
		/// When file represented by destination path is not a directory.
		/// </exception>
		/// <exception cref="FileExistsException">
		/// Thrown when the with the same name already exists in destination directory.
		/// </exception>
		void AddFile(Path destinationDirectoryPath, FileEntry newFile);

		bool FileExists(Path path);
		bool FileExists(Guid fileId);

		FileEntry GetFile(Path path);
		FileEntry GetFile(Guid fileId);

		/// <summary>
		/// Removes a file and all its children in the case of a directory.
		/// </summary>
		/// <param name="fileToRemove">
		/// The path of file to remove.
		/// </param>
		void RemoveFile(Path fileToRemove);

		/// <summary>
		/// The list of files in the root directory.
		/// </summary>
		IEnumerable<FileEntry> RootFiles
		{
			get;
		}

		/// <summary>
		/// Indicates whether the archive has been modified.
		/// </summary>
		/// <remarks>
		/// Archive is marked as modified if any of its files has been changed.
		/// </remarks>
		bool IsModified
		{
			get;
		}

		/// <summary>
		/// Extracts the files from archive.
		/// </summary>
		/// <param name="fileAndDestinationPathPairs">
		/// A collection of pairs representing the path of file to extract along with its destination.
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
		/// <exception cref="FileNotFoundException">
		/// Some file which has been requested to be extracted does not exist.
		/// </exception>
		/// <exception cref="FileExistsException">
		/// Some file already exists in destination path.
		/// </exception>
		/// <exception cref="UnauthorizedAccessException">
		/// The destination directory is read only.
		/// </exception>
		Task ExtractFilesAsync(IReadOnlyCollection<SourceDestinationPathPair> fileAndDestinationPathPairs,
		                       FileExtractionErrorHandler errorHandler,
		                       CancellationToken cancelToken,
		                       IProgress<double?> progress = null);

		/// <summary>
		/// Saves the changes made to the archive.
		/// </summary>
		/// <param name="cancelToken">
		/// A token which indicates whether the saving should be canceled.
		/// </param>
		/// <param name="progress">
		/// An object indicating the progress of the saving.
		/// The progress value represents the percentage of work already done in the range [0.0, 1.0] or
		/// null if the implementation cannot reliably determine the progress.
		/// </param>
		/// <exception cref="OperationCanceledException">
		/// The save has been canceled.
		/// </exception>
		/// <exception cref="UnauthorizedAccessException">
		/// Application doesn't have write rights to specified location.
		/// </exception>
		/// <exception cref="IOException">
		/// An error occurred during the saving of an archive such as when file is used by another process.
		/// </exception>
		Task SaveAsync(CancellationToken cancelToken, IProgress<double?> progress);

		void Close();

		event EventHandler<FileAddedEventArgs>   FileAdded;
		event EventHandler<FileRemovedEventArgs> FileRemoved;
	}
}
