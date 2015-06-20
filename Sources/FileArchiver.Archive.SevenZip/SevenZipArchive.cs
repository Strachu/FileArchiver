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
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using FileArchiver.Archive.SevenZip.Settings;
using FileArchiver.Core.Archive;
using FileArchiver.Core.Services;
using FileArchiver.Core.Utils;

using Path = FileArchiver.Core.ValueTypes.Path;

namespace FileArchiver.Archive.SevenZip
{
	public partial class SevenZipArchive : ArchiveBase
	{
		private readonly SevenZipCommunication.SevenZip mSevenZipApplication;
		private readonly TempFileProvider               mTempFileProvider;

		private readonly Path                           mArchivePath;
		private readonly List<FileEntry>                mOriginalFiles = new List<FileEntry>();

		private readonly IDictionary<int, IList<Guid>>  mSolidBlockFileIdsIndex = new Dictionary<int, IList<Guid>>();

		public SevenZipArchive(Path archivePath,
		                       TempFileProvider tempFileProvider)
		{
			Contract.Requires(archivePath != null);
			Contract.Requires(tempFileProvider != null);

			mArchivePath         = archivePath;
			mSevenZipApplication = new SevenZipCommunication.SevenZip();
			mTempFileProvider    = tempFileProvider;

			CompressionLevel     = CompressionLevel.Normal;
			IsSolid              = true;
		}

		public void ReadEntries(CancellationToken cancelToken)
		{
			var archiveInfo = mSevenZipApplication.ReadArchiveInfo(mArchivePath, cancelToken);

			IsSolid = archiveInfo.Solid;

			foreach(var file in archiveInfo.Files)
			{
				base.AddFile(Path.Root, file);
			}

			foreach(var file in base.RootFiles.Flatten())
			{
				AddToSolidBlockIndex(file);
			}

			base.RootFiles.CopyTo(mOriginalFiles);
		}

		private void AddToSolidBlockIndex(FileEntry file)
		{
			var entryData = file.GetArchiveEntryData();
			if(entryData.SolidBlockIndex == null)
				return;

			var solidBlockIndex = entryData.SolidBlockIndex.Value;
			
			if(!mSolidBlockFileIdsIndex.ContainsKey(solidBlockIndex))
			{
				mSolidBlockFileIdsIndex.Add(solidBlockIndex, new List<Guid>());
			}

			mSolidBlockFileIdsIndex[solidBlockIndex].Add(file.Id);
		}
		
		private IEnumerable<FileEntry> GetAllFilesInSolidBlocksOf(IReadOnlyCollection<FileEntry> filesToExtract)
		{
			var solidBlocksToExtract = filesToExtract.Select(x => x.GetArchiveEntryData().SolidBlockIndex)
			                                         .Distinct()
			                                         .ToList();

			// Special case: empty files do not have assigned any solid block, but they also should be returned
			var filesWithoutSolidBlock = filesToExtract.Where(x => x.GetArchiveEntryData().SolidBlockIndex == null);

			return mSolidBlockFileIdsIndex.Where(x => solidBlocksToExtract.Contains(x.Key))
			                              .SelectMany(x => x.Value)
													.Where(FileExists)
			                              .Select(GetFile)
													.Concat(filesWithoutSolidBlock);
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

		public override Task SaveAsync(CancellationToken cancelToken, IProgress<double?> progress)
		{
			return Task.Factory.StartNew(() =>
			{
				Save(cancelToken, progress);
			},
			cancelToken, TaskCreationOptions.LongRunning, TaskScheduler.Default);
		}

		public CompressionLevel CompressionLevel
		{
			get;
			set;
		}

		public bool IsSolid
		{
			get;
			set;
		}
	}
}
