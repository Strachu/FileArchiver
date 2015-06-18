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
using System.Windows.Forms;

using FileArchiver.Presentation.ArchiveSettings.Framework;
using FileArchiver.Presentation.Utils.Windows.Forms;

using Lang = FileArchiver.Presentation.Properties.Resources;

namespace FileArchiver.Presentation.ArchiveSettings
{
	internal partial class NewArchiveForm : FormBase
	{
		private readonly NewArchiveViewModel mViewModel;

		private readonly int mFormHeightWithoutPanel;

		public NewArchiveForm(NewArchiveViewModel viewModel, ISettingsControlsFactory controlsFactory)
		{
			Contract.Requires(viewModel != null);
			Contract.Requires(controlsFactory != null);

			InitializeComponent();

			mViewModel = viewModel;

			mFormHeightWithoutPanel = base.Height - mSettingsPanel.Height;

			var settingsControls = new ISettingsControl[]
			{
				controlsFactory.CreateDestinationPathBrowser(Lang.DestinationPath, () => mViewModel.DestinationPath),
				controlsFactory.CreateChoiceBox(Lang.ArchiveFormat,                () => mViewModel.ArchiveFormat, () => mViewModel.AvailableArchiveFormats),
				controlsFactory.CreateGroup(Lang.ArchiveSettings,                  () => mViewModel.ArchiveSettingsControls),
			};

			mSettingsPanel.ControlsBinding = () => settingsControls;

			mViewModel.ViewClosingRequested += ClosingRequested;
		}

		private void SettingsPanel_SizeChanged(object sender, EventArgs e)
		{
			base.Height = mFormHeightWithoutPanel + mSettingsPanel.Height;
		}

		private bool mAccepted = false;

		private void Ok_Click(object sender, EventArgs e)
		{
			mViewModel.Accept();

			mAccepted = true;
		}

		private void Cancel_Click(object sender, EventArgs e)
		{
			mViewModel.Cancel();
		}

		private void Form_Closed(object sender, FormClosedEventArgs e)
		{
			if(!mAccepted)
			{
				mViewModel.Cancel();
			}
		}

		private void ClosingRequested(object sender, EventArgs e)
		{
			base.Close();
		}
	}
}
