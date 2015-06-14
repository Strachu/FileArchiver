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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FileArchiver.Presentation.Utils;

using SharpCompress.Compressor.Deflate;

using Lang = FileArchiver.Archive.Zip.Properties.Resources;

namespace FileArchiver.Archive.Zip.Settings
{
	[TypeConverter(typeof(DeflateCompressionLevelTypeConverter))]
	public enum DeflateCompressionLevel
	{
		Fastest,
		Normal,
		Best
	}

	internal class DeflateCompressionLevelStringConverter : MappingTypeConverter<DeflateCompressionLevel, string>
	{
		public DeflateCompressionLevelStringConverter()
		{
			AddMapping(DeflateCompressionLevel.Fastest, Lang.Fast);
			AddMapping(DeflateCompressionLevel.Normal,  Lang.Normal);
			AddMapping(DeflateCompressionLevel.Best,    Lang.Best);
		}
	}

	internal class DeflateCompressionLevelToSharpCompressCompressionLevelConverter : MappingTypeConverter<DeflateCompressionLevel, CompressionLevel>
	{
		public DeflateCompressionLevelToSharpCompressCompressionLevelConverter()
		{
			AddMapping(DeflateCompressionLevel.Fastest, CompressionLevel.BestSpeed);
			AddMapping(DeflateCompressionLevel.Normal,  CompressionLevel.Default);
			AddMapping(DeflateCompressionLevel.Best,    CompressionLevel.BestCompression);
		}
	}

	internal class DeflateCompressionLevelTypeConverter : AggregateTypeConverter
	{
		public DeflateCompressionLevelTypeConverter()
		{
			AddConverter(new DeflateCompressionLevelStringConverter());
			AddConverter(new DeflateCompressionLevelToSharpCompressCompressionLevelConverter());
		}
	}
}
