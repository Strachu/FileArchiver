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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FileArchiver.Core.Archive;

using Path = FileArchiver.Core.ValueTypes.Path;

namespace FileArchiver.Core
{
	/// <summary>
	/// A service which adds files from file system to an archive.
	/// </summary>
	[ContractClass(typeof(IFromFileSystemFileAddingServiceContractClass))]
	public interface IFromFileSystemFileAddingService
	{
		/// <summary>
		/// Adds the files and directories located at given paths to an archive.
		/// </summary>
		/// <param name="archive">
		/// Archive to add the files to.
		/// </param>
		/// <param name="destinationDirectory">
		/// The directory in archive to add the files to.
		/// </param>
		/// <param name="pathsOnFileSystem">
		/// The paths of files on the file system.
		/// </param>
		/// <param name="errorHandler">
		/// An error handler which will be notified of any exception thrown during file adding.
		/// Every exception which is listed in the of exceptions can be handler with this handler.
		/// It will be invoked per file and allow file skipping and handling of errors per file.
		/// </param>
		/// <exception cref="FileExistsException">
		/// Some file cannot be added because a file with its name already exists.
		/// </exception>
		/// <exception cref="FileNotFoundException">
		/// The path is invalid.
		/// </exception>
		/// <exception cref="DirectoryNotFoundException">
		/// The path is invalid.
		/// </exception>
		/// <exception cref="UnauthorizedAccessException">
		/// A user does not have permission to access some file.
		/// </exception>
		/// <exception cref="InvalidOperationException">
		/// The archive does not support multiple files and the method has been called with multiple files or
		/// the archive already has some file.
		/// </exception>
		/// <exception cref="OperationCanceledException">
		/// RetryAction.Abort has been returned from errorHandler.
		/// </exception>
		void AddFiles(IArchive archive,
		              Path destinationDirectory,
		              IReadOnlyCollection<Path> pathsOnFileSystem,
		              FileAddingErrorHandler errorHandler);
	}
}
