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
using System.ComponentModel;

using FileArchiver.Presentation.Utils;

using SharpCompress.Common;

using Lang = FileArchiver.Archive.Zip.Properties.Resources;

namespace FileArchiver.Archive.Zip.Settings
{
	[TypeConverter(typeof(CompressionMethodTypeConverter))]
	public enum CompressionMethod
	{
		NoCompression,
		Deflate,
		BZip2,
		Lzma
	}

	internal class CompressionMethodToStringConverter : MappingTypeConverter<CompressionMethod, string>
	{
		public CompressionMethodToStringConverter()
		{
			AddMapping(CompressionMethod.NoCompression, Lang.NoCompression);
			AddMapping(CompressionMethod.Deflate,       Lang.Deflate);
			AddMapping(CompressionMethod.BZip2,         Lang.BZip2);
			AddMapping(CompressionMethod.Lzma,          Lang.LZMA);
		}
	}

	internal class CompressionMethodToSharpCompressCompressionTypeConverter : MappingTypeConverter<CompressionMethod, CompressionType>
	{
		public CompressionMethodToSharpCompressCompressionTypeConverter()
		{
			AddMapping(CompressionMethod.NoCompression, CompressionType.None);
			AddMapping(CompressionMethod.Deflate,       CompressionType.Deflate);
			AddMapping(CompressionMethod.BZip2,         CompressionType.BZip2);
			AddMapping(CompressionMethod.Lzma,          CompressionType.LZMA);
		}
	}

	internal class CompressionMethodTypeConverter : AggregateTypeConverter
	{
		public CompressionMethodTypeConverter()
		{
			AddConverter(new CompressionMethodToStringConverter());
			AddConverter(new CompressionMethodToSharpCompressCompressionTypeConverter());
		}
	}
}
