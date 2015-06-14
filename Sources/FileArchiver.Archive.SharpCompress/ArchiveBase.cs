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
using System.Threading;
using System.Threading.Tasks;

using FileArchiver.Core.Archive;
using FileArchiver.Core.DirectoryTraversing;
using FileArchiver.Core.Services;
using FileArchiver.Core.Utils.File;

using SharpCompress.Archive;
using SharpCompress.Writer;

using IArchive = SharpCompress.Archive.IArchive;
using Path     = FileArchiver.Core.ValueTypes.Path;

namespace FileArchiver.Archive.SharpCompress
{
	public abstract class ArchiveBase : Core.Archive.ArchiveBase
	{
		private          IArchive         mArchive;
		private readonly TempFileProvider mTempFileProvider;
		private readonly Path             mArchivePath;

		protected ArchiveBase(IArchive archive,
		                      Path archivePath,
		                      TempFileProvider tempFileProvider,
		                      CancellationToken cancelToken)
		{
			Contract.Requires(archive != null);
			Contract.Requires(archivePath != null);
			Contract.Requires(tempFileProvider != null);

			mArchive          = archive;
			mArchivePath      = archivePath;
			mTempFileProvider = tempFileProvider;

			ReadEntries(cancelToken);
		}

		private void ReadEntries(CancellationToken cancelToken)
		{
			var fileListBuilder = new FileEntryHierarchyBuilder();

			foreach(var sharpCompressEntry in mArchive.Entries)
			{
				cancelToken.ThrowIfCancellationRequested();

				var destinationDirectory = new Path(sharpCompressEntry.Key).ParentDirectory;

				fileListBuilder.AddFile(destinationDirectory, sharpCompressEntry.ToFileEntry());
			}

			foreach(var file in fileListBuilder.Build())
			{
				base.AddFile(Path.Root, file);
			}
		}

		protected IArchive SharpCompressArchive
		{
			get { return mArchive; }
		}

		public override Task ExtractFilesAsync(IReadOnlyCollection<SourceDestinationPathPair> fileAndDestinationPathPairs,
		                                       FileExtractionErrorHandler errorHandler,
		                                       CancellationToken cancelToken,
		                                       IProgress<double?> progress = null)
		{
			return Task.Factory.StartNew(() =>
			{
				ExtractFiles(fileAndDestinationPathPairs, errorHandler, cancelToken, progress);
			},
			cancelToken, TaskCreationOptions.LongRunning, TaskScheduler.Default);
		}

		private void ExtractFiles(IEnumerable<SourceDestinationPathPair> fileAndDestinationPathPairs,
		                          FileExtractionErrorHandler errorHandler,
		                          CancellationToken cancelToken,
		                          IProgress<double?> progress = null)
		{
			var extractOperations = fileAndDestinationPathPairs.Select(pair => new
			{
				SourceFile      = base.GetFile(pair.SourcePath),
				DestinationPath = pair.DestinationPath
			}).ToList();

			progress               = progress ?? new Progress<double?>();
			long bytesToExtract    = extractOperations.Sum(file => file.SourceFile.Size);
			var extractionProgress = new CompositeFileProgress(bytes => progress.Report((double)bytes / bytesToExtract));

			foreach(var extractOperation in extractOperations)
			{
				var hierarchyTraverser = new FileEntryHierarchyTraverser();
				var extractingVisitor  = new FileEntryExtractingVisitor(mTempFileProvider, extractOperation.DestinationPath,
				                                                        errorHandler, cancelToken,
				                                                        extractionProgress.GetProgressForNextFile());

				hierarchyTraverser.Traverse(extractingVisitor, extractOperation.SourceFile);
			}
		}

		public override Task SaveAsync(CancellationToken cancelToken, IProgress<double?> progress)
		{
			return Task.Factory.StartNew(() =>
			{
				Save(cancelToken, progress);
			},
			cancelToken, TaskCreationOptions.LongRunning, TaskScheduler.Default);
		}

		private void Save(CancellationToken cancelToken, IProgress<double?> progress)
		{
			progress            = progress ?? new Progress<double?>();
			long bytesToSave    = RootFiles.Sum(file => file.Size);
			var  savingProgress = new CompositeFileProgress(saved => progress.Report((double)saved / bytesToSave));

			var tempFilePath    = PathUtil.GetUniquePath(mArchivePath.ParentDirectory);
			try
			{
				// TODO what will happen when the application cannot write to destination directory??
				WriteEntries(tempFilePath, cancelToken, savingProgress);

				ReplaceArchive(tempFilePath);
			}
			catch
			{
				File.Delete(tempFilePath);
				throw;
			}

			ReloadArchive();	
		}

		/// <summary>
		/// Writes the entries to given destination path.
		/// </summary>
		/// <param name="destinationPath">
		/// The destination path of archive to write entries to.
		/// </param>
		/// <param name="cancelToken">
		/// The cancellation token.
		/// </param>
		/// <param name="progress">
		/// The object which should be notified of writing progress.
		/// </param>
		protected virtual void WriteEntries(Path destinationPath,
		                                    CancellationToken cancelToken,
		                                    CompositeFileProgress progress)
		{
			using(var destinationStream = File.Open(destinationPath, FileMode.Create, FileAccess.Write))
			using(var writer = InitializeWriter(destinationStream))
			{
				foreach(var file in RootFiles)
				{
					var hierarchyTraverser = new FileEntryHierarchyTraverser();
					var writingVisitor     = new FileEntryWritingVisitor(writer, cancelToken, progress.GetProgressForNextFile());

					hierarchyTraverser.Traverse(writingVisitor, file);
				}
			}
		}

		protected abstract IWriter InitializeWriter(Stream destinationStream);

		private void ReplaceArchive(string tempFilePath)
		{
			mArchive.Dispose();

			File.Delete(mArchivePath);
			File.Move(tempFilePath, mArchivePath);
		}

		protected void ReloadArchive()
		{
			mArchive = ArchiveFactory.Open(mArchivePath);

			base.ResetState();

			ReadEntries(CancellationToken.None);
		}

		protected override void Dispose(bool disposeManagedResources)
		{
			if(disposeManagedResources)
			{
				if(mArchive != null)
				{
					mArchive.Dispose();
					mArchive = null;
				}
			}

			base.Dispose(disposeManagedResources);
		}
	}
}
