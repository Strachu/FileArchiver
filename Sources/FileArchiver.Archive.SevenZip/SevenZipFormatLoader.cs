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
using System.Threading;
using System.Threading.Tasks;

using FileArchiver.Core.Archive;
using FileArchiver.Core.Loaders;
using FileArchiver.Core.Services;
using FileArchiver.Core.ValueTypes;

using Lang = FileArchiver.Archive.SevenZip.Properties.Resources;

namespace FileArchiver.Archive.SevenZip
{
	public class SevenZipFormatLoader : IArchiveFormatLoader
	{
		private readonly SevenZipCommunication.SevenZip         mSevenZipApplication;
		private readonly TempFileProvider mTempFileProvider;

		public SevenZipFormatLoader(TempFileProvider tempFileProvider)
		{
			Contract.Requires(tempFileProvider != null);

			mTempFileProvider    = tempFileProvider;
			mSevenZipApplication = new SevenZipCommunication.SevenZip();
		}

		public ArchiveFormatInfo ArchiveFormatInfo
		{
			get
			{
				return new ArchiveFormatInfo
				(
					extension            : ".7z",
					localizedDescription : Lang.ArchiveDescription,
					supportsCompression  : true
				);
			}
		}

		public IArchive CreateNew(Path destinationPath, object settings)
		{
			return new SevenZipArchive(destinationPath, mTempFileProvider);
		}

		public bool IsSupportedArchive(Path path)
		{
			return mSevenZipApplication.IsSevenZipArchive(path);
		}

		public Task<IArchive> LoadAsync(Path path, CancellationToken cancelToken, IProgress<double?> progress)
		{
			if(progress != null)
			{
				progress.Report(null);
			}

			return Task.Run(() =>
			{
				var archive = new SevenZipArchive(path, mTempFileProvider);

				archive.ReadEntries(cancelToken);
				
				return (IArchive)archive;
			},
			cancelToken);
		}
	}
}
