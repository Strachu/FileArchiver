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
using System.Linq;
using FileArchiver.Core.Archive;
using FileArchiver.Core.Loaders;
using FileArchiver.Presentation.ArchiveSettings.Framework;

namespace FileArchiver.Presentation.ArchiveSettings
{
	internal class NewArchiveFormFactory : INewArchiveSettingsScreen
	{
		private readonly ISettingsControlsFactory                      mControlsFactory;
		private readonly IArchiveLoadingService                        mLoadingService;
		private readonly IEnumerable<IArchiveSettingsViewModelFactory> mSettingsFactories;

		public NewArchiveFormFactory(ISettingsControlsFactory controlsFactory, IArchiveLoadingService loadingService,
		                             IEnumerable<IArchiveSettingsViewModelFactory> settingsFactories)
		{
			Contract.Requires(controlsFactory != null);
			Contract.Requires(loadingService != null);
			Contract.Requires(settingsFactories != null);
			Contract.Requires(Contract.ForAll(settingsFactories, x => x != null));

			mControlsFactory   = controlsFactory;
			mLoadingService    = loadingService;
			mSettingsFactories = settingsFactories;
		}

		public NewArchiveSettings Show(string defaultDestinationPath)
		{
			var viewModel = new NewArchiveViewModel(mLoadingService.SupportedFormats.ToList(), mSettingsFactories.ToList());
			var view      = new NewArchiveForm(viewModel, mControlsFactory);

			viewModel.DestinationPath = defaultDestinationPath;

			view.ShowDialog();

			return viewModel.AcceptedSettings;
		}
	}
}
