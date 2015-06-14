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
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FileArchiver.Presentation.Commands.CommandSystem;
using FileArchiver.Presentation.Settings;
using FileArchiver.Presentation.Shell;

namespace FileArchiver.Presentation.Commands
{
	public class SetLanguageCommand : CommandBase
	{
		private readonly IMainView mMainView;
		private readonly string    mLanguageToSet;

		public SetLanguageCommand(IMainView mainView, string languageToSet)
		{
			Contract.Requires(mainView != null);
			Contract.Requires(!String.IsNullOrWhiteSpace(languageToSet));

			mMainView      = mainView;
			mLanguageToSet = languageToSet;
		}

		public override Task ExecuteAsync()
		{
			var chosenCulture = new CultureInfo(mLanguageToSet);

			CultureInfo.DefaultThreadCurrentCulture   = chosenCulture;
			CultureInfo.DefaultThreadCurrentUICulture = chosenCulture;

			// Reset the sizes of controls during first change of the language
			if(ApplicationSettings.Instance.WindowLayout != null)
			{
				ApplicationSettings.Instance.WindowLayout = mMainView.LayoutSettings;

				mMainView.Language = mLanguageToSet;

				mMainView.LayoutSettings = ApplicationSettings.Instance.WindowLayout;
			}
			else
			{
				mMainView.Language = mLanguageToSet;
			}

			ApplicationSettings.Instance.Language = mLanguageToSet;

			return Task.FromResult(0);
		}
	}
}
