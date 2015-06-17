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

using Lang = FileArchiver.Archive.GZip.Properties.Resources;

namespace FileArchiver.Archive.GZip
{
	public class GZipFormatLoader : IArchiveFormatLoader
	{
		private readonly TempFileProvider mTempFileProvider;

		public GZipFormatLoader(TempFileProvider tempFileProvider)
		{
			Contract.Requires(tempFileProvider != null);

			mTempFileProvider = tempFileProvider;
		}

		public ArchiveFormatInfo ArchiveFormatInfo
		{
			get
			{
				return new ArchiveFormatInfo
				(
					extension             : ".gz",
					localizedDescription  : Lang.ArchiveDescription,
					supportsCompression   : true,
					supportsMultipleFiles : false
				);
			}
		}

		public IArchive CreateNew(Path destinationPath, object settings)
		{
			return new GZipArchive(global::SharpCompress.Archive.GZip.GZipArchive.Create(), destinationPath, mTempFileProvider,
			                       CancellationToken.None);
		}

		public bool IsSupportedArchive(Path path)
		{
			return global::SharpCompress.Archive.GZip.GZipArchive.IsGZipFile(path);
		}

		public Task<IArchive> LoadAsync(Path path, CancellationToken cancelToken, IProgress<double?> progress)
		{
			if(progress != null)
			{
				progress.Report(null);
			}

			return Task.Run(() =>
			{
				var sharpCompressArchive = global::SharpCompress.Archive.GZip.GZipArchive.Open(path);

				return (IArchive)new GZipArchive(sharpCompressArchive, path, mTempFileProvider, cancelToken);
			},
			cancelToken);
		}
	}
}
