﻿#region Copyright
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

using System.Diagnostics.Contracts;

namespace FileArchiver.Presentation.ArchiveSettings
{
	/// <summary>
	/// A factory which creates a view model with format specific archive settings.
	/// </summary>
	[ContractClass(typeof(IArchiveSettingsViewModelFactoryContractClass))]
	public interface IArchiveSettingsViewModelFactory
	{
		IArchiveSettingsViewModel CreateSettingsViewModel();

		/// <summary>
		/// An archive extension whose format specific settings' view model can be created with this factory.
		/// </summary>
		string ArchiveExtension { get; }
	}
}
