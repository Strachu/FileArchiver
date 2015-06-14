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
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using FileArchiver.Archive.SharpCompress;

using FileArchiver.Core.Services;

using SharpCompress.Common;
using SharpCompress.Compressor.Deflate;
using SharpCompress.Writer;
using SharpCompress.Writer.Zip;

using Path = FileArchiver.Core.ValueTypes.Path;

namespace FileArchiver.Archive.Zip
{
	public class ZipArchive : SharpCompress.ArchiveBase
	{
		private readonly CompressionType  mCompressionType;
		private readonly CompressionLevel mCompressionLevel;

		public ZipArchive(global::SharpCompress.Archive.Zip.ZipArchive archive,
		                  Path archivePath,
		                  TempFileProvider tempFileProvider,
		                  CancellationToken cancelToken)
			: base(archive, archivePath, tempFileProvider, cancelToken)
		{
			mCompressionType  = GetCompressionTypeInArchive() ?? CompressionType.Deflate;

			// TODO SharpCompress does not read DeflateCompressionLevel from existing archive
			mCompressionLevel = CompressionLevel.Default;
		}

		public ZipArchive(global::SharpCompress.Archive.Zip.ZipArchive archive,
		                  CompressionType compressionMethod,
		                  CompressionLevel compressionLevel,
		                  Path archivePath,
		                  TempFileProvider tempFileProvider,
		                  CancellationToken cancelToken)
			: base(archive, archivePath, tempFileProvider, cancelToken)
		{
			mCompressionType  = compressionMethod;
			mCompressionLevel = compressionLevel;
		}
		
		private CompressionType? GetCompressionTypeInArchive()
		{
			var fileAlreadyInArchive = RootFiles.FirstOrDefault(x => x.GetSharpCompressEntry() != null);
			if(fileAlreadyInArchive == null)
				return null;

			return fileAlreadyInArchive.GetSharpCompressEntry().CompressionType;
		}

		protected override IWriter InitializeWriter(Stream destinationStream)
		{
			var compressionInfo = new CompressionInfo
			{
				Type                    = mCompressionType,
				DeflateCompressionLevel = mCompressionLevel
			};

			return new ZipWriter(destinationStream, compressionInfo, String.Empty);
		}
	}
}
