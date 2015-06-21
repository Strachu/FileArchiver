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

using FileArchiver.Presentation.Utils;

using Lang = FileArchiver.Archive.SevenZip.Properties.Resources;

namespace FileArchiver.Archive.SevenZip.Settings
{
	[TypeConverter(typeof(CompressionLevelStringConverter))]
	public enum CompressionLevel
	{
		NoCompression = 0,
		Fastest       = 1,
		Normal        = 5,
		Good          = 7,
		Best          = 9
	}

	internal class CompressionLevelStringConverter : MappingTypeConverter<CompressionLevel, string>
	{
		public CompressionLevelStringConverter()
		{
			AddMapping(CompressionLevel.NoCompression, Lang.NoCompression);
			AddMapping(CompressionLevel.Fastest,       Lang.FastestCompression);
			AddMapping(CompressionLevel.Normal,        Lang.NormalCompression);
			AddMapping(CompressionLevel.Good,          Lang.GoodCompression);
			AddMapping(CompressionLevel.Best,          Lang.BestCompression);
		}
	}
}
