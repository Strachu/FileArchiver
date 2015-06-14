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
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Threading;
using System.Windows.Forms;

using FileArchiver.Presentation.CommandLine;
using FileArchiver.Presentation.FileListView;
using FileArchiver.Presentation.Settings;
using FileArchiver.Presentation.Shell;

namespace FileArchiver.Presentation
{
	internal class Program
	{
		[STAThread]
		private static void Main(string[] args)
		{
			var catalogs = new List<ComposablePartCatalog>()
			{
				ApplicationBootstraper.CreateApplicationCatalog(),
				ApplicationBootstraper.CreatePluginsCatalog()
			};

			using(var container = ApplicationBootstraper.InitializeContainer(catalogs))
			{
				ApplicationBootstraper.ApplyLanguageFromSettings();

				Application.EnableVisualStyles();
				Application.SetCompatibleTextRenderingDefault(false);

				SynchronizationContext.SetSynchronizationContext(new WindowsFormsSynchronizationContext());

				if(HandleCommandLine(container, args))
					return;

				var mainForm  = (Form)container.GetExportedValue<IMainView>();
				var presenter = container.GetExportedValue<MainPresenter>();

				Application.Run(mainForm);

				ApplicationSettings.Instance.Save();
			}
		}

		private static bool HandleCommandLine(CompositionContainer container, string[] args)
		{
			var commandLineHandlers = container.GetExportedValues<ICommandLineHandler>();
			foreach(var handler in commandLineHandlers)
			{
				if(handler.Handle(args))
				{
					return true;
				}
			}
			return false;
		}
	}
}
