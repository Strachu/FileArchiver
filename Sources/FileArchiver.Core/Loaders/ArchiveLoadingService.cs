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
using FileArchiver.Core.Archive;
using FileArchiver.Core.Services;
using FileArchiver.Core.Utils;
using FileArchiver.Core.ValueTypes;

namespace FileArchiver.Core.Loaders
{
	public class ArchiveLoadingService : IArchiveLoadingService
	{
		private readonly TempFileProvider                  mTempFileProvider;
		private readonly IEnumerable<IArchiveFormatLoader> mArchiveLoaders;

		public ArchiveLoadingService(TempFileProvider tempFileProvider, IEnumerable<IArchiveFormatLoader> archiveLoaders)
		{
			Contract.Requires(tempFileProvider != null);
			Contract.Requires(archiveLoaders != null);
			Contract.Requires(Contract.ForAll(archiveLoaders, loader => loader != null));

			mTempFileProvider = tempFileProvider;
			mArchiveLoaders   = archiveLoaders;
		}

		public IArchive CreateNew(Path archivePath, IReadOnlyCollection<ArchiveFormatWithSettings> formats)
		{
			var format           = formats.Last();
			var archiveLoader    = GetLoaderForExtension(System.IO.Path.GetExtension(format.ArchiveFormat));
			var archive          = archiveLoader.CreateNew(archivePath, format.ArchiveSettings);

			var remainingFormats = formats.WithoutLastElement().ToList();

			if(!remainingFormats.Any())
			{
				return archive;
			}

			var tempPathForNestedArchive = mTempFileProvider.GetUniqueTempFile(archivePath.RemoveExtension().FileName);
			var nestedArchive            = CreateNew(tempPathForNestedArchive, remainingFormats);

			return new ArchiveInArchiveDecorator(archive, nestedArchive, tempPathForNestedArchive);
		}

		public async Task<IArchive> LoadAsync(Path path, CancellationToken cancelToken, IProgress<double?> progress = null)
		{
			var archiveLoader = mArchiveLoaders.FirstOrDefault(loader => loader.IsSupportedArchive(path));
			if(archiveLoader == null)
				throw new NotSupportedFormatException(path);

			var archive = await archiveLoader.LoadAsync(path, cancelToken, progress);

			if(DoesNotHaveEmbeddedArchive(archive))
				return archive;

			var unpackedArchivePath = await ExtractEmbeddedArchiveToTemp(archive, cancelToken, progress);
			var nestedArchive       = await LoadAsync(unpackedArchivePath, cancelToken, progress);

			return new ArchiveInArchiveDecorator(archive, nestedArchive, unpackedArchivePath);
		}

		private IArchiveFormatLoader GetLoaderForExtension(string extension)
		{
			return mArchiveLoaders.Single(loader => String.Equals(loader.ArchiveFormatInfo.Extension, extension, StringComparison.CurrentCultureIgnoreCase));
		}

		private bool DoesNotHaveEmbeddedArchive(IArchive archive)
		{
			var totalFiles = archive.RootFiles.Sum(file => file.EnumerateAllFilesRecursively().Count() + 1);

			return totalFiles != 1 || !IsSupportedArchive(archive.RootFiles.Single().Name);
		}

		private bool IsSupportedArchive(string path)
		{
			var fileExtension = System.IO.Path.GetExtension(path);

			return mArchiveLoaders.Any(loader =>
			{
				return String.Equals(loader.ArchiveFormatInfo.Extension, fileExtension, StringComparison.CurrentCultureIgnoreCase);
			});
		}

		private async Task<Path> ExtractEmbeddedArchiveToTemp(IArchive parentArchive,
		                                                      CancellationToken cancelToken,
		                                                      IProgress<double?> progress)
		{
			var sourceFilePath = parentArchive.RootFiles.Single().Path;
			var destinationPath = mTempFileProvider.GetTempFileFor(parentArchive.RootFiles.Single());

			await parentArchive.ExtractFilesAsync(new SourceDestinationPathPair(sourceFilePath, destinationPath).ToSingleElementList(),
			                                      cancelToken, progress);

			return destinationPath;
		}

		public IEnumerable<ArchiveFormatInfo> SupportedFormats
		{
			get { return mArchiveLoaders.Select(loader => loader.ArchiveFormatInfo); }
		}
	}
}
