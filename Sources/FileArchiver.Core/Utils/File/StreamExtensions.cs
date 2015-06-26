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

using Path = FileArchiver.Core.ValueTypes.Path;

namespace FileArchiver.Core.Utils.File
{
	public static class StreamExtensions
	{
		/// <summary>
		/// Copies the data from a source stream to a file with support of progress reporting and canceling.
		/// </summary>
		/// <param name="sourceStream">
		/// Source stream.
		/// </param>
		/// <param name="destinationPath">
		/// Destination path.
		/// </param>
		/// <param name="mode">
		/// File open mode.
		/// </param>
		/// <param name="cancelToken">
		/// Cancel token.
		/// </param>
		/// <param name="progress">
		/// The object which will be notified about the progress.
		/// Progress value = total bytes written at the moment of report.
		/// </param>
		/// <exception cref="OperationCanceledException">
		/// Operation has been canceled.
		/// </exception>
		/// <remarks>
		/// If the operation is canceled, partially copied file will be removed.
		/// </remarks>
		public static void CopyToFileWithProgress(this Stream sourceStream, Path destinationPath, FileMode mode,
																CancellationToken cancelToken, IProgress<long> progress = null)
		{
			Contract.Requires(sourceStream != null);
			Contract.Requires(destinationPath != null);
			Contract.Requires(destinationPath.ParentDirectory != null);

			Directory.CreateDirectory(destinationPath.ParentDirectory);

			using(var fileStream            = new FileStream(destinationPath, mode, FileAccess.Write, FileShare.None))
			using(var cancellationDecorator = new StreamWithCancellationSupport(fileStream, cancelToken))
			using(var destinationStream     = new ProgressReportingStream(cancellationDecorator, progress))
			{
				try
				{
					sourceStream.CopyTo(destinationStream);
				}
				catch
				{
					fileStream.Close();

					System.IO.File.Delete(destinationPath);
					throw;
				}
			}
		}

		/// <summary>
		/// A version of <see cref="CopyToFileWithProgress"/> which create new file
		/// and throws a FileExistsException exception when the file already exists.
		/// </summary>
		public static void CopyToNewFileWithProgress(this Stream sourceStream,
		                                             Path destinationPath,
		                                             CancellationToken cancelToken,
		                                             IProgress<long> progress = null)
		{
			try
			{
				sourceStream.CopyToFileWithProgress(destinationPath, FileMode.CreateNew, cancelToken, progress);
			}
			catch(IOException e)
			{
				if(System.IO.File.Exists(destinationPath))
				{
					throw new FileExistsException(destinationPath, e);
				}

				throw;
			}
		}
	}
}
