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
using System.Diagnostics.Contracts;
using System.Linq;
using FileArchiver.Presentation.ArchiveSettings;
using FileArchiver.Presentation.ArchiveSettings.Framework;
using FileArchiver.Presentation.ArchiveSettings.Framework.Utils;
using FileArchiver.Presentation.Utils;

using Lang = FileArchiver.Archive.Zip.Properties.Resources;

namespace FileArchiver.Archive.Zip.Settings
{
	internal class ZipArchiveSettingsViewModel : NotifyPropertyChangedHelper, IArchiveSettingsViewModel
	{
		private readonly ISettingsControlsFactory mControlsFactory;

		private CompressionMethod mCompressionMethod;

		public ZipArchiveSettingsViewModel(ISettingsControlsFactory controlsFactory)
		{
			Contract.Requires(controlsFactory != null);

			mControlsFactory = controlsFactory;

			CompressionMethod       = CompressionMethod.Deflate;
			DeflateCompressionLevel = DeflateCompressionLevel.Normal;
		}

		public CompressionMethod CompressionMethod
		{
			get
			{
				return mCompressionMethod;
			}

			set
			{
				SetFieldWithNotification(ref mCompressionMethod, value);

				SetPropertyWithNotification(() => DeflateCompressionLevelVisible, mCompressionMethod == CompressionMethod.Deflate);
			}
		}

		public DeflateCompressionLevel DeflateCompressionLevel
		{
			get;
			set;
		}

		public bool DeflateCompressionLevelVisible
		{
			get;
			private set;
		}

		public IEnumerable<ISettingsControl> SettingControls
		{
			get
			{
				return new ISettingsControl[]
				{
					mControlsFactory.CreateChoiceBox(Lang.CompressionMethod, () => CompressionMethod),
					mControlsFactory.CreateChoiceBox(Lang.CompressionLevel,  () => DeflateCompressionLevel, visibleProperty: () => DeflateCompressionLevelVisible),
				};
			}
		}

		public object ToArchiveSettings()
		{
			return new ZipArchiveSettings()
			{
				CompressionMethod       = CompressionMethod,
				DeflateCompressionLevel = DeflateCompressionLevel
			};
		}
	}
}
