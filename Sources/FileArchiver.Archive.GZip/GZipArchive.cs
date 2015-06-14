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
using FileArchiver.Core.Utils;

using SharpCompress.Writer;
using SharpCompress.Writer.GZip;

using Path = FileArchiver.Core.ValueTypes.Path;

namespace FileArchiver.Archive.GZip
{
	public class GZipArchive : SharpCompress.ArchiveBase
	{
		private readonly TempFileProvider mTempFileProvider;

		public GZipArchive(global::SharpCompress.Archive.GZip.GZipArchive archive,
		                   Path archivePath,
		                   TempFileProvider tempFileProvider,
		                   CancellationToken cancelToken)
			: base(archive, archivePath, tempFileProvider, cancelToken)
		{
			Contract.Requires(tempFileProvider != null);

			mTempFileProvider = tempFileProvider;
		}

		public override void AddFile(Path destinationDirectoryPath, FileEntry newFile)
		{
			if(RootFiles.Any() || newFile.Files.Any())
				throw new InvalidOperationException("GZip archive can contain only a single file.");
	
			base.AddFile(destinationDirectoryPath, newFile);
		}

		public override Task ExtractFilesAsync(IReadOnlyCollection<SourceDestinationPathPair> fileAndDestinationPathPairs,
		                                       FileExtractionErrorHandler errorHandler,
		                                       CancellationToken cancelToken,
		                                       IProgress<double?> progress = null)
		{
			if(!fileAndDestinationPathPairs.Any())
				return Task.FromResult(0);
			
			foreach(var file in fileAndDestinationPathPairs)
			{
				if(!FileExists(file.SourcePath))
					throw new FileNotFoundException(String.Format("The file \"{0}\" does not exist.", file.SourcePath));
			}
			
			return Task.Factory.StartNew(() =>
			{
				ExtractFile(fileAndDestinationPathPairs.Single().DestinationPath, errorHandler, cancelToken, progress);
			},
			cancelToken, TaskCreationOptions.LongRunning, TaskScheduler.Default);
		}

		private void ExtractFile(Path destinationPath,
		                         FileExtractionErrorHandler errorHandler,
		                         CancellationToken cancelToken,
		                         IProgress<double?> progress)
		{
			progress               = progress ?? new Progress<double?>();
			var extractionProgress = new NonMarshallingProgress<long>(bytes => progress.Report(null));

			// As the size of file is not available, the progress can be displayed only when extracting from disk.
			var fileSizeOnDisk = GetFileSizeOnDisk();
			if(fileSizeOnDisk != null)
			{
				extractionProgress = new NonMarshallingProgress<long>(bytes => progress.Report((double)bytes / fileSizeOnDisk));
			}

			var file = RootFiles.Single();
			try
			{
				var hierarchyTraverser = new FileEntryHierarchyTraverser();
				var extractingVisitor = new SharpCompress.FileEntryExtractingVisitor(mTempFileProvider, destinationPath,
				                                                                     errorHandler, cancelToken,
				                                                                     extractionProgress);

				hierarchyTraverser.Traverse(extractingVisitor, file);
			}
			finally
			{
				if(file.DataFilePath == null && !File.Exists(mTempFileProvider.GetTempFileFor(file)))
				{
					// SharpCompress does not rewind the GZip stream, so it the file can be extracted only once.
					// Reload the archive to enable future extractions.
					// It is done in finally block because it has to be even after canceling or error.
					base.ReloadArchive();
				}
			}
		}

		/// <summary>
		/// Returns the uncompressed size of file if it is already extracted.
		/// </summary>
		private long? GetFileSizeOnDisk()
		{
			var file = RootFiles.Single();
			if(file.Size > 0)
				return file.Size;

			if(file.DataFilePath != null)
				return new FileInfo(file.DataFilePath).Length;

			var tempFileInfo = new FileInfo(mTempFileProvider.GetTempFileFor(file));
			if(tempFileInfo.Exists)
				return tempFileInfo.Length;

			return null;
		}

		public override Task SaveAsync(CancellationToken cancelToken, IProgress<double?> progress)
		{
			if(!base.IsModified)
				return Task.FromResult(0);

			return base.SaveAsync(cancelToken, progress);
		}

		protected override IWriter InitializeWriter(Stream destinationStream)
		{
			return new GZipWriter(destinationStream);
		}
	}
}
