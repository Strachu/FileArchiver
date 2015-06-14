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
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using FileArchiver.Core.Archive;

using Path = FileArchiver.Core.ValueTypes.Path;

namespace FileArchiver.Core.Services
{
	/// <summary>
	/// A class whose purpose is to open files in external application and listen for its modification.
	/// </summary>
	public class FileOpeningService : IDisposable
	{
		private readonly TempFileProvider                 mTempFileProvider;
		private IDictionary<FileEntry, FileSystemWatcher> mOpenFileWatchers = new Dictionary<FileEntry, FileSystemWatcher>();

		public FileOpeningService(TempFileProvider tempFileProvider)
		{
			Contract.Requires(tempFileProvider != null);

			mTempFileProvider = tempFileProvider;
		}

		/// <summary>
		/// Extracts and opens the files asynchronously in an external application.
		/// </summary>
		/// <param name="archive">
		/// The archive from which the files should be extracted.
		/// </param>
		/// <param name="filePaths">
		/// The paths of files to extracts. <b>Paths referring to a directory are forbidden.</b>
		/// </param>
		/// <param name="cancelToken">
		/// The cancel token.
		/// </param>
		/// <param name="progress">
		/// The object receiving information about the extraction progress.
		/// Passed value is a percentage indicator in the range [0.0, 1.0] or null
		/// if the progress cannot be determined.
		/// </param>
		/// <exception cref="OperationCanceledException">
		/// The extraction of files has been canceled.
		/// </exception>
		/// <remarks>
		/// If the has been modified in the external application, the update will be automatically
		/// reflected in the archive.
		/// </remarks>
		public async Task OpenFilesAsync(IArchive archive, IEnumerable<Path> filePaths, CancellationToken cancelToken, IProgress<double?> progress)
		{
			var files          = filePaths.Select(archive.GetFile).ToList();
			var filesNotInTemp = files.Where(file => !File.Exists(mTempFileProvider.GetTempFileFor(file))).ToList();

			EnsureNoDirectories(files);

			// TODO there should also be a per file error handler used - the source file (if on disk)
			// can be in use by some application
			await ExtractFilesToTempAsync(archive, filesNotInTemp, cancelToken, progress);

			foreach(var file in files)
			{
				StartListeningForFileChanges(archive, file);

				Process.Start(mTempFileProvider.GetTempFileFor(file));
			}
		}

		private Task ExtractFilesToTempAsync(IArchive archive, IEnumerable<FileEntry> files, CancellationToken cancelToken, IProgress<double?> progress)
		{
			var sourceDestinationPairs = files.Select(file => new SourceDestinationPathPair
			(
				sourcePath      : file.Path,
				destinationPath : mTempFileProvider.GetTempFileFor(file)
			));

			return archive.ExtractFilesAsync(sourceDestinationPairs.ToList(), cancelToken, progress);
		}

		private void EnsureNoDirectories(IEnumerable<FileEntry> files)
		{
			var firstDirectory = files.FirstOrDefault(file => file.IsDirectory);
			if(firstDirectory != null)
				throw new ArgumentException(String.Format("Only files can be opened. \"{0}\" is a directory.", firstDirectory));
		}

		private void StartListeningForFileChanges(IArchive archive, FileEntry file)
		{
			if(mOpenFileWatchers.ContainsKey(file))
				return;

			var watcher = new FileSystemWatcher(mTempFileProvider.GetTempFileFor(file).ParentDirectory)
			{
				EnableRaisingEvents   = true,
				Filter                = file.Name,
				NotifyFilter          = NotifyFilters.LastWrite,
			};

			watcher.Changed += (sender, e) =>
			{
				ReloadFileDataFromDisk(archive, file.Id, new Path(e.FullPath));
			};

			mOpenFileWatchers.Add(file, watcher);
		}

		private void ReloadFileDataFromDisk(IArchive archive, Guid fileInArchiveId, Path fileOnDiskPath)
		{
			// TODO is there a need to catch an exception when some external application blocks the access to the file?
			// TODO what if the file (in archive) has been removed?
			var fileInArchive = archive.GetFile(fileInArchiveId);
			var fileInfo      = new FileInfo(fileOnDiskPath);

			var updatedFileInArchive = fileInArchive.BuildCopy()
			                                        .AsModified()
			                                        .WithDataFromFile(fileInfo)
			                                        .Build();

			archive.UpdateFile(updatedFileInArchive);
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposeManagedResources)
		{
			if(disposeManagedResources)
			{
				if(mOpenFileWatchers != null)
				{
					foreach(var watcher in mOpenFileWatchers.Values)
					{
						watcher.Dispose();
					}
					mOpenFileWatchers = null;
				}
			}
		}
	}
}
