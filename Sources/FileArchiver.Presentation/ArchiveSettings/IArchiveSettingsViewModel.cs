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

using System.Collections.Generic;
using System.Diagnostics.Contracts;
using FileArchiver.Core.Archive;
using FileArchiver.Core.Loaders;
using FileArchiver.Presentation.ArchiveSettings.Framework;

namespace FileArchiver.Presentation.ArchiveSettings
{
	/// <summary>
	/// A view model responsible for controlling format specific archive settings.
	/// </summary>
	[ContractClass(typeof(IArchiveSettingsViewModelContractClass))]
	public interface IArchiveSettingsViewModel
	{
		/// <summary>
		/// The list of controls with user settings to display.
		/// </summary>
		IEnumerable<ISettingsControl> SettingControls { get; }

		/// <summary>
		/// Converts currently entered settings to a format specific structure which can be passed to <see cref="IArchiveFormatLoader.CreateNew"/>.
		/// </summary>
		object ToArchiveSettings();
	}
}
