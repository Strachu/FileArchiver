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

using FileArchiver.Presentation.ArchiveSettings;
using FileArchiver.Presentation.ArchiveSettings.Framework;
using FileArchiver.Presentation.Utils;

using Lang = FileArchiver.Archive.SevenZip.Properties.Resources;

namespace FileArchiver.Archive.SevenZip.Settings
{
	internal class ArchiveSettingsViewModel : NotifyPropertyChangedHelper, IArchiveSettingsViewModel
	{
		private readonly ISettingsControlsFactory mControlsFactory;

		public ArchiveSettingsViewModel(ISettingsControlsFactory controlsFactory)
		{
			Contract.Requires(controlsFactory != null);

			mControlsFactory = controlsFactory;
		}

		private CompressionLevel _mCompressionLevel = CompressionLevel.Normal;
		public CompressionLevel CompressionLevel
		{
			get { return _mCompressionLevel; }
			set { base.SetFieldWithNotification(ref _mCompressionLevel, value); }
		}

		private bool _mSolidCompression = true;
		public bool SolidCompression
		{
			get { return _mSolidCompression; }
			set { base.SetFieldWithNotification(ref _mSolidCompression, value); }
		}

		public IEnumerable<ISettingsControl> SettingControls
		{
			get
			{
				return new ISettingsControl[]
				{
					mControlsFactory.CreateChoiceBox(Lang.CompressionLevel, () => CompressionLevel),
					mControlsFactory.CreateCheckBox(Lang.SolidCompression,  () => SolidCompression)
				};
			}
		}

		public object ToArchiveSettings()
		{
			return new ArchiveSettings
			{
				CompressionLevel = CompressionLevel,
				SolidCompression = SolidCompression
			};
		}
	}
}
